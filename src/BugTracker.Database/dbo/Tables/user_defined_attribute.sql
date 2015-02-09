CREATE TABLE [dbo].[user_defined_attribute] (
    [udf_id]       INT           IDENTITY (1, 1) NOT NULL,
    [udf_name]     NVARCHAR (60) NOT NULL,
    [udf_sort_seq] INT           DEFAULT ((0)) NOT NULL,
    [udf_default]  INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [pk_user_defined_attribute] PRIMARY KEY CLUSTERED ([udf_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [unique_udf_name]
    ON [dbo].[user_defined_attribute]([udf_name] ASC);

