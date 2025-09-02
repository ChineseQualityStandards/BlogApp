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
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Commands;

namespace BlogApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly IConfiguration? _configuration;

        public App()
        {
            // 构建配置对象
            _configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json", optional:false, reloadOnChange:true).Build();
        }

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
            using var context = Container.Resolve<BlogAppContext>();
            // 自动创建数据库和表
            context.Database.EnsureCreated();
        }

        /// <summary>
        /// 服务注册函数
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <exception cref="NotImplementedException">注册容器</exception>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            try
            {
                if(_configuration == null)
                {
                    throw new Exception("_configuration为空.");
                }
                //从配置中读取连接字符串和数据库类型
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var databaseProvider = _configuration["DatabaseProvider"];

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("配置文件中缺少数据库连接字符串，请先配置数据库连接。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DbContextOptions<BlogAppContext> options;

                if (databaseProvider != null && databaseProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
                {
                    // 使用 MySQL
                    options = new DbContextOptionsBuilder<BlogAppContext>()
                        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                        .Options;
                }
                else
                {
                    // 默认使用 SQL Server
                    options = new DbContextOptionsBuilder<BlogAppContext>()
                        .UseSqlServer(connectionString)
                        .Options;
                }

                containerRegistry.RegisterInstance(options);


                // 注册 DbContext
                containerRegistry.Register<BlogAppContext>(() =>
                {
                    var option = Container.Resolve<DbContextOptions<BlogAppContext>>();
                    return new BlogAppContext(option);
                });
                // 注册通用数据库服务 
                containerRegistry.RegisterSingleton<IDatabaseService<User>, DatabaseService<User>>();
                containerRegistry.RegisterSingleton<IDatabaseService<Book>, DatabaseService<Book>>();
                containerRegistry.RegisterSingleton<IDatabaseService<Article>, DatabaseService<Article>>();

                // 注册文章专用服务
                containerRegistry.RegisterSingleton<IArticleService, ArticleService>();
                // 注册其他服务
                containerRegistry.RegisterSingleton<IEncryptService, EncryptService>();
                //containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化数据库连接时发生错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
