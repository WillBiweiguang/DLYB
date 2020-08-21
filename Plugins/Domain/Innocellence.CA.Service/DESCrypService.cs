using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Service
{
    public class DESCrypService
    {
        static string encryptKey = "Yjbl";
        private static byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        #region 加密字符串  
        public static string Encrypt(string str)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider(); 

            byte[] key = Encoding.Unicode.GetBytes(encryptKey);

            byte[] data = Encoding.Unicode.GetBytes(str);

            MemoryStream MStream = new MemoryStream(); 

            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length); 

            CStream.FlushFinalBlock(); 

            return Convert.ToBase64String(MStream.ToArray());
        }
        #endregion

        #region 解密字符串   
        public static string Decrypt(string str)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();

            byte[] key = Encoding.Unicode.GetBytes(encryptKey);

            byte[] data = Convert.FromBase64String(str);

            MemoryStream MStream = new MemoryStream();

            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);

            CStream.FlushFinalBlock();

            return Encoding.Unicode.GetString(MStream.ToArray()); 
        }
        #endregion
    }
}
