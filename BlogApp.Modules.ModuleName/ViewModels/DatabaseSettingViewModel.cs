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

        private IConfiguration? _configuration;

        private readonly string? _configFilePath;
        //将默认端口号提取为常量
        private const string DefaultSqlServerPort = "1433";
        private const string DefaultMySqlPort = "3306";

        #endregion

        #region 属性 


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
                        Port = DefaultSqlServerPort; // SQL Server 默认端口
                    }
                    else if (IsMySql && string.IsNullOrEmpty(Port))
                    {
                        Port = DefaultMySqlPort; // MySQL 默认端口
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
                        Port = DefaultSqlServerPort; // SQL Server 默认端口
                    }
                    else if (IsMySql && string.IsNullOrEmpty(Port))
                    {
                        Port = DefaultMySqlPort; // MySQL 默认端口
                    }
                }
                ; 
            }
        }

        private string? _databaseProvider;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string? DatabaseProvider
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

        private string? _server;
        public string? Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); }
        }

        private string? _port;
        /// <summary>
        /// 端口号
        /// </summary>
        public string? Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }

        private string? _database;
        public string? Database
        {
            get { return _database; }
            set { SetProperty(ref _database, value); }
        }

        private string? _userId;
        public string? UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }

        private string? _password;
        public string? Password
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

            // 添加这行代码来初始化 _configFilePath
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            // 先初始化为一个空的配置
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            // 然后加载实际配置
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
                        if(databaseProvider != null)
                            ParseConnectionString(connectionString, databaseProvider);
                        else
                        {
                            SetMessage("DatabaseProvider is null");
                            return;
                        }
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
                    SetMessage("已创建默认配置文件，请配置数据库连接信息");
                }
            }
            catch (Exception ex)
            {
                SetMessage($"加载配置文件失败: {ex.Message}");
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
                        Port = DefaultMySqlPort;
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
                        Port = DefaultSqlServerPort;
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
                    new JProperty("DatabaseProvider", "SqlServer"), // 添加默认数据库类型
                    CreateDefaultLoggingConfig()
                );

                if(_configFilePath == null)
                {
                    throw new Exception("_configFilePath为空。");
                }
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
        private void TestConnection(PasswordBox box)
        {
            try
            {
                SetMessage("正在测试连接...");

                Password = box.Password;

                if (!ValidateConnectionParameters())
                    return;

                var connectionString = BuildConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception("连接字符串为空。");
                // 调用 TestDatabaseConnection 但不需要检查返回值，因为它会自己设置消息
                TestDatabaseConnection(connectionString);
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

                if (!ValidateConnectionParameters())
                    return;

                var connectionString = BuildConnectionString();

                if (connectionString == null)
                    throw new Exception("连接字符串中有空值");

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
                if (_configFilePath == null)
                {
                    throw new Exception("_configFilePath为空。");
                }
                // 读取现有配置或创建新配置
                JObject config;
                if (File.Exists(_configFilePath))
                {
                    var configJson = File.ReadAllText(_configFilePath);
                    config = JsonConvert.DeserializeObject<JObject>(configJson) ?? new JObject();
                }
                else
                {
                    config = new JObject();
                }

                // 更新或添加连接字符串和数据库类型
                if (config["ConnectionStrings"] is not JObject connectionStrings)
                {
                    connectionStrings = new JObject();
                    config["ConnectionStrings"] = connectionStrings;
                }

                connectionStrings["DefaultConnection"] = connectionString;
                config["DatabaseProvider"] = DatabaseProvider;

                // 确保有日志配置
                if (config["Logging"] == null)
                {
                    config["Logging"] = CreateDefaultLoggingConfig();
                }

                // 保存回文件
                File.WriteAllText(_configFilePath, config.ToString(Formatting.Indented));

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
        private string? BuildConnectionString()
        {
            if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Database) ||
                string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Password))
            {
                return null;
            }
            if (IsMySql)
            {
                // MySQL 连接字符串格式
                return $"Server={Server};Port={Port};Database={Database};User ID={UserId};Password={Password};Connection Timeout=30;";
            }
            else
            {
                // SQL Server 连接字符串格式
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = string.IsNullOrEmpty(Port) || Port == DefaultSqlServerPort ?
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

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private bool TestDatabaseConnection(string connectionString)
        {
            try
            {
                if (IsMySql)
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        SetMessage("MySQL 连接成功!"); // 添加成功消息
                        return true;
                    }
                }
                else
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SetMessage("SQL Server 连接成功!"); // 添加成功消息
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage($"连接失败: {ex.Message}"); // 提供更详细的错误信息
                return false;
            }
        }

        private bool ValidateConnectionParameters()
        {
            if (string.IsNullOrWhiteSpace(Server))
            {
                SetMessage("服务器地址不能为空");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Database))
            {
                SetMessage("数据库名称不能为空");
                return false;
            }

            if (string.IsNullOrWhiteSpace(UserId))
            {
                SetMessage("用户名不能为空");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                SetMessage("密码不能为空");
                return false;
            }
            if (string.IsNullOrEmpty(Port))
            {
                SetMessage("端口号不能为空");
                return false;

            }
            else if(!ValidatePort(Port))
            {
                SetMessage("端口号超出范围(1~65535)");
                return false;
            }


            return true;
        }

        private JObject CreateDefaultLoggingConfig()
        {
            return new JObject(
                new JProperty("LogLevel",
                    new JObject(
                        new JProperty("Default", "Information"),
                        new JProperty("Microsoft", "Warning"),
                        new JProperty("Microsoft.Hosting.Lifetime", "Information")
                    )
                )
            );
        }

        // 验证端口号，范围限定在 0~65535
        private bool ValidatePort(string port)
        {
            if (int.TryParse(port, out int portNumber))
            {
                return portNumber > 0 && portNumber <= 65535;
            }
            return false;
        }

        #endregion
    }
}
