using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.Constants;
using BlogApp.Modules.ModuleName.Views;

namespace BlogApp.Modules.ModuleName
{
    public class ModuleNameModule : IModule
    {

        private readonly IRegionManager _regionManager;

        public ModuleNameModule(IRegionManager regionManager)
        {

            _regionManager = regionManager;

        }

        /// <summary>
        /// 在模块初始化时执行的逻辑
        /// </summary>
        /// <param name="containerProvider"></param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.AnnRegion, "AnnView");
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, "HomeView");
            _regionManager.RegisterViewWithRegion(RegionNames.LeftDrawerRegion, "LeftDrawerView");
            _regionManager.RegisterViewWithRegion(RegionNames.PopupRegion, "LoginView");
            _regionManager.RegisterViewWithRegion(RegionNames.TitleRegion, "TitleView");
            _regionManager.RegisterViewWithRegion(RegionNames.DatabaseSettingRegion, "DatabaseSettingView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AnnView>();
            containerRegistry.RegisterForNavigation<ArticleEditorView>();
            containerRegistry.RegisterForNavigation<BookContentView>();
            containerRegistry.RegisterForNavigation<BookShelfView>();
            containerRegistry.RegisterForNavigation<BookEditorView>();
            containerRegistry.RegisterForNavigation<HomeView>();
            containerRegistry.RegisterForNavigation<LeftDrawerView>();
            containerRegistry.RegisterForNavigation<LoginView>();
            containerRegistry.RegisterForNavigation<TitleView>();
            containerRegistry.RegisterForNavigation<DatabaseSettingView>();
            containerRegistry.RegisterForNavigation<ViewA>();
        }
    }
}
