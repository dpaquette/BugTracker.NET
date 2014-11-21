<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdatePassword.aspx.cs" Inherits="btnet.UpdatePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title id="titl" runat="server">btnet logon</title>
    <link rel="StyleSheet" href="btnet.css" type="text/css">
    <link rel="shortcut icon" href="favicon.ico">
</head>
<body onload="document.forms[0].user.focus()">
    <div style="float: right;">
        <span>
            <a target="_blank" style="font-size: 7pt; font-family: arial; letter-spacing: 1px;" href="http://ifdefined.com/bugtrackernet.html">BugTracker.NET</a>
            <br>
            <a target="_blank" style="font-size: 7pt; font-family: arial; letter-spacing: 1px;" href="http://ifdefined.com/README.html">Help</a>
            <br>
            <a target="_blank" style="font-size: 7pt; font-family: arial; letter-spacing: 1px;" href="mailto:ctrager@yahoo.com">Feedback</a>
            <br>
            <a target="_blank" style="font-size: 7pt; font-family: arial; letter-spacing: 1px;" href="about.html">About</a>
            <br>
            <a target="_blank" style="font-size: 7pt; font-family: arial; letter-spacing: 1px;" href="http://ifdefined.com/README.html">Donate</a>
        </span>
    </div>
    <table border="0">
        <tr>

            <%

                Response.Write(Application["custom_logo"]);

%>
    </table>
    <div class="center" id="resetKeyError" runat="server" visible="false">
        The password reset key is missing. Please check your e-mail for the correct link.
    </div>
    <div class="center" id="passwordMatchError" runat="server" visible="false">
        The new password fields must match.
    </div>
    <div class="center" id="oldPasswordError" runat="server" visible="false">
        The old password is incorrect.
    </div>
    
    <div class="center" id="newPasswordLacksComplexityError" runat="server" visible="false">
        New password is insufficiently compex.
    </div>
    <div align="center" id="passwordChangeDiv" runat="server">
        <table border="0">
            <tr>
                <td>
                    <form class="frm" runat="server">
                        <table border="0">

                            <tr>
                                <td colspan="2" class="smallnote">Due to some poor password security on our side we're requiring that you enter a new password. </td>
                            </tr>
                            <tr>
                                <td class="lbl">Old Password:</td>
                                <td>
                                    <input runat="server" type="password" class="txt" id="oldPassword"></td>
                            </tr>

                            <tr>
                                <td class="lbl">New Password:</td>
                                <td>
                                    <input runat="server" type="password" class="txt" id="newPassword"/></td>
                            </tr>


                            <tr>
                                <td class="lbl">New Password Again:</td>
                                <td>
                                    <input runat="server" type="password" class="txt" id="newPasswordConfirm"/></td>
                            </tr>

                            <tr>
                                <td colspan="2" align="left">
                                    <span runat="server" class="err" id="msg">&nbsp;</span>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2" align="center">
                                    <input class="btn" type="submit" value="Set New Password" runat="server">
                                </td>
                            </tr>

                        </table>
                    </form>

                </td>
            </tr>
        </table>
    </div>
</body>
</html>
