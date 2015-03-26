/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
UPDATE queries SET 
qu_sql = 'select isnull(pr_background_color,''transparent'') [$COLOR], bg_id [id], isnull(bu_flag,0) [$FLAG], 
 bg_short_desc [desc], isnull(pj_name,'''') [project], isnull(og_name,'''') [organization], isnull(ct_name,'''') [category], rpt.us_username [reported by],
 bg_reported_date [reported on], isnull(pr_name,'''') [priority], isnull(asg.us_username,'''') [assigned to],
 isnull(st_name,'''') [status], isnull(lu.us_username,'''') [last updated by], bg_last_updated_date [last updated on]
 from bugs 
 left outer join bug_user on bu_bug = bg_id and bu_user = @ME 
 left outer join users rpt on rpt.us_id = bg_reported_user
 left outer join users asg on asg.us_id = bg_assigned_to_user
 left outer join users lu on lu.us_id = bg_last_updated_user
 left outer join projects on pj_id = bg_project
 left outer join orgs on og_id = bg_org
 left outer join categories on ct_id = bg_category
 left outer join priorities on pr_id = bg_priority
 left outer join statuses on st_id = bg_status
 ',
 qu_columns='$COLOR,id,$FLAG,desc,project,organization,category,reported by,reported on,priority,assigned to,status,last updated by,last updated on'
 WHERE qu_desc = 'all bugs' AND qu_columns = 'ColumnsNeeded'

 UPDATE queries SET
 qu_sql = 'select isnull(pr_background_color,''transparent'') [$COLOR], bg_id [id], isnull(bu_flag,0) [$FLAG], 
 bg_short_desc [desc], isnull(pj_name,'''') [project], isnull(og_name,'''') [organization], isnull(ct_name,'''') [category], rpt.us_username [reported by],
 bg_reported_date [reported on], isnull(pr_name,'''') [priority], isnull(asg.us_username,'''') [assigned to],
 isnull(st_name,'''') [status], isnull(lu.us_username,'''') [last updated by], bg_last_updated_date [last updated on]
 from bugs 
 left outer join bug_user on bu_bug = bg_id and bu_user = @ME 
 left outer join users rpt on rpt.us_id = bg_reported_user
 left outer join users asg on asg.us_id = bg_assigned_to_user
 left outer join users lu on lu.us_id = bg_last_updated_user
 left outer join projects on pj_id = bg_project
 left outer join orgs on og_id = bg_org
 left outer join categories on ct_id = bg_category
 left outer join priorities on pr_id = bg_priority
 left outer join statuses on st_id = bg_status
 where bg_status <> 5 ',
 qu_columns= '$COLOR,id,$FLAG,desc,project,organization,category,reported by,reported on,priority,assigned to,status,last updated by,last updated on'
 WHERE qu_desc = 'open bugs' AND qu_columns = 'ColumnsNeeded'

  UPDATE queries SET
 qu_sql = 'select isnull(pr_background_color,''transparent'') [$COLOR], bg_id [id], isnull(bu_flag,0) [$FLAG], 
 bg_short_desc [desc], isnull(pj_name,'''') [project], isnull(og_name,'''') [organization], isnull(ct_name,'''') [category], rpt.us_username [reported by],
 bg_reported_date [reported on], isnull(pr_name,'''') [priority], isnull(asg.us_username,'''') [assigned to],
 isnull(st_name,'''') [status], isnull(lu.us_username,'''') [last updated by], bg_last_updated_date [last updated on]
 from bugs 
 left outer join bug_user on bu_bug = bg_id and bu_user = @ME 
 left outer join users rpt on rpt.us_id = bg_reported_user
 left outer join users asg on asg.us_id = bg_assigned_to_user
 left outer join users lu on lu.us_id = bg_last_updated_user
 left outer join projects on pj_id = bg_project
 left outer join orgs on og_id = bg_org
 left outer join categories on ct_id = bg_category
 left outer join priorities on pr_id = bg_priority
 left outer join statuses on st_id = bg_status
 where bg_status <> 5 and bg_assigned_to_user = @ME',
 qu_columns= '$COLOR,id,$FLAG,desc,project,organization,category,reported by,reported on,priority,assigned to,status,last updated by,last updated on'
 WHERE qu_desc = 'open bugs assigned to me' AND qu_columns = 'ColumnsNeeded'

 UPDATE queries 
 SET qu_sql = 'select isnull(pr_background_color,''transparent'') [$COLOR], bg_id [id], isnull(bu_flag,0) [$FLAG], 
 bg_short_desc [desc], isnull(pj_name,'''') [project], isnull(og_name,'''') [organization], isnull(ct_name,'''') [category], rpt.us_username [reported by],
 bg_reported_date [reported on], isnull(pr_name,'''') [priority], isnull(asg.us_username,'''') [assigned to],
 isnull(st_name,'''') [status], isnull(lu.us_username,'''') [last updated by], bg_last_updated_date [last updated on]
 from bugs 
 left outer join bug_user on bu_bug = bg_id and bu_user = @ME 
 left outer join users rpt on rpt.us_id = bg_reported_user
 left outer join users asg on asg.us_id = bg_assigned_to_user
 left outer join users lu on lu.us_id = bg_last_updated_user
 left outer join projects on pj_id = bg_project
 left outer join orgs on og_id = bg_org
 left outer join categories on ct_id = bg_category
 left outer join priorities on pr_id = bg_priority
 left outer join statuses on st_id = bg_status
 where bg_status = 3 ',
 qu_columns = '$COLOR,id,$FLAG,desc,project,organization,category,reported by,reported on,priority,assigned to,status,last updated by,last updated on'
 WHERE qu_desc = 'checked in bugs - for QA' AND qu_columns = 'ColumnsNeeded'


 UPDATE queries
 SET qu_sql = 'select case 
 when datediff(d, isnull(bp_date,bg_reported_date), getdate()) > 90 then ''#ff9999''
 when datediff(d, isnull(bp_date,bg_reported_date), getdate()) > 30 then ''#ffcccc'' 
 when datediff(d, isnull(bp_date,bg_reported_date), getdate()) > 7 then ''#ffdddd'' 
 else ''transparent'' end [$COLOR], 
 bg_id [id], bg_short_desc [desc], 
 datediff(d, isnull(bp_date,bg_reported_date), getdate()) [days in status],
 st_name [status], 
 isnull(bp_comment,'''') [last status change], isnull(bp_date,bg_reported_date) [status date]
 from bugs
 inner join statuses on bg_status = st_id
 left outer join bug_posts on bg_id = bp_bug
 and bp_type = ''update''
 and bp_comment like ''changed status from%'' 
 and bp_date in (select max(bp_date) from bug_posts where bp_bug = bg_id) ',
 qu_columns='$COLOR,id,desc,days in status,status,last status change,status date'
 WHERE qu_desc = 'days in status' AND qu_columns = 'ColumnsNeeded' 

 UPDATE queries
 SET qu_sql='select ''transparent'' [$COLOR], bg_id [id], bg_short_desc [desc], 
 substring(bp_comment_search,1,40) [last comment], bp_date [last comment date]
 from bugs
 left outer join bug_posts on bg_id = bp_bug
 and bp_type = ''comment''
 and bp_date in (select max(bp_date) from bug_posts where bp_bug = bg_id) ',
 qu_columns = '$COLOR,id,desc,last comment,last comment date'
 WHERE qu_desc = 'demo last comment as column' AND qu_columns = 'ColumnsNeeded'

 UPDATE queries
 SET qu_sql=' select ''transparent'' [$COLOR], bg_id [id], bg_short_desc [desc], 
 size.bytes, rpt.us_username [reported by] 
 from bugs 
 inner join (select bp_bug, sum(bp_size) bytes 
 from bug_posts 
 where bp_type = ''file''
 group by bp_bug ) size on size.bp_bug = bg_id 
 left outer join users rpt on rpt.us_id = bg_reported_user
 WhErE 1=1',
 qu_columns = '$COLOR,id,desc,bytes,reported by'
 WHERE qu_desc = 'bugs with attachments' AND qu_columns = 'ColumnsNeeded'

  UPDATE queries
 SET qu_sql='select ''transparent'' [$COLOR], bg_id [id], bg_short_desc [desc], 
 isnull(st_name,'''') [status], 
 count(*) [number of related bugs] 
 from bugs 
 inner join bug_relationships on re_bug1 = bg_id 
 inner join statuses on bg_status = st_id 
 /*ENDWHR*/ 
 group by bg_id, bg_short_desc, isnull(st_name,'''') ',
 qu_columns = '$COLOR,id,desc,status,number of related bugs'
 WHERE qu_desc = 'bugs with related bugs' AND qu_columns = 'ColumnsNeeded'

  UPDATE queries
 SET qu_sql='select ''transparent'' [$COLOR], bg_id [id], 
 (isnull(vote_total,0) * 10000) + isnull(bu_vote,0) [$VOTE], 
 bg_short_desc [desc], isnull(st_name,'''') [status] 
 from bugs 
 left outer join bug_user on bu_bug = bg_id and bu_user = @ME 
 left outer join votes_view on vote_bug = bg_id 
 left outer join statuses on st_id = bg_status ',
 qu_columns = '$COLOR,id,$VOTE,desc,status'
 WHERE qu_desc = 'demo votes feature' AND qu_columns = 'ColumnsNeeded'
