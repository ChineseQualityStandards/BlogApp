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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class DatabaseSettingViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        private IConfiguration _configuration;

        #endregion

        #region 属性 

        private string _configFilePath;

        public bool KeepAlive => true;

        private bool _isSqlServer;

        public bool IsSqlServer
        {
            get { return _isSqlServer; }
            set 
            {
                if (SetProperty(ref _isSqlServer, value)) 
                { 
                    IsMySql = !IsSqlServer;
                    DatabaseProvider = IsSqlServer ? "SqlServer" : "MySql";
                    // 切换数据库类型时，设置默认端口
                    if (IsSqlServer && string.IsNullOrEmpty(Port))
                    {
                        Port = "1433"; // SQL Server 默认端口
                    }
                    else if (IsMySql && string.IsNullOrEmpty(Port))
                    {
                        Port = "3306"; // MySQL 默认端口
                    }
                } 
            }
        }

        private bool _isMySql;

        public bool IsMySql
        {
            get { return _isMySql; }
            set 
            {
                if (SetProperty(ref _isMySql, value))
                {
                    IsSqlServer = !IsMySql;
                    DatabaseProvider = IsSqlServer ? "SqlServer" : "MySql";
                    // 切换数据库类型时，设置默认端口
                    if (IsSqlServer && string.IsNullOrEmpty(Port))
                    {
                        Port = "1433"; // SQL Server 默认端口
                    }
                    else if (IsMySql && string.IsNullOrEmpty(Port))
                    {
                        Port = "3306"; // MySQL 默认端口
                    }
                }
                ; 
            }
        }

        private string _databaseProvider;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DatabaseProvider
        {
            get 
            { 
                return _databaseProvider;
            }
            set 
            { 
                SetProperty(ref _databaseProvider, value);
            }
        }

        private string _server;
        public string Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); }
        }

        private string _port;
        /// <summary>
        /// 端口号
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
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

        public DatabaseSettingViewModel(IRegionManager regionManager) : base(regionManager)
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
                    // 读取数据库类型
                    var databaseProvider = _configuration["DatabaseProvider"];

                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        ParseConnectionString(connectionString, databaseProvider);
                    }

                    // 设置数据库类型
                    if (!string.IsNullOrEmpty(databaseProvider))
                    {
                        IsSqlServer = databaseProvider.Equals("SqlServer",StringComparison.OrdinalIgnoreCase);
                        IsMySql = databaseProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase);
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
        private void ParseConnectionString(string connectionString, string databaseProvider)
        {
            try
            {
                if (databaseProvider != null && databaseProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
                {
                    // 解析 MySQL 连接字符串
                    var parameters = connectionString.Split(';');
                    foreach (var param in parameters)
                    {
                        var parts = param.Split('=');
                        if (parts.Length == 2)
                        {
                            var key = parts[0].Trim();
                            var value = parts[1].Trim();

                            switch (key.ToLower())
                            {
                                case "server":
                                    Server = value;
                                    break;
                                case "port":
                                    Port = value;
                                    break;
                                case "database":
                                    Database = value;
                                    break;
                                case "user id":
                                    UserId = value;
                                    break;
                                case "password":
                                    Password = value;
                                    break;
                            }
                        }
                    }
                    // 如果没有解析到端口号，设置默认值
                    if (string.IsNullOrEmpty(Port))
                    {
                        Port = "3306";
                    }
                }
                else
                {

                    // 解析 SQL Server 连接字符串
                    var builder = new SqlConnectionStringBuilder(connectionString);

                    Server = builder.DataSource;
                    Database = builder.InitialCatalog;
                    UserId = builder.UserID;
                    Password = builder.Password;

                    // 尝试从服务器地址中提取端口号
                    if (Server.Contains(","))
                    {
                        var parts = Server.Split(',');
                        if (parts.Length == 2 && int.TryParse(parts[1], out _))
                        {
                            Server = parts[0];
                            Port = parts[1];
                        }
                    }

                    // 如果没有解析到端口号，设置默认值
                    if (string.IsNullOrEmpty(Port))
                    {
                        Port = "1433";
                    }
                }
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
                if (IsMySql)
                {
                    // 测试 MySQL 连接
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        SetMessage("MySQL 连接成功!");
                    }
                }
                else
                {
                    // 测试 SQL Server 连接
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        SetMessage("连接成功!");
                    }
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
                if (IsMySql)
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                    }
                }
                else
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                    }
                }

                // 读取现有配置
                var configJson = File.ReadAllText(_configFilePath);
                dynamic config = JsonConvert.DeserializeObject(configJson);

                // 更新连接字符串和数据库类型
                config["ConnectionStrings"]["DefaultConnection"] = connectionString;
                config["DatabaseProvider"] = DatabaseProvider; // 保存数据库类型

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
            if (IsMySql)
            {
                // MySQL 连接字符串格式
                return $"Server={Server};Port={Port};Database={Database};User ID={UserId};Password={Password};";
            }
            else
            {
                // SQL Server 连接字符串格式
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = string.IsNullOrEmpty(Port) || Port == "1433" ?
                        Server : $"{Server},{Port}",
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
        }



        #endregion
    }
}
