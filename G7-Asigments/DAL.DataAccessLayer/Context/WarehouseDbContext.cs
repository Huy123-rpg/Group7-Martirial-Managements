using DAL.DataAccessLayer.Models._Core;
using DAL.DataAccessLayer.Models._Export;
using DAL.DataAccessLayer.Models._Import;
using DAL.DataAccessLayer.Models._Lookup;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataAccessLayer.Context;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options) { }

    // Core
    public DbSet<User> Users { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<WarehouseZone> WarehouseZones { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Schedule> Schedules { get; set; }

    // Import
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
    public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }

    // Export
    public DbSet<SalesOrder> SalesOrders { get; set; }
    public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
    public DbSet<GoodsIssue> GoodsIssues { get; set; }
    public DbSet<GoodsIssueItem> GoodsIssueItems { get; set; }
    public DbSet<DeliveryOrder> DeliveryOrders { get; set; }

    // Lookup
    public DbSet<LkpUserRole> LkpUserRoles { get; set; }
    public DbSet<LkpDocumentStatus> LkpDocumentStatuses { get; set; }
    public DbSet<LkpScheduleType> LkpScheduleTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // TODO: Scaffold sẽ override file này sau khi chạy DB-first
    }
}
