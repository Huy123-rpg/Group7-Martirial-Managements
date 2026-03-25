-- ========================================================================================
-- SCRIPT TEST DỮ LIỆU TỒN KHO & PHÂN QUYỀN (INVENTORY LOOKUP)
-- V2: Sửa lỗi Tên bảng "inventory" và UNIQUE CONSTRAINT "products"
-- ========================================================================================
USE WarehouseDB; -- Đổi tên Database nếu khác
GO

-- 1. Lấy ID của các Role & User có sẵn
DECLARE @AdminId UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM users WHERE role_id = 1 AND is_active=1);
DECLARE @ManagerId UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM users WHERE role_id = 2 AND is_active=1);
DECLARE @StaffId UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM users WHERE role_id = 3 AND is_active=1);

IF @ManagerId IS NULL OR @StaffId IS NULL
BEGIN
    PRINT 'Chưa có dữ liệu User Manager/Staff! Vui lòng tạo User trước hoặc DB bạn đang thiếu data seed ban đầu.';
    RETURN;
END

-- 2. Tạo 2 Kho Test
DECLARE @Warehouse1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Warehouse2Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO warehouses (id, code, name, city, is_active, costing_method, created_at, updated_at, manager_id)
VALUES 
(@Warehouse1Id, 'WH-TEST-' + LEFT(CONVERT(VARCHAR(255), NEWID()), 4), N'Kho SG (Manager)', N'Hồ Chí Minh', 1, 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), @ManagerId),
(@Warehouse2Id, 'WH-TEST-' + LEFT(CONVERT(VARCHAR(255), NEWID()), 4), N'Kho HN (Staff)', N'Hà Nội', 1, 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), NULL);

-- 3. Tạo Zone cho 2 kho
DECLARE @Zone1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Zone2Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO warehouse_zones (id, warehouse_id, zone_code, zone_name, zone_type, capacity_m3, capacity_pallet, is_active)
VALUES
(@Zone1Id, @Warehouse1Id, 'Z-SG-A', N'Khu A Sài Gòn', 1, 100, 50, 1),
(@Zone2Id, @Warehouse2Id, 'Z-HN-B', N'Khu B Hà Nội', 1, 200, 100, 1);

-- 4. Sử dụng Sản phẩm có sẵn hoặc tạo mới để tránh vi phạm UNIQUE KEY (barcode)
DECLARE @Prod1Id UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM products ORDER BY created_at DESC);
DECLARE @Prod2Id UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM products WHERE id <> @Prod1Id ORDER BY created_at DESC);

IF @Prod1Id IS NULL
BEGIN
    SET @Prod1Id = NEWID();
    INSERT INTO products (id, sku, barcode, product_name, reorder_point, min_stock, safety_stock, is_batch_tracked, is_expiry_tracked, is_active, created_at, updated_at)
    VALUES (@Prod1Id, 'SKU-001', 'BARCODE-001', N'Điện Thoại X', 10, 5, 20, 0, 0, 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET());
END

IF @Prod2Id IS NULL
BEGIN
    SET @Prod2Id = NEWID();
    INSERT INTO products (id, sku, barcode, product_name, reorder_point, min_stock, safety_stock, is_batch_tracked, is_expiry_tracked, is_active, created_at, updated_at)
    VALUES (@Prod2Id, 'SKU-002', 'BARCODE-002', N'Laptop P', 5, 2, 10, 0, 0, 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET());
END


-- 5. Tạo Tồn kho (inventory)
-- Lưu ý: EF Core ánh xạ sang bảng "inventory" dưới dạng số ít.
INSERT INTO inventory (id, product_id, warehouse_id, zone_id, qty_on_hand, qty_reserved, qty_incoming, avg_cost, last_updated)
VALUES
(NEWID(), @Prod1Id, @Warehouse1Id, @Zone1Id, 1500, 100, 200, 25000000, SYSDATETIMEOFFSET()),
(NEWID(), @Prod2Id, @Warehouse1Id, @Zone1Id, 320, 20, 50, 45000000, SYSDATETIMEOFFSET()),
(NEWID(), @Prod1Id, @Warehouse2Id, @Zone2Id, 500, 0, 0, 25000000, SYSDATETIMEOFFSET());

-- 6. Gán Staff vào Kho HN qua Schedule
DECLARE @ScheduleTypeId TINYINT = (SELECT TOP 1 id FROM lkp_schedule_type);
IF @ScheduleTypeId IS NULL SET @ScheduleTypeId = 1;

INSERT INTO schedules (id, title, schedule_type, status_code, warehouse_id, assigned_to, created_by, start_time, is_recurring, created_at, updated_at)
VALUES
(NEWID(), N'Làm việc tại Kho HN', @ScheduleTypeId, 'PENDING', @Warehouse2Id, @StaffId, @AdminId, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET());

PRINT '>>> ĐÃ TẠO DỮ LIỆU TEST THÀNH CÔNG! <<<';
PRINT '- Admin: Thấy cả 2 kho';
PRINT '- Manager (đang cấu hình): Chỉ thấy Kho SG';
PRINT '- Staff (đang cấu hình): Chỉ thấy Kho HN';
