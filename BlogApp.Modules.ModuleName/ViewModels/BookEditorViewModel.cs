using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BlogApp.Core.AppSessions;
using BlogApp.Core.Constants;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;
using BlogApp.Core.Properties;
using BlogApp.Services.Interfaces;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class BookEditorViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;
        private readonly IDatabaseService<Book> _databaseService;

        #endregion

        #region 属性

        private string? viewTitle;

        public string? ViewTitle
        {
            get { return viewTitle; }
            set { SetProperty(ref viewTitle, value); }
        }

        public bool KeepAlive => false;

        private Book? _book;

        public Book? Book
        {
            get { return _book; }
            set { SetProperty(ref _book, value); }
        }

        #endregion

        #region 命令

        public DelegateCommand<string> DelegateCommand { get; set; }

        #endregion

        #region 函数

        public BookEditorViewModel(IRegionManager regionManager, IDatabaseService<Book> databaseService) : base(regionManager)
        {
            _regionManager = regionManager;
            _databaseService = databaseService;
            DelegateCommand = new DelegateCommand<string>(DelegateMethod);
        }

        /// <summary>
        /// 保存书籍
        /// </summary>
        private async Task BookSave()
        {
            try
            {
                if (Book != null)
                {
                    if (BookVerify())
                    {
                        Book.CreatedDate = DateTime.Now;
                        await _databaseService.Add(Book);
                    }
                    else
                    {
                        // 先查出原始实体，避免EF Core跟踪冲突
                        var result = await _databaseService.Get(Book.Id);
                        var dbBook = result.Value?.FirstOrDefault();
                        if (dbBook != null)
                        {
                            // 更新属性
                            dbBook.Title = Book.Title;
                            dbBook.AuthorId = Book.AuthorId;
                            dbBook.Author = Book.Author;
                            dbBook.Color = Book.Color;
                            dbBook.Foreground = Book.Foreground;
                            dbBook.Description = Book.Description;
                            dbBook.CreatedDate = Book.CreatedDate;
                            // 只用查出来的dbBook进行更新，避免传入Book导致跟踪冲突
                            await _databaseService.Update(dbBook);
                        }
                    }
                    NavigatedContentRegionTo("BookShelfView");
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 验证书籍是否存在
        /// </summary>
        /// <returns></returns>
        private bool BookVerify()
        {
            bool flag = true;
            // 获取书籍信息
            if (Book?.Id != 0)
                flag = false;
            return flag;
        }

        private async void DelegateMethod(string command)
        {
            switch (command)
            {
                case "Complete":
                    await BookSave();
                    break;
                case "Cancel":
                    NavigatedContentRegionTo("BookShelfView");
                    break;
                default:
                    break;
            }
        }

        public override void Destroy()
        {
            Book = null;
            AppSession.ClearBook();
            base.Destroy();
        }

        public override void Load()
        {
            base.Load();
            Book = AppSession.Book;
            if (Book == null)
            {
                Book = new Book()
                {
                    Author = AppSession.User?.Name,
                    AuthorId = AppSession.User?.ID ?? 0
                };
            }
            ViewTitle = (Book?.Id == 0) ? "新建书籍" : "管理书籍";
            SetErrorMessage((Book?.Id == 0) ? "正在新建书籍……" : "管理已有书籍……");
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
            Destroy();
        }

        #endregion

    }
}
