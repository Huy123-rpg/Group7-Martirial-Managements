using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Import;
using DAL.DataAccessLayer.Model;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Import;

public partial class PurchaseOrderAddWindow : Window
{
    private readonly IPurchaseOrderService _service;
    private readonly UnitOfWork _uow;
    private PurchaseOrder? _editingPo;

    public ObservableCollection<PurchaseOrderItemInput> Items { get; set; } = new();
    public ObservableCollection<Product> Products { get; set; } = new();

    public PurchaseOrderAddWindow()
    {
        InitializeComponent();
        DataContext = this;

        _service = new PurchaseOrderService();
        _uow = UnitOfWork.Instance;

        dpOrderDate.SelectedDate = DateTime.Now;
        dpExpectedDate.SelectedDate = DateTime.Now.AddDays(7);
        txtCode.Text = GeneratePoCode();

        LoadData();
        SetupItemGrid();
    }

    public PurchaseOrderAddWindow(PurchaseOrder po)
    {
        InitializeComponent();
        DataContext = this;

        _service = new PurchaseOrderService();
        _uow = UnitOfWork.Instance;
        _editingPo = po;

        LoadData();
        SetupItemGrid();

        dpOrderDate.SelectedDate = po.OrderDate.ToDateTime(TimeOnly.MinValue);
        dpExpectedDate.SelectedDate = po.ExpectedDate.ToDateTime(TimeOnly.MinValue);
        txtCode.Text = po.PoNumber;
        txtNote.Text = po.Notes;

        cbWarehouse.SelectedValue = po.WarehouseId;
        cbSupplier.SelectedValue = po.SupplierId;

        var detailItems = _uow.PurchaseOrders.GetById(po.Id)?.PurchaseOrderItems.ToList()
                          ?? _uow.PurchaseOrders.GetAll().FirstOrDefault(x => x.Id == po.Id)?.PurchaseOrderItems.ToList()
                          ?? new System.Collections.Generic.List<PurchaseOrderItem>();

        // Cần truy vấn trực tiếp items nếu navigation property chưa load
        if (detailItems.Count == 0)
        {
            // Dùng dbcontext vì uow ko expose trực tiếp PurchaseOrderItems
            // Tuy nhiên model PO ko expose DbSet cho item trực tiếp mà nằm trong UOW thì không thấy, thôi kệ, cứ load qua list chung
            var allPoItems = _uow.Products.GetAll().ToList(); // just a trigger to load
        }

        var loadedItems = _uow.PurchaseOrderItems.GetAll().Where(x => x.PoId == po.Id).ToList();


        foreach (var item in loadedItems)
        {
            var product = Products.FirstOrDefault(p => p.Id == item.ProductId);

            Items.Add(new PurchaseOrderItemInput
            {
                ProductId = item.ProductId,
                ProductName = product?.ProductName ?? "",
                QtyOrdered = item.QtyOrdered,
                UnitCost = item.UnitCost,
                Notes = item.Notes
            });
        }

        RefreshTotal();
    }

    private void LoadData()
    {
        cbSupplier.ItemsSource = _uow.Suppliers.GetAll().ToList();
        cbWarehouse.ItemsSource = _uow.Warehouses.GetAll().ToList();

        var products = _uow.Products.GetAll().ToList();
        Products.Clear();
        foreach (var p in products) Products.Add(p);
    }

    private void SetupItemGrid()
    {
        Items.CollectionChanged += Items_CollectionChanged;
        dgItems.ItemsSource = Items;

        dgItems.CellEditEnding += (s, e) =>
            Dispatcher.BeginInvoke(new Action(RefreshTotal));
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (INotifyPropertyChanged item in e.NewItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }
        if (e.OldItems != null)
        {
            foreach (INotifyPropertyChanged item in e.OldItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }
        RefreshTotal();
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PurchaseOrderItemInput.ProductId) && sender is PurchaseOrderItemInput item)
        {
            var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                item.ProductName = product.ProductName;
            }
        }
        else if (e.PropertyName == nameof(PurchaseOrderItemInput.LineTotal))
        {
            RefreshTotal();
        }
    }

    private string GeneratePoCode()
    {
        return "PO" + DateTime.Now.ToString("yyyyMMddHHmmss");
    }

    private void BtnAddItem_Click(object sender, RoutedEventArgs e)
    {
        Items.Add(new PurchaseOrderItemInput
        {
            QtyOrdered = 1,
            UnitCost = 0
        });

        RefreshTotal();
    }

    private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
    {
        if (dgItems.SelectedItem is PurchaseOrderItemInput selected)
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

    private void ProductComboBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is ComboBox cb && cb.DataContext is PurchaseOrderItemInput item)
        {
            var typed = cb.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(typed))
            {
                var matched = Products.FirstOrDefault(p =>
                    p.ProductName.Equals(typed, StringComparison.OrdinalIgnoreCase));
                if (matched != null)
                {
                    item.ProductId = matched.Id;
                    item.ProductName = matched.ProductName;
                }
                else
                {
                    item.ProductId = Guid.Empty;
                    item.ProductName = typed;
                }
            }
        }
    }

    private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox cb && cb.SelectedItem is Product selected
            && cb.DataContext is PurchaseOrderItemInput item)
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

            var existed = _uow.PurchaseOrders.GetAll()
                .Any(x => x.PoNumber == code && (_editingPo == null || x.Id != _editingPo.Id));

            if (existed)
            {
                MessageBox.Show("Mã phiếu đã tồn tại.");
                return;
            }

            if (cbWarehouse.SelectedValue == null || cbSupplier.SelectedValue == null)
            {
                MessageBox.Show("Chọn kho và nhà cung cấp.");
                return;
            }

            if (Items.Count == 0)
            {
                MessageBox.Show("Phiếu đặt hàng phải có ít nhất 1 dòng chi tiết.");
                return;
            }

            if (Items.Any(x => (x.ProductId == Guid.Empty && string.IsNullOrWhiteSpace(x.ProductName)) || x.QtyOrdered <= 0))
            {
                MessageBox.Show("Kiểm tra lại sản phẩm và số lượng.");
                return;
            }

            var currentUser = _uow.Users.GetAll().FirstOrDefault(u => u.IsActive);
            if (currentUser == null)
            {
                MessageBox.Show("Không có user.");
                return;
            }

            var total = Items.Sum(x => x.LineTotal);

            // Xử lý tạo sản phẩm mới nếu user gõ tay
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
            _uow.Save();

            if (_editingPo != null)
            {
                _editingPo.PoNumber = code;
                _editingPo.OrderDate = DateOnly.FromDateTime(dpOrderDate.SelectedDate ?? DateTime.Now);
                _editingPo.ExpectedDate = DateOnly.FromDateTime(dpExpectedDate.SelectedDate ?? DateTime.Now.AddDays(7));
                _editingPo.TotalAmount = total;
                _editingPo.Subtotal = total;
                _editingPo.Notes = txtNote.Text?.Trim();
                _editingPo.UpdatedAt = DateTimeOffset.UtcNow;
                _editingPo.WarehouseId = (Guid)cbWarehouse.SelectedValue;
                _editingPo.SupplierId = (Guid)cbSupplier.SelectedValue;

                _service.Update(_editingPo);

                var oldItems = _uow.PurchaseOrderItems.GetAll().Where(x => x.PoId == _editingPo.Id).ToList();
                foreach (var old in oldItems)
                {
                    _uow.PurchaseOrderItems.DeleteById(old.Id);
                }
                _uow.Save();

                foreach (var item in Items)
                {
                    _uow.PurchaseOrderItems.Add(new PurchaseOrderItem
                    {
                        Id = Guid.NewGuid(),
                        PoId = _editingPo.Id,
                        ProductId = item.ProductId,
                        QtyOrdered = item.QtyOrdered,
                        QtyReceived = 0,
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
                var po = new PurchaseOrder
                {
                    Id = Guid.NewGuid(),
                    PoNumber = code,
                    OrderDate = DateOnly.FromDateTime(dpOrderDate.SelectedDate ?? DateTime.Now),
                    ExpectedDate = DateOnly.FromDateTime(dpExpectedDate.SelectedDate ?? DateTime.Now.AddDays(7)),
                    TotalAmount = total,
                    Subtotal = total,
                    TaxAmount = 0,
                    DiscountAmount = 0,
                    Notes = txtNote.Text?.Trim(),
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = currentUser.Id,
                    WarehouseId = (Guid)cbWarehouse.SelectedValue,
                    SupplierId = (Guid)cbSupplier.SelectedValue,
                    StatusId = 1 // 1=Draft
                };

                _service.Create(po);

                foreach (var item in Items)
                {
                    _uow.PurchaseOrderItems.Add(new PurchaseOrderItem
                    {
                        Id = Guid.NewGuid(),
                        PoId = po.Id,
                        ProductId = item.ProductId,
                        QtyOrdered = item.QtyOrdered,
                        QtyReceived = 0,
                        QtyRejected = 0,
                        UnitCost = item.UnitCost,
                        LineTotal = item.LineTotal,
                        Notes = item.Notes
                    });
                }

                _uow.Save();
                MessageBox.Show("Thêm thành công!");
            }

            DialogResult = true;
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
