// ==============================================================================================
// IMPORTS
// ==============================================================================================
using System;
using System.Text;
using System.Security.Cryptography;

// ==============================================================================================
// Program Class
// ==============================================================================================
class Program
{
    // ==========================================================================================
    // Shared Key Constant
    // ==========================================================================================
    static byte[] sharedKey = Encoding.UTF8.GetBytes("12345678901234567890123456789012");

    // ==========================================================================================
    // Encrypt Message Helper Function
    // ==========================================================================================
    static (byte[] nonce, byte[] ciphertext, byte[] tag) EncryptMessage(string plaintext, byte[] key)
    {
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        byte [] nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);
        byte[] ciphertext = new byte[plaintextBytes.Length];
        byte[] tag = new byte[16];
        using AesGcm aesGcm = new AesGcm(key, tagSizeInBytes: 16);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
        return (nonce, ciphertext, tag);
    }

    // ==========================================================================================
    // Decrypt Message Helper Function
    // ==========================================================================================
    static string DecryptMessage(byte[] nonce, byte[] ciphertext, byte[] tag, byte[] key)
    {
        byte[] decryptedBytes = new byte[ciphertext.Length];
        using AesGcm aesGcm = new AesGcm(key, tagSizeInBytes: 16);
        aesGcm.Decrypt(nonce, ciphertext, tag, decryptedBytes);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
    // ==========================================================================================
    // Main Execution Block
    // ==========================================================================================
    static void Main(string[] args)
    {
        string test = "Hello, Encrypted World from C#!";
        var (nonce, ciphertext, tag) = EncryptMessage(test, sharedKey);
        Console.WriteLine($"Nonce: Length = {nonce.Length}, hex string = {Convert.ToHexString(nonce)}");
        Console.WriteLine($"Cipher Text: Length = {ciphertext.Length}, hex string = {Convert.ToHexString(ciphertext)}");
        Console.WriteLine($"Tag: Length = {tag.Length}, hex string = {Convert.ToHexString(tag)}");
        string decryptedTest = DecryptMessage(nonce, ciphertext, tag, sharedKey);
        Console.WriteLine($"Decrypted Message: {decryptedTest}");
        // ======================================================================================
        // Tamper Test
        // ======================================================================================
        byte[] tampered = ciphertext.ToArray();
        tampered[0] ^= 0x01;
        try
        {
            string decrypted = DecryptMessage(nonce, tampered, tag, sharedKey);
            Console.WriteLine($"Decrypted: {decrypted}");
        }
        catch (CryptographicException)
        {
            Console.WriteLine("Integrity check passed: Tampered message rejected!");
        }
    }
}

