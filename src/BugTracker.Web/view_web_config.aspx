<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">


Security security;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);

	// create path
	string path = Request.MapPath(Request.Path);
	path = path.Replace("view_web_config.aspx","Web.config");

	Response.ContentType = "application/xml";
	Response.WriteFile(path);

}


</script>

