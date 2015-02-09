CREATE TABLE [dbo].[orgs] (
    [og_id]                                 INT           IDENTITY (1, 1) NOT NULL,
    [og_name]                               NVARCHAR (80) NOT NULL,
    [og_domain]                             NVARCHAR (80) NULL,
    [og_non_admins_can_use]                 INT           DEFAULT ((0)) NOT NULL,
    [og_external_user]                      INT           DEFAULT ((0)) NOT NULL,
    [og_can_be_assigned_to]                 INT           DEFAULT ((1)) NOT NULL,
    [og_can_only_see_own_reported]          INT           DEFAULT ((0)) NOT NULL,
    [og_can_edit_sql]                       INT           DEFAULT ((0)) NOT NULL,
    [og_can_delete_bug]                     INT           DEFAULT ((0)) NOT NULL,
    [og_can_edit_and_delete_posts]          INT           DEFAULT ((0)) NOT NULL,
    [og_can_merge_bugs]                     INT           DEFAULT ((0)) NOT NULL,
    [og_can_mass_edit_bugs]                 INT           DEFAULT ((0)) NOT NULL,
    [og_can_use_reports]                    INT           DEFAULT ((0)) NOT NULL,
    [og_can_edit_reports]                   INT           DEFAULT ((0)) NOT NULL,
    [og_can_view_tasks]                     INT           DEFAULT ((0)) NOT NULL,
    [og_can_edit_tasks]                     INT           DEFAULT ((0)) NOT NULL,
    [og_can_search]                         INT           DEFAULT ((1)) NOT NULL,
    [og_other_orgs_permission_level]        INT           DEFAULT ((2)) NOT NULL,
    [og_can_assign_to_internal_users]       INT           DEFAULT ((0)) NOT NULL,
    [og_category_field_permission_level]    INT           DEFAULT ((2)) NOT NULL,
    [og_priority_field_permission_level]    INT           DEFAULT ((2)) NOT NULL,
    [og_assigned_to_field_permission_level] INT           DEFAULT ((2)) NOT NULL,
    [og_status_field_permission_level]      INT           DEFAULT ((2)) NOT NULL,
    [og_project_field_permission_level]     INT           DEFAULT ((2)) NOT NULL,
    [og_org_field_permission_level]         INT           DEFAULT ((2)) NOT NULL,
    [og_udf_field_permission_level]         INT           DEFAULT ((2)) NOT NULL,
    [og_tags_field_permission_level]        INT           DEFAULT ((2)) NOT NULL,
    [og_active]                             INT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [pk_orgs] PRIMARY KEY CLUSTERED ([og_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [unique_og_name]
    ON [dbo].[orgs]([og_name] ASC);

