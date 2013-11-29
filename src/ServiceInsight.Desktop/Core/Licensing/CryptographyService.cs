using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NServiceBus.Profiler.Desktop.Core.Licensing
{
    public interface ICryptoService
    {
        string Encrypt(string unencrypted);
        byte[] Encrypt(byte[] buffer);
        string Decrypt(string encrypted);
        byte[] Decrypt(byte[] buffer);
    }

    public class CryptographyService : ICryptoService
    {
        private static readonly byte[] Key = new byte[32]
        {
            234, 158, 249,  22,  57, 254,  12,  27, 249, 193,   3, 119, 196,   2,  87,  35,
             86, 204,  13, 205,  38, 242,  25, 207, 195, 210, 241, 248, 180, 224,  64,  24
        };

        private static readonly byte[] Vector = new byte[16]
        {
            140,  90, 106, 210,  70, 169,  49, 185,
            149, 185, 138, 247, 102,  26, 244, 157 
        };

        private readonly ICryptoTransform _encryptor;
        private readonly ICryptoTransform _decryptor;
        private readonly UTF8Encoding _encoder;

        public CryptographyService()
        {
            var rm = new RijndaelManaged();
            _encryptor = rm.CreateEncryptor(Key, Vector);
            _decryptor = rm.CreateDecryptor(Key, Vector);
            _encoder = new UTF8Encoding();
        }

        public string Encrypt(string unencrypted)
        {
            return Convert.ToBase64String(Encrypt(_encoder.GetBytes(unencrypted)));
        }

        public string Decrypt(string encrypted)
        {
            return _encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
        }

        public byte[] Encrypt(byte[] buffer)
        {
            return Transform(buffer, _encryptor);
        }

        public byte[] Decrypt(byte[] buffer)
        {
            return Transform(buffer, _decryptor);
        }

        protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            var ms = new MemoryStream(buffer.Length);

            try
            {
                using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                {
                    cs.Write(buffer, 0, buffer.Length);
                }

                return ms.ToArray();
            }
            finally
            {
                ms.Dispose();
            }
        }
    }
}