CREATE TABLE [dbo].[custom_col_metadata] (
    [ccm_colorder]      INT             NOT NULL,
    [ccm_dropdown_vals] NVARCHAR (1000) DEFAULT ('') NOT NULL,
    [ccm_sort_seq]      INT             DEFAULT ((0)) NULL,
    [ccm_dropdown_type] VARCHAR (20)    NULL,
    CONSTRAINT [pk_custom_col_metadata] PRIMARY KEY CLUSTERED ([ccm_colorder] ASC)
);

