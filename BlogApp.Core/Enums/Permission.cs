using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Core.Enums
{
    /// <summary>
    /// 权限
    /// </summary>
    public enum Permission
    {
        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 1,
        /// <summary>
        /// 普通用户
        /// </summary>
        User = 2,
        /// <summary>
        /// 临时用户
        /// </summary>
        Temp = 3
    }
}
