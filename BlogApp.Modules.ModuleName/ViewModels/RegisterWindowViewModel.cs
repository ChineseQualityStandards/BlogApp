using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BlogApp.Core.Mvvm;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class RegisterWindowViewModel :RegionViewModelBase
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        #endregion

        #region 属性

        #endregion

        #region 命令

        public DelegateCommand<string> DelegateCommand { get; set; }

        public DelegateCommand<PasswordBox> RegisterCommand { get; }

        #endregion

        #region 函数

        public RegisterWindowViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _regionManager = regionManager;
            LoadMethod();
            DelegateCommand = new DelegateCommand<string>(DelegateMethod);
            RegisterCommand = new DelegateCommand<PasswordBox>(DelegateMethod);
        }

        private void DelegateMethod(string command)
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            switch (command)
            {
                case "close":
                    Application.Current.MainWindow.Show();
                    window?.Close();
                    break;
                case "minimize":
                    window.WindowState = WindowState.Minimized;
                    break;
                default:
                    break;
            }
        }

        private void DelegateMethod(PasswordBox passwordBox)
        {
            MessageBox.Show("注册失败");
        }

        /// <summary>
        /// 加载函数
        /// </summary>
        public void LoadMethod()
        {
            
        }

        #endregion
    }
}
