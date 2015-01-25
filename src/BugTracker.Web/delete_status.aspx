<%@ Page Language="C#" CodeBehind="delete_status.aspx.cs" Inherits="btnet.delete_status" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">

    <div>
        <p>&nbsp;</p>
        <a href="statuses.aspx">back to statuses</a>

        <p>or</p>

        <script>
            function submit_form() {
                var frm = document.getElementById("<%=frm.ClientID%>");
                frm.submit();
                return true;
            }

        </script>
        <form runat="server" id="frm">
            <a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
            <input type="hidden" id="row_id" runat="server" />
        </form>

    </div>

</asp:Content>



