using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.DbContexts;
using BlogApp.Core.Models;
using BlogApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services
{
    /// <summary>
    /// 文章服务实现
    /// </summary>
    public class ArticleService : DatabaseService<Article>, IArticleService
    {
        private readonly BlogAppContext _context;

        public ArticleService(BlogAppContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取指定书目的文章列表（按OrderIndex排序）
        /// </summary>
        public async Task<EFMessage<ObservableCollection<Article>>> GetArticlesByBookIdAsync(int bookId, bool includeText = false)
        {
            try
            {
                IQueryable<Article> query = _context.Articles
                    .Where(a => a.BookId == bookId)
                    .OrderBy(a => a.OrderIndex)
                    .ThenBy(a => a.CreatedDate);

                // 根据参数决定是否包含Text字段
                if (!includeText)
                {
                    query = query.Select(a => new Article
                    {
                        Id = a.Id,
                        BookId = a.BookId,
                        OrderIndex = a.OrderIndex,
                        Title = a.Title,
                        CreatedDate = a.CreatedDate,
                        UpdatedDate = a.UpdatedDate
                    });
                }

                var articles = await query.ToListAsync();
                var collection = new ObservableCollection<Article>(articles);

                return new EFMessage<ObservableCollection<Article>>(true, "查询成功", collection);
            }
            catch (Exception ex)
            {
                return new EFMessage<ObservableCollection<Article>>(false, $"查询失败: {ex.Message}", null);
            }
        }

        /// <summary>
        /// 添加文章并自动设置OrderIndex
        /// </summary>
        public async Task<EFMessage<Article>> AddArticleAsync(Article article, int? insertAfterId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 设置创建和更新时间
                article.CreatedDate = DateTime.UtcNow;
                article.UpdatedDate = DateTime.UtcNow;

                // 处理OrderIndex
                if (insertAfterId.HasValue)
                {
                    // 插入到指定文章之后
                    var afterArticle = await _context.Articles.FindAsync(insertAfterId.Value);
                    if (afterArticle != null)
                    {
                        // 获取插入位置之后的所有文章
                        var articlesToUpdate = await _context.Articles
                            .Where(a => a.BookId == article.BookId && a.OrderIndex > afterArticle.OrderIndex)
                            .OrderBy(a => a.OrderIndex)
                            .ToListAsync();

                        // 设置新文章的OrderIndex
                        article.OrderIndex = afterArticle.OrderIndex + 1;

                        // 更新后续文章的OrderIndex
                        foreach (var art in articlesToUpdate)
                        {
                            art.OrderIndex += 1;
                            art.UpdatedDate = DateTime.UtcNow;
                            _context.Articles.Update(art);
                        }
                    }
                    else
                    {
                        // 如果指定文章不存在，添加到末尾
                        await SetArticleOrderToEnd(article);
                    }
                }
                else
                {
                    // 添加到末尾
                    await SetArticleOrderToEnd(article);
                }

                // 添加到数据库
                var result = await base.Add(article);

                if (result.IsSuccessful)
                {
                    await transaction.CommitAsync();
                    return result;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new EFMessage<Article>(false, "添加失败", null);
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new EFMessage<Article>(false, $"添加失败: {ex.Message}", null);
            }
        }

        /// <summary>
        /// 移动文章到新位置
        /// </summary>
        public async Task<EFMessage<bool>> MoveArticleAsync(int articleId, int newPosition)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var article = await _context.Articles.FindAsync(articleId);
                if (article == null)
                    return new EFMessage<bool>(false, "文章不存在", false);

                // 获取该书目下的所有文章
                var articles = await _context.Articles
                    .Where(a => a.BookId == article.BookId)
                    .OrderBy(a => a.OrderIndex)
                    .ToListAsync();

                // 找到当前文章的位置
                int currentIndex = articles.FindIndex(a => a.Id == articleId);
                if (currentIndex == -1)
                    return new EFMessage<bool>(false, "文章不在该书目中", false);

                // 确保新位置有效
                newPosition = Math.Max(0, Math.Min(newPosition, articles.Count - 1));

                if (currentIndex == newPosition)
                    return new EFMessage<bool>(true, "位置未改变", true);

                // 从列表中移除当前文章
                var movedArticle = articles[currentIndex];
                articles.RemoveAt(currentIndex);

                // 插入到新位置
                articles.Insert(newPosition, movedArticle);

                // 重新分配OrderIndex
                for (int i = 0; i < articles.Count; i++)
                {
                    articles[i].OrderIndex = i;
                    articles[i].UpdatedDate = DateTime.UtcNow;
                    _context.Articles.Update(articles[i]);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new EFMessage<bool>(true, "移动成功", true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new EFMessage<bool>(false, $"移动失败: {ex.Message}", false);
            }
        }

        /// <summary>
        /// 交换两篇文章的位置
        /// </summary>
        public async Task<EFMessage<bool>> SwapArticlesAsync(int articleId1, int articleId2)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var article1 = await _context.Articles.FindAsync(articleId1);
                var article2 = await _context.Articles.FindAsync(articleId2);

                if (article1 == null || article2 == null)
                    return new EFMessage<bool>(false, "文章不存在", false);

                if (article1.BookId != article2.BookId)
                    return new EFMessage<bool>(false, "文章不在同一书目中", false);

                // 交换OrderIndex
                int temp = article1.OrderIndex;
                article1.OrderIndex = article2.OrderIndex;
                article2.OrderIndex = temp;

                article1.UpdatedDate = DateTime.UtcNow;
                article2.UpdatedDate = DateTime.UtcNow;

                _context.Articles.Update(article1);
                _context.Articles.Update(article2);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new EFMessage<bool>(true, "交换成功", true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new EFMessage<bool>(false, $"交换失败: {ex.Message}", false);
            }
        }

        /// <summary>
        /// 删除文章并重新排序
        /// </summary>
        public async Task<EFMessage<Article>> DeleteArticleAsync(int articleId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var article = await _context.Articles.FindAsync(articleId);
                if (article == null)
                    return new EFMessage<Article>(false, "文章不存在", null);

                int bookId = article.BookId;
                int deletedOrderIndex = article.OrderIndex;

                // 删除文章
                var result = await base.Delete(article);

                if (result.IsSuccessful)
                {
                    // 重新排序剩余文章
                    var remainingArticles = await _context.Articles
                        .Where(a => a.BookId == bookId && a.OrderIndex > deletedOrderIndex)
                        .OrderBy(a => a.OrderIndex)
                        .ToListAsync();

                    foreach (var art in remainingArticles)
                    {
                        art.OrderIndex -= 1;
                        art.UpdatedDate = DateTime.UtcNow;
                        _context.Articles.Update(art);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new EFMessage<Article>(true, "删除成功", article);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new EFMessage<Article>(false, $"删除失败: {ex.Message}", null);
            }
        }

        /// <summary>
        /// 重新排序指定书目下的所有文章
        /// </summary>
        public async Task<EFMessage<bool>> ReorderArticlesAsync(int bookId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var articles = await _context.Articles
                    .Where(a => a.BookId == bookId)
                    .OrderBy(a => a.OrderIndex)
                    .ThenBy(a => a.CreatedDate)
                    .ToListAsync();

                for (int i = 0; i < articles.Count; i++)
                {
                    articles[i].OrderIndex = i;
                    articles[i].UpdatedDate = DateTime.UtcNow;
                    _context.Articles.Update(articles[i]);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new EFMessage<bool>(true, "重新排序成功", true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new EFMessage<bool>(false, $"重新排序失败: {ex.Message}", false);
            }
        }

        /// <summary>
        /// 设置文章的OrderIndex为该书目的末尾
        /// </summary>
        private async Task SetArticleOrderToEnd(Article article)
        {
            // 获取当前最大的OrderIndex值
            int maxOrderIndex = await _context.Articles
                .Where(a => a.BookId == article.BookId)
                .MaxAsync(a => (int?)a.OrderIndex) ?? -1;

            // 设置新文章的OrderIndex
            article.OrderIndex = maxOrderIndex + 1;
        }
    }
}
