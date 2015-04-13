<%@ Page Language="C#" CodeBehind="delete_category.aspx.cs" Inherits="btnet.delete_category" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>



<asp:Content ContentPlaceHolderID="body" runat="server">
    <p />
    <div class="align">
        <p>&nbsp</p>
        <a href="categories.aspx">back to categories</a>

        <p>
            or<p />
        <form runat="server">
            <a id="confirm_href" runat="server" data-action="submit"></a>
            <input type="hidden" id="row_id" runat="server">
        </form>
    </div>
</asp:Content>