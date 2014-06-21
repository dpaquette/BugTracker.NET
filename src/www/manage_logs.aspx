<%@ Page language="C#" validateRequest="false" enableEventValidation="false" %>

<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->


<script language="C#" runat="server">

Security security;

string app_data_folder;    
    
///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
    
	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "manage logs";

    app_data_folder = HttpContext.Current.Server.MapPath(null);
    app_data_folder += "\\App_Data\\logs\\"; 
    
    if (!IsPostBack)
    {
        get_files();
    }
   
}

void get_files()
{
    string[] backup_files = System.IO.Directory.GetFiles(app_data_folder, "*.txt");

    if (backup_files.Length == 0)
    {
        MyDataGrid.Visible = false;
        return;
    }

    MyDataGrid.Visible = true;
    
    // sort the files
    ArrayList list = new ArrayList();
    list.AddRange(backup_files);
    list.Sort();

    DataTable dt = new DataTable();
    DataRow dr;

    dt.Columns.Add(new DataColumn("file", typeof(String)));
    dt.Columns.Add(new DataColumn("url", typeof(String)));

    for (int i = list.Count - 1; i != -1; i--)
    {
        dr = dt.NewRow();

        string just_file = System.IO.Path.GetFileName((string)list[i]);
        dr[0] = just_file;
        dr[1] = "download_file.aspx?which=log&filename=" + just_file;

        dt.Rows.Add(dr);
    }

    DataView dv = new DataView(dt);
    
    MyDataGrid.DataSource = dv;
    MyDataGrid.DataBind();
}
       
   
void my_button_click(object sender, DataGridCommandEventArgs e)
{
    if (e.CommandName == "dlt")
    {
        int i = e.Item.ItemIndex;
        string file = MyDataGrid.Items[i].Cells[0].Text;
        System.IO.File.Delete(app_data_folder + file);
        get_files();
    }
   
}

    
</script>

<html>
<head>
<title id="titl" runat="server">btnet backup db</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, "admin"); %>

<div class=align><table border=0><tr><td>

<form  runat="server">

<asp:DataGrid ID="MyDataGrid" runat="server" BorderColor="black" CssClass="datat"
    CellPadding="3" AutoGenerateColumns="false" OnItemCommand="my_button_click">
    <HeaderStyle CssClass="datah"></HeaderStyle>
    <ItemStyle CssClass="datad"></ItemStyle>
    <Columns>
        <asp:BoundColumn HeaderText="File" DataField="file" />
        <asp:HyperLinkColumn HeaderText="Download" Text="Download" DataNavigateUrlField="url"
            Target="_blank" />
        <asp:ButtonColumn HeaderText="Delete" ButtonType="LinkButton" Text="Delete" CommandName="dlt" />
    </Columns>
</asp:DataGrid>
<div class="err" id="msg" runat="server">&nbsp;</div>

</form>
</table>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>