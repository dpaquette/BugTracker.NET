<%@ Page language="C#" CodeBehind="write_posts.aspx.cs" Inherits="btnet.write_posts" AutoEventWireup="True" %>
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">



void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
		
	int bugid = Convert.ToInt32(Request["id"]);
	bool images_inline = (Request["images_inline"] == "1");
	bool history_inline = (Request["history_inline"] == "1");

	int permission_level = Bug.get_bug_permission_level(bugid, User.Identity);
	if (permission_level ==PermissionLevel.None)
	{
		Response.Write("You are not allowed to view this item");
		Response.End();
	}
    
    DataSet ds_posts = PrintBug.get_bug_posts(bugid, User.Identity.GetIsExternalUser(), history_inline);
    
	PrintBug.write_posts(
        ds_posts,
		Response,
		bugid,
		permission_level,
		true, // write links
		images_inline,
		history_inline,
        true, User.Identity);

}

</script>

