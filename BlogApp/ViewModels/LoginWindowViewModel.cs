using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BlogApp.Core.Mvvm;
using Prism.Commands;
using Prism.Ioc;
using Prism.Navigation.Regions;

namespace BlogApp.ViewModels
{
    public class LoginWindowViewModel : RegionViewModelBase
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        #endregion

        #region 属性

        private bool isFlipped;

        public bool IsFlipped
        {
            get { return isFlipped; }
            set { SetProperty(ref isFlipped, value); }
        }


        private string? settingIcon;

        /// <summary>
        /// 设置按钮的图标
        /// </summary>
        public string? SettingIcon
        {
            get { return settingIcon; }
            set { SetProperty(ref settingIcon, value); }
        }


        #endregion

        #region 命令

        public DelegateCommand<string> DelegateCommand { get; set; }

        #endregion

        #region 函数

        public LoginWindowViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _regionManager = regionManager;
            LoadMethod();
            DelegateCommand = new DelegateCommand<string>(DelegateMethod);
        }

        private void DelegateMethod(string command)
        {
            switch (command)
            {
                case "setting":
                    if(string.IsNullOrEmpty(SettingIcon))
                    {
                        SettingIcon = "Settings";
                    }
                    else
                    {
                        SettingIcon = SettingIcon.Equals("Settings") ? "ArrowLeft" : "Settings";
                        IsFlipped = !IsFlipped;
                    }
                    break;
                case "close":
                    Application.Current.Shutdown();
                    break;
                case "minimize":
                    Application.Current.MainWindow.WindowState = WindowState.Minimized;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 加载函数
        /// </summary>
        public void LoadMethod()
        {
            SettingIcon = "Settings";
        }

        #endregion
    }
}
