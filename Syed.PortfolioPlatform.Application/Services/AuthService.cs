using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Syed.PortfolioPlatform.Application.Auth.DTOs;
using Syed.PortfolioPlatform.Application.Common.Interface;
using Syed.PortfolioPlatform.Application.DTOs.Auth;
using Syed.PortfolioPlatform.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<AdminUser> _passwordHasher;

    public AuthService(IAuthRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<AdminUser>();
    }

    public async Task<AdminUser> CreateAdminAsync(string username, string password, Guid roleId)
    {
        var admin = new AdminUser
        {
            Username = username,
            PasswordHash = HashPassword(password),
            RoleId = roleId
        };

        await _repository.AddAdminAsync(admin);
        return admin;
    }


    private int AccessTokenExpiryMinutes =>
        int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "15");

    private int RefreshTokenExpiryDays =>
        int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");

    public async Task<AuthResponse> LoginAsync(string username, string password)
    {
        var admin = await _repository.GetAdminByUsernameAsync(username);

        if (admin == null || !VerifyPassword(password, admin))
            throw new UnauthorizedAccessException("Invalid username or password.");

        var accessToken = GenerateAccessToken(admin);
        var refreshToken = GenerateRefreshToken();

        var dbToken = new RefreshToken
        {
            AdminId = admin.Id,
            Token = HashToken(refreshToken),
            ExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays),
            IsRevoked = false
        };

        await _repository.AddRefreshTokenAsync(dbToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new UnauthorizedAccessException("Refresh token required.");

        var hashedToken = HashToken(request.RefreshToken);

        var storedToken = await _repository.GetRefreshTokenAsync(hashedToken);

        if (storedToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired or revoked.");

        var admin = storedToken.Admin;

        var newAccessToken = GenerateAccessToken(admin);
        var newRefreshToken = GenerateRefreshToken();

        storedToken.Token = HashToken(newRefreshToken);
        storedToken.ExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);

        await _repository.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes)
        };
    }

    private bool VerifyPassword(string password, AdminUser admin)
    {
        var result = _passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }


    private string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null, password);
    }

    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();

        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

    private string GenerateAccessToken(AdminUser admin)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var secret = jwtSettings["Secret"]
            ?? throw new Exception("JWT Secret missing");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName, admin.Username),
            new Claim(ClaimTypes.Role, admin.RoleNavigation?.Name ?? "Admin"),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString()),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Nbf, new DateTimeOffset(now).ToUnixTimeSeconds().ToString()),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expires = now.AddMinutes(
            int.Parse(jwtSettings["AccessTokenExpirationMinutes"] ?? "15"));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        RandomNumberGenerator.Fill(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }


}