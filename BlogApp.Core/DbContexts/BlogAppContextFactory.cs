using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlogApp.Core.Models
{
    namespace BlogApp.Core.DbContexts
    {
        /// <summary>
        /// context工厂类：用于实现 IDbContextFactory<BlogAppContext>
        /// </summary>
        public class BlogAppContextFactory(DbContextOptions<BlogAppContext> options) : IDbContextFactory<BlogAppContext>
        {
            private readonly DbContextOptions<BlogAppContext> _options = options;

            public BlogAppContext CreateDbContext() => new(_options);
        }
    }
}
