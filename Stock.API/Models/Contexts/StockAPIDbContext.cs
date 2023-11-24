using Microsoft.EntityFrameworkCore;

namespace Stock.API.Models.Contexts
{
    public class StockAPIDbContext : DbContext
    {
        public StockAPIDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>().HasData(

                new Stock
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Quantity = 5,
                    CreatedDate = DateTime.Now

                },
                new Stock
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Quantity = 55,
                    CreatedDate = DateTime.Now

                },
                new Stock
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Quantity = 22,
                    CreatedDate = DateTime.Now

                });
        }
    }
}
