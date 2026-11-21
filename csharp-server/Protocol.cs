// Imports
using System;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using System.Buffers.Binary;

// Protocol Class
public static class Protocol
{
    // Public Shared key
    public static readonly byte[] SharedKey = Encoding.UTF8.GetBytes("12345678901234567890123456789012");

    // Private Helper
    private static byte[]? ReadExact(NetworkStream stream, int length)
    {
        byte[] buffer = new byte[length];
        int totalRead = 0;
        while (totalRead < length)
        {
            int bytesRead = stream.Read(buffer, totalRead, length - totalRead);
            if (bytesRead == 0)
            {
                return null;
            }
            totalRead += bytesRead;
        }
        return buffer;
    }

    // Public Method Send Encrypted Message
    public static void SendEncryptedMessage(NetworkStream stream, string plaintext, byte[] key)
    {
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);
        byte[] ciphertext = new byte[plaintextBytes.Length];
        byte[] tag = new byte[16];
        using AesGcm aesGcm = new AesGcm(key, tagSizeInBytes: 16);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
        int payloadLen = 12 + 16 + ciphertext.Length;
        byte[] packet = new byte[4 + payloadLen];
        BinaryPrimitives.WriteUInt32BigEndian(packet.AsSpan(0, 4), (uint)payloadLen);
        nonce.CopyTo(packet.AsSpan(4, 12));
        tag.CopyTo(packet.AsSpan(16, 16));
        ciphertext.CopyTo(packet.AsSpan(32));
        stream.Write(packet, 0, packet.Length);
    }

    // Public Method Recieve Encrypted Message
    public static string? RecieveEncryptedMessage(NetworkStream stream, byte[] key)
    {
        byte[]? header = new byte[4];
        header = ReadExact(stream, 4);
        if (header == null)
        {
            return null;
        }
        uint payloadLen = BinaryPrimitives.ReadUInt32BigEndian(header);
        byte[]? payload = new byte[(int)payloadLen];
        payload = ReadExact(stream, (int)payloadLen);
        if (payload == null)
        {
            return null;
        }
        ReadOnlySpan<byte> payloadSpan = payload;
        ReadOnlySpan<byte> nonce = payloadSpan.Slice(0, 12);
        ReadOnlySpan<byte> tag = payloadSpan.Slice(12, 16);
        ReadOnlySpan<byte> ciphertext = payloadSpan.Slice(28);
        byte [] decryptedBytes = new byte[ciphertext.Length];
        try
        {
            using AesGcm aesGcm = new AesGcm(key, tagSizeInBytes: 16);
            aesGcm.Decrypt(nonce, ciphertext, tag, decryptedBytes);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (CryptographicException)
        {
            Console.WriteLine("[INTEGRITY ALERT] Authentication tag mismatch! Message was tempered with or corrupted.");
            return "[CORRUPTED MESSAGE - REJECTED]";
        }
    }
}
