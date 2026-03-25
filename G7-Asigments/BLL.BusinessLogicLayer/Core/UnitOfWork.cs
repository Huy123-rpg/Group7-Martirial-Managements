using DAL.DataAccessLayer.Context;
using DAL.DataAccessLayer.Models;
using DAL.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BLL.BusinessLogicLayer.Core;

public sealed class UnitOfWork
{
    // ─── Connection String (set once by App before first use) ────────────────
    public static string ConnectionString { get; set; } =
        "Server=.\\SQLEXPRESS;Database=WarehouseDB;User Id=sa;Password=123;TrustServerCertificate=True;";

    // ─── Singleton ───────────────────────────────────────────────────────────
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

    // ─── DbContext ───────────────────────────────────────────────────────────
    private readonly WarehouseDbContext _context;
    public WarehouseDbContext Context => _context;

    private UnitOfWork()
    {
        var options = new DbContextOptionsBuilder<WarehouseDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        _context = new WarehouseDbContext(options);
    }

    // ─── Repositories ────────────────────────────────────────────────────────
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
    public IRepository<Notification> Notifications => new Repository<Notification>(_context);
    public IRepository<LkpScheduleType>  ScheduleTypes  => new Repository<LkpScheduleType>(_context);
    public IRepository<WarehouseZone>    WarehouseZones => new Repository<WarehouseZone>(_context);
    public IRepository<LkpZoneType>      ZoneTypes      => new Repository<LkpZoneType>(_context);
    public IRepository<LkpCostingMethod> CostingMethods => new Repository<LkpCostingMethod>(_context);

    // ─── Save ────────────────────────────────────────────────────────────────
    public int Save()
    {
        try
        {
            return _context.SaveChanges();
        }
        catch
        {
            // Detach all pending changes so the context is usable after failure.
            foreach (var entry in _context.ChangeTracker.Entries()
                         .Where(e => e.State != Microsoft.EntityFrameworkCore.EntityState.Unchanged &&
                                     e.State != Microsoft.EntityFrameworkCore.EntityState.Detached)
                         .ToList())
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            throw;
        }
    }
}
