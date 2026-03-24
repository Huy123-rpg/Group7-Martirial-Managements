using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BLL.BusinessLogicLayer.Services.Import;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Models;

namespace WPF.PresentationLayer.Views.Import;

public partial class GoodsReceiptListView : UserControl
{
    private readonly IGoodsReceiptService _goodsReceiptService;

    public GoodsReceiptListView()
    {
        InitializeComponent();
        _goodsReceiptService = new GoodsReceiptService();
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            bool canApprove = PermissionHelper.CanApproveGoodsReceipt;
            var data = _goodsReceiptService.GetAll()
                .Select(x => new GoodsReceiptListItem
                {
                    GoodsReceipt = x,
                    IsApproveVisible = canApprove
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

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var win = new GoodsReceiptAddWindow();
        win.ShowDialog();
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
                    receipt.StatusId = 2; // Approved Status
                    receipt.ApprovedAt = DateTimeOffset.UtcNow;
                    // Note: Here we'd set receipt.ApprovedBy if we had the currentUser logged in globally
                    
                    _goodsReceiptService.Update(receipt);
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
}