using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Core.Models
{
    /// <summary>
    /// 章节
    /// </summary>
    public class Chapter
    {
        public int Id { get; set; }
        public string? IdToString { get; set; }
        public string? Title { get; set; }
    }
}
