CREATE TABLE [dbo].[git_commits] (
    [gitcom_id]         INT            IDENTITY (1, 1) NOT NULL,
    [gitcom_commit]     CHAR (40)      NULL,
    [gitcom_bug]        INT            NOT NULL,
    [gitcom_repository] NVARCHAR (400) NOT NULL,
    [gitcom_author]     NVARCHAR (100) NOT NULL,
    [gitcom_git_date]   NVARCHAR (100) NOT NULL,
    [gitcom_btnet_date] DATETIME       NOT NULL,
    [gitcom_msg]        NTEXT          NOT NULL,
    CONSTRAINT [pk_git_commits] PRIMARY KEY NONCLUSTERED ([gitcom_id] ASC)
);


GO
CREATE CLUSTERED INDEX [git_bug_index]
    ON [dbo].[git_commits]([gitcom_bug] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [git_unique_commit]
    ON [dbo].[git_commits]([gitcom_commit] ASC);

