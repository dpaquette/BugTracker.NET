<%@ Page language="C#" CodeBehind="ajax.aspx.cs" Inherits="btnet.ajax" AutoEventWireup="True" %>
<!-- #include file = "inc.aspx" -->

<script runat="server">


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	string bugid = Util.sanitize_integer(Request["bugid"]);

	// check permission
	int permission_level = Bug.get_bug_permission_level(Convert.ToInt32(bugid), User.Identity);
	if (permission_level !=PermissionLevel.None)
	{

		Response.Write(@"

<style>
.cmt_text
{
font-family: courier new;
font-size: 8pt;
}
.pst
{
font-size: 7pt;
}
</style>");

        int int_bugid = Convert.ToInt32(bugid);
        DataSet ds_posts = PrintBug.get_bug_posts(int_bugid, User.Identity.GetIsExternalUser(), false);
		int post_cnt = PrintBug.write_posts(
            ds_posts,
			Response,
			int_bugid,
			permission_level,
			false, // write links
			false, // images inline
			false, // history inline
            true, User.Identity);		
	
		// We can't unwrite what we wrote, but let's tell javascript to ignore it.
		if (post_cnt == 0)
		{
			Response.Write ("<!--zeroposts-->");
		}
	
	}
	else
	{
		Response.Write ("");
	}
}

</script>
