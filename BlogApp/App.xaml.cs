using System.Configuration;
using System.Data;
using System.Windows;
using BlogApp.Core.DbContexts;
using BlogApp.Core.Models;
using BlogApp.Modules.ModuleName;
using BlogApp.Services;
using BlogApp.Services.Interfaces;
using BlogApp.Views;
using Microsoft.EntityFrameworkCore;

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
            using (var context = Container.Resolve<BlogAppContext>())
            {
                // 自动创建数据库和表
                context.Database.EnsureCreated();
            }
        }

        /// <summary>
        /// 服务注册函数
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <exception cref="NotImplementedException">注册容器</exception>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 配置 DbContextOptions
            var connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=MarkdownEditor;User ID=sa;Password=1234;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            containerRegistry.RegisterInstance<DbContextOptions<BlogAppContext>>(new DbContextOptionsBuilder<BlogAppContext>().UseSqlServer(connectionString).Options);

            // 注册 DbContext
            containerRegistry.Register<BlogAppContext>(() =>
            {
                var option = Container.Resolve<DbContextOptions<BlogAppContext>>();
                return new BlogAppContext(option);
            });

            containerRegistry.RegisterSingleton<IDatabaseService<User>, DatabaseService<User>>();
            containerRegistry.RegisterSingleton<IEncryptService, EncryptService>();
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
