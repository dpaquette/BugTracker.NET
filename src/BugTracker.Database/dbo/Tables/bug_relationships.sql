CREATE TABLE [dbo].[bug_relationships] (
    [re_id]        INT            IDENTITY (1, 1) NOT NULL,
    [re_bug1]      INT            NOT NULL,
    [re_bug2]      INT            NOT NULL,
    [re_type]      NVARCHAR (500) NULL,
    [re_direction] INT            NOT NULL,
    CONSTRAINT [pk_bug_relationships] PRIMARY KEY CLUSTERED ([re_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [re_index_1]
    ON [dbo].[bug_relationships]([re_bug1] ASC, [re_bug2] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [re_index_2]
    ON [dbo].[bug_relationships]([re_bug2] ASC, [re_bug1] ASC);

