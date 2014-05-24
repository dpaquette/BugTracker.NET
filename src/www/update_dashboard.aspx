<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


Security security;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

	if (security.user.is_admin || security.user.can_use_reports)
	{
		//
	}
	else
	{
		Response.Write ("You are not allowed to use this page.");
		Response.End();
	}

	if (Request.QueryString["ses"] != (string) Session["session_cookie"])
	{
		Response.Write ("session in URL doesn't match session cookie");
		Response.End();
	}

	string action = Request["actn"];

	string sql = "";

	if (action == "add")
	{
		int rp_id = Convert.ToInt32(Util.sanitize_integer(Request["rp_id"]));
		int rp_col = Convert.ToInt32(Util.sanitize_integer(Request["rp_col"]));

		sql = @"
declare @last_row int
set @last_row = -1

select @last_row = max(ds_row) from dashboard_items
where ds_user = $user
and ds_col = $col

if @last_row = -1 or @last_row is null
	set @last_row = 1
else
	set @last_row = @last_row + 1

insert into dashboard_items
(ds_user, ds_report, ds_chart_type, ds_col, ds_row)
values ($user, $report, '$chart_type', $col, @last_row)";

		sql = sql.Replace("$user", Convert.ToString(security.user.usid));
		sql = sql.Replace("$report", Convert.ToString(rp_id));
		sql = sql.Replace("$chart_type", ((string)Request["rp_chart_type"]).Replace("'","''"));
		sql = sql.Replace("$col", Convert.ToString(rp_col));

	}
	else if (action == "delete")
	{
		int ds_id = Convert.ToInt32(Util.sanitize_integer(Request["ds_id"]));
		sql = "delete from dashboard_items where ds_id = $ds_id and ds_user = $user";
		sql = sql.Replace("$ds_id", Convert.ToString(ds_id));
		sql = sql.Replace("$user", Convert.ToString(security.user.usid));
	}
	else if (action == "moveup" || action == "movedown")
	{
		int ds_id = Convert.ToInt32(Util.sanitize_integer(Request["ds_id"]));

		sql = @"
/* swap positions */
declare @other_row int
declare @this_row int
declare @col int

select @this_row = ds_row, @col = ds_col
from dashboard_items
where ds_id = $ds_id and ds_user = $user

set @other_row = @this_row + $delta

update dashboard_items
set ds_row = @this_row
where ds_user = $user
and ds_col = @col
and ds_row = @other_row

update dashboard_items
set ds_row = @other_row
where ds_user = $user
and ds_id = $ds_id
";

		if (action == "moveup")
		{
			sql = sql.Replace("$delta", "-1");
		}
		else
		{
			sql = sql.Replace("$delta", "1");
		}
		sql = sql.Replace("$ds_id", Convert.ToString(ds_id));
		sql = sql.Replace("$user", Convert.ToString(security.user.usid));
	}

	if (action != "")
	{
		btnet.DbUtil.execute_nonquery(sql);
		Response.Redirect ("edit_dashboard.aspx");
	}
	else
	{
		Response.Write("?");
		Response.End();
	}

}

</script>
