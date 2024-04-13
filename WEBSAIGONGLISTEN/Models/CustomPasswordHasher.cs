using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class CustomPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly PasswordHasherCompatibilityMode _compatibilityMode;

    public CustomPasswordHasher(IOptions<PasswordHasherOptions> optionsAccessor = null)
    {
        _compatibilityMode = optionsAccessor?.Value?.CompatibilityMode ?? PasswordHasherCompatibilityMode.IdentityV3;
    }

    public string HashPassword(TUser user, string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return ConvertToBase64Url(hashedBytes);
        }
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        if (_compatibilityMode == PasswordHasherCompatibilityMode.IdentityV2)
        {
            // Nếu bạn muốn hỗ trợ chế độ tương thích với Identity V2
            // Bạn có thể thêm mã xác thực tại đây
        }

        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = ConvertFromBase64Url(hashedPassword);
            var providedHashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(providedPassword));
            if (hashedBytes.Length != providedHashedBytes.Length)
            {
                return PasswordVerificationResult.Failed;
            }
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                if (hashedBytes[i] != providedHashedBytes[i])
                {
                    return PasswordVerificationResult.Failed;
                }
            }
            return PasswordVerificationResult.Success;
        }
    }

    private string ConvertToBase64Url(byte[] bytes)
    {
        return WebEncoders.Base64UrlEncode(bytes);
    }

    private byte[] ConvertFromBase64Url(string base64Url)
    {
        return WebEncoders.Base64UrlDecode(base64Url);
    }
}
