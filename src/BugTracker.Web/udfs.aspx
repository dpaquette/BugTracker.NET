<%@ Page language="C#" CodeBehind="udfs.aspx.cs" Inherits="btnet.udfs" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ Import Namespace="btnet" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript" src="sortable.js"></script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class=align>
<a href=edit_udf.aspx>add new user defined attribute value</a>
</div>
<%


if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "edit_udf.aspx?id=", "delete_udf.aspx?id=");

}
else
{
	Response.Write ("No user defined attributes in the database.");
}
%>
</asp:Content>

