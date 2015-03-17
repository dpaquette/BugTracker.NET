<%@ Page Language="C#" CodeBehind="priorities.aspx.cs" Inherits="btnet.priorities" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet" %>
<%@ Import Namespace="btnet.Models" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-12">
                <a class="btn btn-primary" href="edit_priority.aspx"><i class="glyphicon glyphicon-plus-sign"></i>&nbsp;new priority</a>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-9 col-sm-12">
                <div id="table-loading-indicator">Loading...</div>
                <table id="priorities-table" class="table table-striped table-bordered" style="display: none">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>description</th>
                            <th>sort sequence</th>
                            <th>background color</th>
                            <th>css class</th>
                            <th>default</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (Priority priority in Priorities)
                           {
                        %>
                        <tr>
                            <td><%=priority.Id %></td>
                            <td><%=priority.Name%></td>
                            <td><%=priority.SortOrder%></td>
                            <td>
                                <div style='background-color:<%=priority.BackgroundColor%>'><%=priority.BackgroundColor%></div>
                            </td>
                            <td><%=priority.Style %></td>
                            <td><%=priority.Default == 1 ? "Y" : "N" %></td>
                            <td>
                                <a href="edit_priority.aspx?id=<%=priority.Id %>"><i class="glyphicon glyphicon-edit text-primary" title="Edit Priority"></i></a>
                                <a href="delete_priority.aspx?id=<%=priority.Id %>"><i class="glyphicon glyphicon-trash text-primary" title="Delete Priority"></i></a>
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
            $("#priorities-table").dataTable();
            $("#priorities-table").show();
            $("#table-loading-indicator").hide();
        });
    </script>
</asp:Content>