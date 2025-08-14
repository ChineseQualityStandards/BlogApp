using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BlogApp.Core.Mvvm;
using BlogApp.Modules.ModuleName.Views;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class LoginViewModel : RegionViewModelBase
    {
        #region 字段

        private readonly IContainerExtension _container;

        private readonly IRegionManager _regionManager;

        #endregion

        #region 属性

        private string? account;

        public string? Account
        {
            get { return account; }
            set { SetProperty(ref account, value); }
        }


        #endregion

        #region 命令

        public DelegateCommand<string> DelegateCommand { get; set; }

        public DelegateCommand<PasswordBox> LoginCommand { get; set; }

        #endregion

        #region 函数
        public LoginViewModel(IContainerExtension container, IRegionManager regionManager) : base(regionManager)
        {
            _container = container;
            _regionManager = regionManager;
            DelegateCommand = new DelegateCommand<string>(DelegateMethod);
            LoginCommand = new DelegateCommand<PasswordBox>(LoginMethod);
        }

        private void DelegateMethod(string command)
        {
            switch (command)
            {
                case "Forgoter":
                    Application.Current.MainWindow.Hide();
                    _container.Resolve<ForgotWindow>().Show();
                    break;
                case "Register":
                    Application.Current.MainWindow.Hide();
                    _container.Resolve<RegisterWindow>().Show();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 登录函数
        /// </summary>
        /// <param name="box"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void LoginMethod(PasswordBox box)
        {
            if (box == null)
            {
                SetMessage("box is null");
            }
            else
            {
                if (string.IsNullOrEmpty(Account) || string.IsNullOrEmpty(box.Password))
                {
                    SetMessage("账号或密码不能为空");
                }
                else
                {
                    if (Account.Equals("1") && box.Password.Equals("1"))
                    {
                        // LoginWindow区域管理权限要转移给MainWindow
                        var loginWindow = Application.Current.MainWindow;
                        var mainWindow = _container.Resolve<MainWindow>();
                        // 让新打开的窗口成为程序的MainWindow
                        Application.Current.MainWindow = mainWindow;
                        Prism.Navigation.Regions.RegionManager.SetRegionManager(mainWindow, _regionManager);

                        loginWindow.Close();
                        mainWindow.Show();
                    }
                    else
                    {
                        SetMessage("账号或密码错误");
                    }
                }
            }
        }

        #endregion
    }
}
