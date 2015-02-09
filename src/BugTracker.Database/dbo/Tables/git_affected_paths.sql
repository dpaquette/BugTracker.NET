CREATE TABLE [dbo].[git_affected_paths] (
    [gitap_id]        INT            IDENTITY (1, 1) NOT NULL,
    [gitap_gitcom_id] INT            NOT NULL,
    [gitap_action]    NVARCHAR (8)   NOT NULL,
    [gitap_path]      NVARCHAR (400) NOT NULL,
    CONSTRAINT [pk_git_affected_paths] PRIMARY KEY NONCLUSTERED ([gitap_id] ASC)
);


GO
CREATE CLUSTERED INDEX [gitap_gitcom_index]
    ON [dbo].[git_affected_paths]([gitap_gitcom_id] ASC);

