using DAL.DataAccessLayer.Context;
using DAL.DataAccessLayer.Model;
using DAL.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using DbContextAlias = DAL.DataAccessLayer.Context.WarehouseDbContext;

namespace BLL.BusinessLogicLayer.Core;

public sealed class UnitOfWork
{
    private readonly WarehouseDbContext _context;

    public static string ConnectionString { get; set; } =
        "Server=localhost;Database=WarehouseDB;User Id=sa;Password=hoanganh;TrustServerCertificate=True;";

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
    public IRepository<Schedule> Schedules => new Repository<Schedule>(_context);
    public IRepository<PurchaseOrder> PurchaseOrders => new Repository<PurchaseOrder>(_context);
    public IRepository<GoodsReceipt> GoodsReceipts => new Repository<GoodsReceipt>(_context);
    public IRepository<SalesOrder> SalesOrders => new Repository<SalesOrder>(_context);
    public IRepository<GoodsIssue> GoodsIssues => new Repository<GoodsIssue>(_context);

    public int Save() => _context.SaveChanges();
}