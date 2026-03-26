using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Export;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Export;

public partial class GoodsIssueListView : UserControl
{
    private readonly IGoodsIssueService _goodsIssueService;
    private readonly UnitOfWork _uow;

    public GoodsIssueListView()
    {
        InitializeComponent();
        _goodsIssueService = new GoodsIssueService();
        _uow = UnitOfWork.Instance;
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            bool canApprove = PermissionHelper.CanApproveGoodsIssue;
            bool canEdit = PermissionHelper.CanEditGoodsIssue;
            bool canDelete = PermissionHelper.CanDeleteGoodsIssue;

            var customers = _uow.Customers.GetAll().ToDictionary(c => c.Id, c => c.CustomerName);
            var warehouses = _uow.Warehouses.GetAll().ToDictionary(w => w.Id, w => w.Name);
            var sos = _uow.SalesOrders.GetAll().ToDictionary(s => s.Id, s => s.SoNumber);

            var data = _goodsIssueService.GetAll()
                .Select(x => new GoodsIssueListItem
                {
                    GoodsIssue = x,
                    SoNumber = x.SoId.HasValue && sos.TryGetValue(x.SoId.Value, out var sn) ? sn : "",
                    CustomerName = x.CustomerId.HasValue && customers.TryGetValue(x.CustomerId.Value, out var cn) ? cn : "",
                    WarehouseName = warehouses.TryGetValue(x.WarehouseId, out var wn) ? wn : "",
                    IsApproveVisible = canApprove && x.StatusId == 1,
                    IsEditVisible = canEdit && x.StatusId == 1,
                    IsDeleteVisible = canDelete && x.StatusId == 1
                })
                .ToList();

            dgGoodsIssues.ItemsSource = data;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi tải danh sách phiếu xuất: " + ex.Message);
        }
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        LoadData();
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanEditGoodsIssue)
        {
            MessageBox.Show("Bạn không có quyền sửa phiếu xuất.");
            return;
        }

        if (sender is Button btn && btn.DataContext is GoodsIssueListItem row)
        {
            var win = new GoodsIssueAddWindow(row.GoodsIssue);
            win.ShowDialog();
            LoadData();
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanDeleteGoodsIssue)
        {
            MessageBox.Show("Bạn không có quyền xóa phiếu xuất.");
            return;
        }

        if (sender is Button btn && btn.DataContext is GoodsIssueListItem row)
        {
            var issue = row.GoodsIssue;

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa phiếu {issue.GiNumber}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _goodsIssueService.Delete(issue.Id);
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

    private void dgGoodsIssues_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (dgGoodsIssues.SelectedItem is GoodsIssueListItem row)
        {
            if (row.GoodsIssue.StatusId != 1)
            {
                MessageBox.Show("Phiếu đã duyệt, không thể chỉnh sửa.");
                return;
            }
            var win = new GoodsIssueAddWindow(row.GoodsIssue);
            win.ShowDialog();
            LoadData();
        }
    }

    private void BtnApprove_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionHelper.CanApproveGoodsIssue)
        {
            MessageBox.Show("Chỉ tài khoản Admin mới có quyền duyệt phiếu xuất.", "Không có quyền",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (sender is Button btn && btn.DataContext is GoodsIssueListItem row)
        {
            var issue = row.GoodsIssue;

            if (issue.StatusId != 1)
            {
                MessageBox.Show("Phiếu này đã được duyệt hoặc không ở trạng thái chờ.");
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc muốn duyệt phiếu {issue.GiNumber}?",
                "Xác nhận duyệt",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _goodsIssueService.Approve(issue.Id, SessionManager.CurrentUser!.Id);
                    MessageBox.Show("Duyệt phiếu xuất thành công!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi duyệt phiếu xuất: " + ex.Message);
                }
            }
        }
    }
}
