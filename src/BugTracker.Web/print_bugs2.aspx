<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


String sql;


Security security;
DataSet ds = null;
DataView dv = null;
bool images_inline;
bool history_inline;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "print " + Util.get_setting("PluralBugLabel","bugs");


	// are we doing the query to get the bugs or are we using the cached dataview?
	string qu_id_string = Request.QueryString["qu_id"];

	if (qu_id_string != null)
	{

		// use sql specified in query string
		int qu_id = Convert.ToInt32(qu_id_string);
		sql = @"select qu_sql from queries where qu_id = $1";
		sql = sql.Replace("$1", qu_id_string);
		string bug_sql = (string)btnet.DbUtil.execute_scalar(sql);

		// replace magic variables
		bug_sql = bug_sql.Replace("$ME", Convert.ToString(security.user.usid));
		bug_sql = Util.alter_sql_per_project_permissions(bug_sql,security);

		// all we really need is the bugid, but let's do the same query as print_bugs.aspx
		ds = btnet.DbUtil.get_dataset (bug_sql);
	}
	else
	{
		dv = (DataView) Session["bugs"];
	}

    HttpCookie cookie = Request.Cookies["images_inline"];
    if (cookie == null || cookie.Value == "0")
    {
        images_inline = false;
    }
    else
    {
        images_inline = true;
    }

    cookie = Request.Cookies["history_inline"];
    if (cookie == null || cookie.Value == "0")
    {
        history_inline = false;
    }
    else
    {
        history_inline = true;
    }

}


</script>

<html>
<head>
<title id="titl" runat="server">btnet print bugs detail</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<style>
a {text-decoration: underline; }
a:visited {text-decoration: underline; }
a:hover {text-decoration: underline; }
</style>
</head>

<%

bool firstrow = true;

if (dv != null)
{
	foreach (DataRowView drv in dv)
	{
		if (!firstrow)
		{
			Response.Write ("<hr STYLE='page-break-before: always'>");
		}
		else
		{
			firstrow = false;
		}

		DataRow dr = btnet.Bug.get_bug_datarow(
			(int)drv[1],
			security);

		PrintBug.print_bug(Response, dr, security,
            false /* include style */,
            images_inline,
            history_inline,
            true /*internal_posts */); ;
	}
}
else
{
	if (ds != null)
	{
		foreach (DataRow dr2 in ds.Tables[0].Rows)
		{
			if (!firstrow)
			{
				Response.Write ("<hr STYLE='page-break-before: always'>");
			}
			else
			{
				firstrow = false;
			}

			DataRow dr = btnet.Bug.get_bug_datarow(
				(int)dr2[1],
				security);

			PrintBug.print_bug(Response, dr, security, 
                false, // include style
                images_inline,
                history_inline,
                true); // internal_posts
		}
	}
	else
	{
		Response.Write ("Please recreate the list before trying to print...");
		Response.End();
	}
}

%>

</html>


