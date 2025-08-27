using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.Models;

namespace BlogApp.Services
{
    /// <summary>
    /// GlobalEventService<T> 是一个泛型类，T 表示事件数据的类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GlobalEventService<T>
    {
        /// <summary>
        /// Lazy<T>：确保实例只在第一次访问时创建，实现延迟初始化 
        /// 线程安全：Lazy<T> 默认是线程安全的 
        /// 单例访问：通过 Instance 属性获取唯一实例
        /// </summary>
        private static readonly Lazy<GlobalEventService<T>> _instance =
            new Lazy<GlobalEventService<T>>(() => new GlobalEventService<T>());

        public static GlobalEventService<T> Instance => _instance.Value;
        /// <summary>
        /// Action<T>：接受一个类型为 T 参数的委托
        /// 可为空事件：? 表示事件可能没有订阅者
        /// </summary>
        public event Action<T>? EventIn;
        /// <summary>
        /// 事件发布方法
        /// </summary>
        /// <param name="t">事件数据</param>
        public void PublishEventIn(T t)
        {
            // 空条件操作符：?. 确保只有在有订阅者时才调用 Invoke
            // 避免空引用异常：如果没有订阅者，不会抛出异常
            EventIn?.Invoke(t);
        }
    }
}
