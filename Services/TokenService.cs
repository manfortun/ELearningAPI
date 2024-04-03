using eLearningApi.Enums;
using eLearningApi.Models;
using eLearningApi.UserRolesAndPermissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace eLearningApi.Services;

public class TokenService
{
    public static string GenerateTokenString(IConfiguration config, User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        string jwtKey = config["Jwt:SecretKey"] ?? throw new InvalidOperationException("No secret key in application settings.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"] ?? throw new InvalidOperationException("No issuer in application settings."),
            audience: config["Jwt:Audience"] ?? throw new InvalidOperationException("No audience in application settings."),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string? GetAuthToken(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var value))
        {
            // check prefix
            string prefix = value.ToString().Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
            if (prefix == JwtBearerDefaults.AuthenticationScheme)
            {
                return value.ToString().Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).LastOrDefault();
            }
            return default!;
        }

        return default!;
    }

    public static IEnumerable<Claim> GetClaims(HttpContext context)
    {
        string? token = GetAuthToken(context);
        if (!string.IsNullOrEmpty(token))
        {
            // separate the authentication scheme and the actual token
            string splitToken = token.Split(' ', 2)[0];

            var tokenHandler = new JwtSecurityTokenHandler();
            var parsedToken = tokenHandler.ReadJwtToken(splitToken);

            return parsedToken.Claims;
        }

        return default!;
    }

    public static string? GetId(HttpContext? context)
    {
        if (context is null)
        {
            return default!;
        }

        var claims = GetClaims(context);

        if (claims is null)
        {
            return default!;
        }

        var id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (id is null)
        {
            return default!;
        }

        return id;
    }

    public static Role? GetRole(HttpContext? context)
    {
        if (context is null)
        {
            return default!;
        }

        var claims = GetClaims(context);

        if (claims is null)
        {
            return default!;
        }

        string roleName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;

        if (Enum.TryParse(roleName, out Role role))
        {
            return role;
        }

        return default!;
    }

    public static Token CreateChangePasswordToken(string email)
    {
        return CreateToken(email, TokenType.PasswordReset);
    }

    public static Token CreateEmailVerificationToken(string email)
    {
        return CreateToken(email, TokenType.Verification);
    }

    private static Token CreateToken(string email, TokenType tokenType)
    {
        Token token = new Token();
        token.Email = email;
        token.IsExecuted = false;
        token.TokenType = tokenType.ToString();

        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[sizeof(uint)];
            rng.GetBytes(bytes);

            token.Value = Convert.ToBase64String(bytes);
        }

        return token;
    }
}
