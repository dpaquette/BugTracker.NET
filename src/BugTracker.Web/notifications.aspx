<%@ Page Language="C#" CodeBehind="notifications.aspx.cs" Inherits="btnet.notifications" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet.Models" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="container-fluid">
        <div style="width: 600px;" class="smallnote">
            Email notifications are put into a table into the database and then the system attempts to send them.
        If the system fails to send the notification, it records the reason for the failure with the row.
            <br />
            <br />
            The system makes 3 attempts to send the notification.  After the 3rd attempt,
        you can either give up and delete the unsent notifications
        or you can reset the retry count and let the system continue trying.

        </div>


        <div class="align">
            <a href="edit_queued_notifications.aspx?actn=delete %>">Delete unsent notifications</a>
            <br/>
            <br/>
            <a href="edit_queued_notifications.aspx?actn=reset">Reset retry count to zero</a>
            <br/>
            <br/>
            <a href="edit_queued_notifications.aspx?actn=resend">Try to resend</a>
            <br/>
            <br/>
        </div>

        
        <div class="row">
            <div class="col-md-9 col-sm-12">
                <div id="table-loading-indicator">Loading...</div>
                <table id="notifications-table" class="table table-striped table-bordered" style="display: none">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>data created</th>
                            <th>to</th>
                            <th>bug</th>
                            <th>status</th>
                            <th>retries</th>
                            <th>last error</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (QueuedNotification notification in Notifications)
                           {
                        %>
                        <tr>
                            <td><%=notification.Id %></td>
                            <td><%=notification.DateCreated%></td>
                            <td><%=notification.To%></td>
                            <td><%=notification.BugId%></td>
                            <td><%=notification.Status%></td>
                            <td><%=notification.Retries%></td>
                            <td><%=notification.LastException%></td>
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
            $("#notifications-table").dataTable();
            $("#notifications-table").show();
            $("#table-loading-indicator").hide();
        });
    </script>
</asp:Content>
