<%@ Page Language="C#" CodeBehind="projects.aspx.cs" Inherits="btnet.projects" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript" src="sortable.js"></script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="align">
        <a href="edit_project.aspx">add new project</a>
        <%
            if (ds.Tables[0].Rows.Count > 0)
            {
                SortableHtmlTable.create_from_dataset(
                    Response, ds, "", "", false);
            }
            else
            {
                Response.Write("No projects in the database.");
            }
        %>
    </div>
</asp:Content>
