using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BlogApp.Core.Events;
using BlogApp.Core.Mvvm;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class SqlServerSettingViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        private IConfiguration _configuration;

        #endregion

        #region 属性 

        private string _configFilePath;

        public bool KeepAlive => true;

        private string _server;
        public string Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); }
        }

        private string _database;
        public string Database
        {
            get { return _database; }
            set { SetProperty(ref _database, value); }
        }

        private string _userId;
        public string UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        #endregion

        #region 命令 

        public DelegateCommand<PasswordBox> TestConnectionCommand { get; set; }

        public DelegateCommand<PasswordBox> SaveConnectionCommand { get; set; }


        #endregion

        #region 函数 

        public SqlServerSettingViewModel(IRegionManager regionManager) : base(regionManager)
        {

            _regionManager = regionManager;
            
            _configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

            // 初始化配置
            LoadConfiguration();

            // 初始化命令
            TestConnectionCommand = new DelegateCommand<PasswordBox>(TestConnection);
            SaveConnectionCommand = new DelegateCommand<PasswordBox>(SaveConnection);

        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    // 使用应用程序基目录而不是当前目录
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;

                    _configuration = new ConfigurationBuilder()
                        .SetBasePath(basePath)  // 明确设置基路径
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    var connectionString = _configuration.GetConnectionString("DefaultConnection");

                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        ParseConnectionString(connectionString);
                    }
                }
                else
                {
                    // 创建默认配置文件
                    CreateDefaultConfigFile();
                }
            }
            catch (Exception ex)
            {
                Message = $"加载配置文件失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 解析连接字符串
        /// </summary>
        private void ParseConnectionString(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);

                Server = builder.DataSource;
                Database = builder.InitialCatalog;
                UserId = builder.UserID;
                Password = builder.Password;
            }
            catch (Exception ex)
            {
                SetMessage($"解析连接字符串失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建默认配置文件
        /// </summary>
        private void CreateDefaultConfigFile()
        {
            try
            {
                // 使用 JObject 构建配置，这样可以处理包含点的属性名
                JObject defaultConfig = new JObject(
                    new JProperty("ConnectionStrings",
                        new JObject(
                            new JProperty("DefaultConnection", "")
                        )
                    ),
                    new JObject(
                        new JProperty("Logging",
                            new JObject(
                                new JProperty("LogLevel",
                                    new JObject(
                                        new JProperty("Default", "Information"),
                                        new JProperty("Microsoft", "Warning"),
                                        new JProperty("Microsoft.Hosting.Lifetime", "Information")
                                    )
                                )
                            )
                        )
                    )
                );

                string json = defaultConfig.ToString(Formatting.Indented);
                File.WriteAllText(_configFilePath, json);

                SetMessage("已创建默认配置文件，请配置数据库连接信息");
            }
            catch (Exception ex)
            {
                SetMessage($"创建配置文件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        private async void TestConnection(PasswordBox box)
        {
            try
            {
                SetMessage("正在测试连接...");

                Password = box.Password;

                var connectionString = BuildConnectionString();

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SetMessage("连接成功!");
                }
            }
            catch (Exception ex)
            {
                SetMessage($"连接失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 保存数据库连接配置
        /// </summary>
        private void SaveConnection(PasswordBox box)
        {
            try
            {
                Password = box.Password;

                var connectionString = BuildConnectionString();

                // 测试连接是否有效
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                }

                // 读取现有配置
                var configJson = File.ReadAllText(_configFilePath);
                dynamic config = JsonConvert.DeserializeObject(configJson);

                // 更新连接字符串
                config["ConnectionStrings"]["DefaultConnection"] = connectionString;

                // 保存回文件
                string updatedJson = config.ToString(Formatting.Indented);
                File.WriteAllText(_configFilePath, updatedJson);

                SetMessage("配置已保存并验证成功!");

                // 提示用户需要重启应用
                MessageBox.Show("数据库配置已更新，需要重启应用程序才能生效。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                SetMessage($"保存配置失败: {ex.Message}") ;
            }
        }

        /// <summary>
        /// 构建连接字符串
        /// </summary>
        private string BuildConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = Database,
                UserID = UserId,
                Password = Password,
                ConnectTimeout = 30,
                Encrypt = false,
                TrustServerCertificate = false,
                ApplicationIntent = ApplicationIntent.ReadWrite,
                MultiSubnetFailover = false
            };

            return builder.ToString();
        }



        #endregion
    }
}
