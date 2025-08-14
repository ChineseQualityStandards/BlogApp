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
    public class MainWindowViewModel : RegionViewModelBase
    {
        #region 字段

        private readonly IRegionManager _regionManager;
        /// <summary>
        /// 事件中介：通过事件聚合器实现模块间松耦合的发布/订阅通信。
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        #endregion
        #region 属性 

        private string _title = "Prism Application";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        // 工作区高度
        public double WorkAreaHeight { get { return SystemParameters.WorkArea.Height; } }
        // 工作区宽度
        public double WorkAreaWidth { get { return SystemParameters.WorkArea.Width; } }

        private bool _isLeftDrawerOpen;
        public bool IsLeftDrawerOpen
        {
            get => _isLeftDrawerOpen;
            set => SetProperty(ref _isLeftDrawerOpen, value);
        }

        #endregion

        #region 命令

        #endregion

        #region 函数

        public MainWindowViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) : base(regionManager)
        {
            _eventAggregator = eventAggregator;

            _regionManager = regionManager;
            // 发布事件
            _eventAggregator.GetEvent<DrawerOpenEvent>().Subscribe(OnDrawerOpenChanged);

        }

        /// <summary>
        /// 抽屉开关事件的实现
        /// </summary>
        /// <param name="isOpen">抽屉需要达成的状态</param>
        private void OnDrawerOpenChanged(bool isOpen)
        {

            IsLeftDrawerOpen = isOpen;
        }

        #endregion
    }
}
