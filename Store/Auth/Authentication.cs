using Microsoft.IdentityModel.Tokens;
using Store.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Store.Auth;

public class Authentication
{
    /// <summary>
    /// Verifies a provided password against a stored hash.
    /// </summary>
    /// <param name="providedPassword">The password provided by the user.</param>
    /// <param name="storedHash">The hashed password stored in the database.</param>
    /// <returns>True if the provided password matches the stored hash, otherwise false.</returns>
    public static bool VerifyPassword(string providedPassword, string storedHash)
    {
        // Convert the stored hash from Base64
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        // Extract the salt from the hash
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        // Hash the provided password with the extracted salt
        var pbkdf2 = new Rfc2898DeriveBytes(providedPassword, salt, 10000);
        byte[] computedHash = pbkdf2.GetBytes(20);

        // Compare the computed hash with the stored hash
        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != computedHash[i])
                return false;
        }
        return true;
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the JWT token is to be generated.</param>
    /// <param name="configuration">App configuration containing JWT settings.</param>
    /// <returns>The JWT token.</returns>
    public static string GenerateJwtToken(User user, IConfiguration configuration)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["JwtSettings:DurationInMinutes"])),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = configuration["JwtSettings:Issuer"],
            Audience = configuration["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Hashes a password for storage using PBKDF2.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    public static string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Use the salt to hash the password
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        // Combine the salt and hashed password for storage
        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Validates a JWT token's integrity and expiration.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <param name="configuration">App configuration containing JWT settings.</param>
    /// <returns>True if the token is valid, otherwise false.</returns>
    public static bool ValidateJwtToken(string token, IConfiguration configuration)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.ASCII.GetBytes(configuration["JwtSettings:Key"]); // Assuming your secret is stored in Jwt:Key in your configuration.

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // if you want no tolerance on token's expiration 
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            // Additional custom validation if necessary. For instance:
            // Ensure that the token has an expiration claim and it's still valid
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return true;
        }
        catch
        {
            // Log exception if needed
            return false;
        }
    }
}
