<%@ Page language="C#"%>
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

DataSet ds_tasks;

Security security;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);
	
	if (security.user.is_admin || security.user.can_view_tasks)
	{
		// allowed
	}
	else
	{
		Response.Write("You are not allowed to view tasks");
		Response.End();
	}
	
	ds_tasks = btnet.Util.get_all_tasks(security,0);
	DataView dv = new DataView(ds_tasks.Tables[0]);
	
	btnet.Util.print_as_excel(Response, dv);
	
}

</script>

