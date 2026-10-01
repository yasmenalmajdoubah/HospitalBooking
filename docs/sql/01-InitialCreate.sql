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
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE TABLE [AppointmentStatuses] (
        [Id] int NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [NameAr] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_AppointmentStatuses] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE TABLE [Departments] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [NameAr] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] int NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [NameAr] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [UserName] nvarchar(100) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [FullNameAr] nvarchar(200) NULL,
        [Phone] nvarchar(30) NULL,
        [RoleId] int NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE TABLE [Doctors] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [DepartmentId] int NOT NULL,
        [Specialty] nvarchar(150) NOT NULL,
        [SpecialtyAr] nvarchar(150) NOT NULL,
        [LicenseNumber] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Doctors_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Doctors_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE TABLE [Patients] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NULL,
        [NationalId] nvarchar(50) NULL,
        [DateOfBirth] date NULL,
        [Gender] nvarchar(20) NULL,
        CONSTRAINT [PK_Patients] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Patients_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE TABLE [DoctorSchedules] (
        [Id] int NOT NULL IDENTITY,
        [DoctorId] int NOT NULL,
        [DayOfWeek] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [SlotDurationMinutes] int NOT NULL DEFAULT 30,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_DoctorSchedules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorSchedules_Doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE TABLE [Appointments] (
        [Id] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [DoctorId] int NOT NULL,
        [AppointmentDate] date NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [StatusId] int NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [CreatedByUserId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Appointments_AppointmentStatuses_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [AppointmentStatuses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Appointments_Doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Appointments_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'NameAr') AND [object_id] = OBJECT_ID(N'[AppointmentStatuses]'))
        SET IDENTITY_INSERT [AppointmentStatuses] ON;
    EXEC(N'INSERT INTO [AppointmentStatuses] ([Id], [Name], [NameAr])
    VALUES (1, N''Pending'', N''قيد الانتظار''),
    (2, N''Confirmed'', N''مؤكد''),
    (3, N''Cancelled'', N''ملغي''),
    (4, N''Completed'', N''مكتمل'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'NameAr') AND [object_id] = OBJECT_ID(N'[AppointmentStatuses]'))
        SET IDENTITY_INSERT [AppointmentStatuses] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'NameAr') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([Id], [Name], [NameAr])
    VALUES (1, N''Admin'', N''مدير النظام''),
    (2, N''Doctor'', N''طبيب''),
    (3, N''Receptionist'', N''موظف استقبال''),
    (4, N''Patient'', N''مريض'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'NameAr') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointments_CreatedByUserId] ON [Appointments] ([CreatedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Appointments_DoctorId_AppointmentDate_StartTime] ON [Appointments] ([DoctorId], [AppointmentDate], [StartTime]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointments_PatientId_AppointmentDate] ON [Appointments] ([PatientId], [AppointmentDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointments_StatusId] ON [Appointments] ([StatusId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AppointmentStatuses_Name] ON [AppointmentStatuses] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Departments_Name] ON [Departments] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Doctors_DepartmentId] ON [Doctors] ([DepartmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Doctors_LicenseNumber] ON [Doctors] ([LicenseNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Doctors_UserId] ON [Doctors] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DoctorSchedules_DoctorId_DayOfWeek_StartTime] ON [DoctorSchedules] ([DoctorId], [DayOfWeek], [StartTime]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Patients_NationalId] ON [Patients] ([NationalId]) WHERE [NationalId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Patients_UserId] ON [Patients] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_Name] ON [Roles] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_UserName] ON [Users] ([UserName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001100155_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261001100155_InitialCreate', N'8.0.21');
END;
GO

COMMIT;
GO

