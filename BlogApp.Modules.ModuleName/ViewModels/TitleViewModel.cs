using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BlogApp.Core.Events;
using BlogApp.Core.Mvvm;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class TitleViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region 属性 

        private bool _isLeftDrawerOpen;
        public bool IsLeftDrawerOpen
        {
            get => _isLeftDrawerOpen;
            set
            {
                if (SetProperty(ref _isLeftDrawerOpen, value))
                {
                    // 订阅事件
                    _eventAggregator.GetEvent<DrawerOpenEvent>().Publish(value);
                }
            }
        }

        public bool KeepAlive => true;

        #endregion

        #region 命令 

        public DelegateCommand<string> DelegateCommand { get; set; }


        #endregion

        #region 函数 

        public TitleViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) : base(regionManager)
        {
            _eventAggregator = eventAggregator;

            _regionManager = regionManager;

            DelegateCommand = new DelegateCommand<string>(DelegateMethod);

            // 强制初始化抽屉区域
            IsLeftDrawerOpen = true;
        }

        private void DelegateMethod(string command)
        {
            switch (command)
            {
                // 最小化窗口
                case "Minimize":
                    // 定义当前运行的窗口
                    //var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                    var window = Application.Current.MainWindow;
                    if (window != null)
                    {
                        window.WindowState = WindowState.Minimized;
                    }
                    break;
                // 最大化窗口/退出最大化
                case "MaximizeOrNormal":
                    MaximizeOrNormal();
                    break;
                // 退出程序
                case "Exit":
                    Application.Current.Shutdown();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 最大化或者正常化窗口
        /// </summary>
        private void MaximizeOrNormal()
        {
            //var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var window = Application.Current.MainWindow;
            if (window != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {

                    window.WindowState = WindowState.Normal;
                }
                else
                {
                    window.WindowState = WindowState.Maximized;
                }
            }

        }

        #endregion
    }
}
