using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Core.Models
{
    /// <summary>
    /// 书皮
    /// </summary>
    public class BookCover
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string CoverColor { get; set; } = "Gray";
        public string Foreground { get; set; } = "White";
    }
}
