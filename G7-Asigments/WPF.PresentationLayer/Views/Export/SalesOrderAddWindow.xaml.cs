using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Export;
using DAL.DataAccessLayer.Model;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Export;

public partial class SalesOrderAddWindow : Window
{
    private readonly ISalesOrderService _service;
    private readonly UnitOfWork _uow;
    private SalesOrder? _editingSo;

    public ObservableCollection<SalesOrderItemInput> Items { get; set; } = new();
    public ObservableCollection<Product> Products { get; set; } = new();

    public SalesOrderAddWindow()
    {
        InitializeComponent();
        DataContext = this;
        _service = new SalesOrderService();
        _uow = UnitOfWork.Instance;

        dpOrderDate.SelectedDate = DateTime.Now;
        dpRequiredDate.SelectedDate = DateTime.Now.AddDays(7);
        txtCode.Text = "SO" + DateTime.Now.ToString("yyyyMMddHHmmss");

        LoadData();
        SetupItemGrid();
    }

    public SalesOrderAddWindow(SalesOrder so)
    {
        InitializeComponent();
        DataContext = this;
        _service = new SalesOrderService();
        _uow = UnitOfWork.Instance;
        _editingSo = so;

        LoadData();
        SetupItemGrid();

        dpOrderDate.SelectedDate = so.OrderDate.ToDateTime(TimeOnly.MinValue);
        dpRequiredDate.SelectedDate = so.RequiredDate.ToDateTime(TimeOnly.MinValue);
        txtCode.Text = so.SoNumber;
        txtNote.Text = so.Notes;
        cbCustomer.SelectedValue = so.CustomerId;
        cbWarehouse.SelectedValue = so.WarehouseId;

        var loadedItems = _uow.SalesOrderItems.GetAll().Where(x => x.SoId == so.Id).ToList();
        foreach (var item in loadedItems)
        {
            var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
            Items.Add(new SalesOrderItemInput
            {
                ProductId = item.ProductId,
                ProductName = product?.ProductName ?? "",
                QtyOrdered = item.QtyOrdered,
                UnitPrice = item.UnitPrice,
                Notes = item.Notes
            });
        }
        RefreshTotal();
    }

    private void LoadData()
    {
        cbCustomer.ItemsSource = _uow.Customers.GetAll().ToList();
        cbWarehouse.ItemsSource = _uow.Warehouses.GetAll().ToList();
        var products = _uow.Products.GetAll().ToList();
        Products.Clear();
        foreach (var p in products) Products.Add(p);
    }

    private void SetupItemGrid()
    {
        Items.CollectionChanged += Items_CollectionChanged;
        dgItems.ItemsSource = Items;
        dgItems.CellEditEnding += (s, e) => Dispatcher.BeginInvoke(new Action(RefreshTotal));
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (INotifyPropertyChanged item in e.NewItems)
                item.PropertyChanged += Item_PropertyChanged;
        if (e.OldItems != null)
            foreach (INotifyPropertyChanged item in e.OldItems)
                item.PropertyChanged -= Item_PropertyChanged;
        RefreshTotal();
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SalesOrderItemInput.LineTotal))
            RefreshTotal();
    }

    private void RefreshTotal()
    {
        var total = Items.Sum(x => x.LineTotal);
        txtTotalAmount.Text = $"Tổng tiền: {total:N0}";
    }

    private void BtnAddItem_Click(object sender, RoutedEventArgs e)
    {
        Items.Add(new SalesOrderItemInput { QtyOrdered = 1, UnitPrice = 0 });
        RefreshTotal();
    }

    private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
    {
        if (dgItems.SelectedItem is SalesOrderItemInput selected)
        {
            Items.Remove(selected);
            RefreshTotal();
        }
    }

    private void ProductComboBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is ComboBox cb && cb.DataContext is SalesOrderItemInput item)
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
            && cb.DataContext is SalesOrderItemInput item)
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
                MessageBox.Show("Vui lòng nhập mã SO.");
                return;
            }

            var existed = _uow.SalesOrders.GetAll()
                .Any(x => x.SoNumber == code && (_editingSo == null || x.Id != _editingSo.Id));
            if (existed)
            {
                MessageBox.Show("Mã SO đã tồn tại.");
                return;
            }

            if (cbCustomer.SelectedValue == null || cbWarehouse.SelectedValue == null)
            {
                MessageBox.Show("Chọn khách hàng và kho.");
                return;
            }

            if (Items.Count == 0)
            {
                MessageBox.Show("Đơn bán hàng phải có ít nhất 1 dòng chi tiết.");
                return;
            }

            if (Items.Any(x => x.ProductId == Guid.Empty || x.QtyOrdered <= 0))
            {
                MessageBox.Show("Kiểm tra lại sản phẩm và số lượng.");
                return;
            }

            var currentUser = SessionManager.CurrentUser;
            if (currentUser == null) { MessageBox.Show("Chưa đăng nhập."); return; }

            var total = Items.Sum(x => x.LineTotal);

            if (_editingSo != null)
            {
                _editingSo.SoNumber = code;
                _editingSo.OrderDate = DateOnly.FromDateTime(dpOrderDate.SelectedDate ?? DateTime.Now);
                _editingSo.RequiredDate = DateOnly.FromDateTime(dpRequiredDate.SelectedDate ?? DateTime.Now.AddDays(7));
                _editingSo.TotalAmount = total;
                _editingSo.Subtotal = total;
                _editingSo.Notes = txtNote.Text?.Trim();
                _editingSo.UpdatedAt = DateTimeOffset.UtcNow;
                _editingSo.CustomerId = (Guid)cbCustomer.SelectedValue;
                _editingSo.WarehouseId = (Guid)cbWarehouse.SelectedValue;

                _service.Update(_editingSo);

                var oldItems = _uow.SalesOrderItems.GetAll().Where(x => x.SoId == _editingSo.Id).ToList();
                foreach (var old in oldItems)
                    _uow.SalesOrderItems.DeleteById(old.Id);
                _uow.Save();

                foreach (var item in Items)
                {
                    _uow.SalesOrderItems.Add(new SalesOrderItem
                    {
                        Id = Guid.NewGuid(),
                        SoId = _editingSo.Id,
                        ProductId = item.ProductId,
                        QtyOrdered = item.QtyOrdered,
                        QtyAllocated = 0,
                        QtyDelivered = 0,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.LineTotal,
                        Notes = item.Notes
                    });
                }
                _uow.Save();
                MessageBox.Show("Cập nhật thành công!");
            }
            else
            {
                var so = new SalesOrder
                {
                    Id = Guid.NewGuid(),
                    SoNumber = code,
                    OrderDate = DateOnly.FromDateTime(dpOrderDate.SelectedDate ?? DateTime.Now),
                    RequiredDate = DateOnly.FromDateTime(dpRequiredDate.SelectedDate ?? DateTime.Now.AddDays(7)),
                    TotalAmount = total,
                    Subtotal = total,
                    TaxAmount = 0,
                    DiscountAmount = 0,
                    Notes = txtNote.Text?.Trim(),
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = currentUser.Id,
                    CustomerId = (Guid)cbCustomer.SelectedValue,
                    WarehouseId = (Guid)cbWarehouse.SelectedValue,
                    StatusId = 1
                };

                _service.Create(so);

                foreach (var item in Items)
                {
                    _uow.SalesOrderItems.Add(new SalesOrderItem
                    {
                        Id = Guid.NewGuid(),
                        SoId = so.Id,
                        ProductId = item.ProductId,
                        QtyOrdered = item.QtyOrdered,
                        QtyAllocated = 0,
                        QtyDelivered = 0,
                        UnitPrice = item.UnitPrice,
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

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
}
