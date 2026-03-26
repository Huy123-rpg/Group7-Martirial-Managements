-- Seed 4 tài khoản cho mỗi role (trừ Admin)
-- Password mặc định: 123456 (SHA256)
-- Hash: 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92

DECLARE @hash NVARCHAR(64) = '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92';
DECLARE @now DATETIMEOFFSET = SYSDATETIMEOFFSET();

INSERT INTO users (id, staff_code, full_name, email, password_hash, role_id, is_active, created_at, updated_at)
VALUES
-- ── Manager (role_id = 2) ──────────────────────────────────────────────────
(NEWID(), 'MGR001', N'Nguyễn Văn Manager 1', 'manager1@warehouse.vn', @hash, 2, 1, @now, @now),
(NEWID(), 'MGR002', N'Trần Thị Manager 2',   'manager2@warehouse.vn', @hash, 2, 1, @now, @now),
(NEWID(), 'MGR003', N'Lê Văn Manager 3',     'manager3@warehouse.vn', @hash, 2, 1, @now, @now),
(NEWID(), 'MGR004', N'Phạm Thị Manager 4',   'manager4@warehouse.vn', @hash, 2, 1, @now, @now),

-- ── Staff (role_id = 3) ───────────────────────────────────────────────────
(NEWID(), 'STF001', N'Nguyễn Văn Staff 1',   'staff1@warehouse.vn',   @hash, 3, 1, @now, @now),
(NEWID(), 'STF002', N'Trần Thị Staff 2',     'staff2@warehouse.vn',   @hash, 3, 1, @now, @now),
(NEWID(), 'STF003', N'Lê Văn Staff 3',       'staff3@warehouse.vn',   @hash, 3, 1, @now, @now),
(NEWID(), 'STF004', N'Phạm Thị Staff 4',     'staff4@warehouse.vn',   @hash, 3, 1, @now, @now),

-- ── Accountant (role_id = 4) ──────────────────────────────────────────────
(NEWID(), 'ACC001', N'Nguyễn Văn Accountant 1', 'accountant1@warehouse.vn', @hash, 4, 1, @now, @now),
(NEWID(), 'ACC002', N'Trần Thị Accountant 2',   'accountant2@warehouse.vn', @hash, 4, 1, @now, @now),
(NEWID(), 'ACC003', N'Lê Văn Accountant 3',     'accountant3@warehouse.vn', @hash, 4, 1, @now, @now),
(NEWID(), 'ACC004', N'Phạm Thị Accountant 4',   'accountant4@warehouse.vn', @hash, 4, 1, @now, @now),

-- ── Supplier (role_id = 5) ────────────────────────────────────────────────
(NEWID(), 'SUP001', N'Nguyễn Văn Supplier 1', 'supplier1@warehouse.vn', @hash, 5, 1, @now, @now),
(NEWID(), 'SUP002', N'Trần Thị Supplier 2',   'supplier2@warehouse.vn', @hash, 5, 1, @now, @now),
(NEWID(), 'SUP003', N'Lê Văn Supplier 3',     'supplier3@warehouse.vn', @hash, 5, 1, @now, @now),
(NEWID(), 'SUP004', N'Phạm Thị Supplier 4',   'supplier4@warehouse.vn', @hash, 5, 1, @now, @now);
