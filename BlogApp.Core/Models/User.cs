using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.Enums;

namespace BlogApp.Core.Models
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public User()
        {
            Email = string.Empty;
            Name = string.Empty;
            CreatedTime = DateTime.Now;
            LastLoginTime = DateTime.Now;
            PACTHash = string.Empty;
            PALLTHash = string.Empty;
            HeadPicture = string.Empty;
            Permission = Permission.User; // 假设默认权限
            Signature = string.Empty;
        }
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 账号创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }
        /// <summary>
        /// Password And Created Time 哈希字符串
        /// </summary>
        public string? PACTHash { get; set; }
        /// <summary>
        /// Password And Last Login Time 哈希字符串
        /// </summary>
        public string? PALLTHash { get; set; }
        /// <summary>
        /// 头像转化的字符串
        /// </summary>
        public string? HeadPicture { get; set; }
        /// <summary>
        /// 用户权限
        /// </summary>
        public Permission Permission { get; set; }
        /// <summary>
        /// 个性签名
        /// </summary>
        public string? Signature { get; set; }
    }
}
