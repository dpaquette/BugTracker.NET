<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


Security security;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	if (btnet.Util.get_setting("MemoryLogEnabled","1") != "1")
	{
		Response.End();
	}

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	Response.ContentType = "text/plain";
	Response.AddHeader ("content-disposition","inline; filename=\"memory_log.txt\"");

	List<string> list = (List<string>) Application["log"];

	Response.Write (DateTime.Now.ToString("yyy-MM-dd HH:mm:ss:fff"));
	Response.Write ("\n\n");


	if (list != null)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Response.Write (list[i]);
			Response.Write ("\n");
		}
	}
	else
	{
		Response.Write("list is null");
	}



}

</script>