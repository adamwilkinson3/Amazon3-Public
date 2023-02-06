using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Amazon3.Models
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> Orders { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseCosmos(
        "",
        "",
        databaseName: "");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // configuring Employees
            builder.Entity<Product>()
                    .ToContainer("Products") // ToContainer
                    .HasPartitionKey(e => e.ProductId); // Partition Key
            builder.Entity<OrderItem>()
           .ToContainer("Orders") // ToContainer
           .HasPartitionKey(e => e.OrderId) // Partition Key
           .Property(e => e.OrderId) 
           .ValueGeneratedOnAdd(); 

            builder.Entity<IdentityRole>()
            .Property(b => b.ConcurrencyStamp)
            .IsETagConcurrency();
            builder.Entity<IdentityUser>()
                .Property(b => b.ConcurrencyStamp)
                .IsETagConcurrency();




        }

        
    }
}