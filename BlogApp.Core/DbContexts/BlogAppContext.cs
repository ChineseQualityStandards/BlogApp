using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BlogApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Core.DbContexts
{

    public class BlogAppContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Article> Articles => Set<Article>();

        public BlogAppContext(DbContextOptions<BlogAppContext> dbContext) : base(dbContext) 
        {

        }

    }
}
