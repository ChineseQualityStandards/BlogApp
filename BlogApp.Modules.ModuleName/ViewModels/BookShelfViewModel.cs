using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using BlogApp.Core.AppSessions;
using BlogApp.Core.Constants;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;
using BlogApp.Services.Interfaces;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class BookShelfViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        private readonly IDatabaseService<Book> _databaseService;

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

        public DelegateCommand<string> NavigatedToCommand { get; set; }
        public DelegateCommand<int?> ModifyCommand { get; set; }
        public DelegateCommand<int?> DeleteCommand { get; set; }

        #endregion

        #region 函数

        public BookShelfViewModel(IRegionManager regionManager, IDatabaseService<Book> databaseService) : base(regionManager)
        {
            _regionManager = regionManager;
            _databaseService = databaseService;

            // 初始化命令
            NavigatedToCommand = new DelegateCommand<string>(NavigatedContentRegionTo);
            ModifyCommand = new DelegateCommand<int?>(ModifyBook);
            DeleteCommand = new DelegateCommand<int?>(DeleteBook);

        }

        private void ModifyBook(int? bookId)
        {
            //SetMessage($"修改书籍: {bookId}");
            // 这里实现修改逻辑
            SelectedBook = BookList.Where(o => o.Id.Equals(bookId.Value)).FirstOrDefault();

            AppSession.SetBook(SelectedBook);

            NavigatedContentRegionTo("BookEditorView");
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
                    SetMessage($"删除失败: 实体跟踪冲突。请稍后重试。");
                    // 可选: 重新加载数据以解决冲突
                    Load();
                }
            }
        }

        public override async void Load()
        {
            base.Load();
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
