using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Export;
using DAL.DataAccessLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Export;

public partial class GoodsIssueAddWindow : Window
{
    private readonly IGoodsIssueService _service;
    private readonly UnitOfWork _uow;
    private GoodsIssue? _editingIssue;
    private SalesOrder? _sourceSo;

    private ObservableCollection<GoodsIssueItemInput> _items = new();
    public List<Product> Products { get; private set; } = new();

    public GoodsIssueAddWindow()
    {
        InitializeComponent();
        DataContext = this;

        _service = new GoodsIssueService();
        _uow = UnitOfWork.Instance;

        dpDate.SelectedDate = DateTime.Now;
        txtCode.Text = GenerateIssueCode();

        LoadData();
        SetupItemGrid();
    }

    public GoodsIssueAddWindow(SalesOrder so)
    {
        InitializeComponent();
        DataContext = this;
        _service = new GoodsIssueService();
        _uow = UnitOfWork.Instance;
        _sourceSo = so;

        dpDate.SelectedDate = DateTime.Now;
        txtCode.Text = GenerateIssueCode();

        LoadData();
        SetupItemGrid();

        cbWarehouse.SelectedValue = so.WarehouseId;
        cbCustomer.SelectedValue = so.CustomerId;

        var soItems = _uow.SalesOrderItems.GetAll()
            .Where(x => x.SoId == so.Id).ToList();
        foreach (var soItem in soItems)
        {
            var product = Products.FirstOrDefault(p => p.Id == soItem.ProductId);
            _items.Add(new GoodsIssueItemInput
            {
                ProductId = soItem.ProductId,
                ProductName = product?.ProductName ?? "",
                QtyOrdered = soItem.QtyOrdered,
                QtyIssued = soItem.QtyOrdered,
                UnitPrice = soItem.UnitPrice,
                Notes = soItem.Notes
            });
        }
        RefreshTotal();
        LockFieldsFromSo(so);
    }

    public GoodsIssueAddWindow(GoodsIssue issue)
    {
        InitializeComponent();
        DataContext = this;

        _service = new GoodsIssueService();
        _uow = UnitOfWork.Instance;
        _editingIssue = issue;

        LoadData();
        SetupItemGrid();

        dpDate.SelectedDate = issue.IssueDate.ToDateTime(TimeOnly.MinValue);
        txtCode.Text = issue.GiNumber;
        txtNote.Text = issue.Notes;

        cbWarehouse.SelectedValue = issue.WarehouseId;
        cbCustomer.SelectedValue = issue.CustomerId;

        var detailItems = _uow.GoodsIssueItems.GetAll()
            .Where(x => x.GiId == issue.Id)
            .ToList();

        foreach (var item in detailItems)
        {
            var product = Products.FirstOrDefault(p => p.Id == item.ProductId);

            _items.Add(new GoodsIssueItemInput
            {
                ProductId = item.ProductId,
                ProductName = product?.ProductName ?? "",
                QtyIssued = item.QtyIssued,
                UnitPrice = item.UnitPrice ?? 0,
                Notes = item.Notes
            });
        }

        RefreshTotal();

        if (issue.SoId.HasValue)
        {
            var so = _uow.SalesOrders.GetById(issue.SoId.Value);
            if (so != null)
            {
                var soItemQty = _uow.SalesOrderItems.GetAll()
                    .Where(x => x.SoId == so.Id)
                    .ToDictionary(x => x.ProductId, x => x.QtyOrdered);
                foreach (var item in _items)
                {
                    if (soItemQty.TryGetValue(item.ProductId, out var qty))
                        item.QtyOrdered = qty;
                }
                LockFieldsFromSo(so);
            }
        }
    }

    private void LoadData()
    {
        cbCustomer.ItemsSource = _uow.Customers.GetAll().ToList();
        cbWarehouse.ItemsSource = _uow.Warehouses.GetAll().ToList();
        Products = _uow.Products.GetAll().ToList();
    }

    private void LockFieldsFromSo(SalesOrder so)
    {
        pnlSoRef.Visibility = Visibility.Visible;
        txtSoRef.Text = so.SoNumber;

        var customer = _uow.Customers.GetById(so.CustomerId);
        txtSoCustomer.Text = customer?.CustomerName ?? "";

        var warehouse = _uow.Warehouses.GetById(so.WarehouseId);
        txtSoWarehouse.Text = warehouse?.Name ?? "";

        cbCustomer.IsEnabled = false;
        cbWarehouse.IsEnabled = false;
        pnlItemButtons.Visibility = Visibility.Collapsed;
    }

    private void SetupItemGrid()
    {
        _items.CollectionChanged += Items_CollectionChanged;
        dgItems.ItemsSource = _items;

        dgItems.CellEditEnding += (s, e) =>
            Dispatcher.BeginInvoke(new Action(RefreshTotal));
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
        if (e.PropertyName == nameof(GoodsIssueItemInput.ProductId) && sender is GoodsIssueItemInput item)
        {
            var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                item.ProductName = product.ProductName;
                // Tự động điền giá từ StandardCost của sản phẩm
                item.UnitPrice = product.StandardCost ?? 0;
            }
        }
        else if (e.PropertyName == nameof(GoodsIssueItemInput.QtyIssued) || 
                 e.PropertyName == nameof(GoodsIssueItemInput.UnitPrice) ||
                 e.PropertyName == nameof(GoodsIssueItemInput.LineTotal))
        {
            RefreshTotal();
        }
    }

    private string GenerateIssueCode()
    {
        return "GI" + DateTime.Now.ToString("yyyyMMddHHmmss");
    }

    private void BtnAddItem_Click(object sender, RoutedEventArgs e)
    {
        _items.Add(new GoodsIssueItemInput
        {
            QtyIssued = 1,
            UnitPrice = 0
        });

        RefreshTotal();
    }

    private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
    {
        if (dgItems.SelectedItem is GoodsIssueItemInput selected)
        {
            _items.Remove(selected);
            RefreshTotal();
        }
    }

    private void RefreshTotal()
    {
        var total = _items.Sum(x => x.LineTotal);
        txtTotalAmount.Text = $"Tổng tiền: {total:N0}";
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

            var existed = _uow.GoodsIssues.GetAll()
                .Any(x => x.GiNumber == code && (_editingIssue == null || x.Id != _editingIssue.Id));

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

            if (_items.Count == 0)
            {
                MessageBox.Show("Phiếu xuất phải có ít nhất 1 dòng chi tiết.");
                return;
            }

            if (_items.Any(x => x.ProductId == Guid.Empty || x.QtyIssued <= 0))
            {
                MessageBox.Show("Kiểm tra lại sản phẩm và số lượng trong chi tiết.");
                return;
            }

            var currentUser = SessionManager.CurrentUser;
            if (currentUser == null)
            {
                MessageBox.Show("Chưa đăng nhập.");
                return;
            }
            if (_editingIssue == null && !PermissionHelper.CanCreateGoodsIssue)
            {
                MessageBox.Show("Bạn không có quyền tạo phiếu xuất.");
                return;
            }

            if (_editingIssue != null && !PermissionHelper.CanEditGoodsIssue)
            {
                MessageBox.Show("Bạn không có quyền sửa phiếu xuất.");
                return;
            }

            var total = _items.Sum(x => x.LineTotal);

            if (_editingIssue != null)
            {
                _editingIssue.GiNumber = code;
                _editingIssue.IssueDate = DateOnly.FromDateTime(dpDate.SelectedDate ?? DateTime.Now);
                _editingIssue.TotalAmount = total;
                _editingIssue.Notes = txtNote.Text?.Trim();
                _editingIssue.UpdatedAt = DateTimeOffset.UtcNow;
                _editingIssue.WarehouseId = (Guid)cbWarehouse.SelectedValue;
                _editingIssue.CustomerId = cbCustomer.SelectedValue != null ? (Guid?)cbCustomer.SelectedValue : null;

                _service.Update(_editingIssue);

                var oldItems = _uow.GoodsIssueItems.GetAll().Where(x => x.GiId == _editingIssue.Id).ToList();
                foreach (var old in oldItems)
                {
                    _uow.GoodsIssueItems.DeleteById(old.Id);
                }

                foreach (var item in _items)
                {
                    _uow.GoodsIssueItems.Add(new GoodsIssueItem
                    {
                        Id           = Guid.NewGuid(),
                        GiId         = _editingIssue.Id,
                        ProductId    = item.ProductId,
                        QtyIssued    = item.QtyIssued,
                        QtyRequested = item.QtyIssued,
                        UnitPrice    = item.UnitPrice,
                        UnitCost     = 0,
                        LineTotal    = item.LineTotal,
                        Notes        = item.Notes
                    });
                }

                _uow.Save();
                MessageBox.Show("Cập nhật thành công!");
            }
            else
            {
                var issue = new GoodsIssue
                {
                    Id = Guid.NewGuid(),
                    GiNumber = code,
                    IssueDate = DateOnly.FromDateTime(dpDate.SelectedDate ?? DateTime.Now),
                    TotalAmount = total,
                    Notes = txtNote.Text?.Trim(),
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = currentUser.Id,
                    WarehouseId = (Guid)cbWarehouse.SelectedValue,
                    CustomerId = cbCustomer.SelectedValue != null ? (Guid?)cbCustomer.SelectedValue : null,
                    StatusId = 1,
                    SoId = _sourceSo?.Id
                };

                _service.Create(issue);

                foreach (var item in _items)
                {
                    _uow.GoodsIssueItems.Add(new GoodsIssueItem
                    {
                        Id = Guid.NewGuid(),
                        GiId = issue.Id,
                        ProductId = item.ProductId,
                        QtyIssued = item.QtyIssued,
                        QtyRequested = item.QtyIssued,
                        UnitPrice = item.UnitPrice,
                        UnitCost = 0,
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