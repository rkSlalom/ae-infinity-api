using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

