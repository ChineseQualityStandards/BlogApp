using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BlogApp.Core.DbContexts;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;
using BlogApp.Services.Interfaces;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class RegisterWindowViewModel :RegionViewModelBase
    {
        #region 字段

        private readonly IRegionManager _regionManager;
        private readonly IDatabaseService<User> _databaseService;
        private readonly IEncryptService _encryptService;
        private readonly BlogAppContext _context;

        #endregion

        #region 属性

        private User? user;
        /// <summary>
        /// 用户属性
        /// </summary>
        public User? User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        #endregion

        #region 命令

        public DelegateCommand<string> DelegateCommand { get; set; }

        public DelegateCommand<PasswordBox> RegisterCommand { get; }

        #endregion

        #region 函数

        public RegisterWindowViewModel(IRegionManager regionManager, IDatabaseService<User> databaseService, IEncryptService encryptService, BlogAppContext context) : base(regionManager)
        {
            _regionManager = regionManager;
            _databaseService = databaseService;
            _encryptService = encryptService;
            _context = context;
            Load();
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
                    if(window != null)
                        window.WindowState = WindowState.Minimized;
                    break;
                default:
                    break;
            }
        }

        private async void DelegateMethod(PasswordBox passwordBox)
        {
            try
            {
                // 1. 先验证必填字段
                if (string.IsNullOrEmpty(User?.Email))
                {
                    SetErrorMessage("邮箱不能为空");
                    return;
                }

                if (string.IsNullOrEmpty(User.Name))
                {
                    SetErrorMessage("用户名不能为空");
                    return;
                }

                if (string.IsNullOrEmpty(passwordBox.Password))
                {
                    SetErrorMessage("密码不能为空");
                    return;
                }

                // 2. 验证邮箱格式（简单验证）
                if (!User.Email.Contains("@") || !User.Email.Contains("."))
                {
                    SetErrorMessage("邮箱格式不正确");
                    return;
                }

                // 3. 检查邮箱是否已存在
                var emailExistsResult = await _databaseService.Get("Email", User.Email);
                if (emailExistsResult.IsSuccessful && emailExistsResult.Value != null && emailExistsResult.Value.Any())
                {
                    SetErrorMessage("该邮箱已被注册");
                    return;
                }

                // 4. 检查用户名是否已存在
                var nameExistsResult = await _databaseService.Get("Name", User.Name);
                if (nameExistsResult.IsSuccessful && nameExistsResult.Value != null && nameExistsResult.Value.Any())
                {
                    SetErrorMessage("该用户名已被使用");
                    return;
                }
                User.CreatedTime = DateTime.Now;
                User.LastLoginTime = User.CreatedTime;
                User.PACTHash = _encryptService.EncryptPassword(passwordBox.Password, User.CreatedTime);
                User.PALLTHash = _encryptService.EncryptPassword(passwordBox.Password, User.LastLoginTime);
                EFMessage<User> result = await _databaseService.Add(User);
                if (result.IsSuccessful)
                    DelegateMethod("close");
                else
                {
                    if(result.Code != null)
                        SetErrorMessage(result.Code);
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.Message);
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
