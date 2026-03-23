using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

/// <summary>
/// Bảng người dùng thống nhất — thay thế 4 bảng riêng lẻ trong ERD gốc
/// </summary>
public partial class User
{
    public Guid Id { get; set; }

    public string? StaffCode { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    public byte RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<AiAnomalyLog> AiAnomalyLogAffectedUsers { get; set; } = new List<AiAnomalyLog>();

    public virtual ICollection<AiAnomalyLog> AiAnomalyLogAssignedToNavigations { get; set; } = new List<AiAnomalyLog>();

    public virtual ICollection<AiChatSession> AiChatSessions { get; set; } = new List<AiChatSession>();

    public virtual ICollection<AiModelVersion> AiModelVersions { get; set; } = new List<AiModelVersion>();

    public virtual ICollection<AiReorderSuggestion> AiReorderSuggestions { get; set; } = new List<AiReorderSuggestion>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<DeliveryOrder> DeliveryOrders { get; set; } = new List<DeliveryOrder>();

    public virtual ICollection<GoodsIssue> GoodsIssueApprovedByNavigations { get; set; } = new List<GoodsIssue>();

    public virtual ICollection<GoodsIssue> GoodsIssueCreatedByNavigations { get; set; } = new List<GoodsIssue>();

    public virtual ICollection<GoodsReceipt> GoodsReceiptApprovedByNavigations { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<GoodsReceipt> GoodsReceiptCreatedByNavigations { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<GoodsReceipt> GoodsReceiptValidatedByNavigations { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PurchaseOrder> PurchaseOrderApprovedByNavigations { get; set; } = new List<PurchaseOrder>();

    public virtual ICollection<PurchaseOrder> PurchaseOrderCreatedByNavigations { get; set; } = new List<PurchaseOrder>();

    public virtual LkpUserRole Role { get; set; } = null!;

    public virtual ICollection<SalesOrder> SalesOrderApprovedByNavigations { get; set; } = new List<SalesOrder>();

    public virtual ICollection<SalesOrder> SalesOrderCreatedByNavigations { get; set; } = new List<SalesOrder>();

    public virtual ICollection<Schedule> ScheduleAssignedToNavigations { get; set; } = new List<Schedule>();

    public virtual ICollection<Schedule> ScheduleCreatedByNavigations { get; set; } = new List<Schedule>();

    public virtual ICollection<StockAdjustment> StockAdjustmentApprovedByNavigations { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockAdjustment> StockAdjustmentCreatedByNavigations { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockAdjustment> StockAdjustmentValidatedByNavigations { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockCountItem> StockCountItems { get; set; } = new List<StockCountItem>();

    public virtual ICollection<StockCountSession> StockCountSessionApprovedByNavigations { get; set; } = new List<StockCountSession>();

    public virtual ICollection<StockCountSession> StockCountSessionAssignedToNavigations { get; set; } = new List<StockCountSession>();

    public virtual ICollection<StockCountSession> StockCountSessionCreatedByNavigations { get; set; } = new List<StockCountSession>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    public virtual ICollection<StockTransfer> StockTransferApprovedByNavigations { get; set; } = new List<StockTransfer>();

    public virtual ICollection<StockTransfer> StockTransferCreatedByNavigations { get; set; } = new List<StockTransfer>();

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();

    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
