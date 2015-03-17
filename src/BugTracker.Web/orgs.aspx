<%@ Page Language="C#" CodeBehind="orgs.aspx.cs" Inherits="btnet.orgs" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet" %>
<%@ Import Namespace="btnet.Models" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>
<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript" src="sortable.js"></script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">


    <div class="container-fluid">
        <div class="row">
            <div class="col-md-12">
                <a class="btn btn-primary" href="edit_org.aspx"><i class="glyphicon glyphicon-plus-sign"></i>&nbsp;new org</a>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-9 col-sm-12">
                <div id="table-loading-indicator">Loading...</div>
                <table id="orgs-table" class="table table-striped table-bordered" style="display: none">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>description</th>
                            <th>active</th>
                            <th>can search</th>

                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (Organization org in Organizations)
                           {
                        %>
                        <tr>
                            <td><%=org.Id %></td>
                            <td><%=org.Name%></td>
                            <td><%=org.Active == 1 ? "Y" : "N" %></td>
                            <td><%=org.CanSearch == 1 ? "Y" : "N" %></td>
                            <td>
                                <a href="edit_org.aspx?id=<%=org.Id %>"><i class="glyphicon glyphicon-edit text-primary" title="Edit Organization"></i></a>
                                <a href="delete_org.aspx?id=<%=org.Id %>"><i class="glyphicon glyphicon-trash text-primary" title="Delete Organization"></i></a>
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
            $("#orgs-table").dataTable();
            $("#orgs-table").show();
            $("#table-loading-indicator").hide();
        });
    </script>
</asp:Content>
