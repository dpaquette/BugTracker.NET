<%@ Page Language="C#" CodeBehind="priorities.aspx.cs" Inherits="btnet.priorities" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript" src="sortable.js"></script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="align">
        <a href="edit_priority.aspx">add new priority</a>
        <%
            if (ds.Tables[0].Rows.Count > 0)
            {
                SortableHtmlTable.create_from_dataset(
                    Response, ds, "edit_priority.aspx?id=", "delete_priority.aspx?id=", false);

            }
            else
            {
                Response.Write("No priorities in the database.");
            }
        %>
    </div>
</asp:Content>
