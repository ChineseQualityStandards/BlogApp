using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BlogApp.Core.AppSeesions;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;
using BlogApp.Modules.ModuleName.Views;
using BlogApp.Services.Interfaces;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class LoginViewModel : RegionViewModelBase
    {
        #region 字段

        private readonly IContainerExtension _container;

        private readonly IRegionManager _regionManager;

        private readonly IDatabaseService<User> _databaseService;

        private readonly IEncryptService _encryptService;

        #endregion

        #region 属性

        private string? account;

        public string? Account
        {
            get { return account; }
            set { SetProperty(ref account, value); }
        }

        private User user;
        /// <summary>
        /// 用户属性
        /// </summary>
        public User User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        #endregion

        #region 命令

        public DelegateCommand<string> DelegateCommand { get; set; }

        public DelegateCommand<PasswordBox> LoginCommand { get; set; }

        #endregion

        #region 函数
        public LoginViewModel(IContainerExtension container, IRegionManager regionManager, IDatabaseService<User> databaseService, IEncryptService encryptService) : base(regionManager)
        {
            _container = container;
            _regionManager = regionManager;
            _databaseService = databaseService;
            _encryptService = encryptService;
            DelegateCommand = new DelegateCommand<string>(DelegateMethod);
            LoginCommand = new DelegateCommand<PasswordBox>(LoginMethodAsync);

            Load();
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
        private async void LoginMethodAsync(PasswordBox box)
        {

            if (string.IsNullOrEmpty(User.Email) || string.IsNullOrEmpty(box.Password))
            {
                SetMessage("邮箱或密码不能为空");
            }
            else
            {
                // 3. 检查邮箱是否已存在
                var emailExistsResult = await _databaseService.Get("Email", User.Email);
                if (emailExistsResult.IsSuccessful && emailExistsResult.Value != null && emailExistsResult.Value.Any())
                {
                    var tempUser = emailExistsResult.Value.FirstOrDefault();
                    if(tempUser.PALLTHash.Equals(_encryptService.EncryptPassword(box.Password, tempUser.LastLoginTime)))
                    {
                        //更新登录时间
                        User = tempUser;
                        tempUser = null;
                        User.LastLoginTime = DateTime.Now;
                        User.PALLTHash = _encryptService.EncryptPassword(box.Password, User.LastLoginTime);
                        _databaseService.Update(User);

                        // 用户数据持久化
                        AppSeesion.SetUser(User);

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
                        SetErrorMessage("账号或密码错误");
                    }
                }
                else
                {
                    SetErrorMessage("该邮箱不存在");
                    return;
                }

            }
            
        }

        public override void Load()
        {
            base.Load();
            User = new User();
        }

        #endregion
    }
}
