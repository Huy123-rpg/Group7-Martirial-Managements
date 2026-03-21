using System;
using System.Windows;
using System.Windows.Controls;
using BLL.BusinessLogicLayer.Services.Import;

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
            dgGoodsReceipts.ItemsSource = _goodsReceiptService.GetAll();
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
        MessageBox.Show("Chưa làm form thêm phiếu nhập.");
    }
}