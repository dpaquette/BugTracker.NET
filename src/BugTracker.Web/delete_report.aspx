<%@ Page Language="C#" CodeBehind="delete_report.aspx.cs" Inherits="btnet.delete_report" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <p>&nbsp</p>
        <a href="reports.aspx">back to reports</a>

        <p>or</p>


        <form runat="server">
            <a id="confirm_href" runat="server" data-action="submit"></a>
            <input type="hidden" id="row_id" runat="server">
        </form>

    </div>
</asp:Content>

