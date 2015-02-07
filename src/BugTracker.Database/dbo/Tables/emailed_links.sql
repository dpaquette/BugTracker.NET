CREATE TABLE [dbo].[emailed_links] (
    [el_id]        CHAR (37)      NOT NULL,
    [el_date]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [el_email]     NVARCHAR (120) NOT NULL,
    [el_action]    NVARCHAR (20)  NOT NULL,
    [el_username]  NVARCHAR (40)  NULL,
    [el_user_id]   INT            NULL,
    [el_salt]      INT            NULL,
    [el_password]  NVARCHAR (64)  NULL,
    [el_firstname] NVARCHAR (60)  NULL,
    [el_lastname]  NVARCHAR (60)  NULL,
    CONSTRAINT [pk_emailed_links] PRIMARY KEY CLUSTERED ([el_id] ASC)
);

