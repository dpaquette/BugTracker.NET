<%@ Page Language="C#" %>

<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

//Copyright 2002-2011 Corey Trager
//Distributed under the terms of the GNU General Public License


String sql;

Security security;
DataSet ds;
DataView dv;


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	if (Request["format"] != "excel")
	{
		Util.do_not_cache(Response);
	}


	security = new Security();

	security.check_security(HttpContext.Current, Security.ANY_USER_OK);


	// fetch the sql
	string qu_id_string = Util.sanitize_integer(Request["qu_id"]);

	ds = null;
	dv = null;

	if (qu_id_string != null)
	{

		// use sql specified in query string
		int qu_id = Convert.ToInt32(qu_id_string);
		sql = @"select qu_sql from queries where qu_id = $1";
		sql = sql.Replace("$1", qu_id_string);
		string bug_sql = (string)btnet.DbUtil.execute_scalar(sql);

		// replace magic variables
		bug_sql = bug_sql.Replace("$ME", Convert.ToString(security.user.usid));

		bug_sql = Util.alter_sql_per_project_permissions(bug_sql, security);

		ds = btnet.DbUtil.get_dataset(bug_sql);
		dv = new DataView(ds.Tables[0]);
	}
	else
	{
		dv = (DataView)Session["bugs"];
	}


	if (dv == null)
	{
		Response.Write("Please recreate the list before trying to print...");
		Response.End();
	}

	string format = Request["format"];
	if (format != null && format == "excel")
	{
		btnet.Util.print_as_excel(Response, dv);
	}
	else
	{
		print_as_html();
	}

}

///////////////////////////////////////////////////////////////////////
void print_as_html()
{

	Response.Write("<html><head><link rel='StyleSheet' href='btnet.css' type='text/css'></head><body>");
    
	Response.Write("<table class=bugt border=1>");
	int col;

	for (col = 1; col < dv.Table.Columns.Count; col++)
	{

		Response.Write("<td class=bugh>\n");
		if (dv.Table.Columns[col].ColumnName == "$FLAG")
		{
			Response.Write("flag");
		}
		else if (dv.Table.Columns[col].ColumnName == "$SEEN")
		{
			Response.Write("new");
		}
		else
		{
			Response.Write(dv.Table.Columns[col].ColumnName);
		}
		Response.Write("</td>");
	}

	foreach (DataRowView drv in dv)
	{
		Response.Write("<tr>");
		for (col = 1; col < dv.Table.Columns.Count; col++)
		{
			if (dv.Table.Columns[col].ColumnName == "$FLAG")
			{
				int flag = (int)drv[col];
				string cls = "wht";
				if (flag == 1) cls = "red";
				else if (flag == 2) cls = "grn";

				Response.Write("<td class=datad><span class=" + cls + ">&nbsp;</span>");

			}
			else if (dv.Table.Columns[col].ColumnName == "$SEEN")
			{
				int seen = (int)drv[col];
				string cls = "old";
				if (seen == 0)
				{
					cls = "new";
				}
				else
				{
					cls = "old";
				}
				Response.Write("<td class=datad><span class=" + cls + ">&nbsp;</span>");

			}
			else
			{
				Type datatype = dv.Table.Columns[col].DataType;

				if (Util.is_numeric_datatype(datatype))
				{
					Response.Write("<td class=bugd align=right>");
				}
				else
				{
					Response.Write("<td class=bugd>");
				}

				// write the data
				if (drv[col].ToString() == "")
				{
					Response.Write("&nbsp;");
				}
				else
				{
					Response.Write(Server.HtmlEncode(drv[col].ToString()).Replace("\n", "<br>"));
				}
			}
			Response.Write("</td>");
		}
		Response.Write("</tr>");
	}

	Response.Write("</table></body></html>");
}


</script>

