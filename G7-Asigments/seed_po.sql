USE [WarehouseDB]
GO

IF NOT EXISTS (SELECT 1 FROM warehouses)
BEGIN
    INSERT INTO warehouses (id, code, name, address, city, is_active, costing_method, created_at, updated_at)
    VALUES 
    (NEWID(), 'WH-01', N'Kho Tổng (Hà Nội)', N'123 Đường Láng, Đống Đa', N'Hà Nội', 1, 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
    (NEWID(), 'WH-02', N'Kho Miền Nam', N'456 Lê Lợi, Quận 1', N'TP.HCM', 1, 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
    
    PRINT 'Inserted dummy Warehouses'
END
GO

IF NOT EXISTS (SELECT 1 FROM suppliers)
BEGIN
    INSERT INTO suppliers (id, supplier_code, supplier_name, contact_person, phone, email, address, is_active, portal_enabled, created_at, updated_at)
    VALUES 
    (NEWID(), 'SUP-001', N'Công ty CP Nhựa Duy Tân', N'Nguyễn Văn A', '0901234567', 'contact@duytan.com', N'TP.HCM', 1, 0, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
    (NEWID(), 'SUP-002', N'Nhà máy Sắt thép Hòa Phát', N'Trần Thị B', '0987654321', 'sales@hoaphat.vn', N'Bắc Ninh', 1, 0, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
    
    PRINT 'Inserted dummy Suppliers'
END
GO
