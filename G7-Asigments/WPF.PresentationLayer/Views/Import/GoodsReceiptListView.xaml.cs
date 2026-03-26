using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Import;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Import;

public partial class GoodsReceiptListView : UserControl
{
    private readonly IGoodsReceiptService _goodsReceiptService;
    private readonly UnitOfWork _uow;

    public GoodsReceiptListView()
    {
        InitializeComponent();
        _goodsReceiptService = new GoodsReceiptService();
        _uow = UnitOfWork.Instance;
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            bool canApprove = PermissionHelper.CanApproveGoodsReceipt;
            bool canEdit = PermissionHelper.CanEditGoodsReceipt;
            bool canDelete = PermissionHelper.CanDeleteGoodsReceipt;

            var suppliers = _uow.Suppliers.GetAll().ToDictionary(s => s.Id, s => s.SupplierName);
            var warehouses = _uow.Warehouses.GetAll().ToDictionary(w => w.Id, w => w.Name);
            var pos = _uow.PurchaseOrders.GetAll().ToDictionary(p => p.Id, p => p.PoNumber);

            var data = _goodsReceiptService.GetAll()
                .Select(x => new GoodsReceiptListItem
                {
                    GoodsReceipt = x,
                    PoNumber = x.PoId.HasValue && pos.TryGetValue(x.PoId.Value, out var pn) ? pn : "",
                    SupplierName = suppliers.TryGetValue(x.SupplierId, out var sn) ? sn : "",
                    WarehouseName = warehouses.TryGetValue(x.WarehouseId, out var wn) ? wn : "",
                    IsApproveVisible = canApprove && x.StatusId == 1,
                    IsEditVisible = canEdit && x.StatusId == 1,
                    IsDeleteVisible = canDelete && x.StatusId == 1,
                    IsCancelVisible = canApprove && x.StatusId == 3,
                })
                .ToList();

            dgGoodsReceipts.ItemsSource = data;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi tải danh sách phiếu nhập: " + ex.Message);
        }
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        LoadData();
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is GoodsReceiptListItem row)
        {
            var win = new GoodsReceiptAddWindow(row.GoodsReceipt);
            win.ShowDialog();
            LoadData();
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is GoodsReceiptListItem row)
        {
            var receipt = row.GoodsReceipt;

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa phiếu {receipt.GrnNumber}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _goodsReceiptService.Delete(receipt.Id);
                    MessageBox.Show("Xóa thành công!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa phiếu: " + ex.Message);
                }
            }
        }
    }

    private void dgGoodsReceipts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgGoodsReceipts.SelectedItem is GoodsReceiptListItem row)
        {
            if (row.GoodsReceipt.StatusId != 1)
            {
                MessageBox.Show("Phiếu đã duyệt, không thể chỉnh sửa.");
                return;
            }
            var win = new GoodsReceiptAddWindow(row.GoodsReceipt);
            win.ShowDialog();
            LoadData();
        }
    }

    private void BtnApprove_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanApproveGoodsReceipt)
        {
            MessageBox.Show("Chỉ tài khoản Admin mới có quyền duyệt phiếu nhập.", "Không có quyền",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (sender is Button btn && btn.DataContext is GoodsReceiptListItem row)
        {
            var receipt = row.GoodsReceipt;

            if (receipt.StatusId != 1)
            {
                MessageBox.Show("Phiếu này đã được duyệt hoặc không ở trạng thái chờ.");
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc muốn duyệt phiếu {receipt.GrnNumber}?",
                "Xác nhận duyệt",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _goodsReceiptService.Approve(receipt.Id, SessionManager.CurrentUser!.Id);
                    MessageBox.Show("Duyệt phiếu thành công!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi duyệt phiếu: " + ex.Message);
                }
            }
        }
    }

    private void BtnVoid_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanApproveGoodsReceipt)
        {
            MessageBox.Show("Chỉ tài khoản Admin mới có quyền hủy phiếu nhập.", "Không có quyền",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (sender is Button btn && btn.DataContext is GoodsReceiptListItem row)
        {
            var dialog = new WPF.PresentationLayer.Views.Shared.InputDialog(
                "Nhập lý do hủy phiếu:", "Xác nhận hủy phiếu");
            if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.Result))
            {
                MessageBox.Show("Vui lòng nhập lý do hủy.");
                return;
            }

            try
            {
                _goodsReceiptService.Cancel(row.GoodsReceipt.Id, SessionManager.CurrentUser!.Id, dialog.Result);
                MessageBox.Show("Hủy phiếu thành công!");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hủy phiếu: " + ex.Message);
            }
        }
    }
}
