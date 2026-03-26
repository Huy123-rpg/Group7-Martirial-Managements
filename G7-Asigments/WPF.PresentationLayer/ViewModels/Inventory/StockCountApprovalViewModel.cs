using BLL.BusinessLogicLayer.Services.InventoryManagement;
using DAL.DataAccessLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Views.Inventory;

namespace WPF.PresentationLayer.ViewModels.Inventory;

public class StockCountApprovalViewModel : BaseViewModel
{
    private readonly IStockCountApprovalService _service;

    public ObservableCollection<StockCountSession> Sessions { get; } = new();

    private StockCountSession? _selectedSession;
    public StockCountSession? SelectedSession
    {
        get => _selectedSession;
        set
        {
            if (SetField(ref _selectedSession, value))
            {
                OnPropertyChanged(nameof(CanApproveOrReject));
                OnPropertyChanged(nameof(CanViewDetail));
            }
        }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetField(ref _searchText, value)) LoadData();
        }
    }

    private byte _filterStatus = 0; // 0 = All
    public byte FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetField(ref _filterStatus, value)) LoadData();
        }
    }

    public ObservableCollection<dynamic> StatusOptions { get; } = new()
    {
        new { Code = (byte)0, Label = "Tất cả" },
        new { Code = (byte)6, Label = "Chờ duyệt" },
        new { Code = (byte)3, Label = "Đã duyệt" },
        new { Code = (byte)4, Label = "Từ chối" }
    };

    // Role Checks
    public bool IsAdmin => SessionManager.IsAdmin;
    public bool IsManager => SessionManager.IsManager;
    public bool IsAccountant => SessionManager.CurrentUser?.Role?.RoleCode?.Equals("ACCOUNTANT", StringComparison.OrdinalIgnoreCase) == true;

    // Command Eligibility
    public bool CanApproveOrReject => SelectedSession != null && SelectedSession.StatusId == 6 && IsManager;
    public bool CanViewDetail => SelectedSession != null;

    public RelayCommand ApproveCommand { get; }
    public RelayCommand RejectCommand { get; }
    public RelayCommand ViewDetailCommand { get; }

    public StockCountApprovalViewModel()
    {
        _service = new StockCountApprovalService();

        ApproveCommand = new RelayCommand(Approve, () => CanApproveOrReject);
        RejectCommand  = new RelayCommand(Reject, () => CanApproveOrReject);
        ViewDetailCommand = new RelayCommand(ViewDetail, () => CanViewDetail);

        LoadData();
    }

    public void LoadData()
    {
        try
        {
            IEnumerable<StockCountSession> data;

            if (IsManager)
                data = _service.GetByManagerWarehouse(SessionManager.CurrentUser!.Id);
            else 
                data = _service.GetAll();

            // Filter Text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var q = SearchText.Trim().ToLower();
                data = data.Where(s => 
                    (s.SessionCode != null && s.SessionCode.ToLower().Contains(q)) ||
                    (s.Warehouse != null && s.Warehouse.Name.ToLower().Contains(q))
                );
            }

            // Filter Status
            if (FilterStatus != 0)
            {
                data = data.Where(s => s.StatusId == FilterStatus);
            }

            Sessions.Clear();
            foreach (var s in data) Sessions.Add(s);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Approve()
    {
        if (SelectedSession == null) return;

        var result = MessageBox.Show(
            $"Bạn có chắc chắn muốn DUYỆT phiếu kiểm kho {SelectedSession.SessionCode}?\n" +
            "Hệ thống sẽ ngay lập tức cập nhật Tồn Kho bằng với số lượng thực tế đếm được.",
            "Xác nhận Duyệt", MessageBoxButton.YesNo, MessageBoxImage.Information);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                _service.Approve(SelectedSession.Id, SessionManager.CurrentUser!.Id);
                MessageBox.Show("Đã duyệt phiếu thành công. Tồn kho đã được cập nhật.", "Thành công");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi duyệt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void Reject()
    {
        if (SelectedSession == null) return;

        var result = MessageBox.Show(
            $"Bạn có chắc chắn muốn TỪ CHỐI phiếu kiểm kho {SelectedSession.SessionCode}?",
            "Xác nhận Từ chối", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                _service.Reject(SelectedSession.Id, SessionManager.CurrentUser!.Id, "Quản lý từ chối phiếu");
                MessageBox.Show("Đã từ chối phiếu kiểm kho.", "Thành công");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi từ chối: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ViewDetail()
    {
        if (SelectedSession == null) return;
        var win = new StockCountApprovalDetailWindow(SelectedSession);
        
        var mainWin = Application.Current.MainWindow;
        if (mainWin != null && mainWin != win)
        {
            win.Owner = mainWin;
        }
        
        win.ShowDialog();
    }
}
