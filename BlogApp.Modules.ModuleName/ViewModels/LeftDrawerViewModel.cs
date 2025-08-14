using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.Constants;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class LeftDrawerViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        #endregion

        #region 属性

        public bool KeepAlive => true;
        /// <summary>
        /// 按钮属性
        /// </summary>
        public ObservableCollection<NavItem> NavItems { get; } = new()
            {
                new NavItem { Title = "首页", Parameter = "HomeView" },
                new NavItem { Title = "书架", Parameter = "BookShelfView" },
                new NavItem { Title = "内容", Parameter = "BookContentView" },
                new NavItem { Title = "新建文章", Parameter = "ArticleEditorView" },
            };

        #endregion

        #region 命令

        public DelegateCommand<string> DelegateCommand { get; set; }

        #endregion

        #region 函数

        public LeftDrawerViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _regionManager = regionManager;

            DelegateCommand = new DelegateCommand<string>(DelegateMethod);


        }

        private void DelegateMethod(string command)
        {
            switch (command)
            {

                default:
                    RegionToView(command);
                    break;
            }
        }

        public void RegionToView(string viewName)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName);


        }

        #endregion
    }
}
