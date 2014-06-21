<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

String sql;

Security security;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "edit project per-user permissions";


	if (!IsPostBack)
	{

        string project_id_string = Util.sanitize_integer(Request["id"]);
        
		if (Request["projects"] != null)
		{
			back_href.InnerText = "back to projects";
			back_href.HRef = "projects.aspx";
		}
		else
		{
			back_href.InnerText = "back to project";
            back_href.HRef = "edit_project.aspx?id=" + project_id_string;
		}


		sql = @"Select us_username, us_id, isnull(pu_permission_level,$dpl) [pu_permission_level]
			from users
			left outer join project_user_xref on pu_user = us_id
			and pu_project = $pj
			order by us_username;
			select pj_name from projects where pj_id = $pj;";

        sql = sql.Replace("$pj", project_id_string);
		sql = sql.Replace("$dpl", Util.get_setting("DefaultPermissionLevel","2"));

        DataSet ds = btnet.DbUtil.get_dataset(sql);

        MyDataGrid.DataSource=ds.Tables[0].DefaultView;
        MyDataGrid.DataBind();

		titl.InnerText = "Permissions for " + (string) ds.Tables[1].Rows[0][0];

	}
	else
	{
		on_update();
	}

}



///////////////////////////////////////////////////////////////////////
void on_update()
{

	// now update all the recs
	string sql_batch = "";
	RadioButton rb;
	string permission_level;

	foreach (DataGridItem dgi in MyDataGrid.Items)
	{
		sql = @" if exists (select * from project_user_xref where pu_user = $us and pu_project = $pj)
		            update project_user_xref set pu_permission_level = $pu
		            where pu_user = $us and pu_project = $pj
		         else
		            insert into project_user_xref (pu_user, pu_project, pu_permission_level)
		            values ($us, $pj, $pu); ";

		sql = sql.Replace("$pj", Util.sanitize_integer(Request["id"]));
		sql = sql.Replace("$us", Convert.ToString(dgi.Cells[1].Text));

		rb = (RadioButton) dgi.FindControl("none");
		if (rb.Checked)
		{
			permission_level = "0";
		}
		else
		{
			rb = (RadioButton) dgi.FindControl("readonly");
			if (rb.Checked)
			{
				permission_level = "1";
			}
			else
			{
				rb = (RadioButton) dgi.FindControl("reporter");
				if (rb.Checked)
				{
					permission_level = "3";
				}
				else
				{
					permission_level = "2";
				}
			}
		}



		sql = sql.Replace("$pu", permission_level);


		// add to the batch
		sql_batch += sql;

	}

	btnet.DbUtil.execute_nonquery(sql_batch);
	msg.InnerText = "Permissions have been updated.";

}



</script>

<html>
<head>
<title id="titl" runat="server">btnet edit user</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, "admin"); %>

<div class=align><table border=0><tr><td>
<a id="back_href" runat="server" href="">back</a>

<p>

<form class=frm runat="server">
	<table border=0 cellpadding=3>

	<tr>
	<td colspan=2 class=lbl><span id="this_page_title" runat="server" class=smallnote></span></td>
	</tr>

	<tr>
	<td colspan=2>
	<ASP:DataGrid id="MyDataGrid" runat="server" BorderColor="black" CssClass="datat" CellPadding="3" AutoGenerateColumns="false">
		<HeaderStyle cssclass="datah"></HeaderStyle>
		<ItemStyle cssclass="datad"></ItemStyle>
		<Columns>

		<asp:BoundColumn HeaderText="User" DataField="us_username"/>

		<asp:BoundColumn HeaderText="User" DataField="us_id" Visible="False"/>

		<asp:TemplateColumn HeaderText="Permissions">
			<ItemTemplate>
				<asp:RadioButton GroupName="permissions" text="none" value=0 ID="none" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 0 ) %>/>

				<asp:RadioButton GroupName="permissions" text="view only" value=1 ID="readonly" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 1 ) %>/>

				<asp:RadioButton GroupName="permissions" text="report (add and comment only)" value=3 ID="reporter" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 3 ) %>/>

				<asp:RadioButton GroupName="permissions" text="all (add and edit)" value=2 ID="edit" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 2 ) %>/>

			</ItemTemplate>
		</asp:TemplateColumn>

		</Columns>
	</ASP:DataGrid>



	<tr>
	<td colspan=2 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>


	<tr>
	<td colspan=2 align=center>
	<input runat="server" class=btn type=submit id="sub" value="Update">
	<td>&nbsp</td>
	</td>
	</tr>

	</table>
</form>
</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


