using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace RelatedWordsAPI.App.Helpers
{
    public static class PasswordHash
    {
        internal static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>Salted and hashed password.</returns>
        public static string Hash(string password, string salt)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] passwordSalt = Encoding.UTF8.GetBytes($"{password}{salt}");
                byte[] saltedHashedBytes = mySHA256.ComputeHash(passwordSalt);
                string saltedHashedPass = Convert.ToBase64String(saltedHashedBytes);
                return saltedHashedPass;
            }
        }

        /// <summary>
        /// Returns Tuple (salt, hashed password).
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Tuple (salt, hashed password)</returns>
        public static Tuple<string, string> GenerateSaltAndHash(string password)
        {
            string salt = GetUniqueKey(32);
            return new Tuple<string, string>(salt, Hash(password, salt));
        }

        public static string GetUniqueKey(int size)
        {
            byte[] data = new byte[4 * size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }
    }
}
