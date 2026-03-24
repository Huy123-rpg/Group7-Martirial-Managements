namespace WPF.PresentationLayer.Helpers;

public class ScheduleSlot
{
    public Guid   Id             { get; set; }
    public string Title          { get; set; } = "";
    public string StatusCode     { get; set; } = "";
    public Guid?  AssignedToId   { get; set; }
    public string AssignedToName { get; set; } = "";
}

public class CalendarDayModel
{
    public DateTime           Date           { get; set; }
    public bool               IsCurrentMonth { get; set; }
    public List<ScheduleSlot> Slots          { get; set; } = [];

    public bool   IsToday     => Date.Date == DateTime.Today;
    public bool   HasConflict => Slots
        .Where(s => s.AssignedToId.HasValue)
        .GroupBy(s => s.AssignedToId)
        .Any(g => g.Count() > 1);

    public List<ScheduleSlot> VisibleSlots => Slots.Take(3).ToList();
    public int    ExtraCount  => Math.Max(0, Slots.Count - 3);
    public bool   HasExtra    => ExtraCount > 0;
    public string ExtraLabel  => $"+{ExtraCount} thêm";

    public string BusyTooltip =>
        Slots.Count == 0 ? "Không có lịch" :
        string.Join("\n", Slots
            .GroupBy(s => string.IsNullOrEmpty(s.AssignedToName) ? "(Chưa phân công)" : s.AssignedToName)
            .Select(g => $"• {g.Key}: {string.Join(", ", g.Select(s => s.Title))}"));
}
