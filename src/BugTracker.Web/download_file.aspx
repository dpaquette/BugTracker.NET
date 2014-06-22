<%@ Page language="C#"%>
<%@ Import Namespace="System.Data.SqlClient" %>
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">

//Copyright 2002-2011 Corey Trager
//Distributed under the terms of the GNU General Public License

Security security;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);


    string which = Request["which"];        
    string filename = Request["filename"];

    if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(which))
    {
        Response.End();
    }

    string path = "";
    
    if (which == "backup")
    {
        path = HttpContext.Current.Server.MapPath(null) + "\\App_Data\\" + filename;
    }
    else if (which == "log")
    {
        path = HttpContext.Current.Server.MapPath(null) + "\\App_Data\\logs\\" + filename;
    }
    else
    {
        Response.End();
    }
        
    Response.ContentType = btnet.Util.filename_to_content_type(filename);
	Response.AddHeader ("content-disposition","attachment; filename=\"" + filename + "\"");
    
    
    if (Util.get_setting("UseTransmitFileInsteadOfWriteFile", "0") == "1")
    {
        Response.TransmitFile(path);
    }
    else
    {
        Response.WriteFile(path);
    }

} 

</script>

