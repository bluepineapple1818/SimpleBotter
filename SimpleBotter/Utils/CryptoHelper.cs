using System;
using System.Security.Cryptography;
using System.Text;

namespace SimpleBotter.Utils
{
    public static class CryptoHelper
    {
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
        
        public static byte[] GenerateSharedSecret()
        {
            byte[] secret = new byte[16];
            rng.GetBytes(secret);
            return secret;
        }
        
        public static string DecryptRSA(byte[] encryptedData, string pemPrivateKey)
        {
            // Simplified stub — real implementation would use RSA key
            return Encoding.UTF8.GetString(encryptedData);
        }
        
        public static byte[] EncryptRSA(byte[] data, byte[] publicKeyDer)
        {
            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(publicKeyDer, out _);
            return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }
    }
}