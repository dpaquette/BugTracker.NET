CREATE TABLE [dbo].[bug_posts] (
    [bp_id]                         INT             IDENTITY (1, 1) NOT NULL,
    [bp_bug]                        INT             NOT NULL,
    [bp_type]                       VARCHAR (8)     NOT NULL,
    [bp_user]                       INT             NOT NULL,
    [bp_date]                       DATETIME        NOT NULL,
    [bp_comment]                    NTEXT           NOT NULL,
    [bp_comment_search]             NTEXT           NULL,
    [bp_email_from]                 NVARCHAR (800)  NULL,
    [bp_email_to]                   NVARCHAR (800)  NULL,
    [bp_file]                       NVARCHAR (1000) NULL,
    [bp_size]                       INT             NULL,
    [bp_content_type]               NVARCHAR (200)  NULL,
    [bp_parent]                     INT             NULL,
    [bp_original_comment_id]        INT             NULL,
    [bp_hidden_from_external_users] INT             DEFAULT ((0)) NOT NULL,
    [bp_email_cc]                   NVARCHAR (800)  NULL,
    CONSTRAINT [pk_bug_posts] PRIMARY KEY NONCLUSTERED ([bp_id] ASC)
);


GO
CREATE CLUSTERED INDEX [bp_index_1]
    ON [dbo].[bug_posts]([bp_bug] ASC);

