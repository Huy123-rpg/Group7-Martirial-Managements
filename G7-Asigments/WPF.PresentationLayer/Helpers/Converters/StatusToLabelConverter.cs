using BLL.BusinessLogicLayer.Services.Scheduling;
using System.Globalization;
using System.Windows.Data;

namespace WPF.PresentationLayer.Helpers.Converters;

public class StatusToLabelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value as string) switch
        {
            ScheduleService.StatusDraft      => "Đã lên lịch",
            ScheduleService.StatusInProgress => "Đang thực hiện",
            ScheduleService.StatusCompleted  => "Hoàn thành",
            ScheduleService.StatusCancelled  => "Đã hủy",
            ScheduleService.StatusMissed     => "Bỏ lỡ",
            _                                => value?.ToString() ?? string.Empty,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
