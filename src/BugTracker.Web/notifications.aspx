<%@ Page language="C#" CodeBehind="notifications.aspx.cs" Inherits="btnet.notifications" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>
<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript" src="sortable.js"></script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div style="width: 600px;" class=smallnote>

Email notifications are put into a table into the database and then the system attempts to send them.
If the system fails to send the notification, it records the reason for the failure with the row.
<br><br>
The system makes 3 attempts to send the notification.  After the 3rd attempt,
you can either give up and delete the unsent notifications
or you can reset the retry count and let the system continue trying.

</div>

<p>
<div class=align>
<a href="edit_queued_notifications.aspx?actn=delete&ses=<% Response.Write(ses); %>" >Delete unsent notifications</a>
<br>
<br>
<a href="edit_queued_notifications.aspx?actn=reset&ses=<% Response.Write(ses); %>">Reset retry count to zero</a>
<br>
<br>
<a href="edit_queued_notifications.aspx?actn=resend&ses=<% Response.Write(ses); %>" >Try to resend</a>
<br>
<br>

<%

    if (ds.Tables[0].Rows.Count > 0)
    {
        SortableHtmlTable.create_from_dataset(
            Response, ds, "", "");

    }
    else
    {
        Response.Write("No queued email notifications in the database.");
    }

%>
</div>

</asp:Content>
