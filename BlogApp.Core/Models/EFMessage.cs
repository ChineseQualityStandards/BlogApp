using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Core.Models 
{
    /// <summary>
    /// EF Core返回状态信息
    /// </summary>
    public class EFMessage<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccessful { get; set; }
        /// <summary>
        /// 状态信息
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 返回实体
        /// </summary>
        public T? Value{ get; set; }

        public EFMessage(bool isSuccessful,T t) : this(isSuccessful, "", t)
        {
            
        }

        public EFMessage(bool isSuccessful, string code, T t) 
        {
            IsSuccessful = isSuccessful;
            Code = code;
            Value = t;
        }
    }
}
