CREATE TABLE [dbo].[categories] (
    [ct_id]       INT           IDENTITY (1, 1) NOT NULL,
    [ct_name]     NVARCHAR (80) NOT NULL,
    [ct_sort_seq] INT           DEFAULT ((0)) NOT NULL,
    [ct_default]  INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [pk_categories] PRIMARY KEY CLUSTERED ([ct_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [unique_ct_name]
    ON [dbo].[categories]([ct_name] ASC);

