using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Export;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Export;

public partial class SalesOrderListView : UserControl
{
    private readonly ISalesOrderService _service;
    private readonly UnitOfWork _uow;

    public SalesOrderListView()
    {
        InitializeComponent();
        _service = new SalesOrderService();
        _uow = UnitOfWork.Instance;
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            bool canApprove = PermissionHelper.CanApproveSalesOrder;
            bool canCreate = PermissionHelper.CanCreateSalesOrder;
            bool canDelete = PermissionHelper.CanDeleteSalesOrder;

            var customers = _uow.Customers.GetAll().ToDictionary(c => c.Id, c => c.CustomerName);
            var warehouses = _uow.Warehouses.GetAll().ToDictionary(w => w.Id, w => w.Name);

            var data = _service.GetAll()
                .Select(so => new SalesOrderListItem
                {
                    SalesOrder = so,
                    CustomerName = customers.TryGetValue(so.CustomerId, out var cn) ? cn : "",
                    WarehouseName = warehouses.TryGetValue(so.WarehouseId, out var wn) ? wn : "",
                    IsSubmitVisible = canCreate && so.StatusId == 1,
                    IsApproveVisible = canApprove && so.StatusId == 2,
                    IsCreateIssueVisible = canCreate && so.StatusId == 3,
                    IsEditVisible = canCreate && so.StatusId == 1,
                    IsDeleteVisible = canDelete && so.StatusId == 1,
                    IsCancelVisible = canApprove && (so.StatusId == 2 || so.StatusId == 3)
                })
                .ToList();

            dgSalesOrders.ItemsSource = data;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi tải danh sách SO: " + ex.Message);
        }
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadData();

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanCreateSalesOrder)
        {
            MessageBox.Show("Bạn không có quyền tạo đơn bán hàng.");
            return;
        }
        var win = new SalesOrderAddWindow();
        win.ShowDialog();
        LoadData();
    }

    private void BtnSubmit_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanCreateSalesOrder)
        {
            MessageBox.Show("Bạn không có quyền gửi duyệt đơn bán hàng.");
            return;
        }
        if (sender is Button btn && btn.DataContext is SalesOrderListItem row)
        {
            var result = MessageBox.Show($"Gửi duyệt đơn {row.SoNumber}?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _service.Submit(row.SalesOrder.Id);
                    MessageBox.Show("Đã gửi duyệt! Chờ quản lý phê duyệt.");
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }

    private void BtnApprove_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanApproveSalesOrder)
        {
            MessageBox.Show("Bạn không có quyền duyệt đơn bán hàng.");
            return;
        }
        if (sender is Button btn && btn.DataContext is SalesOrderListItem row)
        {
            var result = MessageBox.Show($"Duyệt đơn {row.SoNumber}?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _service.Approve(row.SalesOrder.Id, SessionManager.CurrentUser!.Id);
                    MessageBox.Show("Duyệt thành công! Có thể tạo phiếu xuất kho.");
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }

    private void BtnCreateIssue_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is SalesOrderListItem row)
        {
            var win = new GoodsIssueAddWindow(row.SalesOrder);
            win.ShowDialog();
            LoadData();
        }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is SalesOrderListItem row)
        {
            var win = new SalesOrderAddWindow(row.SalesOrder);
            win.ShowDialog();
            LoadData();
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is SalesOrderListItem row)
        {
            var result = MessageBox.Show($"Xóa đơn {row.SoNumber}?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var items = _uow.SalesOrderItems.GetAll()
                        .Where(x => x.SoId == row.SalesOrder.Id).ToList();
                    foreach (var item in items)
                        _uow.SalesOrderItems.DeleteById(item.Id);
                    _uow.Save();
                    _service.Delete(row.SalesOrder.Id);
                    MessageBox.Show("Xóa thành công!");
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanApproveSalesOrder)
        {
            MessageBox.Show("Bạn không có quyền hủy đơn bán hàng.");
            return;
        }
        if (sender is Button btn && btn.DataContext is SalesOrderListItem row)
        {
            var result = MessageBox.Show($"Hủy đơn {row.SoNumber}? Thao tác này không thể hoàn tác.",
                "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _service.Cancel(row.SalesOrder.Id, SessionManager.CurrentUser!.Id);
                    MessageBox.Show("Đã hủy đơn bán hàng.");
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }
}
