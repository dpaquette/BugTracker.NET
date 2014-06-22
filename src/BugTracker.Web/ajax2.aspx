<%@ Page language="C#"%>
<!-- #include file = "inc.aspx" -->

<script runat="server">


Security security;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	// will this be too slow?

	// we could index on bg_short_desc and then do '$str%' rather than '%$str%'

	try
	{
		string sql = @"select distinct top 10 bg_short_desc from bugs
			where bg_short_desc like '%$str%'
			order by 1";

		// if you don't use permissions, comment out this line for speed?
		sql = Util.alter_sql_per_project_permissions(sql, security);

		string text = Request["q"];
		sql = sql.Replace("$str",text.Replace("'","''"));

		DataSet ds = btnet.DbUtil.get_dataset(sql);


		if (ds.Tables[0].Rows.Count > 0)
		{
			Response.Write ("<select id='suggest_select' class='suggest_select'	size=6 ");
			Response.Write (" onclick='select_suggestion(this)' onkeydown='return suggest_sel_onkeydown(this, event)'>");
			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				Response.Write("<option>");
				Response.Write(dr[0]);
				Response.Write("</option>");
			}
			Response.Write("</select>");
		}
		else
		{
			Response.Write("");
		}
	}
	catch(Exception)
	{
		Response.Write("");
	}
}


</script>

