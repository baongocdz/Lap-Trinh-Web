using System;
using System.Collections.Generic;
using CUOIKY.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace CUOIKY.Repository;

public partial class LTWDbContext : DbContext
{
    public LTWDbContext()
    {
    }

    public LTWDbContext(DbContextOptions<LTWDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Description> Descriptions { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=NGOCDZ\\SQLEXPRESS02;Initial Catalog=LTW;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Description>(entity =>
        {
            entity.HasKey(e => e.DescriptionId).HasName("PK__DESCRIPT__A58A9FEBD498DBF4");

            entity.ToTable("DESCRIPTION");

            entity.Property(e => e.DescriptionId).HasColumnName("DescriptionID");
            entity.Property(e => e.Description1)
                .HasMaxLength(500)
                .HasColumnName("Description");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.Descriptions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DESCRIPTI__Produ__4E88ABD4");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__IMAGE__7516F4EC67F063F4");

            entity.ToTable("IMAGE");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.DescriptionId).HasColumnName("DescriptionID");
            entity.Property(e => e.ImageName).HasMaxLength(255);

            entity.HasOne(d => d.Description).WithMany(p => p.Images)
                .HasForeignKey(d => d.DescriptionId)
                .HasConstraintName("FK__IMAGE__Descripti__5812160E");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__ORDER__C3905BAFDB686A18");

            entity.ToTable("ORDER");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.TotalPrice).HasColumnType("money");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ORDER_PRODUCT");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ORDER__UserID__5165187F");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__PRODUCT__B40CC6ED85A56880");

            entity.ToTable("PRODUCT");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.DescriptionId).HasColumnName("DescriptionID");
            entity.Property(e => e.Gmail).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Description).WithMany(p => p.Products)
                .HasForeignKey(d => d.DescriptionId)
                .HasConstraintName("FK__PRODUCT__Descrip__4D94879B");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("FK__PRODUCT__Product__4CA06362");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("PK__ProductT__A1312F4EA4810B21");

            entity.ToTable("ProductType");

            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.ProductTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLE__8AFACE3AFD0D7CD7");

            entity.ToTable("ROLE");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USER__1788CCAC0D7A0AF2");

            entity.ToTable("USER");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Balance).HasColumnType("money");
            entity.Property(e => e.Gmail).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.UserNameString).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__USER__RoleID__47DBAE45");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
