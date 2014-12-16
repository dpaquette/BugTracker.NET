<%@ Page Language="C#" CodeBehind="categories.aspx.cs" Inherits="btnet.categories" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <a href="edit_category.aspx">add new category</a>
        </p>
        <%

            if (ds.Tables[0].Rows.Count > 0)
            {
                btnet.SortableHtmlTable.create_from_dataset(
                    Response, ds, "edit_category.aspx?id=", "delete_category.aspx?id=");

            }
            else
            {
                Response.Write("No categories in the database.");
            }

%>
    </div>
</asp:Content>
