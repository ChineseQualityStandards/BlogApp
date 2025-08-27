using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Core.Models
{
    /// <summary>
    /// 书本
    /// </summary>
    public class Book
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 书名
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// 作者Id
        /// </summary>
        public int AuthorId {  get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string? Author {  get; set; }
        /// <summary>
        /// 封面颜色
        /// </summary>
        public string? Color { get; set; } = "#FFFFFFFF";
        /// <summary>
        /// 字体颜色
        /// </summary>
        public string? Foreground { get; set; } = "#FF000000";
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedDate { get; set; }
    }
}
