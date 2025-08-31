using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.AppSessions;
using BlogApp.Core.Constants;
using BlogApp.Core.Events;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;
using BlogApp.Services;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class LeftDrawerViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region 属性

        private string _userName = "未登录";
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _signature = "请先登录";
        /// <summary>
        /// 用户签名
        /// </summary>
        public string Signature
        {
            get => _signature;
            set => SetProperty(ref _signature, value);
        }

        private string _userId = "UID:未知";
        /// <summary>
        /// 用户ID显示
        /// </summary>
        public string UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        private User? _currentUser;
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public User? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public bool KeepAlive => true;
        /// <summary>
        /// 按钮属性
        /// </summary>
        public ObservableCollection<NavItem> NavItems { get; } = new()
            {
                new NavItem { Title = "首页", Parameter = "HomeView" },
                new NavItem { Title = "书架", Parameter = "BookShelfView" },
            };

        #endregion

        #region 命令

        public DelegateCommand<string> DelegateCommand { get; set; }

        #endregion

        #region 函数

        public LeftDrawerViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) : base(regionManager)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            DelegateCommand = new DelegateCommand<string>(DelegateMethod);

            // 订阅用户登录事件
            _eventAggregator.GetEvent<UserLoggedInEvent>().Subscribe(OnUserLoggedIn);

            

        }

        public override void Load()
        {
            base.Load();
            
            
            if (CurrentUser != null) 
            {
                UserName = CurrentUser.Name ?? "未知用户";
                Signature = CurrentUser.Signature ?? "";
                UserId = $"UID:{CurrentUser.ID:D9}"; // 格式化为9位数字
            }
            else
            {
                UserName = "未登录";
                Signature = "请先登录";
                UserId = "UID:未知";
            }
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

        public override void Destroy()
        {
            // 取消事件订阅
            _eventAggregator.GetEvent<UserLoggedInEvent>().Unsubscribe(OnUserLoggedIn);
            base.Destroy();
        }

        // 用户登录事件处理
        private void OnUserLoggedIn()
        {
            //CurrentUser = user;
            CurrentUser = AppSession.User;
            Load(); // 刷新用户信息
        }
/*
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            // 每次导航到该页面时刷新用户信息
            Load();
        }
*/
        public void RegionToView(string viewName)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
        }

        #endregion
    }
}
