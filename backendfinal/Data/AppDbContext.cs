using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backendfinal.Entities;

namespace backendfinal.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aisle> Aisles { get; set; }

    public virtual DbSet<Bay> Bays { get; set; }

    public virtual DbSet<Bin> Bins { get; set; }

    public virtual DbSet<BinLocation> BinLocations { get; set; }

    public virtual DbSet<BinType> BinTypes { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemShipment> ItemShipments { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<Shelf> Shelves { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<ShipmentAction> ShipmentActions { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("warehouse", "alphabet_enum", new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });

        modelBuilder.Entity<Aisle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aisle_pkey");
        });

        modelBuilder.Entity<Bay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bay_pkey");

            entity.HasOne(d => d.Aisle).WithMany(p => p.Bays).HasConstraintName("bay_aisle_fkey");
        });

        modelBuilder.Entity<Bin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bin_pkey");

            entity.HasOne(d => d.BinTypeNavigation).WithMany(p => p.Bins).HasConstraintName("bin_bintype_fkey");
        });

        modelBuilder.Entity<BinLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bay_location_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('warehouse.bay_location_id_seq'::regclass)");

            entity.HasOne(d => d.Bin).WithMany(p => p.BinLocations).HasConstraintName("bay_location_bin_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.BinLocations).HasConstraintName("bin_location_item_id_fkey");

            entity.HasOne(d => d.Shelf).WithMany(p => p.BinLocations).HasConstraintName("bay_location_shelf_fkey");
        });

        modelBuilder.Entity<BinType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bin_type_pkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_pkey");

            entity.HasOne(d => d.Vendor).WithMany(p => p.Items).HasConstraintName("vendor_id_fk");
        });

        modelBuilder.Entity<ItemShipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_shipment_pkey");

            entity.HasOne(d => d.Action).WithMany(p => p.ItemShipments).HasConstraintName("action_id_fk");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemShipments).HasConstraintName("item_id_fk");

            entity.HasOne(d => d.Shipment).WithMany(p => p.ItemShipments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shipment_id_fk");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_item_pkey");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderItems).HasConstraintName("order_item_item_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("order_item_order_id_fkey");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('warehouse.order_id_seq'::regclass)");
            entity.Property(e => e.DateOrdered).HasDefaultValueSql("now()");
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<Shelf>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shelf_pkey");

            entity.HasOne(d => d.Bay).WithMany(p => p.Shelves).HasConstraintName("shelf_bay_fkey");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shipment_pkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Shipments).HasConstraintName("order_id_fk");
        });

        modelBuilder.Entity<ShipmentAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("action_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('warehouse.action_id_seq'::regclass)");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vendor_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
