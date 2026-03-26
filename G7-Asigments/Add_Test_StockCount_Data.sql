-- ========================================================================================
-- SCRIPT MẪU DATA THỰC HIỆN KIỂM KHO (DÀNH CHO STAFF)
-- ========================================================================================
USE WarehouseDB;
GO
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- 1. Tìm Staff, Warehouse, Zone, Product hiện có để tạo Session Kiểm Kho
DECLARE @AdminId UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM users WHERE role_id = 1 AND is_active = 1);
DECLARE @StaffId UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM users WHERE role_id = 3 AND is_active = 1);

DECLARE @WarehouseId UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM warehouses WHERE name LIKE '%HN%' OR code LIKE '%TEST%');
IF @WarehouseId IS NULL SET @WarehouseId = (SELECT TOP 1 id FROM warehouses);

DECLARE @ZoneId UNIQUEIDENTIFIER = (SELECT TOP 1 id FROM warehouse_zones WHERE warehouse_id = @WarehouseId);
-- Nếu thiếu data cơ bản thì báo lỗi
IF @StaffId IS NULL OR @WarehouseId IS NULL
BEGIN
    PRINT 'Không tìm thấy dữ liệu Staff hoặc Warehouse. Vui lòng kiểm tra lại data mẫu!';
    RETURN;
END

-- 2. Tạo Stock Count Session
DECLARE @SessionId UNIQUEIDENTIFIER = NEWID();

INSERT INTO stock_count_sessions 
(id, session_code, warehouse_id, zone_id, status_id, count_type, created_by, assigned_to, planned_date, created_at)
VALUES 
(@SessionId, 'SC-TEST-' + LEFT(CONVERT(VARCHAR(255), NEWID()), 4), @WarehouseId, @ZoneId, 2, 'cycle', @AdminId, @StaffId, CAST(SYSDATETIMEOFFSET() AS DATE), SYSDATETIMEOFFSET());

-- Tạo Stock Count Items bằng cách lấy 8 sản phẩm (hoặc ít hơn)
INSERT INTO stock_count_items
(id, session_id, product_id, zone_id, batch_number, qty_system, unit_cost)
SELECT TOP 8
    NEWID(), 
    @SessionId, 
    id, 
    @ZoneId, 
    'B-TST-' + LEFT(CONVERT(VARCHAR(255), NEWID()), 4), 
    FLOOR(RAND(CHECKSUM(NEWID())) * 100) + 10, -- Số lượng tồn HT giả (từ 10 đến 110)
    ROUND(RAND(CHECKSUM(NEWID())) * 1000000 + 10000, 0) -- Giá ngẫu nhiên
FROM products;

PRINT '>>> TẠO PHIẾU KIỂM KHO TEST THÀNH CÔNG! <<<';
PRINT '- Vui lòng Đăng nhập bằng tài khoản Staff (RoleId = 3).';
PRINT '- Vào màn hình "Thực hiện kiểm kho", bạn sẽ thấy 1 phiếu chờ đếm.';
