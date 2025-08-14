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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlogApp.Modules.ModuleName.Views
{
    /// <summary>
    /// TitleView.xaml 的交互逻辑
    /// </summary>
    public partial class TitleView : UserControl
    {
        public TitleView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按下左键事件
        /// </summary>
        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 拖动当前活动窗体
            //Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive).DragMove();
            Application.Current.MainWindow.DragMove();
        }
    }
}
