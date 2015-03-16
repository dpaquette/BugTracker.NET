<%@ Page Language="C#" CodeBehind="delete_customfield.aspx.cs" Inherits="btnet.delete_customfield" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <p>&nbsp</p>
        <a href="customfields.aspx">back to custom fields</a>

        <p>
            or<p />

        <form runat="server" id="frm">
            <a id="confirm_href" runat="server" data-action="submit"></a>
            <input type="hidden" id="row_id" runat="server">
        </form>
    </div>
</asp:Content>
