-- Script SQL pour créer les tables BankTransfers et Payments
-- Exécutez ce script dans SQL Server Management Studio ou via sqlcmd

USE [CreditCardDB];
GO

-- Créer la table BankTransfers si elle n'existe pas
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BankTransfers')
BEGIN
    CREATE TABLE [BankTransfers] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [FromAccount] nvarchar(100) NOT NULL,
        [ToAccount] nvarchar(100) NOT NULL,
        [BeneficiaryName] nvarchar(200) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Currency] nvarchar(3) NOT NULL,
        [Description] nvarchar(500) NULL,
        [TransferType] nvarchar(50) NOT NULL,
        [TransferDate] datetime2 NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_BankTransfers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BankTransfers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_BankTransfers_UserId] ON [BankTransfers] ([UserId]);
    CREATE INDEX [IX_BankTransfers_TransferDate] ON [BankTransfers] ([TransferDate]);
    CREATE INDEX [IX_BankTransfers_Status] ON [BankTransfers] ([Status]);
    
    PRINT 'Table BankTransfers created successfully.';
END
ELSE
BEGIN
    PRINT 'Table BankTransfers already exists.';
END
GO

-- Créer la table Payments si elle n'existe pas
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Payments')
BEGIN
    CREATE TABLE [Payments] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [CreditCardId] uniqueidentifier NULL,
        [PaymentType] nvarchar(100) NOT NULL,
        [MerchantName] nvarchar(200) NOT NULL,
        [ReferenceNumber] nvarchar(100) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Currency] nvarchar(3) NOT NULL,
        [Description] nvarchar(500) NULL,
        [PaymentDate] datetime2 NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [ReceiptNumber] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Payments_CreditCards_CreditCardId] FOREIGN KEY ([CreditCardId]) REFERENCES [CreditCards] ([Id]) ON DELETE SET NULL
    );
    
    CREATE INDEX [IX_Payments_UserId] ON [Payments] ([UserId]);
    CREATE INDEX [IX_Payments_CreditCardId] ON [Payments] ([CreditCardId]);
    CREATE INDEX [IX_Payments_PaymentDate] ON [Payments] ([PaymentDate]);
    CREATE INDEX [IX_Payments_Status] ON [Payments] ([Status]);
    CREATE INDEX [IX_Payments_PaymentType] ON [Payments] ([PaymentType]);
    
    PRINT 'Table Payments created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Payments already exists.';
END
GO

PRINT 'Script completed.';
