-- Create Users table
CREATE TABLE [Users] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [Role] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [UQ_Users_Username] UNIQUE ([Username]),
    CONSTRAINT [UQ_Users_Email] UNIQUE ([Email])
);

-- Create UserRoles table
CREATE TABLE [UserRoles] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserName] NVARCHAR(100) NOT NULL,
    [RoleName] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedBy] NVARCHAR(100) NULL,
    [UpdatedBy] NVARCHAR(100) NULL,
    [UpdatedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(100) NULL,
    [DeletedAt] DATETIME2 NULL,
    CONSTRAINT [IX_UserRoles_UserName_IsDeleted] UNIQUE ([UserName], [IsDeleted]) WHERE [IsDeleted] = 0
);

-- Create Coupons table
CREATE TABLE [Coupons] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Code] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [DiscountType] INT NOT NULL,
    [DiscountValue] DECIMAL(18,2) NOT NULL,
    [StartDate] DATETIME2 NULL,
    [EndDate] DATETIME2 NULL,
    [MaxUsageCount] INT NULL,
    [CurrentUsageCount] INT NOT NULL DEFAULT 0,
    [CreatedById] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [UQ_Coupons_Code] UNIQUE ([Code]),
    CONSTRAINT [FK_Coupons_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users]([Id])
);

-- Create CouponAssignments table
CREATE TABLE [CouponAssignments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [CouponId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [AssignedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsUsed] BIT NOT NULL DEFAULT 0,
    [UsedAt] DATETIME2 NULL,
    CONSTRAINT [FK_CouponAssignments_Coupons_CouponId] FOREIGN KEY ([CouponId]) REFERENCES [Coupons]([Id]),
    CONSTRAINT [FK_CouponAssignments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [UQ_CouponAssignments_CouponId_UserId] UNIQUE ([CouponId], [UserId])
);

-- Insert default users
INSERT INTO [Users] ([Username], [Email], [PasswordHash], [Role])
VALUES 
    ('admin', 'admin@example.com', '$2a$11$kj4FkiCkz0lwIBpYPg.oIuQ5jGtS.t6DwDGGqzj3ZdQbRWQ2n0dJC', 1), -- Password: admin123
    ('manager', 'manager@example.com', '$2a$11$3O4HzLqSTJbSRZG1HWA3YerjKF9y/AHqTEEBmU1yq5v0kMh9ccHkm', 2), -- Password: manager123
    ('user', 'user@example.com', '$2a$11$Da1gBqP5pYuHPGZqxqEzh.CQtZ2TNu/8yRkx3.B3XgBMoSKl8H6f.', 3); -- Password: user123

-- Insert Windows Authentication test role (replace 'YourWindowsUsername' with your actual Windows username)
INSERT INTO [UserRoles] ([UserName], [RoleName], [CreatedAt], [IsDeleted])
VALUES ('YourWindowsUsername', 'Admin', GETUTCDATE(), 0);

-- Create sample coupons
INSERT INTO [Coupons] ([Code], [Description], [DiscountType], [DiscountValue], [StartDate], [EndDate], [MaxUsageCount], [CreatedById])
VALUES 
    ('WELCOME10', '10% off for new users', 0, 10.00, GETUTCDATE(), DATEADD(MONTH, 1, GETUTCDATE()), 100, 1),
    ('SUMMER2025', 'Summer sale fixed discount', 1, 25.00, '2025-06-01', '2025-08-31', 500, 1),
    ('VIP50', '50% off for VIP members', 0, 50.00, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE()), NULL, 1);

-- Assign some coupons
INSERT INTO [CouponAssignments] ([CouponId], [UserId], [AssignedAt], [IsUsed])
VALUES 
    (1, 3, GETUTCDATE(), 0),
    (2, 2, GETUTCDATE(), 0),
    (3, 2, GETUTCDATE(), 0);

-- Create indexes for better performance
CREATE INDEX [IX_Coupons_CreatedById] ON [Coupons]([CreatedById]);
CREATE INDEX [IX_CouponAssignments_CouponId] ON [CouponAssignments]([CouponId]);
CREATE INDEX [IX_CouponAssignments_UserId] ON [CouponAssignments]([UserId]);
CREATE INDEX [IX_UserRoles_UserName_RoleName] ON [UserRoles]([UserName], [RoleName]) INCLUDE ([IsDeleted]);