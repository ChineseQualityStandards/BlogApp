using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Services.Interfaces;

namespace BlogApp.Services
{
    public class EncryptService : IEncryptService
    {
        public string EncryptPassword(string password, DateTime saltTime)
        {
            // 将时间转换为盐值
            string salt = saltTime.ToString("yyyyMMddHHmmssfff");
            // 密码和盐值组成新的字符串
            string saltPassword = string.Concat(password, salt);
            // 使用SHA256进行哈希加密
            using (SHA256 sha256 = SHA256.Create())
            {
                // 加盐字符串转换成哈希字节数组
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltPassword));

                // 将字节数组转换成字符串
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) 
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool VerifyPassword(string inputPassword, string storedPassword, DateTime saltTime)
        {
            string encryptedInput = EncryptPassword(inputPassword, saltTime);
            return encryptedInput.Equals(storedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
