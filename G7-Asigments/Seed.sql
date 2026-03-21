USE [WarehouseDB]
GO

-- ═══════════════════════════════════════════════════════════════
-- SEED DATA — chạy sau khi đã chạy Db.sql
-- ═══════════════════════════════════════════════════════════════

-- ── 1. Vai trò người dùng ─────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM lkp_user_roles)
BEGIN
    INSERT INTO lkp_user_roles (role_id, role_code, role_name) VALUES
    (1, 'ADMIN',      N'Quản trị viên'),
    (2, 'MANAGER',    N'Quản lý kho'),
    (3, 'STAFF',      N'Nhân viên'),
    (4, 'ACCOUNTANT', N'Kế toán'),
    (5, 'SUPPLIER',   N'Nhà cung cấp')
    PRINT 'lkp_user_roles: inserted 5 rows'
END
ELSE
    PRINT 'lkp_user_roles: already has data, skipped'
GO

-- ── 2. Tài khoản Admin đầu tiên ──────────────────────────────
-- Đăng nhập: email = admin@warehouse.local / password = Admin@123
-- Hash = SHA-256(UTF-8("Admin@123")) — khớp với C# AuthService
IF NOT EXISTS (SELECT 1 FROM users WHERE email = 'admin@warehouse.local')
BEGIN
    DECLARE @hash NVARCHAR(64) =
        LOWER(CONVERT(NVARCHAR(64),
            HASHBYTES('SHA2_256', CAST('Admin@123' AS VARCHAR(MAX))),
        2))

    INSERT INTO users (id, full_name, email, password_hash, role_id, is_active, created_at, updated_at)
    VALUES (
        NEWID(),
        N'Quản trị viên',
        'admin@warehouse.local',
        @hash,
        1,
        1,
        SYSDATETIMEOFFSET(),
        SYSDATETIMEOFFSET()
    )
    PRINT 'users: inserted admin (email: admin@warehouse.local / password: Admin@123)'
END
ELSE
    PRINT 'users: admin already exists, skipped'
GO

-- ── 3. Kiểm tra kết quả ──────────────────────────────────────
SELECT role_id, role_code, role_name FROM lkp_user_roles ORDER BY role_id
SELECT full_name, email, role_id, is_active FROM users
GO
