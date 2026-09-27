

-- Migration for PMC 
--Add-Migration AddArticleIndexes -StartupProject Syed.PortfolioPlatform.Web

--Script-Migration -Project Syed.PortfolioPlatform.Infrastructure -StartupProject Syed.PortfolioPlatform.Web -From AddContactMessagesTable -To AddArticleIndexes -Output AddArticleIndexes.sql



BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Articles]') AND [c].[name] = N'ImageUrl');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Articles] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [dbo].[Articles] ALTER COLUMN [ImageUrl] nvarchar(500) NOT NULL;
GO

CREATE INDEX [IX_Articles_CreatedOn] ON [dbo].[Articles] ([CreatedOn]);
GO

CREATE INDEX [IX_Articles_IsPublished_IsDeleted] ON [dbo].[Articles] ([IsPublished], [IsDeleted]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260309124643_AddArticleIndexes', N'8.0.24');
GO

COMMIT;
GO

