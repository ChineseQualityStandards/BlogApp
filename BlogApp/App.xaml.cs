using System.Configuration;
using System.Data;
using System.Windows;
using BlogApp.Modules.ModuleName;
using BlogApp.Views;

namespace BlogApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// 创建主应用
        /// </summary>
        /// <returns>打开的窗口</returns>
        protected override Window CreateShell()
        {
            return Container.Resolve<LoginWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        /// <summary>
        /// 服务注册函数
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <exception cref="NotImplementedException">注册容器</exception>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            //containerRegistry.RegisterSingleton<IMessageService, MessageService>();
        }

        /// <summary>
        /// 配置Prism使用的IModuleCatalog
        /// </summary>
        /// <param name="moduleCatalog">这是ModuleManager所需的目录定义。</param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
        }
    }

}
