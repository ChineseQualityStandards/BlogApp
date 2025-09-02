using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using BlogApp.Core.AppSessions;
using BlogApp.Core.Constants;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;
using BlogApp.Services;
using BlogApp.Services.Interfaces;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class BookShelfViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        private readonly IDatabaseService<Book> _databaseService;

        private readonly IArticleService _articleService;

        #endregion

        #region 属性

        public bool KeepAlive => false;

        private ObservableCollection<Book>? bookList = new();

        public ObservableCollection<Book>? BookList
        {
            get { return bookList; }
            set { SetProperty(ref bookList, value); }
        }

        private Book? selectedBook = new Book();

        public Book? SelectedBook
        {
            get { return selectedBook; }
            set { SetProperty(ref selectedBook, value); }
        }

        #endregion

        #region 命令

        public DelegateCommand<int?> DeleteCommand { get; set; }
        public DelegateCommand<int?> EnterCommand { get; set; }
        public DelegateCommand<string> NavigatedToCommand { get; set; }
        public DelegateCommand<int?> ModifyCommand { get; set; }

        #endregion

        #region 函数

        public BookShelfViewModel(IRegionManager regionManager, IDatabaseService<Book> databaseService, IArticleService articleService) : base(regionManager)
        {
            _regionManager = regionManager;
            _databaseService = databaseService;
            _articleService = articleService;

            // 初始化命令
            DeleteCommand = new DelegateCommand<int?>(DeleteBook);
            EnterCommand = new DelegateCommand<int?>(EnterBook);
            NavigatedToCommand = new DelegateCommand<string>(NavigatedContentRegionTo);
            ModifyCommand = new DelegateCommand<int?>(ModifyBook);

        }

        /// <summary>
        /// 删除书籍
        /// </summary>
        private async void DeleteBook(int? bookId)
        {
            if (!bookId.HasValue)
            {
                SetMessage("无效的书籍ID");
                return;
            }
            //SelectedBook = _databaseService.Get(bookId.Value).Result.Value.FirstOrDefault();
            SelectedBook = BookList?.FirstOrDefault(o => o.Id.Equals(bookId.Value));
            if (SelectedBook == null)
            {
                SetMessage("要删除的书籍不存在");
                return;
            }
            var result = MessageBox.Show($"确定要删除《{SelectedBook.Title}》吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // 先删除所有相关章节
                    var articlesResult = await _articleService.GetArticlesByBookIdAsync(bookId.Value, false);
                    if (articlesResult.IsSuccessful && articlesResult.Value != null && articlesResult.Value.Count > 0)
                    {
                        foreach (var article in articlesResult.Value)
                        {
                            var deleteArticleResult = await _articleService.DeleteArticleAsync(article.Id);
                            if (!deleteArticleResult.IsSuccessful)
                            {
                                SetMessage($"删除章节 '{article.Title}' 失败: {deleteArticleResult.Code}");
                                // 可以选择继续删除其他章节，或者中断操作
                            }
                        }
                    }

                    var deleteResult = await _databaseService.Delete(SelectedBook);

                    if (deleteResult.IsSuccessful)
                    {
                        if (BookList != null && BookList.Count > 0)
                            if (BookList.Contains(SelectedBook))
                            {
                                BookList.Remove(SelectedBook);
                            }
                        SetMessage($"已成功删除《{SelectedBook.Title}》");
                    }
                    else
                    {
                        SetMessage($"删除失败:{deleteResult.Code}");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    // 处理实体跟踪冲突
                    SetMessage($"删除失败: 实体跟踪冲突。请稍后重试。/r/n{ex.Message}");
                    // 可选: 重新加载数据以解决冲突
                    Load();
                }
            }
        }

        private void EnterBook(int? bookId)
        {
            if(bookId == null)
            {
                SetMessage("无效的书籍ID");
                return;
            }
            SetMessage($"{bookId.Value}");
            if (!bookId.HasValue)
            {
                SetMessage("无效的书籍ID");
                return;
            }
            SelectedBook = BookList?.FirstOrDefault(o => o.Id.Equals(bookId.Value));
            if (SelectedBook == null)
            {
                SetMessage("要打开的书籍不存在");
                return;
            }
            try
            {
                //SelectedBook = BookList.FirstOrDefault(o => o.Id.Equals(bookId.Value));

                //AppSession.SetBook(SelectedBook);

                // 创建导航参数，包含BookId
                var parameters = new NavigationParameters
                {
                    { "BookId", bookId.Value }
                };

                // 导航到BookContentView并传递参数
                _regionManager.RequestNavigate(RegionNames.ContentRegion, "BookContentView", parameters);
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message);
            }
        }

        public override async void Load()
        {
            base.Load();
            if(AppSession.User == null)
            {
                SetMessage("本地缓存用户信息为空，可能需要重启应用。");
                return;
            }
            var result = await _databaseService.Get("AuthorId", AppSession.User.ID.ToString());
            if (result != null && result.Value != null)
            {
                BookList = result.Value;
                if (BookList.Count == 0)
                    SetMessage("你的书架空空如也。");
                else
                    SetMessage($"已查到{BookList.Count}本书。");
            }
            else
            {
                BookList = new ObservableCollection<Book>();
                SetMessage("获取书籍数据失败。");
            }
        }

        private void ModifyBook(int? bookId)
        {
            if(BookList == null)
            {
                SetMessage("书籍列表不存在，可能需要重新加载列表。");
                return ;
            }
            if (bookId == null)
            {
                SetMessage("无效的书籍ID");
                return;
            }
            // 这里实现修改逻辑
            SelectedBook = BookList.Where(o => o.Id.Equals(bookId.Value)).FirstOrDefault();
            if (SelectedBook == null)
            {
                SetMessage("书籍不存在，可能需要重新选择书籍。");
                return;
            }
            AppSession.SetBook(SelectedBook);

            NavigatedContentRegionTo("BookEditorView");
        }



        

        /// <summary>
        /// 导航到目标页面
        /// </summary>
        /// <param name="viewName"></param>
        public void NavigatedContentRegionTo(string viewName)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Load();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
            BookList = new();
        }

        #endregion
    }
}
