using BLL.BusinessLogicLayer.Services.InventoryManagement;
using DAL.DataAccessLayer.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Views.Inventory;

namespace WPF.PresentationLayer.ViewModels.Inventory;

public class StockCountExecutionListViewModel : BaseViewModel
{
    private readonly IStockCountExecutionService _service;

    public ObservableCollection<StockCountSession> Sessions { get; set; } = new();

    public ICommand LoadSessionsCommand { get; }
    public ICommand OpenDetailCommand { get; }

    public StockCountExecutionListViewModel()
    {
        _service = new StockCountExecutionService();
        LoadSessionsCommand = new RelayCommand(LoadSessions);
        OpenDetailCommand = new RelayCommand<StockCountSession>(OpenDetail);

        LoadSessions();
    }

    private void LoadSessions()
    {
        var user = SessionManager.CurrentUser;
        if (user == null || !SessionManager.IsStaff) return;

        Sessions.Clear();
        var data = _service.GetAssignedSessions(user.Id);
        foreach (var session in data)
        {
            Sessions.Add(session);
        }
    }

    private void OpenDetail(StockCountSession? session)
    {
        if (session == null) return;

        var detailWindow = new StockCountExecutionDetailWindow(session);
        detailWindow.ShowDialog();
        
        // Refresh after dialog closes
        LoadSessions();
    }
}
