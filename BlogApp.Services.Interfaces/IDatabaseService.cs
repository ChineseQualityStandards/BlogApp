using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.Models;

namespace BlogApp.Services.Interfaces
{
    /// <summary>
    /// 数据库操作服务接口
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public interface IDatabaseService<T>
    {
        /// <summary>
        /// 添加
        /// </summary>
        Task<EFMessage<T>> Add(T t);
        /// <summary>
        /// 删除
        /// </summary>
        Task<EFMessage<T>> Delete(T t);
        /// <summary>
        /// 查询
        /// </summary>
        Task<EFMessage<ObservableCollection<T>>> Get(int id);
        /// <summary>
        /// 查询
        /// </summary>
        Task<EFMessage<ObservableCollection<T>>> Get(string key, string condition);
        /// <summary>
        /// 更新
        /// </summary>
        Task<EFMessage<T>> Update(T t);
    }
}
