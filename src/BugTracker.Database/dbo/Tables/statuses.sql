CREATE TABLE [dbo].[statuses] (
    [st_id]       INT           IDENTITY (1, 1) NOT NULL,
    [st_name]     NVARCHAR (60) NOT NULL,
    [st_sort_seq] INT           DEFAULT ((0)) NOT NULL,
    [st_style]    NVARCHAR (30) NULL,
    [st_default]  INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [pk_statuses] PRIMARY KEY CLUSTERED ([st_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [unique_st_name]
    ON [dbo].[statuses]([st_name] ASC);

