<%@ Page Language="C#" CodeBehind="orgs.aspx.cs" Inherits="btnet.orgs" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>
<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript" src="sortable.js"></script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="align">
        <a href="edit_org.aspx">add new org</a>
        <%
            if (ds.Tables[0].Rows.Count > 0)
            {
                SortableHtmlTable.create_from_dataset(
                    Response, ds, "", "", false);
            }
            else
            {
                Response.Write("No orgs in the database.");
            }

        %>
    </div>

</asp:Content>
