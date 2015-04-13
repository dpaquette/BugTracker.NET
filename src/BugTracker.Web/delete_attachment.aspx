<%@ Page Language="C#" CodeBehind="delete_attachment.aspx.cs" Inherits="btnet.delete_attachment" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <form method="POST" runat="server">
        <p>&nbsp</p>
        <a id="back_href" runat="server" href="">back to <% Response.Write(btnet.Util.get_setting("SingularBugLabel", "bug")); %>
        </a>

        <p />
        or<p />



        <a id="confirm_href" runat="server" data-action="submit"></a>
        <input type="hidden" id="row_id" runat="server">
    </form>
</asp:Content>

