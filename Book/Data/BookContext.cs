using Microsoft.EntityFrameworkCore;
using BookAPI.Models;

namespace BookAPI.Data
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions contextOptions) : base(contextOptions) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Book> Books {get;set;}
        public DbSet<User> Users { get; set; }
        public DbSet<OrderBook> Orders { get; set; }
    }
}
