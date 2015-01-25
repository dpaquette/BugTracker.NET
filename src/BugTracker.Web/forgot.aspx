<%@ Page Language="C#" CodeBehind="forgot.aspx.cs" Inherits="btnet.forgot" AutoEventWireup="True" MasterPageFile="LoggedOut.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="body">

    <div align="center">
        <table border="0">
            <tr>
                <td>

                    <form class="frm" runat="server">
                        <table border="0">

                            <tr>
                                <td colspan="2" class="smallnote">Enter Username or Email or both</td>
                            </tr>

                            <tr>
                                <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                <td class="lbl">Username:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="username" size="40" maxlength="40" /></td>
                            </tr>

                            <tr>
                                <td class="lbl">Email:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="email" size="40" maxlength="40" /></td>
                            </tr>

                            <tr>
                                <td colspan="2" align="left">
                                    <span runat="server" class="err" id="msg">&nbsp;</span>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2" align="center">
                                    <input class="btn" type="submit" value="Send password info to my email" runat="server" />
                                </td>
                            </tr>

                        </table>
                    </form>

                    <a href="default.aspx">Return to login page</a>

                </td>
            </tr>
        </table>

    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="footerScripts">
    <script>
        $(function () {
            $("<%=email.ClientID%>").focus();
        });
    </script>
</asp:Content>