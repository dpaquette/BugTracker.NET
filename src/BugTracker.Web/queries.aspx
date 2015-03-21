<%@ Page Language="C#" CodeBehind="queries.aspx.cs" Inherits="btnet.queries" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="btnet" %>
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
                <table id="queries-table" class="table table-striped table-bordered" style="display: none">
                    <thead>
                        <tr>
                            <th>query</th>
                            <th>visibility</th>
                            <th>view</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (DataRow queryRow in ds.Tables[0].Rows)
                           {
                        %>
                        <tr>
                            <td><%=queryRow["query"]%></td>
                            <td><%=queryRow["visibility"]%></td>
                            <td><a href='bugs.aspx?queryId=<%=queryRow["id"]%>'>view</a></td>
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
            $("#queries-table").dataTable();
            $("#queries-table").show();
            $("#table-loading-indicator").hide();
        });
    </script>
</asp:Content>