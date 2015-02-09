CREATE TABLE [dbo].[projects] (
    [pj_id]                          INT            IDENTITY (1, 1) NOT NULL,
    [pj_name]                        NVARCHAR (80)  NOT NULL,
    [pj_active]                      INT            DEFAULT ((1)) NOT NULL,
    [pj_default_user]                INT            NULL,
    [pj_auto_assign_default_user]    INT            NULL,
    [pj_auto_subscribe_default_user] INT            NULL,
    [pj_enable_pop3]                 INT            NULL,
    [pj_pop3_username]               VARCHAR (50)   NULL,
    [pj_pop3_password]               NVARCHAR (20)  NULL,
    [pj_pop3_email_from]             NVARCHAR (120) NULL,
    [pj_enable_custom_dropdown1]     INT            DEFAULT ((0)) NOT NULL,
    [pj_enable_custom_dropdown2]     INT            DEFAULT ((0)) NOT NULL,
    [pj_enable_custom_dropdown3]     INT            DEFAULT ((0)) NOT NULL,
    [pj_custom_dropdown_label1]      NVARCHAR (80)  NULL,
    [pj_custom_dropdown_label2]      NVARCHAR (80)  NULL,
    [pj_custom_dropdown_label3]      NVARCHAR (80)  NULL,
    [pj_custom_dropdown_values1]     NVARCHAR (800) NULL,
    [pj_custom_dropdown_values2]     NVARCHAR (800) NULL,
    [pj_custom_dropdown_values3]     NVARCHAR (800) NULL,
    [pj_default]                     INT            DEFAULT ((0)) NOT NULL,
    [pj_description]                 NVARCHAR (200) NULL,
    CONSTRAINT [pk_projects] PRIMARY KEY CLUSTERED ([pj_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [unique_pj_name]
    ON [dbo].[projects]([pj_name] ASC);

