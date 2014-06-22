<%@ Page language="C#" validateRequest="false" %>
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

    
	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "edit web config";

    string which_file = "";
    string file_name = "";
    
    if (!IsPostBack)
    {
        which_file = Request["which"];
        
        // default to footer
        if (string.IsNullOrEmpty(which_file))
        {
            which_file = "footer";
        }

        file_name = get_file_name(which_file);
        msg.InnerHtml = "&nbsp;";
    }
    else
    {
        which_file = which.Value;

        if (string.IsNullOrEmpty(which_file))
        {
            Response.End();
        }

        file_name = get_file_name(which_file); 

        if (file_name == "")
            Response.End();
        
        // save to disk
        string path = HttpContext.Current.Server.MapPath(null);
        path += "\\custom\\";

        System.IO.StreamWriter sw = System.IO.File.CreateText(path + file_name);
        sw.Write(myedit.Value);
        sw.Close();
        sw.Dispose();

        // save in Application (memory)
        Application[System.IO.Path.GetFileNameWithoutExtension(file_name)] = myedit.Value;
        
        msg.InnerHtml = file_name + " was saved.";
    }

    load_file_into_control(file_name);

    which.Value = which_file;    
}

void load_file_into_control(string file_name)
{
    string path = HttpContext.Current.Server.MapPath(null);
    path += "\\custom\\" + file_name;

    System.IO.StreamReader sr = System.IO.File.OpenText(path);
    myedit.Value = sr.ReadToEnd();
    sr.Close();
    sr.Dispose();
}    
        
string get_file_name(string which_file)
{
    string file_name = "";

    if (which_file == "css")
    {
        file_name = "btnet_custom.css";
    }
    else if (which_file == "footer")
    {
        file_name = "custom_footer.html";
    }
    else if (which_file == "header")
    {
        file_name = "custom_header.html";
    }
    else if (which_file == "logo")
    {
        file_name = "custom_logo.html";
    }
    else if (which_file == "welcome")
    {
        file_name = "custom_welcome.html";
    }

    return file_name;
    
}   
</script>

<html>
<head>
<title id="titl" runat="server">btnet edit web config</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script language="Javascript" type="text/javascript" src="edit_area/edit_area_full.js"></script>

<script>
    editAreaLoader.init({
        id: "myedit"	// id of the textarea to transform
			, start_highlight: true	// if start with highlight
			, toolbar: "search, go_to_line, undo, redo, help"
			, browsers: "all"
			, language: "en"
			, syntax: "sql"
			, allow_toggle: false
			, min_width: 800
			, min_height: 400
    });

    function load_custom_file() {
        var sel = document.getElementById("which")
        window.location = "edit_custom_html.aspx?which=" + sel.options[sel.selectedIndex].value
    }    
</script>		

</head>
<body>
<% security.write_menu(Response, "admin"); %>

<div class=align><table border=0 style="margin-left:20px; margin-top:20px; width:80%;"><tr><td>
<form runat="server">

Select custom html file: 
<select id="which" onchange="load_custom_file()" runat="server">
<option value="css">btnet_custom.css</option>
<option value="footer">customer_footer.html</option>
<option value="header">customer_header.html</option>
<option value="logo">customer_logo.html</option>
<option value="welcome">customer_welcome.html</option>
</select>

<p>

<textarea id="myedit" runat="server" style="width:100%"></textarea>
<p>

<div class="err" id="msg" runat="server">&nbsp;</div>

<div><input type=submit value="Save"  class="btn"></div>

</form>
</table>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>