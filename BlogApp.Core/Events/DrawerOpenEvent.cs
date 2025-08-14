using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Core.Events
{
    /// <summary>
    /// 关于DrawerHost控件中Drawer的事件
    /// </summary>
    public class DrawerOpenEvent : PubSubEvent<bool>
    {
    }
}
