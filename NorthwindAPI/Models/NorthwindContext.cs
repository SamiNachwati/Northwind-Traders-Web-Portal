using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NorthwindAPI.Models;

public partial class NorthwindContext : DbContext
{
    public NorthwindContext()
    {
    }

    public NorthwindContext(DbContextOptions<NorthwindContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=northwind");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryName, "category_name");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(15)
                .HasColumnName("category_name");
            entity.Property(e => e.Description)
                .HasMaxLength(75)
                .HasColumnName("description");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasIndex(e => e.CategoryId, "categories_products");

            entity.HasIndex(e => e.CategoryId, "category_id");

            entity.HasIndex(e => e.ProductName, "product_name");

            entity.HasIndex(e => e.SupplierId, "supplier_id");

            entity.HasIndex(e => e.SupplierId, "suppliers_products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Discontinued).HasColumnName("discontinued");
            entity.Property(e => e.ProductName)
                .HasMaxLength(40)
                .HasColumnName("product_name");
            entity.Property(e => e.QuantityPerUnit)
                .HasMaxLength(20)
                .HasColumnName("quantity_per_unit");
            entity.Property(e => e.ReorderLevel)
                .HasDefaultValueSql("((0))")
                .HasColumnName("reorder_level");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.UnitPrice)
                .HasDefaultValueSql("((0))")
                .HasColumnType("money")
                .HasColumnName("unit_price");
            entity.Property(e => e.UnitsInStock)
                .HasDefaultValueSql("((0))")
                .HasColumnName("units_in_stock");
            entity.Property(e => e.UnitsOnOrder)
                .HasDefaultValueSql("((0))")
                .HasColumnName("units_on_order");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_products_categories");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
