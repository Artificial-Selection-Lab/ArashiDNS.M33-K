﻿using System.IO.Compression;
using System.Text;

namespace ArashiDNS.M3
{
    internal class Table
    {
        public static byte[] ConfuseBytes(byte[] plaintext, string key)
        {

            var result = new byte[plaintext.Length];
            key = key.Trim().ToUpper();
            var keyIndex = 0;

            for (var i = 0; i < plaintext.Length; i++)
            {
                keyIndex %= key.Length;
                result[i] = (byte) ((plaintext[i] + (key[keyIndex] - 65)) % 256);
                keyIndex++;
            }

            return result;
        }

        public static byte[] DeConfuseBytes(byte[] ciphertext, string key)
        {
            var result = new byte[ciphertext.Length];
            key = key.Trim().ToUpper();
            var keyIndex = 0;

            for (var i = 0; i < ciphertext.Length; i++)
            {
                keyIndex %= key.Length;
                result[i] = (byte) ((ciphertext[i] + 256 - (key[keyIndex] - 65)) % 256);
                keyIndex++;
            }

            return result;
        }

        public static string ConfuseString(string plaintext, string key)
        {
            var ciphertext = "";
            var origin = Encoding.ASCII.GetBytes(plaintext.ToUpper());
            var keys = Encoding.ASCII.GetBytes(key.ToUpper());

            for (var i = 0; i < origin.Length; i++)
            {
                int asciiCode = origin[i];
                asciiCode = asciiCode + keys[i % keys.Length] - 'A';

                if (asciiCode > 'Z') asciiCode -= 26;
                var byteArray = new[] {(byte) asciiCode};
                var strCharacter = new ASCIIEncoding().GetString(byteArray);
                ciphertext += strCharacter;
            }

            return ciphertext;
        }

        public static string DeConfuseString(string ciphertext, string key)
        {
            var plaintext = "";
            var origin = Encoding.ASCII.GetBytes(ciphertext.ToUpper());
            var keys = Encoding.ASCII.GetBytes(key.ToUpper());

            for (var i = 0; i < origin.Length; i++)
            {

                int asciiCode = origin[i];
                asciiCode = asciiCode - keys[i % keys.Length] + 'A';
                if (asciiCode < 'A') asciiCode += 26;
                var byteArray = new[] {(byte) asciiCode};
                var strCharacter = new ASCIIEncoding().GetString(byteArray);

                plaintext += strCharacter;
            }

            return plaintext;
        }
    }

    internal class BrotliCompress
    {
        public static byte[] Compress(byte[] bytes)
        {
            using var memoryStream = new MemoryStream();
            using (var brotliStream = new BrotliStream(memoryStream, CompressionLevel.Optimal))
                brotliStream.Write(bytes, 0, bytes.Length);
            return memoryStream.ToArray();
        }

        public static byte[] Decompress(byte[] bytes)
        {
            using var memoryStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            using (var decompressStream = new BrotliStream(memoryStream, CompressionMode.Decompress))
                decompressStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
}
