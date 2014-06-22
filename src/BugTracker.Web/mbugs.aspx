<%@ Page language="C#"%>
<!--
Copyright 2002-2013 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

Security security;
DataSet ds;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

    Util.do_not_cache(Response);

    security = new Security();
    security.check_security(HttpContext.Current, Security.ANY_USER_OK);
    if (btnet.Util.get_setting("EnableMobile", "0") == "0")
    {
        Response.Write("BugTracker.NET EnableMobile is not set to 1 in Web.config");
        Response.End();
    }

    titl.InnerText = btnet.Util.get_setting("AppTitle", "BugTracker.NET") + " - List ";
    my_header.InnerText = titl.InnerText;
    create.InnerText = "Create " + btnet.Util.capitalize_first_letter(btnet.Util.get_setting("SingularBugLabel", "bug"));
    only_mine_label.InnerText = "Show only " + btnet.Util.get_setting("PluralBugLabel", "bugs") + " reported by or assigned to me";
       
    
    string bug_sql = @"
select top 200
bg_id [id],
bg_short_desc [desc], 
pj_name [project],
rpt.us_username [reported_user],
asg.us_username [assigned_user],
st_name [status],
bg_last_updated_date [last_updated]
from bugs
left outer join users rpt on rpt.us_id = bg_reported_user
left outer join users asg on asg.us_id = bg_assigned_to_user
--left outer join users lu on lu.us_id = bg_last_updated_user
left outer join projects on pj_id = bg_project
--left outer join orgs on og_id = bg_org
--left outer join categories on ct_id = bg_category
--left outer join priorities on pr_id = bg_priority
left outer join statuses on st_id = bg_status
$WHERE$
order by bg_last_updated_date desc";

    if (only_mine.Checked)
    {
        bug_sql = bug_sql.Replace("$WHERE$", 
            "where bg_reported_user = " 
            + Convert.ToString(security.user.usid) 
            + " or bg_assigned_to_user = " 
            + Convert.ToString(security.user.usid));
    }
    else {
        bug_sql = bug_sql.Replace("$WHERE$", "");
    }
    
    bug_sql = Util.alter_sql_per_project_permissions(bug_sql,security);

    ds = btnet.DbUtil.get_dataset(bug_sql);
    
}

</script>

<html> 
<head> 
<title id=titl runat=server>Title</title> 
<meta name="viewport" content="width=device-width, initial-scale=1"> 
<link rel="stylesheet" href="jquery/jquery.mobile-1.2.0.min.css" />
<link rel="stylesheet" href="mbtnet_base.css" />
<script src="jquery/jquery-1.8.2.min.js"></script>
<script src="jquery/jquery.mobile-1.2.0.min.js"></script>
</head> 
<body> 
<div class="page" data-role="page" data-cache="never">

	<div data-role="header">
		<h1 id=my_header runat=server>Header</h1>
	</div><!-- /header -->

<div data-role="content">	

<a id=create class="ui-submit" data-ajax='false' href="mbug.aspx?id=0" data-role="button" data-icon="arrow-r" data-iconpos="right" runat=server>Create Something</a>

<script>
    function submit_me() { 
        document.getElementById("frm").submit()    
    }
</script>
<form style='margin-top: 15px;' id=frm method=get action=mbugs.aspx runat=server>
<input data-mini="true" type=checkbox id=only_mine name=only_mine runat=server onchange="submit_me()"/>
<label id=only_mine_label for="only_mine" runat=server>Show only</label>
</form>

<ul data-role="listview" data-theme="d" data-divider-theme="d" data-filter="true" data-filter-placeholder="Search...">

<% 
    foreach (DataRow dr in ds.Tables[0].Rows)
    {
        string s = @"
<li><a data-ajax='false' href=mbug.aspx?id=$ID$>
<div class=list_desc>$DESC$</div>
<div class=list_details>
    <div class=list_left>
        <span class=list_id>#$ID$</span>
        <br>
        <span class=list_project>$PROJECT$</span>
    </div>
    <div class=list_right>
        Reported by $REPORTED_USER$<br>$ASSIGNED_USER$
        <br>
        <span class=list_status>$STATUS$</span>
    </div>
</div>
</a></li>";
        
        s = s.Replace("$ID$", HttpUtility.HtmlEncode(Convert.ToString(dr["id"])));
        s = s.Replace("$DESC$", HttpUtility.HtmlEncode(Convert.ToString(dr["desc"])));
        s = s.Replace("$PROJECT$", HttpUtility.HtmlEncode(Convert.ToString(dr["project"])));
        s = s.Replace("$STATUS$", HttpUtility.HtmlEncode(Convert.ToString(dr["status"])));
        s = s.Replace("$REPORTED_USER$", HttpUtility.HtmlEncode(Convert.ToString(dr["reported_user"])));
        String assigned_user = Convert.ToString(dr["assigned_user"]);
        if (assigned_user != "")
        {
            s = s.Replace("$ASSIGNED_USER$", "Assigned to " + HttpUtility.HtmlEncode(assigned_user));
        }
        else {
            s = s.Replace("$ASSIGNED_USER$", "Unassigned");
        }

        Response.Write(s);
    }    
%>

</ul>

</div><!-- /end the ul -->

</div><!-- /content -->
</div><!-- /page -->

</body>
</html>

