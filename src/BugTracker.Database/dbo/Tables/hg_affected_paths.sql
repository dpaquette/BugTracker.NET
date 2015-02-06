CREATE TABLE [dbo].[hg_affected_paths] (
    [hgap_id]       INT            IDENTITY (1, 1) NOT NULL,
    [hgap_hgrev_id] INT            NOT NULL,
    [hgap_action]   NVARCHAR (8)   NOT NULL,
    [hgap_path]     NVARCHAR (400) NOT NULL,
    CONSTRAINT [pk_hg_affected_paths] PRIMARY KEY NONCLUSTERED ([hgap_id] ASC)
);


GO
CREATE CLUSTERED INDEX [hgap_hgrev_index]
    ON [dbo].[hg_affected_paths]([hgap_hgrev_id] ASC);

