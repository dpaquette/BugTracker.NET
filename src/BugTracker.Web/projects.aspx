<%@ Page Language="C#" CodeBehind="projects.aspx.cs" Inherits="btnet.projects" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet.Models" %>
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
                        <% foreach (Project project in Projects)
                           {
                        %>
                        <tr>
                            <td><%=project.Id %></td>
                            <td><%=project.Name%></td>
                            <td><%=project.Active == 1 ? "Y" : "N" %></td>
                            <td><%=project.DefaultUser == 1 ? "Y" : "N" %></td>
                            <td><%=project.AutoAssignToDefaultUser == 1 ? "Y" : "N" %></td>
                            <td><%=project.AutoSubscribeDefaultUser == 1 ? "Y" : "N" %></td>
                            <td><%=project.EnablePOP3 == 1 ? "Y" : "N" %></td>
                            <td><%=project.POP3UserName %></td>
                            <td><%=project.POP3SourceEMail %></td>
                            <td><%=project.Default == 1 ? "Y" : "N" %></td>
                            <td>
                                <a href="edit_project.aspx?id=<%=project.Id %>"><i class="glyphicon glyphicon-edit text-primary" title="Edit Project"></i></a>
                                <a href="edit_user_permissions2.aspx?projects=y&id=<%=project.Id %>"><i class="glyphicon glyphicon-user text-primary" title="Edit User Permissions"></i></a>
                                <a href="delete_project.aspx?id=<%=project.Id %>"><i class="glyphicon glyphicon-trash text-primary" title="Delete Project"></i></a>
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
