using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WEB_API.Entities;

public partial class WebApiClass3Context : DbContext
{
    public WebApiClass3Context()
    {
    }

    public WebApiClass3Context(DbContextOptions<WebApiClass3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CityShipping> CityShippings { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Manage> Manages { get; set; }

    public virtual DbSet<MethodPayment> MethodPayments { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Shipping> Shippings { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<User2> User2s { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-DKL7C0F\\SQLEXPRESS;Database=web_api_class3;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__carts__3213E83FD8B7562A");

            entity.ToTable("carts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BuyQty)
                .HasDefaultValueSql("((1))")
                .HasColumnName("buy_qty");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__carts__product_i__44FF419A");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__carts__user_id__440B1D61");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83F087830B3");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "UQ__categori__72E12F1B509A6EAE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CityShipping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__City_shi__3213E83F8645F55D");

            entity.ToTable("City_shipping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PriceShipping)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("price_shipping");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceNo).HasName("PK__Invoice__D796B226FE06C70A");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceNo).HasMaxLength(50);
            entity.Property(e => e.City)
                .HasMaxLength(250)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(250)
                .HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(250)
                .HasColumnName("status");
            entity.Property(e => e.TotalMoney)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("total_money");
        });

        modelBuilder.Entity<Manage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__manage__3213E83F5767A07A");

            entity.ToTable("manage");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<MethodPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MethodPa__3213E83FB95AFB62");

            entity.ToTable("MethodPayment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__orders__3213E83F6673E3F4");

            entity.ToTable("orders");

            entity.HasIndex(e => e.InvoiceId, "UQ__orders__F58DFD48DD104A48").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GrandTotal)
                .HasColumnType("decimal(14, 2)")
                .HasColumnName("grand_total");
            entity.Property(e => e.IdCityShip).HasColumnName("id_city_ship");
            entity.Property(e => e.InvoiceId)
                .HasMaxLength(50)
                .HasColumnName("invoice_id");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.ShipingId).HasColumnName("shiping_id");
            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("shipping_address");
            entity.Property(e => e.StatusId)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status_id");
            entity.Property(e => e.Tel)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tel");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.IdCityShipNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdCityShip)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_city_shipping");

            entity.HasOne(d => d.Invoice).WithOne(p => p.Order)
                .HasForeignKey<Order>(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_invoice");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_payment_method");

            entity.HasOne(d => d.Shiping).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShipingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__orders__shiping___7A672E12");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_status");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__orders__user_id__4AB81AF0");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_pr__3213E83FB8E44CFD");

            entity.ToTable("order_products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BuyQty)
                .HasDefaultValueSql("((1))")
                .HasColumnName("buy_qty");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(14, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order_pro__order__4F7CD00D");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__order_pro__produ__4E88ABD4");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__products__3213E83FC4E81B9A");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(14, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("thumbnail");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__products__catego__3D5E1FD2");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3213E83FFE088589");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExpiredAt)
                .HasColumnType("datetime")
                .HasColumnName("expiredAt");
            entity.Property(e => e.IsRevoked)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isRevoked");
            entity.Property(e => e.IsUsed)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isUsed");
            entity.Property(e => e.IsUsedAt)
                .HasColumnType("datetime")
                .HasColumnName("isUsedAt");
            entity.Property(e => e.JwtId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("jwtId");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__userI__5EBF139D");
        });

        modelBuilder.Entity<Shipping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipping__3213E83FA2D865A1");

            entity.ToTable("Shipping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Status__3213E83F81A42E43");

            entity.ToTable("Status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FDBD3E2B0");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E616417A291E7").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("fullname");
            entity.Property(e => e.Tel)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tel");
        });

        modelBuilder.Entity<User2>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User2__3213E83F38B6B9EB");

            entity.ToTable("User2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Cỉty)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cỉty");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Telephone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telephone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
