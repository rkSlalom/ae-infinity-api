using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Entities;
using AeInfinity.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IMapper mapper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<LoginResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists (case-insensitive)
        var emailNormalized = request.Email.ToUpper();
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.EmailNormalized == emailNormalized, cancellationToken);

        if (existingUser != null)
        {
            throw new ValidationException("Email already registered");
        }

        // Hash password with BCrypt
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create new user
        var user = new User
        {
            Email = request.Email,
            EmailNormalized = emailNormalized,
            DisplayName = request.DisplayName,
            PasswordHash = passwordHash,
            AvatarUrl = null, // Initially null per spec (can be set post-registration via profile editing)
            IsEmailVerified = false,
            LastLoginAt = DateTime.UtcNow, // Set on registration for auto-login
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate JWT token (auto-login)
        var token = _jwtTokenService.GenerateToken(user);
        var expiresAt = _jwtTokenService.GetTokenExpiration();

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = _mapper.Map<UserDto>(user)
        };
    }
}

