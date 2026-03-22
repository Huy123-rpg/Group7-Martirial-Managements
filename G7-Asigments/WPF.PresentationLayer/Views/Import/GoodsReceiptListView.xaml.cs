using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BLL.BusinessLogicLayer.Services.Import;
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
            var data = _goodsReceiptService.GetAll()
                .Select(x => new GoodsReceiptListItem
                {
                    GoodsReceipt = x
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
}