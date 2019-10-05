ALTER TABLE [dbo].[Assignments] DROP CONSTRAINT [FK_Assignments_SantaUsers_Giver]
GO
ALTER TABLE [dbo].[Abandoned] DROP CONSTRAINT [FK_Abandoned_SantaUsers]
GO
ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_Messages_SantaUsers_RecipientId]
GO
ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_Messages_SantaUsers_SenderId]
GO
ALTER TABLE [dbo].[Assignments] DROP CONSTRAINT [FK_Assignments_SantaUsers_Rec]
GO



truncate table [SantaAdmins]
truncate table [SantaUsers]
truncate table [SantaSettings]
truncate table [Messages]
truncate table [Abandoned]
truncate table [Assignments]
GO

ALTER TABLE [dbo].[Messages]  WITH NOCHECK ADD  CONSTRAINT [FK_Messages_SantaUsers_RecipientId] FOREIGN KEY([RecipientId])
REFERENCES [dbo].[SantaUsers] ([Id])
GO

ALTER TABLE [dbo].[Messages] NOCHECK CONSTRAINT [FK_Messages_SantaUsers_RecipientId]
GO

ALTER TABLE [dbo].[Messages]  WITH NOCHECK ADD  CONSTRAINT [FK_Messages_SantaUsers_SenderId] FOREIGN KEY([Id])
REFERENCES [dbo].[SantaUsers] ([Id])
GO

ALTER TABLE [dbo].[Messages] NOCHECK CONSTRAINT [FK_Messages_SantaUsers_SenderId]
GO

ALTER TABLE [dbo].[Abandoned]  WITH CHECK ADD  CONSTRAINT [FK_Abandoned_SantaUsers] FOREIGN KEY([SantaUserId])
REFERENCES [dbo].[SantaUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Abandoned] CHECK CONSTRAINT [FK_Abandoned_SantaUsers]
GO

ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD  CONSTRAINT [FK_Assignments_SantaUsers_Giver] FOREIGN KEY([GiverId])
REFERENCES [dbo].[SantaUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Assignments] CHECK CONSTRAINT [FK_Assignments_SantaUsers_Giver]
GO

ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD  CONSTRAINT [FK_Assignments_SantaUsers_Rec] FOREIGN KEY([RecepientId])
REFERENCES [dbo].[SantaUsers] ([Id])
GO

ALTER TABLE [dbo].[Assignments] CHECK CONSTRAINT [FK_Assignments_SantaUsers_Rec]
GO


