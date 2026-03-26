using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Import;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Import;

public partial class PurchaseOrderListView : UserControl
{
    private readonly IPurchaseOrderService _service;
    private readonly UnitOfWork _uow;

    public PurchaseOrderListView()
    {
        InitializeComponent();
        _service = new PurchaseOrderService();
        _uow = UnitOfWork.Instance;
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            bool canApprove = PermissionHelper.CanApprovePurchaseOrder;
            bool canCreate = PermissionHelper.CanCreatePurchaseOrder;
            bool canDelete = PermissionHelper.CanDeletePurchaseOrder;

            var data = _service.GetAll()
                .Select(po => new PurchaseOrderListItem
                {
                    PurchaseOrder = po,
                    IsSubmitVisible = canCreate && po.StatusId == 1,
                    IsApproveVisible = canApprove && po.StatusId == 2,
                    IsCreateReceiptVisible = canCreate && po.StatusId == 3,
                    IsEditVisible = canCreate && po.StatusId == 1,
                    IsDeleteVisible = canDelete && po.StatusId == 1,
                    IsCancelVisible = canApprove && (po.StatusId == 2 || po.StatusId == 3)
                })
                .ToList();

            dgPurchaseOrders.ItemsSource = data;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi tải danh sách PO: " + ex.Message);
        }
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadData();

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanCreatePurchaseOrder)
        {
            MessageBox.Show("Bạn không có quyền tạo đơn đặt hàng.");
            return;
        }
        var win = new PurchaseOrderAddWindow();
        win.ShowDialog();
        LoadData();
    }

    private void BtnSubmit_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanCreatePurchaseOrder)
        {
            MessageBox.Show("Bạn không có quyền gửi duyệt đơn đặt hàng.");
            return;
        }
        if (sender is Button btn && btn.DataContext is PurchaseOrderListItem row)
        {
            var result = MessageBox.Show($"Gửi duyệt đơn {row.PoNumber}?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _service.Submit(row.PurchaseOrder.Id);
                    MessageBox.Show("Đã gửi duyệt! Chờ quản lý phê duyệt.");
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }

    private void BtnApprove_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanApprovePurchaseOrder)
        {
            MessageBox.Show("Bạn không có quyền duyệt đơn đặt hàng.");
            return;
        }
        if (sender is Button btn && btn.DataContext is PurchaseOrderListItem row)
        {
            var result = MessageBox.Show($"Duyệt đơn {row.PoNumber}?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _service.Approve(row.PurchaseOrder.Id, SessionManager.CurrentUser!.Id);
                    MessageBox.Show("Duyệt thành công! Có thể tạo phiếu nhập kho.");
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }

    private void BtnCreateReceipt_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is PurchaseOrderListItem row)
        {
            var win = new GoodsReceiptAddWindow(row.PurchaseOrder);
            win.ShowDialog();
            LoadData();
        }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is PurchaseOrderListItem row)
        {
            var win = new PurchaseOrderAddWindow(row.PurchaseOrder);
            win.ShowDialog();
            LoadData();
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is PurchaseOrderListItem row)
        {
            var result = MessageBox.Show($"Xóa đơn {row.PoNumber}?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var items = _uow.PurchaseOrderItems.GetAll()
                        .Where(x => x.PoId == row.PurchaseOrder.Id).ToList();
                    foreach (var item in items)
                        _uow.PurchaseOrderItems.DeleteById(item.Id);
                    _uow.Save();
                    _service.Delete(row.PurchaseOrder.Id);
                    MessageBox.Show("Xóa thành công!");
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanApprovePurchaseOrder)
        {
            MessageBox.Show("Bạn không có quyền hủy đơn đặt hàng.");
            return;
        }
        if (sender is Button btn && btn.DataContext is PurchaseOrderListItem row)
        {
            var result = MessageBox.Show($"Hủy đơn {row.PoNumber}? Thao tác này không thể hoàn tác.",
                "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _service.Cancel(row.PurchaseOrder.Id, SessionManager.CurrentUser!.Id);
                    MessageBox.Show("Đã hủy đơn đặt hàng.");
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }
}
