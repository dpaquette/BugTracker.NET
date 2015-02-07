CREATE TABLE [dbo].[dashboard_items] (
    [ds_id]         INT         IDENTITY (1, 1) NOT NULL,
    [ds_user]       INT         NOT NULL,
    [ds_report]     INT         NOT NULL,
    [ds_chart_type] VARCHAR (8) NOT NULL,
    [ds_col]        INT         NOT NULL,
    [ds_row]        INT         NOT NULL,
    CONSTRAINT [pk_dashboard_items] PRIMARY KEY NONCLUSTERED ([ds_id] ASC)
);


GO
CREATE CLUSTERED INDEX [ds_user_index]
    ON [dbo].[dashboard_items]([ds_user] ASC);

