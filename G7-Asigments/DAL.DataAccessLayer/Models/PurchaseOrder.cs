namespace DAL.DataAccessLayer.Models;

public partial class PurchaseOrder
{
    public string? SupplierName => Supplier?.SupplierName;
    public string? WarehouseName => Warehouse?.Name;
    public string StatusText => Status?.StatusName ?? string.Empty;
}
