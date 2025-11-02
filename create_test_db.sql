CREATE DATABASE CouponTestDb;
GO

USE CouponTestDb;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);
GO

CREATE TABLE Coupons (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(50) UNIQUE NOT NULL,
    Description NVARCHAR(500),
    DiscountAmount INT NOT NULL,
    DiscountType NVARCHAR(20) NOT NULL,
    ExpirationDate DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById INT NOT NULL,
    FOREIGN KEY (CreatedById) REFERENCES Users(Id)
);
GO

CREATE TABLE CouponAssignments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CouponId INT NOT NULL,
    UserId INT NOT NULL,
    IsUsed BIT NOT NULL DEFAULT 0,
    UsedAt DATETIME2 NULL,
    AssignedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (CouponId) REFERENCES Coupons(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE (CouponId, UserId)
);
GO

-- Insert test users
INSERT INTO Users (Username, Email, PasswordHash, Role) VALUES 
('admin', 'admin@example.com', '$2a$11$8vGuJEd4p9jQj098CnD12u1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t1u2v3w4x5y6z7a8b9c0d1e2f3g4h5i6j7k8l9m0n1o2p3q4r5s6t7u8v9w0x1y2z3a4b5c6d7e8f9g0h1i2j3k4l5m6n7o8p9q0r1s2t3u4v5w6x7y8z9', 'Admin'),
('manager', 'manager@example.com', '$2a$11$8vGuJEd4p9jQj098CnD12u1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t1u2v3w4x5y6z7a8b9c0d1e2f3g4h5i6j7k8l9m0n1o2p3q4r5s6t7u8v9w0x1y2z3a4b5c6d7e8f9g0h1i2j3k4l5m6n7o8p9q0r1s2t3u4v5w6x7y8z9', 'Manager'),
('user1', 'user1@example.com', '$2a$11$8vGuJEd4p9jQj098CnD12u1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t1u2v3w4x5y6z7a8b9c0d1e2f3g4h5i6j7k8l9m0n1o2p3q4r5s6t7u8v9w0x1y2z3a4b5c6d7e8f9g0h1i2j3k4l5m6n7o8p9q0r1s2t3u4v5w6x7y8z9', 'User'),
('user2', 'user2@example.com', '$2a$11$8vGuJEd4p9jQj098CnD12u1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t1u2v3w4x5y6z7a8b9c0d1e2f3g4h5i6j7k8l9m0n1o2p3q4r5s6t7u8v9w0x1y2z3a4b5c6d7e8f9g0h1i2j3k4l5m6n7o8p9q0r1s2t3u4v5w6x7y8z9', 'User'),
('user3', 'user3@example.com', '$2a$11$8vGuJEd4p9jQj098CnD12u1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t1u2v3w4x5y6z7a8b9c0d1e2f3g4h5i6j7k8l9m0n1o2p3q4r5s6t7u8v9w0x1y2z3a4b5c6d7e8f9g0h1i2j3k4l5m6n7o8p9q0r1s2t3u4v5w6x7y8z9', 'User');
GO

-- Insert test coupons
INSERT INTO Coupons (Code, Description, DiscountAmount, DiscountType, CreatedById) VALUES 
('SAVE10', '10% off your purchase', 10, 'percentage', 2),
('SAVE20', '20% off your purchase', 20, 'percentage', 2),
('FLAT5', '£5 off your purchase', 5, 'fixed', 2),
('WELCOME', 'Welcome discount', 15, 'percentage', 2);
GO

-- Assign some coupons to users
INSERT INTO CouponAssignments (CouponId, UserId) VALUES 
(1, 3),
(2, 3),
(3, 4),
(1, 5);
GO