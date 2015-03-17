<%@ Page language="C#" CodeBehind="udfs.aspx.cs" Inherits="btnet.udfs" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ Import Namespace="System.IdentityModel.Metadata" %>
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
                <a class="btn btn-primary" href="edit_udf.aspx"><i class="glyphicon glyphicon-plus-sign"></i>&nbsp;new user defined attribute value</a>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-9 col-sm-12">
                <div id="table-loading-indicator">Loading...</div>
                <table id="udfs-table" class="table table-striped table-bordered" style="display: none">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>name</th>
                            <th>sort order</th>
                            <th>default</th>

                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (UserDefinedAttribute udf in Attributes)
                           {
                        %>
                        <tr>
                            <td><%=udf.udf_id %></td>
                            <td><%=udf.udf_name%></td>
                            <td><%=udf.udf_sort_seq%></td>
                            <td><%=udf.udf_default== 1 ? "Y" : "N" %></td>
                            <td>
                                <a href="edit_udf.aspx?id=<%=udf.udf_id %>"><i class="glyphicon glyphicon-edit text-primary" title="Edit User Defined Attribute"></i></a>
                                <a href="delete_udf.aspx?id=<%=udf.udf_id %>"><i class="glyphicon glyphicon-trash text-primary" title="Delete User Defined Attribute"></i></a>
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
            $("#udfs-table").dataTable();
            $("#udfs-table").show();
            $("#table-loading-indicator").hide();
        });
    </script>
</asp:Content>


