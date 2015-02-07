CREATE TABLE [dbo].[bugs] (
    [bg_id]                             INT            IDENTITY (1, 1) NOT NULL,
    [bg_short_desc]                     NVARCHAR (200) NOT NULL,
    [bg_reported_user]                  INT            NOT NULL,
    [bg_reported_date]                  DATETIME       NOT NULL,
    [bg_status]                         INT            NOT NULL,
    [bg_priority]                       INT            NOT NULL,
    [bg_org]                            INT            NOT NULL,
    [bg_category]                       INT            NOT NULL,
    [bg_project]                        INT            NOT NULL,
    [bg_assigned_to_user]               INT            NULL,
    [bg_last_updated_user]              INT            NULL,
    [bg_last_updated_date]              DATETIME       NULL,
    [bg_user_defined_attribute]         INT            NULL,
    [bg_project_custom_dropdown_value1] NVARCHAR (120) NULL,
    [bg_project_custom_dropdown_value2] NVARCHAR (120) NULL,
    [bg_project_custom_dropdown_value3] NVARCHAR (120) NULL,
    [bg_tags]                           NVARCHAR (200) NULL,
    CONSTRAINT [pk_bugs] PRIMARY KEY CLUSTERED ([bg_id] ASC)
);

