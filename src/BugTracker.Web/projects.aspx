<%@ Page Language="C#" CodeBehind="projects.aspx.cs" Inherits="btnet.projects" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="btnet" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">

    <div class="container-fluid">
        <div class="row">
            <div class="col-md-12">
                <a class="btn btn-primary" href="edit_project.aspx"><i class="glyphicon glyphicon-plus-sign"></i>&nbsp;new project</a>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-9 col-sm-12">
                <table id="projects-table" class="table table-striped table-bordered">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>project</th>
                            <th>active</th>
                            <th>default user</th>
                            <th>auto assign<br />
                                default user</th>
                            <th>auto subscribe<br />
                                default user</th>
                            <th>receive items<br />
                                via pop3</th>
                            <th>pop3 username</th>
                            <th>from email<br />
                                address</th>
                            <th>default</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (DataRow dataRow in ds.Tables[0].Rows)
                           {
                               string projectId = Convert.ToString(dataRow["id"]);
                        %>
                        <tr>
                            <td><%=projectId %></td>
                            <td><%=dataRow["project"] %></td>
                            <td><%=dataRow["active"] %></td>
                            <td><%=dataRow["default user"] %></td>
                            <td><%=dataRow["auto assign default user"] %></td>
                            <td><%=dataRow["auto subscribe default user"] %></td>
                            <td><%=dataRow["receive items via pop3"] %></td>
                            <td><%=dataRow["pop3 username"] %></td>
                            <td><%=dataRow["from email address"] %></td>
                            <td><%=dataRow["default"] %></td>
                            <td>
                                <a href="edit_project.aspx?id=<%=projectId %>"><i class="glyphicon glyphicon-edit text-primary" title="Edit Project"></i></a>
                                <a href="edit_user_permissions2.aspx?projects=y&id=<%=projectId %>"><i class="glyphicon glyphicon-user text-primary" title="Edit User Permissions"></i></a>
                                <a href="delete_project.aspx?id=<%=projectId %>"><i class="glyphicon glyphicon-trash text-primary" title="Delete Project"></i></a>
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
            $("#projects-table").dataTable();
        });
    </script>
</asp:Content>
