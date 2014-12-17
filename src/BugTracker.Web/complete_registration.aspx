<%@ Page Language="C#" CodeBehind="complete_registration.aspx.cs" Inherits="btnet.complete_registration" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div align="center">
        <table border="0">
            <tr>
                <td>

                    <div runat="server" class="err" id="msg">&nbsp;</div>
                    <p>
                        <a href="default.aspx">Go to login page</a>
                </td>
            </tr>
        </table>

    </div>
</asp:Content>
