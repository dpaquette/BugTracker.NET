<%@ Page Language="C#" CodeBehind="delete_org.aspx.cs" Inherits="btnet.delete_org" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <p>&nbsp</p>
        <a href="orgs.aspx">back to organizations</a>

        <p>
            or<p />


        <form runat="server">
            <a id="confirm_href" runat="server" data-action="submit"></a>
            <input type="hidden" id="row_id" runat="server">
        </form>
</asp:Content>