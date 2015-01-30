<%@ Page Language="C#" CodeBehind="queries.aspx.cs" Inherits="btnet.queries" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ Import Namespace="btnet" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript" src="sortable.js"></script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="align">
        <%

            if (ds.Tables[0].Rows.Count > 0)
            {
                SortableHtmlTable.create_from_dataset(
                    Response, ds, "", "", false);
            }
            else
            {
                Response.Write("No queries in the database.");
            }

        %>
    </div>

</asp:Content>

