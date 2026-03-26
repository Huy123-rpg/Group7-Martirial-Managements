using DAL.DataAccessLayer.Model;
using System.Windows;

namespace WPF.PresentationLayer.Views.Inventory;

public partial class StockCountApprovalDetailWindow : Window
{
    public StockCountApprovalDetailWindow(StockCountSession session)
    {
        InitializeComponent();

        TxtSessionCode.Text = $"Phiếu: {session.SessionCode}";
        TxtWarehouse.Text   = session.Warehouse?.Name ?? session.WarehouseId.ToString();
        TxtSchedule.Text    = session.ScheduleTitle ?? "Kiểm kho thủ công";
        TxtStatus.Text      = session.Status?.StatusName ?? session.StatusId.ToString();
        
        TxtAssignedTo.Text  = session.AssignedToNavigation?.FullName ?? "(Chưa giao)";
        TxtCompletedAt.Text = session.CompletedAt?.LocalDateTime.ToString("dd/MM/yyyy HH:mm") ?? "(Chưa hoàn thành)";
        
        if (session.ApprovedByNavigation != null && session.ApprovedAt.HasValue)
            TxtApprovedInfo.Text = $"Duyệt bởi {session.ApprovedByNavigation.FullName} lúc {session.ApprovedAt.Value.LocalDateTime:dd/MM/yyyy HH:mm}";

        TxtNotes.Text = session.Notes;

        ItemsGrid.ItemsSource = session.StockCountItems;
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
