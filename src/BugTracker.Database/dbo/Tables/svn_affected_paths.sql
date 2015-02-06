CREATE TABLE [dbo].[svn_affected_paths] (
    [svnap_id]        INT            IDENTITY (1, 1) NOT NULL,
    [svnap_svnrev_id] INT            NOT NULL,
    [svnap_action]    NVARCHAR (8)   NOT NULL,
    [svnap_path]      NVARCHAR (400) NOT NULL,
    CONSTRAINT [pk_svn_affected_paths] PRIMARY KEY NONCLUSTERED ([svnap_id] ASC)
);


GO
CREATE CLUSTERED INDEX [svn_revision_index]
    ON [dbo].[svn_affected_paths]([svnap_svnrev_id] ASC);

