CREATE TABLE [dbo].[svn_revisions] (
    [svnrev_id]         INT            IDENTITY (1, 1) NOT NULL,
    [svnrev_revision]   INT            NOT NULL,
    [svnrev_bug]        INT            NOT NULL,
    [svnrev_repository] NVARCHAR (400) NOT NULL,
    [svnrev_author]     NVARCHAR (100) NOT NULL,
    [svnrev_svn_date]   NVARCHAR (100) NOT NULL,
    [svnrev_btnet_date] DATETIME       NOT NULL,
    [svnrev_msg]        NTEXT          NOT NULL,
    CONSTRAINT [pk_svn_revisions] PRIMARY KEY NONCLUSTERED ([svnrev_id] ASC)
);


GO
CREATE CLUSTERED INDEX [svn_bug_index]
    ON [dbo].[svn_revisions]([svnrev_bug] ASC);

