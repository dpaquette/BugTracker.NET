<%@ Page Language="C#" CodeBehind="delete_user.aspx.cs" Inherits="btnet.delete_user" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <p>&nbsp</p>
        <a href="users.aspx">back to users</a>

        <p>or</p>

        <form runat="server" id="frm">
            <a id="confirm_href" runat="server" data-action="submit"></a>
            <input type="hidden" id="row_id" runat="server">
        </form>
    </div>
</asp:Content>