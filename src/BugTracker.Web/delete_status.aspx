<%@ Page Language="C#" CodeBehind="delete_status.aspx.cs" Inherits="btnet.delete_status" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">

    <div>
        <p>&nbsp;</p>
        <a href="statuses.aspx">back to statuses</a>

        <p>or</p>

        <form runat="server" id="frm">
            <a id="confirm_href" runat="server" data-action="submit"></a>
            <input type="hidden" id="row_id" runat="server" />
        </form>

    </div>

</asp:Content>



