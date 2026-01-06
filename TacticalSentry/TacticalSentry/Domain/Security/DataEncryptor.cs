using System;
using System.Text;

namespace TacticalSentry.Domain.Security
{
    public static class DataEncryptor
    {
        public static string EncryptString(string plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }
    }
}