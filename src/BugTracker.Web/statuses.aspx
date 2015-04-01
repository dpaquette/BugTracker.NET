<%@ Page language="C#" CodeBehind="statuses.aspx.cs" Inherits="btnet.statuses" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ Import Namespace="btnet" %>
<%@ Import Namespace="btnet.Models" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">
    
        <div class="container-fluid">
        <div class="row">
            <div class="col-md-12">
                <a class="btn btn-primary"  href="edit_status.aspx"><i class="glyphicon glyphicon-plus-sign"></i>&nbsp;new status</a>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-9 col-sm-12">
                <div id="table-loading-indicator">Loading...</div>
                <table id="statuses-table" class="table table-striped table-bordered" style="display: none">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>status</th>
                            <th>sort order</th>
                            <th>css class</th>
                            <th>default</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (Status status in Statuses)
                           {
                        %>
                        <tr>
                            <td><%=status.Id %></td>
                            <td><%=status.Name%></td>
                            <td><%=status.SortOrder%></td>
                            <td><%=status.Style%></td>
                            <td><%=status.Default == 1 ? "Y" : "N" %></td>
                            <td>
                                <a href="edit_status.aspx?id=<%=status.Id %>"><i class="glyphicon glyphicon-edit text-primary" title="Edit Project"></i></a>
                                <a href="delete_status.aspx?id=<%=status.Id %>"><i class="glyphicon glyphicon-trash text-primary" title="Delete Project"></i></a>
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
            $("#statuses-table").dataTable();
            $("#statuses-table").show();
            $("#table-loading-indicator").hide();
        });
    </script>
</asp:Content>
