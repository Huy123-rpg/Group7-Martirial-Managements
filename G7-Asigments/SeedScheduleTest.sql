USE [WarehouseDB]
GO

-- ═══════════════════════════════════════════════════════════
-- SEED DỮ LIỆU MẪU — Test luồng Lập lịch
-- ═══════════════════════════════════════════════════════════

-- ── 1. Costing Methods ────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM lkp_costing_methods)
    INSERT INTO lkp_costing_methods (method_id, method_code, method_name) VALUES
    (1, 'FIFO', N'Nhập trước xuất trước'),
    (2, 'LIFO', N'Nhập sau xuất trước'),
    (3, 'WAVG', N'Bình quân gia quyền')
GO

-- ── 2. Schedule Types ─────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM lkp_schedule_types)
    INSERT INTO lkp_schedule_types (type_id, type_code, type_name) VALUES
    (1, 'IMPORT',    N'Nhập hàng'),
    (2, 'EXPORT',    N'Xuất hàng'),
    (3, 'STOCKTAKE', N'Kiểm kho'),
    (4, 'TRANSFER',  N'Điều chuyển kho'),
    (5, 'MAINTAIN',  N'Bảo trì kho')
GO

-- ── 3. Staff accounts (password: Admin@123) ───────────────────
DECLARE @hash NVARCHAR(64) = LOWER(CONVERT(NVARCHAR(64),
    HASHBYTES('SHA2_256', CAST('Admin@123' AS VARCHAR(MAX))), 2))

IF NOT EXISTS (SELECT 1 FROM users WHERE email = 'staff1@warehouse.local')
    INSERT INTO users (id, full_name, email, password_hash, role_id, staff_code, is_active, created_at, updated_at)
    VALUES (NEWID(), N'Trần Thị Staff Một', 'staff1@warehouse.local',
            @hash, 3, 'STF-001', 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())

IF NOT EXISTS (SELECT 1 FROM users WHERE email = 'staff2@warehouse.local')
    INSERT INTO users (id, full_name, email, password_hash, role_id, staff_code, is_active, created_at, updated_at)
    VALUES (NEWID(), N'Lê Văn Staff Hai', 'staff2@warehouse.local',
            @hash, 3, 'STF-002', 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
GO

-- ── 4. Warehouses (manager_id = NULL — tất cả Manager đều thấy)
IF NOT EXISTS (SELECT 1 FROM warehouses WHERE code = 'WH-HCM-01')
    INSERT INTO warehouses (id, code, name, address, city, manager_id, is_active, costing_method, created_at, updated_at)
    VALUES (NEWID(), 'WH-HCM-01', N'Kho Hồ Chí Minh 01',
            N'123 Nguyễn Văn Linh, Q7', N'Hồ Chí Minh',
            NULL, 1, 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())

IF NOT EXISTS (SELECT 1 FROM warehouses WHERE code = 'WH-HN-01')
    INSERT INTO warehouses (id, code, name, address, city, manager_id, is_active, costing_method, created_at, updated_at)
    VALUES (NEWID(), 'WH-HN-01', N'Kho Hà Nội 01',
            N'456 Giải Phóng, Hoàng Mai', N'Hà Nội',
            NULL, 1, 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
GO

-- ── 5. Schedules mẫu ─────────────────────────────────────────
DECLARE @managerId   UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM users WHERE role_id = 2 AND is_active = 1 ORDER BY created_at)
DECLARE @staffId1    UNIQUEIDENTIFIER = (SELECT id FROM users WHERE email = 'staff1@warehouse.local')
DECLARE @staffId2    UNIQUEIDENTIFIER = (SELECT id FROM users WHERE email = 'staff2@warehouse.local')
DECLARE @warehouseId UNIQUEIDENTIFIER = (SELECT id FROM warehouses WHERE code = 'WH-HCM-01')
DECLARE @now         DATETIMEOFFSET   = SYSDATETIMEOFFSET()

DELETE FROM schedules WHERE title LIKE N'[DEMO]%'

INSERT INTO schedules (id, title, schedule_type, status_code, warehouse_id,
     start_time, end_time, created_by, assigned_to, description, is_recurring, created_at, updated_at)
VALUES
(NEWID(), N'[DEMO] Nhập hàng tháng 4 - Lô A',
 1, 'scheduled', @warehouseId,
 DATEADD(DAY,1,@now), DATEADD(HOUR,10,DATEADD(DAY,1,@now)),
 @managerId, @staffId1, N'Nhập lô hàng điện tử từ NCC ABC', 0, @now, @now),

(NEWID(), N'[DEMO] Xuất hàng Khách hàng XYZ',
 2, 'scheduled', @warehouseId,
 DATEADD(HOUR,2,@now), DATEADD(HOUR,5,@now),
 @managerId, @staffId1, N'Xuất đơn SO-2024-001', 0, @now, @now),

(NEWID(), N'[DEMO] Kiểm kho quý 1',
 3, 'in_progress', @warehouseId,
 DATEADD(HOUR,-3,@now), DATEADD(HOUR,2,@now),
 @managerId, @staffId2, N'Kiểm kê khu vực A', 0, @now, @now),

(NEWID(), N'[DEMO] Nhập hàng tháng 3 - Lô cuối',
 1, 'done', @warehouseId,
 DATEADD(DAY,-3,@now), DATEADD(HOUR,8,DATEADD(DAY,-3,@now)),
 @managerId, @staffId2, N'Đã nhập 500 sản phẩm', 0, @now, @now),

(NEWID(), N'[DEMO] Bảo trì kho tháng 3',
 5, 'cancelled', @warehouseId,
 DATEADD(DAY,-1,@now), DATEADD(HOUR,4,DATEADD(DAY,-1,@now)),
 @managerId, @staffId1, N'Hủy do thiếu nhân lực', 0, @now, @now),

(NEWID(), N'[DEMO] Điều chuyển sang Kho HN',
 4, 'scheduled', @warehouseId,
 DATEADD(DAY,3,@now), DATEADD(HOUR,6,DATEADD(DAY,3,@now)),
 @managerId, NULL, N'Chưa phân công', 0, @now, @now)
GO

-- ── 6. Kết quả ───────────────────────────────────────────────
SELECT type_id, type_code, type_name FROM lkp_schedule_types ORDER BY type_id
SELECT code, name FROM warehouses WHERE is_active = 1
SELECT full_name, email, role_id FROM users WHERE role_id IN (2,3) ORDER BY role_id
SELECT title, status_code FROM schedules WHERE title LIKE '[DEMO]%'
GO
