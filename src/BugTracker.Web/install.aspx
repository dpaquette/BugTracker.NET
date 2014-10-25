<%@ Page language="C#" CodeBehind="install.aspx.cs" Inherits="btnet.install" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);

	string dbname = Request["dbname"];
	
	if (!string.IsNullOrEmpty(dbname))
	{
		dbname = dbname.Replace("'","''");;
		try
		{
			// don't allow lots of dbs to be created by somebody malicious
			if (Application["dbs"] == null)
			{
				Application["dbs"] = 0;
			}

			int dbs = (int) Application["dbs"];
			
			if (dbs > 10)
			{
				Response.End();
			}
			
			Application["dbs"] = ++dbs;
			
			btnet.DbUtil.GetConnection();
			var sql = new SQLString(@"use master
				create database @db");

			sql = sql.AddParameterWithValue("db", dbname);
			btnet.DbUtil.execute_nonquery(sql);

			Response.Write ("<font color=red><b>Database Created.</b></font>");
		}
		catch (Exception ex)
		{
			Response.Write ("<font color=red><b>" + ex.Message + "</b></font>");
		}
	}


}

</script>


<html>
<head>
<title id="titl" runat="server">btnet install</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<p>
<b>How to get BugTracker.NET up and running</b>
<p>
1) Create a SQL Server database.    You can use the form below.
<p>
2) Update the database connection string in Web.config to point to your database.
<p>
3) Copy/paste the text in the file <a target=_blank href=setup.sql>"setup.sql"</a> into <a target=_blank href=query.aspx>this form</a> and run it.
<p>
4) Logon at <a href=default.aspx>default.aspx</a> using user "admin" and password "admin"
<p>
You probably should spend time looking at the README.HTML and Web.config files.   If you have any questions, post them to the <a href=http://sourceforge.net/projects/btnet/forums/forum/226938>Help Forum</a>.

<hr>
<form action="install.aspx" method="GET">
Database Name:&nbsp;<input name=dbname>
<br>
<input type=submit value="Create Database">
</form>

</body>
</html>
