<%@ Page language="C#" CodeBehind="reports.aspx.cs" Inherits="btnet.reports" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ Import Namespace="btnet" %>
<%@ Import Namespace="btnet.Security" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript" src="sortable.js"></script>    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
<div class=align>


<% if (User.IsInRole(BtnetRoles.Admin)|| User.Identity.GetCanEditReports()) { %>
<a href='edit_report.aspx'>add new report</a>&nbsp;&nbsp;&nbsp;&nbsp;
<% } %>

<a href='dashboard.aspx'>dashboard</a>

<%

    if (ds.Tables[0].Rows.Count > 0)
    {
	    SortableHtmlTable.create_from_dataset(
		    Response, ds, "", "", false);

    }
    else
    {
	    Response.Write ("No reports in the database.");
    }

%>
</div>
    
</asp:Content>
