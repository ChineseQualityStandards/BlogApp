using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BlogApp.Modules.ModuleName.Views
{
    /// <summary>
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!PasswordBox.Password.Equals(RePasswordBox.Password))
            {
                RegisterButton.IsEnabled = false;
                ErrorMessage.Text = "两次输入的密码不一致";
            }
            else
            {
                RegisterButton.IsEnabled = true;
                ErrorMessage.Text = "";
            }
        }

        

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!PasswordBox.Password.Equals(RePasswordBox.Password))
            {
                RegisterButton.IsEnabled = false;
                ErrorMessage.Text = "两次输入的密码不一致";
            }
            else
            {
                RegisterButton.IsEnabled = true;
                ErrorMessage.Text = "";
            }
        }
    }
}
