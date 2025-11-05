using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AeInfinity.Application.Features.Statistics.Queries.GetUserStats;

public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, UserStatsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetUserStatsQueryHandler> _logger;
    private const int CacheExpirationMinutes = 5;
    private const int PerformanceWarningThresholdMs = 500;

    public GetUserStatsQueryHandler(
        IApplicationDbContext context, 
        ICacheService cacheService,
        ILogger<GetUserStatsQueryHandler> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<UserStatsDto> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var cacheKey = $"user-stats:{request.UserId}";

        try
        {
            // Check cache first
            var cachedStats = await _cacheService.GetAsync<UserStatsDto>(cacheKey);
            if (cachedStats != null)
            {
                stopwatch.Stop();
                _logger.LogDebug(
                    "Statistics cache HIT for User {UserId}. Retrieved in {ElapsedMs}ms",
                    request.UserId,
                    stopwatch.ElapsedMilliseconds);
                return cachedStats;
            }

            _logger.LogDebug("Statistics cache MISS for User {UserId}. Calculating...", request.UserId);
            
            // Calculate statistics
            var stats = await CalculateStatistics(request.UserId, cancellationToken);

            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, stats, TimeSpan.FromMinutes(CacheExpirationMinutes));

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Log performance warning if query is slow
            if (elapsedMs > PerformanceWarningThresholdMs)
            {
                _logger.LogWarning(
                    "PERFORMANCE: Statistics query for User {UserId} took {ElapsedMs}ms (threshold: {ThresholdMs}ms). " +
                    "Lists: {ListsOwned}, Items: {ItemsCreated}. Consider adding database indexes.",
                    request.UserId,
                    elapsedMs,
                    PerformanceWarningThresholdMs,
                    stats.TotalListsOwned,
                    stats.TotalItemsCreated);
            }
            else
            {
                _logger.LogInformation(
                    "Statistics calculated for User {UserId} in {ElapsedMs}ms",
                    request.UserId,
                    elapsedMs);
            }

            return stats;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Failed to calculate statistics for User {UserId} after {ElapsedMs}ms",
                request.UserId,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    private async Task<UserStatsDto> CalculateStatistics(Guid userId, CancellationToken cancellationToken)
    {
        // Count lists owned by user
        var totalListsOwned = await _context.Lists
            .Where(l => !l.IsDeleted && l.OwnerId == userId)
            .CountAsync(cancellationToken);

        // Count lists shared with user (collaborator but not owner)
        var totalListsShared = await _context.UserToLists
            .Where(utl => !utl.IsDeleted && 
                         utl.UserId == userId && 
                         !utl.IsPending)
            .Join(_context.Lists,
                utl => utl.ListId,
                l => l.Id,
                (utl, l) => new { utl, l })
            .Where(x => !x.l.IsDeleted && x.l.OwnerId != userId)
            .CountAsync(cancellationToken);

        // Count items created by user
        var totalItemsCreated = await _context.ListItems
            .Where(li => !li.IsDeleted && li.CreatedBy == userId)
            .CountAsync(cancellationToken);

        // Count items purchased by user
        var totalItemsPurchased = await _context.ListItems
            .Where(li => !li.IsDeleted && 
                        li.IsPurchased && 
                        li.PurchasedBy == userId)
            .CountAsync(cancellationToken);

        // Count active collaborations (excluding owned lists)
        var totalActiveCollaborations = await _context.UserToLists
            .Where(utl => !utl.IsDeleted && 
                         utl.UserId == userId && 
                         !utl.IsPending)
            .Join(_context.Lists,
                utl => utl.ListId,
                l => l.Id,
                (utl, l) => new { utl, l })
            .Where(x => !x.l.IsDeleted && x.l.OwnerId != userId)
            .CountAsync(cancellationToken);

        // Get last activity timestamp (latest of: list created, item created, item updated, item purchased)
        var lastListActivity = await _context.Lists
            .Where(l => !l.IsDeleted && l.OwnerId == userId)
            .MaxAsync(l => (DateTime?)l.UpdatedAt, cancellationToken);

        var lastItemActivity = await _context.ListItems
            .Where(li => !li.IsDeleted && 
                        (li.CreatedBy == userId || li.UpdatedBy == userId))
            .MaxAsync(li => (DateTime?)li.UpdatedAt, cancellationToken);

        var lastPurchaseActivity = await _context.ListItems
            .Where(li => !li.IsDeleted && 
                        li.IsPurchased && 
                        li.PurchasedBy == userId)
            .MaxAsync(li => (DateTime?)li.PurchasedAt, cancellationToken);

        var lastActivityAt = new[] { lastListActivity, lastItemActivity, lastPurchaseActivity }
            .Where(d => d.HasValue)
            .Max();

        return new UserStatsDto
        {
            TotalListsOwned = totalListsOwned,
            TotalListsShared = totalListsShared,
            TotalItemsCreated = totalItemsCreated,
            TotalItemsPurchased = totalItemsPurchased,
            TotalActiveCollaborations = totalActiveCollaborations,
            LastActivityAt = lastActivityAt
        };
    }
}

