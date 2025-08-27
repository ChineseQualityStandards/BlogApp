using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlogApp.Core.Models;
using BlogApp.Core.Properties;

namespace BlogApp.Core.AppSessions
{
    /// <summary>
    /// 静态数据 - 用于处理持久化数据
    /// </summary>
    public class AppSession
    {
        #region User对象

        public static User? User 
        {
            get => DeserializeUser(Settings.Default.CurrentUser);
            set
            {
                Settings.Default.CurrentUser = SerializeUser(value);
                Settings.Default.Save();
            } 
        }

        /// <summary>
        /// 设置用户会话
        /// </summary>
        /// <param name="user">用户对象</param>
        public static void SetUser(User user)
        {
            User = user;
        }

        /// <summary>
        /// 清除用户会话
        /// </summary>
        public static void ClearUser()
        {
            User = null;
        }

        /// <summary>
        /// 序列化用户对象为JSON字符串
        /// </summary>
        public static string SerializeUser(User? user)
        {
            if (user == null)
                return string.Empty;
            return JsonSerializer.Serialize(user);
        }

        /// <summary>
        /// 从JSON字符串反序列化为用户对象
        /// </summary>
        private static User? DeserializeUser(string json) 
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<User>(json);
        }

        #endregion

        #region Book对象

        public static Book? Book
        {
            get => DeserializeBook(Settings.Default.CurrentBook);
            set
            {
                Settings.Default.CurrentBook = SerializeBook(value);
                Settings.Default.Save();
            }
        }

        /// <summary>
        /// 设置书本会话
        /// </summary>
        /// <param name="user">书籍</param>
        public static void SetBook(Book book)
        {
            Book = book;
        }

        /// <summary>
        /// 清除用户会话
        /// </summary>
        public static void ClearBook()
        {
            Book = null;
        }

        /// <summary>
        /// 序列化用户对象为JSON字符串
        /// </summary>
        public static string SerializeBook(Book? book)
        {
            if (book == null)
                return string.Empty;
            return JsonSerializer.Serialize(book);
        }

        /// <summary>
        /// 从JSON字符串反序列化为用户对象
        /// </summary>
        private static Book? DeserializeBook(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<Book>(json);
        }
        
        #endregion
    }
}
