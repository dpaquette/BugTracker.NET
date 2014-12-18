<%@ Page Language="C#" CodeBehind="change_password.aspx.cs" Inherits="btnet.change_password" ValidateRequest="false" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">


    <div align="center">
        <table border="0">
            <tr>
                <td>
                    <form class="frm" runat="server">
                        <table border="0">

                            <tr>
                                <td class="lbl">New Password:</td>
                                <td>
                                    <input runat="server" type="password" class="txt" id="password" size="20" maxlength="20" autocomplete="off"></td>
                            </tr>

                            <tr>
                                <td class="lbl">Reenter Password:</td>
                                <td>
                                    <input runat="server" type="password" class="txt" id="confirm" size="20" maxlength="20" autocomplete="off"></td>
                            </tr>

                            <tr>
                                <td colspan="2" align="left">
                                    <span runat="server" class="err" id="msg">&nbsp;</span>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2" align="center">
                                    <input class="btn" type="submit" value="Change password" runat="server">
                                </td>
                            </tr>

                        </table>
                    </form>

                    <a href="default.aspx">Go to login page</a>

                </td>
            </tr>
        </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="footerScripts">
    <script>
    $(function(){document.forms[0].password.focus();});
   </script>
</asp:Content>

