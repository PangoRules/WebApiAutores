using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiAutores.DTOs;

namespace WebApiAutores.Services
{
    public class HashService
    {
        public HashResultDto Hash(string plainText)
        {
            var sal = new byte[16];

            using(var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(sal);
            }

            return Hash(plainText, sal);
        }

        public HashResultDto Hash(string plainText, byte[] sal)
        {
            var derivedKey = KeyDerivation.Pbkdf2(password: plainText,
                salt: sal,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 31);

            var hash = Convert.ToBase64String(derivedKey);

            return new HashResultDto()
            {
                Hash = hash,
                Sal = sal,
            };
        }
    }
}
