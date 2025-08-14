using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class BookShelfViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        #endregion

        #region 属性

        public bool KeepAlive => true;

        private ObservableCollection<BookCover> bookList;

        public ObservableCollection<BookCover> BookList
        {
            get { return bookList; }
            set { SetProperty(ref bookList, value); }
        }


        #endregion

        #region 命令

        #endregion

        #region 函数

        public BookShelfViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _regionManager = regionManager;

            BookList = new ObservableCollection<BookCover>() {
                new BookCover()
                {
                    Id = 0,
                    Author="诗三百",
                    Title="C#从入门到入土",
                    CoverColor="Gray",
                    Foreground="White"
                },
                new BookCover()
                {
                    Id = 1,
                    Author="蔺太微",
                    Title="临时抱佛脚",
                    CoverColor="Gold",
                    Foreground="Black"
                },
                new BookCover()
                {
                    Id = 2,
                    Author="蔡徐坤",
                    Title="基尼台妹",
                    CoverColor="Pink",
                    Foreground="Black"
                }

            };
        }

        #endregion
    }
}
