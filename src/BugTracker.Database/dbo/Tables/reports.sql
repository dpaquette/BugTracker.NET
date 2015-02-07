CREATE TABLE [dbo].[reports] (
    [rp_id]         INT            IDENTITY (1, 1) NOT NULL,
    [rp_desc]       NVARCHAR (200) NOT NULL,
    [rp_sql]        NTEXT          NOT NULL,
    [rp_chart_type] VARCHAR (8)    NOT NULL,
    CONSTRAINT [pk_reports] PRIMARY KEY CLUSTERED ([rp_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [unique_rp_desc]
    ON [dbo].[reports]([rp_desc] ASC);

