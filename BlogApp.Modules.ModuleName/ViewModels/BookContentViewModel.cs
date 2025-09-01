using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BlogApp.Core.Constants;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;
using BlogApp.Services.Interfaces;
using ControlzEx.Standard;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class BookContentViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        private readonly IArticleService _articleService;

        #endregion

        #region 属性

        private int _currentBookId;

        public bool KeepAlive => true;

        private ObservableCollection<Article>? list;
        /// <summary>
        /// 章节列表
        /// </summary>
        public ObservableCollection<Article>? List
        {
            get { return list; }
            set { SetProperty(ref list, value); }
        }

        private Article _SelectedArticle;

        public Article SelectedArticle
        {
            get { return _SelectedArticle; }
            set { SetProperty(ref _SelectedArticle, value); }
        }


        #endregion

        #region 命令

        public DelegateCommand AddCommand { get; set; }

        public DelegateCommand<Article> ArticleSelectedCommand { get; private set; }


        public DelegateCommand<Article> EditCommand { get; private set; }
        public DelegateCommand<Article> MoveUpCommand { get; private set; }
        public DelegateCommand<Article> MoveDownCommand { get; private set; }
        public DelegateCommand<Article> DeleteCommand { get; private set; }

        #endregion

        #region 函数

        public BookContentViewModel(IRegionManager regionManager, IArticleService articleService) : base(regionManager)
        {
            _regionManager = regionManager;
            _articleService = articleService;

            AddCommand = new DelegateCommand(AddMethod);

            ArticleSelectedCommand = new DelegateCommand<Article>(article =>
            {
                if (article != null)
                {
                    LoadArticleContentAsync(article.Id);
                }
            });

            EditCommand = new DelegateCommand<Article>(EditArticle);
            MoveUpCommand = new DelegateCommand<Article>(MoveArticleUp);
            MoveDownCommand = new DelegateCommand<Article>(MoveArticleDown);
            DeleteCommand = new DelegateCommand<Article>(DeleteArticle);

        }

        

        private void AddMethod()
        {
            var parameters = new NavigationParameters
            {
                {   "Article", 
                    new Article()
                    {
                        Id = 0,
                        BookId = _currentBookId,
                    } 
                }
            };

            _regionManager.RequestNavigate(RegionNames.ContentRegion, "ArticleEditorView", parameters);

        }

        /// <summary>
        /// 异步加载指定书目的文章列表
        /// </summary>
        private async void LoadArticlesAsync(int bookId)
        {
            try
            {
                // 调用Article服务获取文章列表（不包含Text字段以提高性能）
                var result = await _articleService.GetArticlesByBookIdAsync(bookId, false);
                if(result.IsSuccessful && result.Value != null)
                {
                    if(result.Value.Count == 0)
                    {
                        SetMessage($"文章数量为{result.Value.Count}");
                    }
                    else if (result.Value.Count > 0)
                    {
                        // 更新数据
                        List = result.Value;
                        SetMessage($"文章数量为{result.Value.Count}");
                    }
                }
                else
                {
                    SetMessage($"加载文章列表失败: {result.Code}");
                }
            }
            catch (Exception ex)
            {
                SetMessage($"加载文章列表时发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 异步加载选中的文章内容
        /// </summary>
        private async void LoadArticleContentAsync(int articleId)
        {
            if (articleId <= 0) return;
            try
            {
                // 调用通用数据库服务获取完整的文章内容（包括Text字段）
                var result = await _articleService.Get(articleId);
                if (result.Value.Any())
                {
                    SelectedArticle = result.Value.First();
                }
                else
                {
                    throw new Exception("无法加载文章内容");
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message);
            }
        }

        private async void EditArticle(Article article)
        {
            try
            {
                // 调用通用数据库服务获取完整的文章内容（包括Text字段）
                var result = await _articleService.Get(article.Id);
                if (result.Value.Any())
                {
                    article= result.Value.First();
                    if (article != null)
                    {
                        var parameters = new NavigationParameters
                    {
                        { "Article", article }
                    };
                        _regionManager.RequestNavigate(RegionNames.ContentRegion, "ArticleEditorView", parameters);
                    }
                }
                else
                {
                    throw new Exception("无法加载文章内容");
                }
            }
            catch (Exception ex)
            {

                SetMessage(ex.Message);
            }
        }

        /// <summary>
        /// 文章上移
        /// </summary>
        private async void MoveArticleUp(Article article)
        {
            if (article == null || List == null || List.Count <= 1) return;

            int currentIndex = List.IndexOf(article);
            if (currentIndex <= 0) return; // 已经是第一个，不能上移

            try
            {
                // 调用服务交换文章位置
                var result = await _articleService.SwapArticlesAsync(article.Id, List[currentIndex - 1].Id);
                if (result.IsSuccessful)
                {
                    // 更新本地列表
                    List.Move(currentIndex, currentIndex - 1);
                    SetMessage("文章已上移");
                }
                else
                {
                    SetMessage($"上移失败: {result.Code}");
                }
            }
            catch (Exception ex)
            {
                SetMessage($"上移失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 文章下移
        /// </summary>
        private async void MoveArticleDown(Article article)
        {
            if (article == null || List == null || List.Count <= 1) return;

            int currentIndex = List.IndexOf(article);
            if (currentIndex >= List.Count - 1) return; // 已经是最后一个，不能下移

            try
            {
                // 调用服务交换文章位置
                var result = await _articleService.SwapArticlesAsync(article.Id, List[currentIndex + 1].Id);
                if (result.IsSuccessful)
                {
                    // 更新本地列表
                    List.Move(currentIndex, currentIndex + 1);
                    SetMessage("文章已下移");
                }
                else
                {
                    SetMessage($"下移失败: {result.Code}");
                }
            }
            catch (Exception ex)
            {
                SetMessage($"下移失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        private async void DeleteArticle(Article article)
        {
            if (article == null || List == null) return;

            // 确认删除
            var result = MessageBox.Show($"确定要删除文章 '{article.Title}' 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                // 调用服务删除文章
                var deleteResult = await _articleService.DeleteArticleAsync(article.Id);
                if (deleteResult.IsSuccessful)
                {
                    // 从本地列表中移除
                    List.Remove(article);
                    SetMessage("文章已删除");

                    // 如果删除的是当前选中的文章，清空内容显示
                    if (SelectedArticle?.Id == article.Id)
                    {
                        SelectedArticle = new Article();
                    }
                }
                else
                {
                    SetMessage($"删除失败: {deleteResult.Code}");
                }
            }
            catch (Exception ex)
            {
                SetMessage($"删除失败: {ex.Message}");
            }
        }

        // 当导航到该视图时调用
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            // 从导航参数中获取BookId
            if (navigationContext.Parameters.ContainsKey("BookId")) 
            {
                _currentBookId = navigationContext.Parameters.GetValue<int>("BookId");
                // 加载该BookId下的文章列表
                LoadArticlesAsync(_currentBookId);
            }
            SelectedArticle = new Article();
        }

        #endregion
    }
}
