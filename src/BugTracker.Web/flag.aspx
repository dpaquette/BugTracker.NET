<%@ Page Language="C#" CodeBehind="flag.aspx.cs" Inherits="btnet.flag" AutoEventWireup="True" %>

<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


    ///////////////////////////////////////////////////////////////////////
    void Page_Load(Object sender, EventArgs e)
    {
        Util.do_not_cache(Response);

        if (!User.IsInRole(BtnetRoles.Guest))
        {

            int bugid = Convert.ToInt32(Util.sanitize_integer(Request["bugid"]));

            int permission_level = Bug.get_bug_permission_level(bugid, User.Identity);
            if (permission_level == PermissionLevel.None)
            {
                Response.End();
            }

            int flag = Convert.ToInt32(Util.sanitize_integer(Request["flag"]));

            var sql = new SQLString(@"
if not exists (select bu_bug from bug_user where bu_bug = @bg and bu_user = @us)
	insert into bug_user (bu_bug, bu_user, bu_flag, bu_seen, bu_vote) values(@bg, @us, 1, 0, 0) 
update bug_user set bu_flag = @fl, bu_flag_datetime = getdate() where bu_bug = @bg and bu_user = @us and bu_flag <> @fl");

            sql = sql.AddParameterWithValue("bg", Convert.ToString(bugid));
            sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));
            sql = sql.AddParameterWithValue("fl", Convert.ToString(flag));

            DbUtil.execute_nonquery(sql);

        }


    }



</script>
