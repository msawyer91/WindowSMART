using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Cryptography
{
    public sealed class DoubleEncryptor
    {
        private SHA256CryptoServiceProvider hashSha256;
        private AesCryptoServiceProvider passwordProvider;
        private AesCryptoServiceProvider doubleEncryptor;
        private byte[] byteHash, byteHash2, byteBuffer;

        /// <summary>
        /// Creates a Double Encryptor using predefined passwords.
        /// </summary>
        public DoubleEncryptor()
        {
            hashSha256 = new SHA256CryptoServiceProvider();
            byteHash = hashSha256.ComputeHash(UTF8Encoding.UTF8.GetBytes("Taryn10Is28Pretty07"));
            byteHash2 = hashSha256.ComputeHash(UTF8Encoding.UTF8.GetBytes("Joshua04Is28Handsome10"));
            hashSha256.Clear();
            hashSha256 = null;

            passwordProvider = new AesCryptoServiceProvider();
            passwordProvider.Key = byteHash;
            passwordProvider.Mode = CipherMode.CBC;

            doubleEncryptor = new AesCryptoServiceProvider();
            doubleEncryptor.Key = byteHash2;
            doubleEncryptor.Mode = CipherMode.ECB;

            byteHash = null;
            byteHash2 = null;
        }

        /// <summary>
        /// Creates a Double Encryptor using passwords that you specify.
        /// </summary>
        /// <param name="firstPassword">Password for the first level of encryption.</param>
        /// <param name="secondPassword">Password for the second level of encryption.</param>
        public DoubleEncryptor(String firstPassword, String secondPassword)
        {
            hashSha256 = new SHA256CryptoServiceProvider();
            byteHash = hashSha256.ComputeHash(UTF8Encoding.UTF8.GetBytes(firstPassword));
            byteHash2 = hashSha256.ComputeHash(UTF8Encoding.UTF8.GetBytes(secondPassword));
            hashSha256.Clear();
            hashSha256 = null;

            passwordProvider = new AesCryptoServiceProvider();
            passwordProvider.Key = byteHash;
            passwordProvider.Mode = CipherMode.CBC;

            doubleEncryptor = new AesCryptoServiceProvider();
            doubleEncryptor.Key = byteHash2;
            doubleEncryptor.Mode = CipherMode.ECB;

            byteHash = null;
            byteHash2 = null;
        }

        /// <summary>
        /// Sets an Initialization Vector for the CBC encryption.
        /// </summary>
        /// <param name="component">String to create the IV.</param>
        public void SetCbcRequired(String component)
        {
            byte[] required = UTF8Encoding.UTF8.GetBytes(component);
            passwordProvider.IV = required;
            required = null;
        }

        /// <summary>
        /// Encrypts the specified plaintext, returning the ciphertext.
        /// </summary>
        /// <param name="plainText">Plaintext to encrypt.</param>
        /// <returns>Ciphertext.</returns>
        public String Encrypt(String plainText)
        {
            byteBuffer = UTF8Encoding.UTF8.GetBytes(plainText);
            String intermediate = Convert.ToBase64String(passwordProvider.CreateEncryptor().TransformFinalBlock(
                byteBuffer, 0, byteBuffer.Length));
            byteBuffer = UTF8Encoding.UTF8.GetBytes(intermediate);
            return Convert.ToBase64String(doubleEncryptor.CreateEncryptor().TransformFinalBlock(
                byteBuffer, 0, byteBuffer.Length));
        }

        /// <summary>
        /// Attempts to decrypt the specified ciphertext, returning the plaintext. If the password(s) used when the
        /// DoubleEncryptor was constructed are wrong, this method will throw an exception.
        /// </summary>
        /// <param name="aes">Ciphertext to decrypt.</param>
        /// <returns>Plaintext.</returns>
        public String Decrypt(String aes)
        {
            byteBuffer = Convert.FromBase64String(aes);
            String intermediate = UTF8Encoding.UTF8.GetString(doubleEncryptor.CreateDecryptor().TransformFinalBlock(
                byteBuffer, 0, byteBuffer.Length));
            byteBuffer = Convert.FromBase64String(intermediate);
            return UTF8Encoding.UTF8.GetString(passwordProvider.CreateDecryptor().TransformFinalBlock(
                byteBuffer, 0, byteBuffer.Length));
        }
    }
}
