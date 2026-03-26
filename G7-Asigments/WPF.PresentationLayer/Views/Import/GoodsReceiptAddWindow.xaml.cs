using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Import;
using DAL.DataAccessLayer.Models;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Import;

public partial class GoodsReceiptAddWindow : Window
{
    private readonly IGoodsReceiptService _service;
    private readonly UnitOfWork _uow;
    private GoodsReceipt? _editingReceipt;
    private PurchaseOrder? _sourcePo;

    public ObservableCollection<GoodsReceiptItemInput> Items { get; set; } = new();
    public List<Product> Products { get; set; } = new();

    public GoodsReceiptAddWindow()
    {
        InitializeComponent();
        DataContext = this;

        _service = new GoodsReceiptService();
        _uow = UnitOfWork.Instance;

        dpDate.SelectedDate = DateTime.Now;
        txtCode.Text = GenerateReceiptCode();

        LoadData();
        SetupItemGrid();
    }

    public GoodsReceiptAddWindow(PurchaseOrder po)
    {
        InitializeComponent();
        DataContext = this;
        _service = new GoodsReceiptService();
        _uow = UnitOfWork.Instance;
        _sourcePo = po;

        dpDate.SelectedDate = DateTime.Now;
        txtCode.Text = GenerateReceiptCode();

        LoadData();
        SetupItemGrid();

        cbSupplier.SelectedValue = po.SupplierId;
        cbWarehouse.SelectedValue = po.WarehouseId;

        var poItems = _uow.PurchaseOrderItems.GetAll()
            .Where(x => x.PoId == po.Id).ToList();
        foreach (var poItem in poItems)
        {
            var product = Products.FirstOrDefault(p => p.Id == poItem.ProductId);
            Items.Add(new GoodsReceiptItemInput
            {
                ProductId = poItem.ProductId,
                ProductName = product?.ProductName ?? "",
                QtyOrdered = poItem.QtyOrdered,
                QtyReceived = poItem.QtyOrdered,
                UnitCost = poItem.UnitCost,
                Notes = poItem.Notes
            });
        }
        RefreshTotal();
        LockFieldsFromPo(po);
    }

    public GoodsReceiptAddWindow(GoodsReceipt receipt)
    {
        InitializeComponent();
        DataContext = this;

        _service = new GoodsReceiptService();
        _uow = UnitOfWork.Instance;
        _editingReceipt = receipt;

        LoadData();
        SetupItemGrid();

        dpDate.SelectedDate = receipt.ReceiptDate.ToDateTime(TimeOnly.MinValue);
        txtCode.Text = receipt.GrnNumber;
        txtNote.Text = receipt.Notes;
        cbWarehouse.SelectedValue = receipt.WarehouseId;
        cbSupplier.SelectedValue = receipt.SupplierId;

        var detailItems = _uow.GoodsReceiptItems.GetAll()
            .Where(x => x.GrnId == receipt.Id)
            .ToList();

        foreach (var item in detailItems)
        {
            var product = Products.FirstOrDefault(p => p.Id == item.ProductId);

            Items.Add(new GoodsReceiptItemInput
            {
                ProductId = item.ProductId,
                ProductName = product?.ProductName ?? "",
                QtyReceived = item.QtyReceived,
                UnitCost = item.UnitCost,
                Notes = item.Notes
            });
        }

        RefreshTotal();

        if (receipt.PoId.HasValue)
        {
            var po = _uow.PurchaseOrders.GetById(receipt.PoId.Value);
            if (po != null)
            {
                var poItemQty = _uow.PurchaseOrderItems.GetAll()
                    .Where(x => x.PoId == po.Id)
                    .ToDictionary(x => x.ProductId, x => x.QtyOrdered);
                foreach (var item in Items)
                {
                    if (poItemQty.TryGetValue(item.ProductId, out var qty))
                        item.QtyOrdered = qty;
                }
                LockFieldsFromPo(po);
            }
        }
    }

    private void LoadData()
    {
        cbSupplier.ItemsSource = _uow.Suppliers.GetAll().ToList();
        cbWarehouse.ItemsSource = _uow.Warehouses.GetAll().ToList();
        Products = _uow.Products.GetAll().ToList();
    }

    private void SetupItemGrid()
    {
        Items.CollectionChanged += Items_CollectionChanged;
        dgItems.ItemsSource = Items;

        dgItems.CellEditEnding += (s, e) =>
            Dispatcher.BeginInvoke(new Action(RefreshTotal));
    }

    private void LockFieldsFromPo(PurchaseOrder po)
    {
        // Show PO reference banner
        pnlPoRef.Visibility = Visibility.Visible;
        txtPoRef.Text = po.PoNumber;

        var supplier = _uow.Suppliers.GetById(po.SupplierId);
        txtPoSupplier.Text = supplier?.SupplierName ?? "";

        var warehouse = _uow.Warehouses.GetById(po.WarehouseId);
        txtPoWarehouse.Text = warehouse?.Name ?? "";

        // Lock fields that come from PO
        cbSupplier.IsEnabled = false;
        cbWarehouse.IsEnabled = false;
        pnlItemButtons.Visibility = Visibility.Collapsed;

        // Lock product selection in grid (products are fixed from PO)
        dgItems.IsReadOnly = false; // still allow qty/price edit
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (INotifyPropertyChanged item in e.OldItems)
                item.PropertyChanged -= Item_PropertyChanged;
        }
        if (e.NewItems != null)
        {
            foreach (INotifyPropertyChanged item in e.NewItems)
                item.PropertyChanged += Item_PropertyChanged;
        }
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GoodsReceiptItemInput.ProductId) && sender is GoodsReceiptItemInput item)
        {
            var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                item.ProductName = product.ProductName;
                // Xoá dòng gán giá tự động: item.UnitCost = product.StandardCost ?? 0;
            }
        }
        else if (e.PropertyName == nameof(GoodsReceiptItemInput.LineTotal))
        {
            RefreshTotal();
        }
    }

    private string GenerateReceiptCode()
    {
        return "GR" + DateTime.Now.ToString("yyyyMMddHHmmss");
    }

    private void BtnAddItem_Click(object sender, RoutedEventArgs e)
    {
        Items.Add(new GoodsReceiptItemInput
        {
            QtyReceived = 1,
            UnitCost = 0
        });

        RefreshTotal();
    }

    private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
    {
        if (dgItems.SelectedItem is GoodsReceiptItemInput selected)
        {
            Items.Remove(selected);
            RefreshTotal();
        }
    }

    private void RefreshTotal()
    {
        var total = Items.Sum(x => x.LineTotal);
        txtTotalAmount.Text = $"Tổng tiền: {total:N0}";
    }

    // Khi user gõ tay tên sản phẩm rồi click ra ngoài → lưu ProductName
    private void ProductComboBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is ComboBox cb && cb.DataContext is GoodsReceiptItemInput item)
        {
            var typed = cb.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(typed))
            {
                // Thử khớp với sản phẩm có sẵn
                var matched = Products.FirstOrDefault(p =>
                    p.ProductName.Equals(typed, StringComparison.OrdinalIgnoreCase));
                if (matched != null)
                {
                    item.ProductId = matched.Id;
                    item.ProductName = matched.ProductName;
                }
                else
                {
                    // Sản phẩm mới gõ tay
                    item.ProductId = Guid.Empty;
                    item.ProductName = typed;
                }
            }
        }
    }

    // Khi user chọn từ dropdown → gán tên sản phẩm
    private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox cb && cb.SelectedItem is Product selected
            && cb.DataContext is GoodsReceiptItemInput item)
        {
            item.ProductId = selected.Id;
            item.ProductName = selected.ProductName;
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var code = txtCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Vui lòng nhập mã phiếu.");
                return;
            }

            var existed = _uow.GoodsReceipts.GetAll()
                .Any(x => x.GrnNumber == code && (_editingReceipt == null || x.Id != _editingReceipt.Id));

            if (existed)
            {
                MessageBox.Show("Mã phiếu đã tồn tại.");
                return;
            }

            if (cbWarehouse.SelectedValue == null)
            {
                MessageBox.Show("Chọn kho.");
                return;
            }

            if (cbSupplier.SelectedValue == null)
            {
                MessageBox.Show("Chọn nhà cung cấp.");
                return;
            }

            if (Items.Count == 0)
            {
                MessageBox.Show("Phiếu nhập phải có ít nhất 1 dòng chi tiết.");
                return;
            }

            if (Items.Any(x => (x.ProductId == Guid.Empty && string.IsNullOrWhiteSpace(x.ProductName)) || x.QtyReceived <= 0))
            {
                MessageBox.Show("Kiểm tra lại sản phẩm và số lượng.");
                return;
            }

            var currentUser = SessionManager.CurrentUser;
            if (currentUser == null)
            {
                MessageBox.Show("Chưa đăng nhập.");
                return;
            }

            var total = Items.Sum(x => x.LineTotal);

            if (_editingReceipt != null)
            {
                _editingReceipt.GrnNumber = code;
                _editingReceipt.ReceiptDate = DateOnly.FromDateTime(dpDate.SelectedDate ?? DateTime.Now);
                _editingReceipt.TotalAmount = total;
                _editingReceipt.Notes = txtNote.Text?.Trim();
                _editingReceipt.UpdatedAt = DateTimeOffset.UtcNow;
                _editingReceipt.WarehouseId = (Guid)cbWarehouse.SelectedValue;
                _editingReceipt.SupplierId = (Guid)cbSupplier.SelectedValue;

                _service.Update(_editingReceipt);

                var oldItems = _uow.GoodsReceiptItems.GetAll()
                    .Where(x => x.GrnId == _editingReceipt.Id)
                    .ToList();

                foreach (var old in oldItems)
                {
                    _uow.GoodsReceiptItems.DeleteById(old.Id);
                }

                foreach (var item in Items)
                {
                    if (item.ProductId == Guid.Empty && !string.IsNullOrWhiteSpace(item.ProductName))
                    {
                        var newProd = new Product
                        {
                            Id = Guid.NewGuid(),
                            Sku = "SP-" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
                            Barcode = "BC-" + Guid.NewGuid().ToString("N")[..10].ToUpper(),
                            ProductName = item.ProductName.Trim(),
                            IsActive = true,
                            CreatedAt = DateTimeOffset.UtcNow,
                            UpdatedAt = DateTimeOffset.UtcNow,
                            StandardCost = item.UnitCost,
                            ReorderPoint = 0,
                            MinStock = 0,
                            SafetyStock = 0,
                            IsBatchTracked = false,
                            IsExpiryTracked = false
                        };
                        _uow.Products.Add(newProd);
                        item.ProductId = newProd.Id;
                    }
                }
                // Lưu sản phẩm mới trước khi thêm dòng chi tiết
                _uow.Save();

                foreach (var item in Items)
                {
                    _uow.GoodsReceiptItems.Add(new GoodsReceiptItem
                    {
                        Id = Guid.NewGuid(),
                        GrnId = _editingReceipt.Id,
                        ProductId = item.ProductId,
                        QtyReceived = item.QtyReceived,
                        QtyAccepted = item.QtyReceived,
                        QtyRejected = 0,
                        UnitCost = item.UnitCost,
                        LineTotal = item.LineTotal,
                        Notes = item.Notes
                    });
                }

                _uow.Save();
                MessageBox.Show("Cập nhật thành công!");
            }
            else
            {
                var receipt = new GoodsReceipt
                {
                    Id = Guid.NewGuid(),
                    GrnNumber = code,
                    ReceiptDate = DateOnly.FromDateTime(dpDate.SelectedDate ?? DateTime.Now),
                    TotalAmount = total,
                    Notes = txtNote.Text?.Trim(),
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = currentUser.Id,
                    WarehouseId = (Guid)cbWarehouse.SelectedValue,
                    SupplierId = (Guid)cbSupplier.SelectedValue,
                    StatusId = 1,
                    PoId = _sourcePo?.Id
                };

                _service.Create(receipt);

                foreach (var item in Items)
                {
                    if (item.ProductId == Guid.Empty && !string.IsNullOrWhiteSpace(item.ProductName))
                    {
                        var newProd = new Product
                        {
                            Id = Guid.NewGuid(),
                            Sku = "SP-" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
                            Barcode = "BC-" + Guid.NewGuid().ToString("N")[..10].ToUpper(),
                            ProductName = item.ProductName.Trim(),
                            IsActive = true,
                            CreatedAt = DateTimeOffset.UtcNow,
                            UpdatedAt = DateTimeOffset.UtcNow,
                            StandardCost = item.UnitCost,
                            ReorderPoint = 0,
                            MinStock = 0,
                            SafetyStock = 0,
                            IsBatchTracked = false,
                            IsExpiryTracked = false
                        };
                        _uow.Products.Add(newProd);
                        item.ProductId = newProd.Id;
                    }
                }
                // Lưu sản phẩm mới trước khi thêm dòng chi tiết
                _uow.Save();

                foreach (var item in Items)
                {
                    _uow.GoodsReceiptItems.Add(new GoodsReceiptItem
                    {
                        Id = Guid.NewGuid(),
                        GrnId = receipt.Id,
                        ProductId = item.ProductId,
                        QtyReceived = item.QtyReceived,
                        QtyAccepted = item.QtyReceived,
                        QtyRejected = 0,
                        UnitCost = item.UnitCost,
                        Notes = item.Notes
                    });
                }

                _uow.Save();
                MessageBox.Show("Thêm thành công!");
            }

            Close();
        }
        catch (Exception ex)
        {
            var msg = "Lỗi: " + ex.Message;
            if (ex.InnerException != null) msg += "\nChi tiết: " + ex.InnerException.Message;
            MessageBox.Show(msg);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}