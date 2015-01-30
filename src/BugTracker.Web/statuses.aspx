<%@ Page language="C#" CodeBehind="statuses.aspx.cs" Inherits="btnet.statuses" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ Import Namespace="btnet" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript" src="sortable.js"></script>    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
<div class=align>
<a href=edit_status.aspx>add new status</a>
</div>
<%


if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "edit_status.aspx?id=", "delete_status.aspx?id=");

}
else
{
	Response.Write ("No statuses in the database.");
}
%>
    

</asp:Content>


