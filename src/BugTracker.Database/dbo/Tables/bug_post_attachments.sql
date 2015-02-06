CREATE TABLE [dbo].[bug_post_attachments] (
    [bpa_id]      INT   IDENTITY (1, 1) NOT NULL,
    [bpa_post]    INT   NOT NULL,
    [bpa_content] IMAGE NOT NULL,
    CONSTRAINT [pk_bug_post_attachements] PRIMARY KEY CLUSTERED ([bpa_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [bpa_index]
    ON [dbo].[bug_post_attachments]([bpa_post] ASC);

