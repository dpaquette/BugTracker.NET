CREATE TABLE [dbo].[project_user_xref] (
    [pu_id]               INT IDENTITY (1, 1) NOT NULL,
    [pu_project]          INT NOT NULL,
    [pu_user]             INT NOT NULL,
    [pu_auto_subscribe]   INT DEFAULT ((0)) NOT NULL,
    [pu_permission_level] INT DEFAULT ((2)) NOT NULL,
    [pu_admin]            INT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [pk_project_user_xref] PRIMARY KEY CLUSTERED ([pu_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [pu_index_1]
    ON [dbo].[project_user_xref]([pu_project] ASC, [pu_user] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [pu_index_2]
    ON [dbo].[project_user_xref]([pu_user] ASC, [pu_project] ASC);

