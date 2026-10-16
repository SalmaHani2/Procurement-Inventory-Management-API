using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProcurementApi.Domain.Entities;
using ProcurementApi.Infrastructure.Identity;
using System.Reflection.Emit;

namespace ProcurementApi.Infrastructure.Persistence
{
    public class ProcurementDbContext(DbContextOptions<ProcurementDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Supplier> Suppliers => Set<Supplier>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

        public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();

        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(s => s.Name).HasMaxLength(200).IsRequired();
                entity.Property(s => s.ContactEmail).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Sku).HasMaxLength(50).IsRequired();
                entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
                entity.Property(p => p.UnitPrice).HasColumnType("decimal(12,2)");
                entity.HasIndex(p => p.Sku).IsUnique();
                entity.Ignore(p => p.NeedsReorder);
                entity.HasOne(p => p.Supplier)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.Property(po => po.OrderNumber).HasMaxLength(30).IsRequired();
                entity.Property(po => po.Status).HasConversion<string>().HasMaxLength(20);
                entity.Property(po => po.RejectionReason).HasMaxLength(500);
                entity.HasIndex(po => po.OrderNumber).IsUnique();
                entity.Ignore(po => po.Total);
                entity.HasOne(po => po.Supplier)
                    .WithMany()
                    .HasForeignKey(po => po.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseOrderLine>(entity =>
            {
                entity.Property(l => l.UnitPrice).HasColumnType("decimal(12,2)");
                entity.Ignore(l => l.LineTotal);
                entity.HasOne(l => l.PurchaseOrder)
                    .WithMany(po => po.Lines)
                    .HasForeignKey(l => l.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(l => l.Product)
                    .WithMany()
                    .HasForeignKey(l => l.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.Property(t => t.TransactionType).HasConversion<string>().HasMaxLength(20);
                entity.Property(t => t.Notes).HasMaxLength(500);
                entity.HasOne(t => t.Product)
                    .WithMany()
                    .HasForeignKey(t => t.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(t => t.PurchaseOrder)
                    .WithMany()
                    .HasForeignKey(t => t.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}