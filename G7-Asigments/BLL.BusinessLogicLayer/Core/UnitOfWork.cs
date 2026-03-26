using DAL.DataAccessLayer.Context;
using DAL.DataAccessLayer.Models;
using DAL.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using DbContextAlias = DAL.DataAccessLayer.Context.WarehouseDbContext;

namespace BLL.BusinessLogicLayer.Core;

public sealed class UnitOfWork
{
    private readonly WarehouseDbContext _context;

    public static string ConnectionString { get; set; } =
        "Server=.;Database=WarehouseDB;User Id=sa;Password=123;TrustServerCertificate=True;";

    private static UnitOfWork? _instance;
    private static readonly object _lock = new();

    public static UnitOfWork Instance
    {
        get
        {
            if (_instance == null)
                lock (_lock)
                    _instance ??= new UnitOfWork();
            return _instance;
        }
    }

    public WarehouseDbContext Context => _context;

    private UnitOfWork()
    {
        var options = new DbContextOptionsBuilder<WarehouseDbContext>()
            .UseSqlServer(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        _context = new WarehouseDbContext(options);
    }

    public IRepository<User> Users => new Repository<User>(_context);
    public IRepository<LkpUserRole> UserRoles => new Repository<LkpUserRole>(_context);
    public IRepository<Warehouse> Warehouses => new Repository<Warehouse>(_context);
    public IRepository<Product> Products => new Repository<Product>(_context);
    public IRepository<Supplier> Suppliers => new Repository<Supplier>(_context);
    public IRepository<Customer> Customers => new Repository<Customer>(_context);
    public IRepository<Inventory> Inventories => new Repository<Inventory>(_context);
    public IRepository<Schedule> Schedules => new Repository<Schedule>(_context);
    public IRepository<PurchaseOrder> PurchaseOrders => new Repository<PurchaseOrder>(_context);
    public IRepository<GoodsReceipt> GoodsReceipts => new Repository<GoodsReceipt>(_context);
    public IRepository<SalesOrder> SalesOrders => new Repository<SalesOrder>(_context);
    public IRepository<GoodsIssue> GoodsIssues => new Repository<GoodsIssue>(_context);
    public IRepository<GoodsIssueItem> GoodsIssueItems => new Repository<GoodsIssueItem>(_context);
    public IRepository<GoodsReceiptItem> GoodsReceiptItems => new Repository<GoodsReceiptItem>(_context);
    public IRepository<PurchaseOrderItem> PurchaseOrderItems => new Repository<PurchaseOrderItem>(_context);
    public IRepository<SalesOrderItem> SalesOrderItems => new Repository<SalesOrderItem>(_context);
    public IRepository<Notification> Notifications => new Repository<Notification>(_context);
    public IRepository<LkpScheduleType>  ScheduleTypes   => new Repository<LkpScheduleType>(_context);
    public IRepository<WarehouseZone>    WarehouseZones  => new Repository<WarehouseZone>(_context);
    public IRepository<LkpZoneType>      ZoneTypes       => new Repository<LkpZoneType>(_context);
    public IRepository<LkpCostingMethod> CostingMethods  => new Repository<LkpCostingMethod>(_context);
    public IRepository<WarehouseStaff>     WarehouseStaffs    => new Repository<WarehouseStaff>(_context);
    public IRepository<StockTransaction>   StockTransactions  => new Repository<StockTransaction>(_context);

    // ─── Save ────────────────────────────────────────────────────────────────
    public int Save()
    {
        try
        {
            return _context.SaveChanges();
        }
        finally
        {
            _context.ChangeTracker.Clear(); // Luôn xoá cache để tránh "kẹt" lệnh lỗi
        }
    }
}