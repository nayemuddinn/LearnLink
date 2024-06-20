using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace LearnLink.Controllers
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;

        public static string HashPassword(string password)
        {
        
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

     
            string base64Hash = Convert.ToBase64String(hashBytes);


            return string.Format("$HASH|V1${0}", base64Hash);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Extract the bytes
            string[] split = hashedPassword.Replace("$HASH|V1$", "").Split('$');
            byte[] hashBytes = Convert.FromBase64String(split[0]);

   
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}