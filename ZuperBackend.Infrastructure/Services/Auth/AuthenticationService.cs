using ZuperBackend.Application.DTOs.Auth;
using ZuperBackend.Application.Services.Auth;
using ZuperBackend.Infrastructure.Persistence;

namespace ZuperBackend.Infrastructure.Services.Auth;

public class AuthenticationService : IAuthenticationService
{
    private readonly ZuperBackendDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticationService(ZuperBackendDbContext dbContext, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request)
    {
        // Buscar el usuario por email
        var user = _dbContext.Users
            .FirstOrDefault(u => u.Email == request.Email && !u.IsDeleted);

        if (user == null)
        {
            return null;
        }

        // Validar contraseña
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return null;
        }

        // Generar tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.Role,
            user.TenantId
        );

        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Guardar el refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        // Devolver respuesta
        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtTokenService.GetAccessTokenExpirationSeconds(),
            User = new UserInfoDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                TenantId = user.TenantId
            }
        };
    }

    /// <summary>
    /// Verifica una contraseña contra su hash (PBKDF2)
    /// </summary>
    private bool VerifyPassword(string password, string hash)
    {
        try
        {
            var parts = hash.Split('$');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = Convert.FromBase64String(parts[1]);

            using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(
                password,
                salt,
                10000,
                System.Security.Cryptography.HashAlgorithmName.SHA256))
            {
                var computedHash = pbkdf2.GetBytes(32);
                return computedHash.SequenceEqual(storedHash);
            }
        }
        catch
        {
            return false;
        }
    }
}
