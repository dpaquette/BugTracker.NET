CREATE TABLE [dbo].[sessions] (
    [se_id]   CHAR (37) NOT NULL,
    [se_date] DATETIME  DEFAULT (getdate()) NOT NULL,
    [se_user] INT       NOT NULL,
    CONSTRAINT [pk_sessions] PRIMARY KEY CLUSTERED ([se_id] ASC)
);

