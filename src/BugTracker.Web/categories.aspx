<%@ Page Language="C#" CodeBehind="categories.aspx.cs" Inherits="btnet.categories" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet.Models" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-12">
                <a class="btn btn-primary" href="edit_category.aspx"><i class="glyphicon glyphicon-plus-sign"></i>&nbsp;new category</a>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-9 col-sm-12">
                <div id="table-loading-indicator">Loading...</div>
                <table id="categories-table" class="table table-striped table-bordered" style="display: none">
                    <thead>
                        <tr>
                            <th>name</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (Category category in _categories)
                           {
                        %>
                        <tr>
                            <td><%=category.Name%></td>
                            <td>
                                <a href="edit_category.aspx?id=<%:category.Id %>"><i class="glyphicon glyphicon-edit text-primary" title="Edit Category"></i></a>
                            </td>
                        </tr>
                        <%} %>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="footerScripts" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#categories-table").dataTable();
            $("#categories-table").show();
            $("#table-loading-indicator").hide();
        });
    </script>
</asp:Content>
