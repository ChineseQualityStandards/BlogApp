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
    public class ForgotWindowViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        #endregion

        #region 属性

        public bool KeepAlive => true;

        #endregion

        #region 命令

        public DelegateCommand<string> DelegateCommand { get; set; }

        public DelegateCommand<PasswordBox> RegisterCommand { get; set; }

        #endregion

        #region 函数

        public ForgotWindowViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _regionManager = regionManager;
            DelegateCommand = new DelegateCommand<string>(DelegateMethod);
            RegisterCommand = new DelegateCommand<PasswordBox>(DelegateMethod);
        }

        private void DelegateMethod(PasswordBox box)
        {
            SetMessage("修改密码失败");
            //throw new NotImplementedException();
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


        #endregion
    }
}
