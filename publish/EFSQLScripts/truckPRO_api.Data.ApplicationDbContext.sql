IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905025522_init'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240905025522_init', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905222336_InitialCreate'
)
BEGIN
    CREATE TABLE [Companys] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Companys] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905222336_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(50) NOT NULL,
        [LastName] nvarchar(50) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [Role] int NOT NULL,
        [CompanyId] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_Companys_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companys] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905222336_InitialCreate'
)
BEGIN
    CREATE TABLE [LogEntries] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [StartTime] datetime2 NOT NULL,
        [EndTime] datetime2 NULL,
        [LogEntryType] int NOT NULL,
        CONSTRAINT [PK_LogEntries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LogEntries_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905222336_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_LogEntries_UserId] ON [LogEntries] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905222336_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Users_CompanyId] ON [Users] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905222336_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240905222336_InitialCreate', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905224853_SecondM'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240905224853_SecondM', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225012_ThirdM'
)
BEGIN
    ALTER TABLE [LogEntries] DROP CONSTRAINT [FK_LogEntries_Users_UserId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225012_ThirdM'
)
BEGIN
    ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Companys_CompanyId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225012_ThirdM'
)
BEGIN
    ALTER TABLE [Users] DROP CONSTRAINT [PK_Users];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225012_ThirdM'
)
BEGIN
    EXEC sp_rename N'[Users]', N'User';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225012_ThirdM'
)
BEGIN
    EXEC sp_rename N'[User].[IX_Users_CompanyId]', N'IX_User_CompanyId', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225012_ThirdM'
)
BEGIN
    ALTER TABLE [User] ADD CONSTRAINT [PK_User] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225012_ThirdM'
)
BEGIN
    ALTER TABLE [LogEntries] ADD CONSTRAINT [FK_LogEntries_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225012_ThirdM'
)
BEGIN
    ALTER TABLE [User] ADD CONSTRAINT [FK_User_Companys_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companys] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225012_ThirdM'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240905225012_ThirdM', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225150_TableNames'
)
BEGIN
    ALTER TABLE [User] DROP CONSTRAINT [FK_User_Companys_CompanyId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225150_TableNames'
)
BEGIN
    ALTER TABLE [Companys] DROP CONSTRAINT [PK_Companys];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225150_TableNames'
)
BEGIN
    EXEC sp_rename N'[Companys]', N'Company';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225150_TableNames'
)
BEGIN
    ALTER TABLE [Company] ADD CONSTRAINT [PK_Company] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225150_TableNames'
)
BEGIN
    ALTER TABLE [User] ADD CONSTRAINT [FK_User_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225150_TableNames'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240905225150_TableNames', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225237_TableName'
)
BEGIN
    ALTER TABLE [LogEntries] DROP CONSTRAINT [FK_LogEntries_User_UserId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225237_TableName'
)
BEGIN
    ALTER TABLE [LogEntries] DROP CONSTRAINT [PK_LogEntries];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225237_TableName'
)
BEGIN
    EXEC sp_rename N'[LogEntries]', N'LogEntry';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225237_TableName'
)
BEGIN
    EXEC sp_rename N'[LogEntry].[IX_LogEntries_UserId]', N'IX_LogEntry_UserId', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225237_TableName'
)
BEGIN
    ALTER TABLE [LogEntry] ADD CONSTRAINT [PK_LogEntry] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225237_TableName'
)
BEGIN
    ALTER TABLE [LogEntry] ADD CONSTRAINT [FK_LogEntry_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905225237_TableName'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240905225237_TableName', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911220956_UpdateLogEntrySchema'
)
BEGIN
    ALTER TABLE [LogEntry] ADD [ImageUrls] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911220956_UpdateLogEntrySchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240911220956_UpdateLogEntrySchema', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240913204255_CompanyIdValidation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240913204255_CompanyIdValidation', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240913204517_CompanyIdMayBeNull'
)
BEGIN
    ALTER TABLE [User] DROP CONSTRAINT [FK_User_Company_CompanyId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240913204517_CompanyIdMayBeNull'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'CompanyId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [User] ALTER COLUMN [CompanyId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240913204517_CompanyIdMayBeNull'
)
BEGIN
    ALTER TABLE [User] ADD CONSTRAINT [FK_User_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240913204517_CompanyIdMayBeNull'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240913204517_CompanyIdMayBeNull', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240916002954_AddEmailVerification'
)
BEGIN
    ALTER TABLE [User] ADD [EmailVerificationToken] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240916002954_AddEmailVerification'
)
BEGIN
    ALTER TABLE [User] ADD [EmailVerified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240916002954_AddEmailVerification'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240916002954_AddEmailVerification', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240916012142_EmailVerificationTokenCanBeNUll'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240916012142_EmailVerificationTokenCanBeNUll', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240916012322_EmailVerificationTokenCanBeNUll2'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'EmailVerificationToken');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [User] ALTER COLUMN [EmailVerificationToken] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240916012322_EmailVerificationTokenCanBeNUll2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240916012322_EmailVerificationTokenCanBeNUll2', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240916230016_AddPendingUserTableFinal'
)
BEGIN
    CREATE TABLE [PendingUser] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CompanyId] int NOT NULL,
        [InvitationSent] bit NOT NULL,
        CONSTRAINT [PK_PendingUser] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240916230016_AddPendingUserTableFinal'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240916230016_AddPendingUserTableFinal', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240917001025_UpdatePendingUserTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240917001025_UpdatePendingUserTable', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240917024339_AddLogEntryApprovalByManagerfield'
)
BEGIN
    ALTER TABLE [LogEntry] ADD [IsApprovedByManager] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240917024339_AddLogEntryApprovalByManagerfield'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240917024339_AddLogEntryApprovalByManagerfield', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240918013800_ChageDatabase'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240918013800_ChageDatabase', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241012162918_newdb'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241012162918_newdb', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241023023250_AddNotificationsTable'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Email');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [User] ALTER COLUMN [Email] nvarchar(450) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241023023250_AddNotificationsTable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_User_Email] ON [User] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241023023250_AddNotificationsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241023023250_AddNotificationsTable', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241030051101_AddVpsMySqlServer'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241030051101_AddVpsMySqlServer', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241031044338_AddVPSMSSQL'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241031044338_AddVPSMSSQL', N'8.0.8');
END;
GO

COMMIT;
GO

