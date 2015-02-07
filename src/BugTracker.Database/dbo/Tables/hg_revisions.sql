CREATE TABLE [dbo].[hg_revisions] (
    [hgrev_id]         INT            IDENTITY (1, 1) NOT NULL,
    [hgrev_revision]   INT            NULL,
    [hgrev_bug]        INT            NOT NULL,
    [hgrev_repository] NVARCHAR (400) NOT NULL,
    [hgrev_author]     NVARCHAR (100) NOT NULL,
    [hgrev_hg_date]    NVARCHAR (100) NOT NULL,
    [hgrev_btnet_date] DATETIME       NOT NULL,
    [hgrev_msg]        NTEXT          NOT NULL,
    CONSTRAINT [pk_hg_revisions] PRIMARY KEY NONCLUSTERED ([hgrev_id] ASC)
);


GO
CREATE CLUSTERED INDEX [hg_bug_index]
    ON [dbo].[hg_revisions]([hgrev_bug] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [hg_unique_revision]
    ON [dbo].[hg_revisions]([hgrev_revision] ASC);

