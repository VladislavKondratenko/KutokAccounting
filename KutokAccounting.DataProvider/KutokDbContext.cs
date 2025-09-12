    using KutokAccounting.DataProvider.Configurations;
using Microsoft.EntityFrameworkCore;
using KutokAccounting.DataProvider.Models;

namespace KutokAccounting.DataProvider;

public class KutokDbContext : DbContext
{
    public DbSet<Store> Stores { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    
    public KutokDbContext(DbContextOptions<KutokDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StoreConfiguration());
        modelBuilder.ApplyConfiguration(new VendorConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionTypeConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
    }
}