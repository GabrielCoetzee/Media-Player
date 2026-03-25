using System;
using System.Security.Cryptography;
using System.Text;

namespace Generic.Extensions
{
    public static class StringExtensions
    {
        public static string Encrypt(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            var encoding = new UTF8Encoding();
            byte[] plain = encoding.GetBytes(s);
            byte[] secret = ProtectedData.Protect(plain, null, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(secret);
        }
        public static string Decrypt(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            byte[] secret = Convert.FromBase64String(s);
            byte[] plain = ProtectedData.Unprotect(secret, null, DataProtectionScope.CurrentUser);
            var encoding = new UTF8Encoding();

            return encoding.GetString(plain);

        }

        public static string ReplaceTwoSucceedingNewLinesWithOne(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;

            if (s.Contains($"\n\n"))
                return s.Replace($"\n\n", Environment.NewLine);

            return s;
        }

        /// <summary>
        /// Detect if accent / color scheme was added during runtime with the "Auto Adjust Accent" feature
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsRuntimeAccent(this string s)
        {
            return s.StartsWith("#");
        }
    }
}
