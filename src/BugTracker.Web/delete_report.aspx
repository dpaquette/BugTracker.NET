<%@ Page Language="C#" CodeBehind="delete_report.aspx.cs" Inherits="btnet.delete_report" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <p>&nbsp</p>
        <a href="reports.aspx">back to reports</a>

        <p>or</p>


        <form runat="server">
            <a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
            <input type="hidden" id="row_id" runat="server">
        </form>

    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="headerScripts" runat="server">
    <script>
        function submit_form() {
            var frm = document.getElementById("<%: Form.ClientID %>");
            frm.submit();
            return true;
        }

    </script>
</asp:Content>

