CREATE TABLE [dbo].[priorities] (
    [pr_id]               INT           IDENTITY (1, 1) NOT NULL,
    [pr_name]             NVARCHAR (60) NOT NULL,
    [pr_sort_seq]         INT           DEFAULT ((0)) NOT NULL,
    [pr_background_color] NVARCHAR (14) NOT NULL,
    [pr_style]            NVARCHAR (30) NULL,
    [pr_default]          INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [pk_priorities] PRIMARY KEY CLUSTERED ([pr_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [unique_pr_name]
    ON [dbo].[priorities]([pr_name] ASC);

