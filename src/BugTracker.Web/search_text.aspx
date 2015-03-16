<%@ Page Language="C#" CodeBehind="search_text.aspx.cs" Inherits="btnet.search_text" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="System.Data" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">

    <div class="container-fluid">
        <div class="row">
            <div class="col-md-9 col-sm-12">
                <div id="table-loading-indicator">Loading...</div>
                <table id="bugs-table" class="table table-striped table-bordered" style="display: none">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>desc</th>
                            <th>context</th>
                            <th>source</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (DataRow bugRow in _searchResults.Rows)
                           {
                        %>
                        <tr>
                            <td><%=bugRow["id"] %></td>
                            <td><a href="edit_bug.aspx?id=<%=bugRow["id"] %>">
                                <%=bugRow["desc"]%></a>
                            </td>
                            <td><%=bugRow["search_text"] %></td>
                            <td><%=bugRow["search_source"] %></td>
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
            $("#bugs-table").dataTable({
                ordering: false,
                searching: false,
                paging: false
            });
            $("#bugs-table").show();
            $("#table-loading-indicator").hide();
        });
    </script>
</asp:Content>
