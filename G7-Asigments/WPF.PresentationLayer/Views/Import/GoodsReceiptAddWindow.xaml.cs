using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Import;
using DAL.DataAccessLayer.Model;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Import;

public partial class GoodsReceiptAddWindow : Window
{
    private readonly IGoodsReceiptService _service;
    private readonly UnitOfWork _uow;
    private GoodsReceipt? _editingReceipt;

    private ObservableCollection<GoodsReceiptItemInput> _items = new();
    private List<Product> _products = new();

    public GoodsReceiptAddWindow()
    {
        InitializeComponent();

        _service = new GoodsReceiptService();
        _uow = UnitOfWork.Instance;

        dpDate.SelectedDate = DateTime.Now;
        txtCode.Text = GenerateReceiptCode();

        LoadData();
        SetupItemGrid();
    }

    public GoodsReceiptAddWindow(GoodsReceipt receipt)
    {
        InitializeComponent();

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
            var product = _products.FirstOrDefault(p => p.Id == item.ProductId);

            _items.Add(new GoodsReceiptItemInput
            {
                ProductId = item.ProductId,
                ProductName = product?.ProductName ?? "",
                QtyReceived = item.QtyReceived,
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
        _products = _uow.Products.GetAll().ToList();
    }

    private void SetupItemGrid()
    {
        dgItems.ItemsSource = _items;

        var col = dgItems.Columns[0] as DataGridComboBoxColumn;
        if (col != null)
        {
            col.ItemsSource = _products;
            col.SelectedValuePath = "Id";
            col.DisplayMemberPath = "ProductName";
        }

        dgItems.CellEditEnding += (s, e) =>
            Dispatcher.BeginInvoke(new Action(RefreshTotal));
    }

    private string GenerateReceiptCode()
    {
        return "GR" + DateTime.Now.ToString("yyyyMMddHHmmss");
    }

    private void BtnAddItem_Click(object sender, RoutedEventArgs e)
    {
        _items.Add(new GoodsReceiptItemInput
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
            _items.Remove(selected);
            RefreshTotal();
        }
    }

    private void RefreshTotal()
    {
        foreach (var item in _items)
        {
            var product = _products.FirstOrDefault(p => p.Id == item.ProductId);
            item.ProductName = product?.ProductName ?? "";
        }

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

            if (_items.Count == 0)
            {
                MessageBox.Show("Phiếu nhập phải có ít nhất 1 dòng chi tiết.");
                return;
            }

            if (_items.Any(x => x.ProductId == Guid.Empty || x.QtyReceived <= 0))
            {
                MessageBox.Show("Kiểm tra lại sản phẩm và số lượng.");
                return;
            }

            var currentUser = _uow.Users.GetAll().FirstOrDefault();
            if (currentUser == null)
            {
                MessageBox.Show("Không có user.");
                return;
            }

            var total = _items.Sum(x => x.LineTotal);

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

                foreach (var item in _items)
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
                    StatusId = 1
                };

                _service.Create(receipt);

                foreach (var item in _items)
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
            MessageBox.Show("Lỗi: " + ex.ToString());
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}