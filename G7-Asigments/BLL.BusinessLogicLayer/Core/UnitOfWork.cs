using DAL.DataAccessLayer.Context;
using DAL.DataAccessLayer.Models._Core;
using DAL.DataAccessLayer.Models._Export;
using DAL.DataAccessLayer.Models._Import;
using DAL.DataAccessLayer.Models._Lookup;
using DAL.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BLL.BusinessLogicLayer.Core;

public sealed class UnitOfWork
{
    // ─── Connection String (set once by App before first use) ────────────────
    public static string ConnectionString { get; set; } =
        "Server=localhost;Database=WarehouseDB;User Id=sa;Password=123;TrustServerCertificate=True;";

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

    private UnitOfWork()
    {
        var options = new DbContextOptionsBuilder<WarehouseDbContext>()
            .UseSqlServer(ConnectionString)
            .UseSnakeCaseNamingConvention()
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
    public IRepository<Schedule> Schedules => new Repository<Schedule>(_context);
    public IRepository<PurchaseOrder> PurchaseOrders => new Repository<PurchaseOrder>(_context);
    public IRepository<GoodsReceipt> GoodsReceipts => new Repository<GoodsReceipt>(_context);
    public IRepository<SalesOrder> SalesOrders => new Repository<SalesOrder>(_context);
    public IRepository<GoodsIssue> GoodsIssues => new Repository<GoodsIssue>(_context);

    // ─── Save ────────────────────────────────────────────────────────────────
    public int Save() => _context.SaveChanges();
}
