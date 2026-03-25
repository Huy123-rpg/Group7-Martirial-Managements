using Microsoft.EntityFrameworkCore;
using DAL.DataAccessLayer.Model;

namespace DAL.DataAccessLayer.Context;

public partial class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AiAnomalyLog> AiAnomalyLogs { get; set; }

    public virtual DbSet<AiChatSession> AiChatSessions { get; set; }

    public virtual DbSet<AiForecast> AiForecasts { get; set; }

    public virtual DbSet<AiModelVersion> AiModelVersions { get; set; }

    public virtual DbSet<AiReorderSuggestion> AiReorderSuggestions { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DeliveryOrder> DeliveryOrders { get; set; }

    public virtual DbSet<DocumentSequence> DocumentSequences { get; set; }

    public virtual DbSet<GoodsIssue> GoodsIssues { get; set; }

    public virtual DbSet<GoodsIssueItem> GoodsIssueItems { get; set; }

    public virtual DbSet<GoodsReceipt> GoodsReceipts { get; set; }

    public virtual DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<LkpAnomalySeverity> LkpAnomalySeverities { get; set; }

    public virtual DbSet<LkpCostingMethod> LkpCostingMethods { get; set; }

    public virtual DbSet<LkpDeliveryStatus> LkpDeliveryStatuses { get; set; }

    public virtual DbSet<LkpDocumentStatus> LkpDocumentStatuses { get; set; }

    public virtual DbSet<LkpScheduleType> LkpScheduleTypes { get; set; }

    public virtual DbSet<LkpTransactionType> LkpTransactionTypes { get; set; }

    public virtual DbSet<LkpUserRole> LkpUserRoles { get; set; }

    public virtual DbSet<LkpZoneType> LkpZoneTypes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    public virtual DbSet<SalesOrder> SalesOrders { get; set; }

    public virtual DbSet<SalesOrderItem> SalesOrderItems { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<StockAdjustment> StockAdjustments { get; set; }

    public virtual DbSet<StockAdjustmentItem> StockAdjustmentItems { get; set; }

    public virtual DbSet<StockCountItem> StockCountItems { get; set; }

    public virtual DbSet<StockCountSession> StockCountSessions { get; set; }

    public virtual DbSet<StockTransaction> StockTransactions { get; set; }

    public virtual DbSet<StockTransfer> StockTransfers { get; set; }

    public virtual DbSet<StockTransferItem> StockTransferItems { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<UnitsOfMeasure> UnitsOfMeasures { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VExpiringSoon> VExpiringSoons { get; set; }

    public virtual DbSet<VInventorySummary> VInventorySummaries { get; set; }

    public virtual DbSet<VPendingPoReceipt> VPendingPoReceipts { get; set; }

    public virtual DbSet<VSupplierPerformance> VSupplierPerformances { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseZone> WarehouseZones { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=WarehouseDB;User Id=sa;Password=hoanganh;TrustServerCertificate=True");
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<AiAnomalyLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ai_anoma__3213E83F9D6F76E2");

            entity.ToTable("ai_anomaly_logs");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AffectedUserId).HasColumnName("affected_user_id");
            entity.Property(e => e.AnomalyScore)
                .HasColumnType("decimal(5, 4)")
                .HasColumnName("anomaly_score");
            entity.Property(e => e.AnomalyType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("anomaly_type");
            entity.Property(e => e.AssignedTo).HasColumnName("assigned_to");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DetectedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("detected_at");
            entity.Property(e => e.ModelVersion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("model_version");
            entity.Property(e => e.RefId).HasColumnName("ref_id");
            entity.Property(e => e.RefTable)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ref_table");
            entity.Property(e => e.RefType)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ref_type");
            entity.Property(e => e.ResolutionNote).HasColumnName("resolution_note");
            entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");
            entity.Property(e => e.SeverityId)
                .HasDefaultValue((byte)2)
                .HasColumnName("severity_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("open")
                .HasColumnName("status");

            entity.HasOne(d => d.AffectedUser).WithMany(p => p.AiAnomalyLogAffectedUsers)
                .HasForeignKey(d => d.AffectedUserId)
                .HasConstraintName("FK__ai_anomal__affec__1975C517");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.AiAnomalyLogAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__ai_anomal__assig__1C5231C2");

            entity.HasOne(d => d.Severity).WithMany(p => p.AiAnomalyLogs)
                .HasForeignKey(d => d.SeverityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ai_anomal__sever__178D7CA5");
        });

        modelBuilder.Entity<AiChatSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ai_chat___3213E83FDEEAD47B");

            entity.ToTable("ai_chat_sessions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Context).HasColumnName("context");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.Messages)
                .HasDefaultValue("[]")
                .HasColumnName("messages");
            entity.Property(e => e.SessionTitle)
                .HasMaxLength(200)
                .HasColumnName("session_title");
            entity.Property(e => e.TokenCount)
                .HasDefaultValue(0)
                .HasColumnName("token_count");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AiChatSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ai_chat_s__user___2116E6DF");
        });

        modelBuilder.Entity<AiForecast>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ai_forec__3213E83FDF3D7F6E");

            entity.ToTable("ai_forecasts");

            entity.HasIndex(e => new { e.ProductId, e.WarehouseId, e.ForecastDate, e.ForecastPeriod, e.ModelVersion }, "UQ_forecast").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ActualDemand)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("actual_demand");
            entity.Property(e => e.ConfidenceScore)
                .HasColumnType("decimal(5, 4)")
                .HasColumnName("confidence_score");
            entity.Property(e => e.FeaturesUsed).HasColumnName("features_used");
            entity.Property(e => e.ForecastDate).HasColumnName("forecast_date");
            entity.Property(e => e.ForecastPeriod)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("weekly")
                .HasColumnName("forecast_period");
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("generated_at");
            entity.Property(e => e.MapeError)
                .HasColumnType("decimal(6, 4)")
                .HasColumnName("mape_error");
            entity.Property(e => e.ModelName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("default")
                .HasColumnName("model_name");
            entity.Property(e => e.ModelVersion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("model_version");
            entity.Property(e => e.PredictedDemand)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("predicted_demand");
            entity.Property(e => e.PredictedMax)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("predicted_max");
            entity.Property(e => e.PredictedMin)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("predicted_min");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.Product).WithMany(p => p.AiForecasts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ai_foreca__produ__019E3B86");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.AiForecasts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ai_foreca__wareh__02925FBF");
        });

        modelBuilder.Entity<AiModelVersion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ai_model__3213E83FA90993BD");

            entity.ToTable("ai_model_versions");

            entity.HasIndex(e => new { e.ModelName, e.Version }, "UQ_model_version").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AccuracyScore)
                .HasColumnType("decimal(6, 4)")
                .HasColumnName("accuracy_score");
            entity.Property(e => e.Config).HasColumnName("config");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeployedAt).HasColumnName("deployed_at");
            entity.Property(e => e.DeprecatedAt).HasColumnName("deprecated_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MapeScore)
                .HasColumnType("decimal(6, 4)")
                .HasColumnName("mape_score");
            entity.Property(e => e.ModelName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("model_name");
            entity.Property(e => e.TrainingDate).HasColumnName("training_date");
            entity.Property(e => e.TrainingRecords).HasColumnName("training_records");
            entity.Property(e => e.Version)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("version");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AiModelVersions)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ai_model___creat__2B947552");
        });

        modelBuilder.Entity<AiReorderSuggestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ai_reord__3213E83FF33C1F85");

            entity.ToTable("ai_reorder_suggestions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.DaysOfStock)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("days_of_stock");
            entity.Property(e => e.ModelVersion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("model_version");
            entity.Property(e => e.PoId).HasColumnName("po_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyOnHand)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_on_hand");
            entity.Property(e => e.RejectionNote).HasColumnName("rejection_note");
            entity.Property(e => e.ReorderPoint)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("reorder_point");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.SuggestedDate).HasColumnName("suggested_date");
            entity.Property(e => e.SuggestedQty)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("suggested_qty");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TriggerReason).HasColumnName("trigger_reason");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.AiReorderSuggestions)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__ai_reorde__appro__10E07F16");

            entity.HasOne(d => d.Po).WithMany(p => p.AiReorderSuggestions)
                .HasForeignKey(d => d.PoId)
                .HasConstraintName("FK__ai_reorde__po_id__11D4A34F");

            entity.HasOne(d => d.Product).WithMany(p => p.AiReorderSuggestions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ai_reorde__produ__0C1BC9F9");

            entity.HasOne(d => d.Supplier).WithMany(p => p.AiReorderSuggestions)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__ai_reorde__suppl__0E04126B");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.AiReorderSuggestions)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ai_reorde__wareh__0D0FEE32");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LoggedAt });

            entity.ToTable("audit_logs");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.LoggedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("logged_at");
            entity.Property(e => e.Action)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("action");
            entity.Property(e => e.ChangedFields).HasColumnName("changed_fields");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("ip_address");
            entity.Property(e => e.NewValues).HasColumnName("new_values");
            entity.Property(e => e.OldValues).HasColumnName("old_values");
            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.RequestId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("request_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("table_name");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(500)
                .HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__audit_log__user___30592A6F");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83F47369A94");

            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryCode, "UQ__categori__BC9D1E7CD486CB95").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CategoryCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("category_code");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__categorie__paren__6FE99F9F");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__customer__3213E83F86CE1049");

            entity.ToTable("customers");

            entity.HasIndex(e => e.CustomerCode, "UQ__customer__6A9E4CB78D7AAD47").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreditLimit)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("credit_limit");
            entity.Property(e => e.CustomerCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("customer_code");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(150)
                .HasColumnName("customer_name");
            entity.Property(e => e.CustomerType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("retail")
                .HasColumnName("customer_type");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PaymentTerms)
                .HasMaxLength(100)
                .HasColumnName("payment_terms");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tax_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<DeliveryOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__delivery__3213E83FB97DEC55");

            entity.ToTable("delivery_orders");

            entity.HasIndex(e => e.DoNumber, "UQ__delivery__0EAF28386EB5607F").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ActualDate).HasColumnName("actual_date");
            entity.Property(e => e.AssignedTo).HasColumnName("assigned_to");
            entity.Property(e => e.CarrierName)
                .HasMaxLength(100)
                .HasColumnName("carrier_name");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("contact_phone");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DeliveredAt).HasColumnName("delivered_at");
            entity.Property(e => e.DeliveryAddress)
                .HasMaxLength(500)
                .HasColumnName("delivery_address");
            entity.Property(e => e.DeliveryProof)
                .HasMaxLength(500)
                .HasColumnName("delivery_proof");
            entity.Property(e => e.DoNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("do_number");
            entity.Property(e => e.FailureReason).HasColumnName("failure_reason");
            entity.Property(e => e.GiId).HasColumnName("gi_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PlannedDate).HasColumnName("planned_date");
            entity.Property(e => e.RecipientName)
                .HasMaxLength(100)
                .HasColumnName("recipient_name");
            entity.Property(e => e.SoId).HasColumnName("so_id");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("status_id");
            entity.Property(e => e.TrackingNumber)
                .HasMaxLength(100)
                .HasColumnName("tracking_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleInfo)
                .HasMaxLength(100)
                .HasColumnName("vehicle_info");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.DeliveryOrders)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__delivery___assig__1E6F845E");

            entity.HasOne(d => d.Customer).WithMany(p => p.DeliveryOrders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__delivery___custo__1B9317B3");

            entity.HasOne(d => d.Gi).WithMany(p => p.DeliveryOrders)
                .HasForeignKey(d => d.GiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__delivery___gi_id__19AACF41");

            entity.HasOne(d => d.So).WithMany(p => p.DeliveryOrders)
                .HasForeignKey(d => d.SoId)
                .HasConstraintName("FK__delivery___so_id__1A9EF37A");

            entity.HasOne(d => d.Status).WithMany(p => p.DeliveryOrders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__delivery___statu__1D7B6025");
        });

        modelBuilder.Entity<DocumentSequence>(entity =>
        {
            entity.HasKey(e => new { e.Prefix, e.SeqYear }).HasName("PK_doc_seq");

            entity.ToTable("document_sequences");

            entity.Property(e => e.Prefix)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("prefix");
            entity.Property(e => e.SeqYear)
                .HasDefaultValueSql("(datepart(year,getdate()))")
                .HasColumnName("seq_year");
            entity.Property(e => e.CurrentNumber).HasColumnName("current_number");
            entity.Property(e => e.FormatTemplate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("{PREFIX}-{YYYY}-{NNNNN}")
                .HasColumnName("format_template");
        });

        modelBuilder.Entity<GoodsIssue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__goods_is__3213E83F2B7BFC68");

            entity.ToTable("goods_issues", tb =>
                {
                    tb.HasTrigger("trg_gi_update_inventory");
                    tb.HasTrigger("trg_goods_issues_updated");
                });

            entity.HasIndex(e => e.GiNumber, "UQ__goods_is__2EFC9A085843A129").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.GiNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("gi_number");
            entity.Property(e => e.IssueDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("issue_date");
            entity.Property(e => e.IssueType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("sale")
                .HasColumnName("issue_type");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.RejectionReason).HasColumnName("rejection_reason");
            entity.Property(e => e.SoId).HasColumnName("so_id");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("status_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.GoodsIssueApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__goods_iss__appro__0697FACD");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GoodsIssueCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_iss__creat__05A3D694");

            entity.HasOne(d => d.Customer).WithMany(p => p.GoodsIssues)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__goods_iss__custo__7FEAFD3E");

            entity.HasOne(d => d.So).WithMany(p => p.GoodsIssues)
                .HasForeignKey(d => d.SoId)
                .HasConstraintName("FK__goods_iss__so_id__7EF6D905");

            entity.HasOne(d => d.Status).WithMany(p => p.GoodsIssues)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_iss__statu__02C769E9");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.GoodsIssues)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_iss__wareh__00DF2177");
        });

        modelBuilder.Entity<GoodsIssueItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__goods_is__3213E83F8DDDEB06");

            entity.ToTable("goods_issue_items");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("batch_number");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.GiId).HasColumnName("gi_id");
            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([qty_issued]*[unit_cost])", true)
                .HasColumnType("decimal(31, 7)")
                .HasColumnName("line_total");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyIssued)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_issued");
            entity.Property(e => e.QtyRequested)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_requested");
            entity.Property(e => e.SoItemId).HasColumnName("so_item_id");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_cost");
            entity.Property(e => e.UnitPrice)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_price");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.Gi).WithMany(p => p.GoodsIssueItems)
                .HasForeignKey(d => d.GiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_iss__gi_id__0E391C95");

            entity.HasOne(d => d.Product).WithMany(p => p.GoodsIssueItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_iss__produ__10216507");

            entity.HasOne(d => d.SoItem).WithMany(p => p.GoodsIssueItems)
                .HasForeignKey(d => d.SoItemId)
                .HasConstraintName("FK__goods_iss__so_it__0F2D40CE");

            entity.HasOne(d => d.Zone).WithMany(p => p.GoodsIssueItems)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__goods_iss__zone___11158940");
        });

        modelBuilder.Entity<GoodsReceipt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__goods_re__3213E83F73F7B8DA");

            entity.ToTable("goods_receipts", tb =>
                {
                    tb.HasTrigger("trg_goods_receipts_updated");
                    tb.HasTrigger("trg_grn_update_inventory");
                });

            entity.HasIndex(e => e.GrnNumber, "UQ__goods_re__4C7026B2298FCC41").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.GrnNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("grn_number");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PoId).HasColumnName("po_id");
            entity.Property(e => e.ReceiptDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("receipt_date");
            entity.Property(e => e.RejectionReason).HasColumnName("rejection_reason");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("status_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierRef)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("supplier_ref");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");
            entity.Property(e => e.ValidatedAt).HasColumnName("validated_at");
            entity.Property(e => e.ValidatedBy).HasColumnName("validated_by");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.GoodsReceiptApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__goods_rec__appro__503BEA1C");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GoodsReceiptCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__creat__4F47C5E3");

            entity.HasOne(d => d.Po).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.PoId)
                .HasConstraintName("FK__goods_rec__po_id__4A8310C6");

            entity.HasOne(d => d.Status).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__statu__4E53A1AA");

            entity.HasOne(d => d.Supplier).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__suppl__4B7734FF");

            entity.HasOne(d => d.ValidatedByNavigation).WithMany(p => p.GoodsReceiptValidatedByNavigations)
                .HasForeignKey(d => d.ValidatedBy)
                .HasConstraintName("FK__goods_rec__valid__51300E55");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__wareh__4C6B5938");
        });

        modelBuilder.Entity<GoodsReceiptItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__goods_re__3213E83F6933ECDB");

            entity.ToTable("goods_receipt_items");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("batch_number");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.GrnId).HasColumnName("grn_id");
            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([qty_accepted]*[unit_cost])", true)
                .HasColumnType("decimal(31, 7)")
                .HasColumnName("line_total");
            entity.Property(e => e.ManufactureDate).HasColumnName("manufacture_date");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PoItemId).HasColumnName("po_item_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyAccepted)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_accepted");
            entity.Property(e => e.QtyReceived)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_received");
            entity.Property(e => e.QtyRejected)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_rejected");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_cost");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.Grn).WithMany(p => p.GoodsReceiptItems)
                .HasForeignKey(d => d.GrnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__grn_i__58D1301D");

            entity.HasOne(d => d.PoItem).WithMany(p => p.GoodsReceiptItems)
                .HasForeignKey(d => d.PoItemId)
                .HasConstraintName("FK__goods_rec__po_it__59C55456");

            entity.HasOne(d => d.Product).WithMany(p => p.GoodsReceiptItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__produ__5AB9788F");

            entity.HasOne(d => d.Zone).WithMany(p => p.GoodsReceiptItems)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__goods_rec__zone___5BAD9CC8");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inventor__3213E83F8626A2B6");

            entity.ToTable("inventory");

            entity.HasIndex(e => new { e.ProductId, e.WarehouseId, e.ZoneId }, "UQ_inventory").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AvgCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("avg_cost");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("last_updated");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyAvailable)
                .HasComputedColumnSql("([qty_on_hand]-[qty_reserved])", true)
                .HasColumnType("decimal(13, 3)")
                .HasColumnName("qty_available");
            entity.Property(e => e.QtyIncoming)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_incoming");
            entity.Property(e => e.QtyOnHand)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_on_hand");
            entity.Property(e => e.QtyReserved)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_reserved");
            entity.Property(e => e.TotalValue)
                .HasComputedColumnSql("([qty_on_hand]*[avg_cost])", true)
                .HasColumnType("decimal(31, 7)")
                .HasColumnName("total_value");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inventory__produ__208CD6FA");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inventory__wareh__2180FB33");

            entity.HasOne(d => d.Zone).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__inventory__zone___22751F6C");
        });

        modelBuilder.Entity<LkpAnomalySeverity>(entity =>
        {
            entity.HasKey(e => e.SeverityId).HasName("PK__lkp_anom__08B3F2B18D57054C");

            entity.ToTable("lkp_anomaly_severity");

            entity.HasIndex(e => e.SeverityCode, "UQ__lkp_anom__F7A3E331B89EA6C9").IsUnique();

            entity.Property(e => e.SeverityId).HasColumnName("severity_id");
            entity.Property(e => e.SeverityCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("severity_code");
            entity.Property(e => e.SeverityName)
                .HasMaxLength(30)
                .HasColumnName("severity_name");
        });

        modelBuilder.Entity<LkpCostingMethod>(entity =>
        {
            entity.HasKey(e => e.MethodId).HasName("PK__lkp_cost__747727B67FD65744");

            entity.ToTable("lkp_costing_methods");

            entity.HasIndex(e => e.MethodCode, "UQ__lkp_cost__151BA0F02E1DB417").IsUnique();

            entity.Property(e => e.MethodId).HasColumnName("method_id");
            entity.Property(e => e.MethodCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("method_code");
            entity.Property(e => e.MethodName)
                .HasMaxLength(50)
                .HasColumnName("method_name");
        });

        modelBuilder.Entity<LkpDeliveryStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__lkp_deli__3683B531FC671588");

            entity.ToTable("lkp_delivery_status");

            entity.HasIndex(e => e.StatusCode, "UQ__lkp_deli__4157B021B4CA2A9E").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status_code");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<LkpDocumentStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__lkp_docu__3683B531E5AD1AE6");

            entity.ToTable("lkp_document_status");

            entity.HasIndex(e => e.StatusCode, "UQ__lkp_docu__4157B02124988090").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status_code");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<LkpScheduleType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__lkp_sche__2C000598E6C54F8A");

            entity.ToTable("lkp_schedule_types");

            entity.HasIndex(e => e.TypeCode, "UQ__lkp_sche__2CB4DBF57C7B7959").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("type_code");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<LkpTransactionType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__lkp_tran__2C000598934D81CB");

            entity.ToTable("lkp_transaction_types");

            entity.HasIndex(e => e.TypeCode, "UQ__lkp_tran__2CB4DBF525C88185").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("type_code");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<LkpUserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__lkp_user__760965CC2A6DF789");

            entity.ToTable("lkp_user_roles");

            entity.HasIndex(e => e.RoleCode, "UQ__lkp_user__BAE63075FCB651C6").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("role_code");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<LkpZoneType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__lkp_zone__2C000598EF415824");

            entity.ToTable("lkp_zone_types");

            entity.HasIndex(e => e.TypeCode, "UQ__lkp_zone__2CB4DBF57C909B51").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("type_code");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__notifica__3213E83FD9D86BE3");

            entity.ToTable("notifications");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.Channel)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("in_app")
                .HasColumnName("channel");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.ReadAt).HasColumnName("read_at");
            entity.Property(e => e.RefId).HasColumnName("ref_id");
            entity.Property(e => e.RefType)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ref_type");
            entity.Property(e => e.SentAt).HasColumnName("sent_at");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__notificat__user___7908F585");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__products__3213E83FB4B4F2C0");

            entity.ToTable("products", tb => tb.HasTrigger("trg_products_updated"));

            entity.HasIndex(e => e.Barcode, "IX_products_barcode").HasFilter("([barcode] IS NOT NULL)");

            entity.HasIndex(e => e.CategoryId, "IX_products_category");

            entity.HasIndex(e => e.ProductName, "IX_products_name");

            entity.HasIndex(e => e.Sku, "IX_products_sku");

            entity.HasIndex(e => e.Barcode, "UQ__products__C16E36F804DDAB6F").IsUnique();

            entity.HasIndex(e => e.Sku, "UQ__products__DDDF4BE745CE7416").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Attributes).HasColumnName("attributes");
            entity.Property(e => e.Barcode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("barcode");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsBatchTracked).HasColumnName("is_batch_tracked");
            entity.Property(e => e.IsExpiryTracked).HasColumnName("is_expiry_tracked");
            entity.Property(e => e.MaxStock)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("max_stock");
            entity.Property(e => e.MinStock)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("min_stock");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductName)
                .HasMaxLength(200)
                .HasColumnName("product_name");
            entity.Property(e => e.ReorderPoint)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("reorder_point");
            entity.Property(e => e.SafetyStock)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("safety_stock");
            entity.Property(e => e.ShelfLifeDays).HasColumnName("shelf_life_days");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sku");
            entity.Property(e => e.StandardCost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("standard_cost");
            entity.Property(e => e.UomId).HasColumnName("uom_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");
            entity.Property(e => e.VolumeM3)
                .HasColumnType("decimal(10, 6)")
                .HasColumnName("volume_m3");
            entity.Property(e => e.WeightKg)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("weight_kg");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__products__catego__7D439ABD");

            entity.HasOne(d => d.Uom).WithMany(p => p.Products)
                .HasForeignKey(d => d.UomId)
                .HasConstraintName("FK__products__uom_id__7E37BEF6");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__purchase__3213E83F7FC813E9");

            entity.ToTable("purchase_orders", tb => tb.HasTrigger("trg_purchase_orders_updated"));

            entity.HasIndex(e => e.PoNumber, "UQ__purchase__1130754902888601").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AiSuggestionId).HasColumnName("ai_suggestion_id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("VND")
                .HasColumnName("currency");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.ExpectedDate).HasColumnName("expected_date");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("order_date");
            entity.Property(e => e.PaymentTerms)
                .HasMaxLength(100)
                .HasColumnName("payment_terms");
            entity.Property(e => e.PoNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("po_number");
            entity.Property(e => e.RejectionReason).HasColumnName("rejection_reason");
            entity.Property(e => e.Source)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("manual")
                .HasColumnName("source");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("status_id");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TaxAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tax_amount");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.PurchaseOrderApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__purchase___appro__32AB8735");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseOrderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___creat__31B762FC");

            entity.HasOne(d => d.Status).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___statu__30C33EC3");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___suppl__2DE6D218");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___wareh__2EDAF651");
        });

        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__purchase__3213E83F2D79EB99");

            entity.ToTable("purchase_order_items");

            entity.HasIndex(e => new { e.PoId, e.ProductId }, "UQ_po_item").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([qty_ordered]*[unit_cost])", true)
                .HasColumnType("decimal(31, 7)")
                .HasColumnName("line_total");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PoId).HasColumnName("po_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyOrdered)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_ordered");
            entity.Property(e => e.QtyReceived)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_received");
            entity.Property(e => e.QtyRejected)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_rejected");
            entity.Property(e => e.TaxRate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("tax_rate");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_cost");

            entity.HasOne(d => d.Po).WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(d => d.PoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___po_id__40F9A68C");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___produ__41EDCAC5");
        });

        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sales_or__3213E83F6A9F27B3");

            entity.ToTable("sales_orders", tb => tb.HasTrigger("trg_sales_orders_updated"));

            entity.HasIndex(e => e.SoNumber, "UQ__sales_or__52A5E190A64E0507").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("VND")
                .HasColumnName("currency");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("order_date");
            entity.Property(e => e.PaymentTerms)
                .HasMaxLength(100)
                .HasColumnName("payment_terms");
            entity.Property(e => e.RejectionReason).HasColumnName("rejection_reason");
            entity.Property(e => e.RequiredDate).HasColumnName("required_date");
            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(500)
                .HasColumnName("shipping_address");
            entity.Property(e => e.SoNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("so_number");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("status_id");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.TaxAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tax_amount");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.SalesOrderApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__sales_ord__appro__681373AD");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SalesOrderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sales_ord__creat__671F4F74");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sales_ord__custo__634EBE90");

            entity.HasOne(d => d.Status).WithMany(p => p.SalesOrders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sales_ord__statu__662B2B3B");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.SalesOrders)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sales_ord__wareh__6442E2C9");
        });

        modelBuilder.Entity<SalesOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sales_or__3213E83F66547E84");

            entity.ToTable("sales_order_items");

            entity.HasIndex(e => new { e.SoId, e.ProductId }, "UQ_so_item").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.DiscountPct)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_pct");
            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([qty_ordered]*[unit_price])", true)
                .HasColumnType("decimal(31, 7)")
                .HasColumnName("line_total");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyAllocated)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_allocated");
            entity.Property(e => e.QtyDelivered)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_delivered");
            entity.Property(e => e.QtyOrdered)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_ordered");
            entity.Property(e => e.SoId).HasColumnName("so_id");
            entity.Property(e => e.TaxRate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("tax_rate");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Product).WithMany(p => p.SalesOrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sales_ord__produ__756D6ECB");

            entity.HasOne(d => d.So).WithMany(p => p.SalesOrderItems)
                .HasForeignKey(d => d.SoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sales_ord__so_id__74794A92");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__schedule__3213E83F8213E5B8");

            entity.ToTable("schedules");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AssignedTo).HasColumnName("assigned_to");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IsRecurring).HasColumnName("is_recurring");
            entity.Property(e => e.Metadata).HasColumnName("metadata");
            entity.Property(e => e.RecurrenceRule)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("recurrence_rule");
            entity.Property(e => e.RefId).HasColumnName("ref_id");
            entity.Property(e => e.RefType)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ref_type");
            entity.Property(e => e.ReminderMinutes)
                .HasDefaultValue(30)
                .HasColumnName("reminder_minutes");
            entity.Property(e => e.ScheduleType).HasColumnName("schedule_type");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("scheduled")
                .HasColumnName("status_code");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.ScheduleAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__schedules__assig__7167D3BD");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ScheduleCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__schedules__creat__7073AF84");

            entity.HasOne(d => d.ScheduleTypeNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.ScheduleType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__schedules__sched__6BAEFA67");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__schedules__wareh__6E8B6712");
        });

        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__stock_ad__3213E83F69089D29");

            entity.ToTable("stock_adjustments");

            entity.HasIndex(e => e.AdjNumber, "UQ__stock_ad__D9908653602A4EF9").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AdjNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("adj_number");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)2)
                .HasColumnName("status_id");
            entity.Property(e => e.ValidatedAt).HasColumnName("validated_at");
            entity.Property(e => e.ValidatedBy).HasColumnName("validated_by");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.StockAdjustmentApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__stock_adj__appro__4A4E069C");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockAdjustmentCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_adj__creat__4959E263");

            entity.HasOne(d => d.Session).WithMany(p => p.StockAdjustments)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("FK__stock_adj__sessi__4589517F");

            entity.HasOne(d => d.Status).WithMany(p => p.StockAdjustments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_adj__statu__4865BE2A");

            entity.HasOne(d => d.ValidatedByNavigation).WithMany(p => p.StockAdjustmentValidatedByNavigations)
                .HasForeignKey(d => d.ValidatedBy)
                .HasConstraintName("FK__stock_adj__valid__4B422AD5");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockAdjustments)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_adj__wareh__467D75B8");
        });

        modelBuilder.Entity<StockAdjustmentItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__stock_ad__3213E83FD809B3C2");

            entity.ToTable("stock_adjustment_items");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AdjId).HasColumnName("adj_id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("batch_number");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyAfter)
                .HasComputedColumnSql("([qty_before]+[qty_change])", true)
                .HasColumnType("decimal(13, 3)")
                .HasColumnName("qty_after");
            entity.Property(e => e.QtyBefore)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_before");
            entity.Property(e => e.QtyChange)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_change");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_cost");
            entity.Property(e => e.ValueChange)
                .HasComputedColumnSql("([qty_change]*[unit_cost])", true)
                .HasColumnType("decimal(31, 7)")
                .HasColumnName("value_change");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.Adj).WithMany(p => p.StockAdjustmentItems)
                .HasForeignKey(d => d.AdjId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_adj__adj_i__5006DFF2");

            entity.HasOne(d => d.Product).WithMany(p => p.StockAdjustmentItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_adj__produ__50FB042B");

            entity.HasOne(d => d.Zone).WithMany(p => p.StockAdjustmentItems)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__stock_adj__zone___51EF2864");
        });

        modelBuilder.Entity<StockCountItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__stock_co__3213E83FF200D257");

            entity.ToTable("stock_count_items");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("batch_number");
            entity.Property(e => e.CountedAt).HasColumnName("counted_at");
            entity.Property(e => e.CountedBy).HasColumnName("counted_by");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyCounted)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_counted");
            entity.Property(e => e.QtySystem)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_system");
            entity.Property(e => e.QtyVariance)
                .HasComputedColumnSql("(case when [qty_counted] IS NOT NULL then [qty_counted]-[qty_system]  end)", true)
                .HasColumnType("decimal(13, 3)")
                .HasColumnName("qty_variance");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_cost");
            entity.Property(e => e.VarianceValue)
                .HasComputedColumnSql("(case when [qty_counted] IS NOT NULL then ([qty_counted]-[qty_system])*[unit_cost]  end)", true)
                .HasColumnType("decimal(32, 7)")
                .HasColumnName("variance_value");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.CountedByNavigation).WithMany(p => p.StockCountItems)
                .HasForeignKey(d => d.CountedBy)
                .HasConstraintName("FK__stock_cou__count__40C49C62");

            entity.HasOne(d => d.Product).WithMany(p => p.StockCountItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_cou__produ__3CF40B7E");

            entity.HasOne(d => d.Session).WithMany(p => p.StockCountItems)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_cou__sessi__3BFFE745");

            entity.HasOne(d => d.Zone).WithMany(p => p.StockCountItems)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__stock_cou__zone___3DE82FB7");
        });

        modelBuilder.Entity<StockCountSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__stock_co__3213E83F4741BE63");

            entity.ToTable("stock_count_sessions");

            entity.HasIndex(e => e.SessionCode, "UQ__stock_co__615A1EA739654F66").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.AssignedTo).HasColumnName("assigned_to");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CountType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("full")
                .HasColumnName("count_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PlannedDate).HasColumnName("planned_date");
            entity.Property(e => e.SessionCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("session_code");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("status_id");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.StockCountSessionApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__stock_cou__appro__373B3228");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.StockCountSessionAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__stock_cou__assig__36470DEF");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockCountSessionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_cou__creat__3552E9B6");

            entity.HasOne(d => d.Status).WithMany(p => p.StockCountSessions)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_cou__statu__32767D0B");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockCountSessions)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_cou__wareh__2F9A1060");

            entity.HasOne(d => d.Zone).WithMany(p => p.StockCountSessions)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__stock_cou__zone___308E3499");
        });

        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__stock_tr__3213E83F95B5A6BA");

            entity.ToTable("stock_transactions");

            entity.HasIndex(e => e.TxnCode, "UQ__stock_tr__11B3720A0CD3CE99").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("batch_number");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PerformedBy).HasColumnName("performed_by");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyBalance)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_balance");
            entity.Property(e => e.QtyChange)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_change");
            entity.Property(e => e.RefId).HasColumnName("ref_id");
            entity.Property(e => e.RefItemId).HasColumnName("ref_item_id");
            entity.Property(e => e.RefType)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ref_type");
            entity.Property(e => e.TotalValue)
                .HasComputedColumnSql("(abs([qty_change])*[unit_cost])", true)
                .HasColumnType("decimal(31, 7)")
                .HasColumnName("total_value");
            entity.Property(e => e.TxnAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("txn_at");
            entity.Property(e => e.TxnCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("txn_code");
            entity.Property(e => e.TxnTypeId).HasColumnName("txn_type_id");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_cost");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.PerformedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__perfo__29E1370A");

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__produ__2610A626");

            entity.HasOne(d => d.TxnType).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.TxnTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__txn_t__251C81ED");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__wareh__2704CA5F");

            entity.HasOne(d => d.Zone).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__stock_tra__zone___27F8EE98");
        });

        modelBuilder.Entity<StockTransfer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__stock_tr__3213E83FC3F31118");

            entity.ToTable("stock_transfers");

            entity.HasIndex(e => e.TransferNumber, "UQ__stock_tr__5155023278F8BFE6").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.FromWarehouseId).HasColumnName("from_warehouse_id");
            entity.Property(e => e.FromZoneId).HasColumnName("from_zone_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("status_id");
            entity.Property(e => e.ToWarehouseId).HasColumnName("to_warehouse_id");
            entity.Property(e => e.ToZoneId).HasColumnName("to_zone_id");
            entity.Property(e => e.TransferDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("transfer_date");
            entity.Property(e => e.TransferNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("transfer_number");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.StockTransferApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__stock_tra__appro__5E54FF49");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockTransferCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__creat__5D60DB10");

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.StockTransferFromWarehouses)
                .HasForeignKey(d => d.FromWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__from___57A801BA");

            entity.HasOne(d => d.FromZone).WithMany(p => p.StockTransferFromZones)
                .HasForeignKey(d => d.FromZoneId)
                .HasConstraintName("FK__stock_tra__from___589C25F3");

            entity.HasOne(d => d.Status).WithMany(p => p.StockTransfers)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__statu__5C6CB6D7");

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.StockTransferToWarehouses)
                .HasForeignKey(d => d.ToWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__to_wa__59904A2C");

            entity.HasOne(d => d.ToZone).WithMany(p => p.StockTransferToZones)
                .HasForeignKey(d => d.ToZoneId)
                .HasConstraintName("FK__stock_tra__to_zo__5A846E65");
        });

        modelBuilder.Entity<StockTransferItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__stock_tr__3213E83F2AA2A191");

            entity.ToTable("stock_transfer_items");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("batch_number");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyRequested)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_requested");
            entity.Property(e => e.QtyTransferred)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_transferred");
            entity.Property(e => e.TransferId).HasColumnName("transfer_id");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_cost");

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransferItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__produ__6501FCD8");

            entity.HasOne(d => d.Transfer).WithMany(p => p.StockTransferItems)
                .HasForeignKey(d => d.TransferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock_tra__trans__640DD89F");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__supplier__3213E83FBF294DAB");

            entity.ToTable("suppliers");

            entity.HasIndex(e => e.SupplierCode, "UQ__supplier__A82CE469C5E40C6A").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasDefaultValue("Vietnam")
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LeadTimeDays)
                .HasDefaultValue(7)
                .HasColumnName("lead_time_days");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PaymentTerms)
                .HasMaxLength(100)
                .HasColumnName("payment_terms");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.PortalEnabled).HasColumnName("portal_enabled");
            entity.Property(e => e.PortalUserId).HasColumnName("portal_user_id");
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("rating");
            entity.Property(e => e.SupplierCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("supplier_code");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(150)
                .HasColumnName("supplier_name");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tax_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.PortalUser).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.PortalUserId)
                .HasConstraintName("FK__suppliers__porta__114A936A");
        });

        modelBuilder.Entity<UnitsOfMeasure>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__units_of__3213E83FEF850227");

            entity.ToTable("units_of_measure");

            entity.HasIndex(e => e.UomCode, "UQ__units_of__31E2176B843E24CE").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BaseUomId).HasColumnName("base_uom_id");
            entity.Property(e => e.ConversionFactor)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(12, 6)")
                .HasColumnName("conversion_factor");
            entity.Property(e => e.UomCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("uom_code");
            entity.Property(e => e.UomName)
                .HasMaxLength(50)
                .HasColumnName("uom_name");

            entity.HasOne(d => d.BaseUom).WithMany(p => p.InverseBaseUom)
                .HasForeignKey(d => d.BaseUomId)
                .HasConstraintName("FK__units_of___base___76969D2E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F6407C975");

            // HasTrigger báo cho EF Core biết table có trigger → tự xử lý OUTPUT clause
            entity.ToTable("users", tb => tb.HasTrigger("trg_users_updated"));

            entity.HasIndex(e => e.IsActive, "IX_users_active");

            entity.HasIndex(e => e.Email, "IX_users_email");

            entity.HasIndex(e => e.RoleId, "IX_users_role");

            entity.HasIndex(e => e.StaffCode, "UQ__users__097F3286A88F16F6").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__users__AB6E61643F9308CD").IsUnique();

            // ValueGeneratedNever: code luôn tự set → EF không cần đọc lại từ DB
            // → không cần OUTPUT clause → không conflict với trigger
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .ValueGeneratedNever()
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .ValueGeneratedNever()
                .HasColumnName("is_active");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.StaffCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("staff_code");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedNever()
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__users__role_id__571DF1D5");
        });

        modelBuilder.Entity<VExpiringSoon>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_expiring_soon");

            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("batch_number");
            entity.Property(e => e.DaysUntilExpiry).HasColumnName("days_until_expiry");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.ProductName)
                .HasMaxLength(200)
                .HasColumnName("product_name");
            entity.Property(e => e.QtyAtRisk)
                .HasColumnType("decimal(38, 3)")
                .HasColumnName("qty_at_risk");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sku");
            entity.Property(e => e.ValueAtRisk)
                .HasColumnType("decimal(38, 7)")
                .HasColumnName("value_at_risk");
            entity.Property(e => e.WarehouseName)
                .HasMaxLength(100)
                .HasColumnName("warehouse_name");
            entity.Property(e => e.ZoneCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("zone_code");
        });

        modelBuilder.Entity<VInventorySummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_inventory_summary");

            entity.Property(e => e.AvgCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("avg_cost");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastUpdated).HasColumnName("last_updated");
            entity.Property(e => e.ProductName)
                .HasMaxLength(200)
                .HasColumnName("product_name");
            entity.Property(e => e.QtyAvailable)
                .HasColumnType("decimal(13, 3)")
                .HasColumnName("qty_available");
            entity.Property(e => e.QtyIncoming)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_incoming");
            entity.Property(e => e.QtyOnHand)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_on_hand");
            entity.Property(e => e.QtyReserved)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_reserved");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sku");
            entity.Property(e => e.StockStatus)
                .HasMaxLength(14)
                .IsUnicode(false)
                .HasColumnName("stock_status");
            entity.Property(e => e.TotalValue)
                .HasColumnType("decimal(31, 7)")
                .HasColumnName("total_value");
            entity.Property(e => e.UomCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("uom_code");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("warehouse_code");
            entity.Property(e => e.WarehouseName)
                .HasMaxLength(100)
                .HasColumnName("warehouse_name");
            entity.Property(e => e.ZoneCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("zone_code");
            entity.Property(e => e.ZoneType)
                .HasMaxLength(50)
                .HasColumnName("zone_type");
        });

        modelBuilder.Entity<VPendingPoReceipt>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_pending_po_receipts");

            entity.Property(e => e.DaysOverdue).HasColumnName("days_overdue");
            entity.Property(e => e.ExpectedDate).HasColumnName("expected_date");
            entity.Property(e => e.OutstandingValue)
                .HasColumnType("decimal(32, 7)")
                .HasColumnName("outstanding_value");
            entity.Property(e => e.PoNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("po_number");
            entity.Property(e => e.ProductName)
                .HasMaxLength(200)
                .HasColumnName("product_name");
            entity.Property(e => e.QtyOrdered)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_ordered");
            entity.Property(e => e.QtyOutstanding)
                .HasColumnType("decimal(13, 3)")
                .HasColumnName("qty_outstanding");
            entity.Property(e => e.QtyReceived)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("qty_received");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sku");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(150)
                .HasColumnName("supplier_name");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("unit_cost");
        });

        modelBuilder.Entity<VSupplierPerformance>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_supplier_performance");

            entity.Property(e => e.AvgDelayDays).HasColumnName("avg_delay_days");
            entity.Property(e => e.RejectionRatePct)
                .HasColumnType("numeric(38, 6)")
                .HasColumnName("rejection_rate_pct");
            entity.Property(e => e.SupplierCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("supplier_code");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(150)
                .HasColumnName("supplier_name");
            entity.Property(e => e.TotalGrns).HasColumnName("total_grns");
            entity.Property(e => e.TotalPos).HasColumnName("total_pos");
            entity.Property(e => e.TotalPurchaseValue)
                .HasColumnType("decimal(38, 7)")
                .HasColumnName("total_purchase_value");
            entity.Property(e => e.TotalQtyReceived)
                .HasColumnType("decimal(38, 3)")
                .HasColumnName("total_qty_received");
            entity.Property(e => e.TotalQtyRejected)
                .HasColumnType("decimal(38, 3)")
                .HasColumnName("total_qty_rejected");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__warehous__3213E83F7FEA6947");

            entity.ToTable("warehouses");

            entity.HasIndex(e => e.Code, "UQ__warehous__357D4CF90BE1807F").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.CostingMethod)
                .HasDefaultValue((byte)1)
                .HasColumnName("costing_method");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CostingMethodNavigation).WithMany(p => p.Warehouses)
                .HasForeignKey(d => d.CostingMethod)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__warehouse__costi__619B8048");

            entity.HasOne(d => d.Manager).WithMany(p => p.Warehouses)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__warehouse__manag__5EBF139D");
        });

        modelBuilder.Entity<WarehouseZone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__warehous__3213E83F855206DB");

            entity.ToTable("warehouse_zones");

            entity.HasIndex(e => new { e.WarehouseId, e.ZoneCode }, "UQ_zone").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CapacityM3)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("capacity_m3");
            entity.Property(e => e.CapacityPallet).HasColumnName("capacity_pallet");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
            entity.Property(e => e.ZoneCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("zone_code");
            entity.Property(e => e.ZoneName)
                .HasMaxLength(100)
                .HasColumnName("zone_name");
            entity.Property(e => e.ZoneType)
                .HasDefaultValue((byte)1)
                .HasColumnName("zone_type");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseZones)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__warehouse__wareh__68487DD7");

            entity.HasOne(d => d.ZoneTypeNavigation).WithMany(p => p.WarehouseZones)
                .HasForeignKey(d => d.ZoneType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__warehouse__zone___6A30C649");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
