CREATE TABLE [dbo].[bug_tasks] (
    [tsk_id]                 INT            IDENTITY (1, 1) NOT NULL,
    [tsk_bug]                INT            NOT NULL,
    [tsk_created_user]       INT            NOT NULL,
    [tsk_created_date]       DATETIME       NOT NULL,
    [tsk_last_updated_user]  INT            NOT NULL,
    [tsk_last_updated_date]  DATETIME       NOT NULL,
    [tsk_assigned_to_user]   INT            NULL,
    [tsk_planned_start_date] DATETIME       NULL,
    [tsk_actual_start_date]  DATETIME       NULL,
    [tsk_planned_end_date]   DATETIME       NULL,
    [tsk_actual_end_date]    DATETIME       NULL,
    [tsk_planned_duration]   DECIMAL (6, 2) NULL,
    [tsk_actual_duration]    DECIMAL (6, 2) NULL,
    [tsk_duration_units]     NVARCHAR (20)  NULL,
    [tsk_percent_complete]   INT            NULL,
    [tsk_status]             INT            NULL,
    [tsk_sort_sequence]      INT            NULL,
    [tsk_description]        NVARCHAR (400) NULL,
    CONSTRAINT [pk_bug_tasks] PRIMARY KEY NONCLUSTERED ([tsk_id] ASC)
);


GO
CREATE CLUSTERED INDEX [tsk_index_1]
    ON [dbo].[bug_tasks]([tsk_bug] ASC);

