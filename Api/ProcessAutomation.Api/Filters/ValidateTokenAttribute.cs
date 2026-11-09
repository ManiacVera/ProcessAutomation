using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidateTokenAttribute : TypeFilterAttribute
{
    public ValidateTokenAttribute() : base(typeof(ValidateTokenFilter))
    {
    }

    private class ValidateTokenFilter : IAsyncActionFilter
    {
        private readonly IMemoryCache memorycache;
        // Must match exactly the 32-byte SECRET_KEY used in Rust
        private static readonly byte[] SecretKey = Encoding.UTF8.GetBytes("A1b2C3d4E5f6G7h8I9j0K1l2M3n4O5p6");

        public ValidateTokenFilter(IMemoryCache memorycache)
        {
            this.memorycache = memorycache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var skipValidation = context.ActionDescriptor.EndpointMetadata.Any(em => em is SkipTokenValidationAttribute);
            if (skipValidation)
            {
                await next();
                return;
            }
                
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Token", out var extractedTokenHeader) || 
                string.IsNullOrWhiteSpace(extractedTokenHeader))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token missing in headers." });
                return;
            }

            string encryptedTokenBase64 = extractedTokenHeader.ToString();
            string decryptedToken;

            try
            {
                decryptedToken = DecryptToken(encryptedTokenBase64);
                if (string.IsNullOrEmpty(decryptedToken))
                {
                    context.Result = new UnauthorizedObjectResult(new { message = "Invalid token format or decryption failed." });
                    return;
                }
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid or tampered token." });
                return;
            }

            if (!memorycache.TryGetValue(encryptedTokenBase64, out _))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token not found, expired, or already used." });
                return;
            }

            memorycache.Remove(encryptedTokenBase64);
            await next();
        }

        private string DecryptToken(string base64Ciphertext)
        {
            byte[] combinedBytes = Convert.FromBase64String(base64Ciphertext);

            const int nonceSize = 12;
            const int tagSize = 16;

            if (combinedBytes.Length < nonceSize + tagSize)
            {
                throw new ArgumentException("Encrypted token length is invalid.");
            }

            byte[] nonce = new byte[nonceSize];
            Buffer.BlockCopy(combinedBytes, 0, nonce, 0, nonceSize);

            byte[] tag = new byte[tagSize];
            Buffer.BlockCopy(combinedBytes, combinedBytes.Length - tagSize, tag, 0, tagSize);

            int ciphertextLength = combinedBytes.Length - nonceSize - tagSize;
            byte[] ciphertext = new byte[ciphertextLength];
            Buffer.BlockCopy(combinedBytes, nonceSize, ciphertext, 0, ciphertextLength);

            byte[] plaintextBytes = new byte[ciphertextLength];

            using (var aesGcm = new AesGcm(SecretKey, tagSize))
            {
                aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes);
            }

            return Encoding.UTF8.GetString(plaintextBytes);
        }
    }
}