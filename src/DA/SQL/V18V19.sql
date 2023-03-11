--script-migration -From v17_EffortDecimal -to v19_logex

DROP TABLE [Log];

GO

CREATE TABLE [Log] (
    [LogID] int NOT NULL IDENTITY,
    [DateTime] datetime2 NOT NULL,
    [Event] int NOT NULL,
    [Note] nvarchar(max) NULL,
    [Operation] nvarchar(max) NULL,
    [Severity] int NOT NULL,
    [Text] nvarchar(max) NULL,
    [Token] nvarchar(max) NULL,
    [UserID] int NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY ([LogID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170922132847_v18_Log', N'2.0.0-rtm-26452');

GO

ALTER TABLE [Log] ADD [Client] nvarchar(15) NULL;

GO

ALTER TABLE [Log] ADD [ClientDetails] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170922145412_v19_LogEx', N'2.0.0-rtm-26452');

GO

