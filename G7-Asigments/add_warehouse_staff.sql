-- ============================================================
-- Migration: thêm bảng warehouse_staff
-- Cho phép gán nhiều staff vào 1 kho (Admin quản lý)
-- ============================================================

IF NOT EXISTS (
    SELECT 1 FROM sys.tables WHERE name = 'warehouse_staff'
)
BEGIN
    CREATE TABLE warehouse_staff (
        warehouse_id  UNIQUEIDENTIFIER NOT NULL,
        user_id       UNIQUEIDENTIFIER NOT NULL,
        assigned_by   UNIQUEIDENTIFIER NOT NULL,
        assigned_at   DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),

        CONSTRAINT PK_warehouse_staff PRIMARY KEY (warehouse_id, user_id),
        CONSTRAINT FK_whs_warehouse FOREIGN KEY (warehouse_id) REFERENCES warehouses(id) ON DELETE CASCADE,
        CONSTRAINT FK_whs_user      FOREIGN KEY (user_id)      REFERENCES users(id)       ON DELETE CASCADE,
        CONSTRAINT FK_whs_assigned  FOREIGN KEY (assigned_by)  REFERENCES users(id)
    );

    PRINT 'Created table warehouse_staff';
END
ELSE
    PRINT 'Table warehouse_staff already exists';
