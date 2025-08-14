using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Services.Interfaces
{
    public interface IEncryptService
    {
        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="password">密码明文</param>
        /// <param name="saltTime">加盐时间</param>
        /// <returns>加密后的密码</returns>
        string EncryptPassword(string password, DateTime saltTime);
        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="inputPassword">输入的密码</param>
        /// <param name="storedPassword">已加密的密码</param>
        /// <param name="saltTime">加盐时间</param>
        /// <returns>密码是否正确</returns>
        bool VerifyPassword(string inputPassword, string storedPassword, DateTime saltTime);
    }
}
