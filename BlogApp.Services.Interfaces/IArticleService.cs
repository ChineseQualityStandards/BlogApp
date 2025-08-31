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
    /// 文章服务接口
    /// </summary>
    public interface IArticleService : IDatabaseService<Article>
    {
        /// <summary>
        /// 获取指定书目的文章列表（按OrderIndex排序）
        /// </summary>
        Task<EFMessage<ObservableCollection<Article>>> GetArticlesByBookIdAsync(int bookId, bool includeText = false);

        /// <summary>
        /// 添加文章并自动设置OrderIndex
        /// </summary>
        Task<EFMessage<Article>> AddArticleAsync(Article article, int? insertAfterId = null);

        /// <summary>
        /// 移动文章到新位置
        /// </summary>
        Task<EFMessage<bool>> MoveArticleAsync(int articleId, int newPosition);

        /// <summary>
        /// 交换两篇文章的位置
        /// </summary>
        Task<EFMessage<bool>> SwapArticlesAsync(int articleId1, int articleId2);

        /// <summary>
        /// 删除文章并重新排序
        /// </summary>
        Task<EFMessage<Article>> DeleteArticleAsync(int articleId);

        /// <summary>
        /// 重新排序指定书目下的所有文章
        /// </summary>
        Task<EFMessage<bool>> ReorderArticlesAsync(int bookId);
    }
}
