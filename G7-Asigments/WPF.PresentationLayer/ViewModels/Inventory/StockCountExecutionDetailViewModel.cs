using BLL.BusinessLogicLayer.Services.InventoryManagement;
using DAL.DataAccessLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Inventory;

public class StockCountExecutionDetailViewModel : BaseViewModel
{
    private readonly IStockCountExecutionService _service;

    public StockCountSession Session { get; }

    public ObservableCollection<StockCountItem> Items { get; set; } = new();

    public string WindowTitle => $"Kiểm Kho: {Session.SessionCode} - {Session.Warehouse?.Name}";

    public bool CanEdit => Session.StatusId == 2; // Only Pending counts can be edited

    public ICommand SaveDraftCommand { get; }
    public ICommand CompleteCommand { get; }

    public StockCountExecutionDetailViewModel(StockCountSession session)
    {
        _service = new StockCountExecutionService();
        Session = session;

        SaveDraftCommand = new RelayCommand(SaveDraft, () => CanEdit);
        CompleteCommand = new RelayCommand(Complete, () => CanEdit);

        LoadItems();
    }

    private void LoadItems()
    {
        Items.Clear();
        var data = _service.GetSessionItems(Session.Id);
        foreach (var item in data)
        {
            Items.Add(item);
        }
    }

    private void SaveDraft()
    {
        try
        {
            var user = SessionManager.CurrentUser;
            if (user == null) return;

            _service.SaveCount(Session.Id, Items, isComplete: false, staffId: user.Id, notes: Session.Notes);
            MessageBox.Show("Đã lưu nháp tiến độ kiểm kho.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Reload to reflect IN_PROGRESS status
            OnPropertyChanged(nameof(Session));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi lưu nháp: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Complete()
    {
        var result = MessageBox.Show(
            "Bạn có chắc chắn muốn chốt số liệu đếm này? Sau khi hoàn thành sẽ không thể thay đổi.", 
            "Xác nhận hoàn thành", 
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var user = SessionManager.CurrentUser;
            if (user == null) return;

            _service.SaveCount(Session.Id, Items, isComplete: true, staffId: user.Id, notes: Session.Notes);
            Session.StatusId = 6; // Locally update so UI refreshes correctly
            MessageBox.Show("Đã chốt phiếu kiểm kho thành công. Vui lòng chờ Quản lý duyệt.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            
            OnPropertyChanged(nameof(CanEdit));
            // Let the Window Code-Behind handle closing the dialog
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi chốt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
