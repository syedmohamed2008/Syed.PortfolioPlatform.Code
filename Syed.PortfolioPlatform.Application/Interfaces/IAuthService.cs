using Syed.PortfolioPlatform.Application.Auth.DTOs;
using Syed.PortfolioPlatform.Application.DTOs.Auth;

public interface IAuthService
{
    /// <summary>
    /// Login using username and password, returns JWT + refresh token
    /// </summary>
    /// <param name="username">Admin username</param>
    /// <param name="password">Admin password</param>
    /// <returns>AuthResponse with AccessToken, RefreshToken, ExpiresAt</returns>
    Task<AuthResponse> LoginAsync(string username, string password);

    /// <summary>
    /// Refreshes an existing access token using a valid refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token string</param>
    /// <returns>AuthResponse with new AccessToken and expiration</returns>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
}
