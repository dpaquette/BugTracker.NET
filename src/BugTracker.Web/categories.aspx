<%@ Page Language="C#" CodeBehind="categories.aspx.cs" Inherits="btnet.categories" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <a href="edit_category.aspx">add new category</a>
        </p>
        <%

            if (_categories.Any())
            {%>
                
        <table class="table">
            <thead>
                <tr>
                    <th></th>
                    <th>
                        Name
                    </th>
                </tr>
            </thead>
            <tbody>
                <%foreach(var category in _categories){ %>
                <tr>
                    <td>
                        <a href="edit_category.aspx?id=<%:category.Id %>">Edit</a>
                    </td>
                    <td>
                        <%: category.Name %>
                    </td>
                </tr>
                <%} %>
            </tbody>
        </table>
            <%}
            else
            {
                Response.Write("No categories in the database.");
            }

%>
    </div>
</asp:Content>
