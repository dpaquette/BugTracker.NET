CREATE TABLE [dbo].[users] (
    [us_id]                           INT             IDENTITY (1, 1) NOT NULL,
    [us_username]                     NVARCHAR (40)   NOT NULL,
    [us_salt]                         NVARCHAR (200)  NULL,
    [us_password]                     NVARCHAR (200)  NULL,
    [us_firstname]                    NVARCHAR (60)   NULL,
    [us_lastname]                     NVARCHAR (60)   NULL,
    [us_email]                        NVARCHAR (120)  NULL,
    [us_admin]                        INT             DEFAULT ((0)) NOT NULL,
    [us_default_query]                INT             DEFAULT ((0)) NOT NULL,
    [us_enable_notifications]         INT             DEFAULT ((1)) NOT NULL,
    [us_auto_subscribe]               INT             DEFAULT ((0)) NOT NULL,
    [us_auto_subscribe_own_bugs]      INT             DEFAULT ((0)) NULL,
    [us_auto_subscribe_reported_bugs] INT             DEFAULT ((0)) NULL,
    [us_send_notifications_to_self]   INT             DEFAULT ((0)) NULL,
    [us_active]                       INT             DEFAULT ((1)) NOT NULL,
    [us_bugs_per_page]                INT             NULL,
    [us_forced_project]               INT             NULL,
    [us_reported_notifications]       INT             DEFAULT ((4)) NOT NULL,
    [us_assigned_notifications]       INT             DEFAULT ((4)) NOT NULL,
    [us_subscribed_notifications]     INT             DEFAULT ((4)) NOT NULL,
    [us_signature]                    NVARCHAR (1000) NULL,
    [us_use_fckeditor]                INT             DEFAULT ((0)) NOT NULL,
    [us_enable_bug_list_popups]       INT             DEFAULT ((1)) NOT NULL,
    [us_created_user]                 INT             DEFAULT ((1)) NOT NULL,
    [us_org]                          INT             DEFAULT ((1)) NOT NULL,
    [us_most_recent_login_datetime]   DATETIME        NULL,
    [password_reset_key]              NVARCHAR (200)  NULL,
    CONSTRAINT [pk_users] PRIMARY KEY CLUSTERED ([us_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [unique_us_username]
    ON [dbo].[users]([us_username] ASC);

