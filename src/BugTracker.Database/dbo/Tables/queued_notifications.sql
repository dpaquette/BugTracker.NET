CREATE TABLE [dbo].[queued_notifications] (
    [qn_id]             INT             IDENTITY (1, 1) NOT NULL,
    [qn_date_created]   DATETIME        NOT NULL,
    [qn_bug]            INT             NOT NULL,
    [qn_user]           INT             NOT NULL,
    [qn_status]         NVARCHAR (30)   NOT NULL,
    [qn_retries]        INT             NOT NULL,
    [qn_last_exception] NVARCHAR (1000) NOT NULL,
    [qn_to]             NVARCHAR (200)  NOT NULL,
    [qn_from]           NVARCHAR (200)  NOT NULL,
    [qn_subject]        NVARCHAR (400)  NOT NULL,
    [qn_body]           NTEXT           NOT NULL,
    CONSTRAINT [pk_queued_notificatons] PRIMARY KEY CLUSTERED ([qn_id] ASC)
);

