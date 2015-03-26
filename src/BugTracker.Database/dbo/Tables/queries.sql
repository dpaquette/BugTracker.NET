CREATE TABLE [dbo].[queries] (
    [qu_id]      INT            IDENTITY (1, 1) NOT NULL,
    [qu_desc]    NVARCHAR (200) NOT NULL,
    [qu_sql]     NVARCHAR(MAX) NOT NULL,
    [qu_default] INT            NULL,
    [qu_user]    INT            NULL,
    [qu_org]     INT            NULL,
	[qu_columns] NVARCHAR(MAX) NOT NULL DEFAULT 'ColumnsNeeded'
    CONSTRAINT [pk_queries] PRIMARY KEY CLUSTERED ([qu_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [unique_qu_desc]
    ON [dbo].[queries]([qu_desc] ASC, [qu_user] ASC, [qu_org] ASC);

