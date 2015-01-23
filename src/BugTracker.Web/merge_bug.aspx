<%@ Page Language="C#" CodeBehind="merge_bug.aspx.cs" Inherits="btnet.merge_bug" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">


    <div class="align">
        <table border="0">
            <tr>
                <td>

                    <a id="back_href" runat="server" href="">back to <% Response.Write(Util.get_setting("SingularBugLabel", "bug")); %></a>
                    <!--<a id="confirm_href" runat="server" href="">confirm delete</a>
</a>-->
                    <p>
                        Merge all comments, attachments, and subscriptions
from "FROM" <% Response.Write(Util.get_setting("SingularBugLabel", "bug")); %>
into "INTO" <% Response.Write(Util.get_setting("SingularBugLabel", "bug")); %>.
                            <br>
                        <span class="err">Note:&nbsp;&nbsp;"FROM" <% Response.Write(Util.get_setting("SingularBugLabel", "bug")); %>
                        will be deleted!</span>
                    <p>

                        <form runat="server" class="frm">
                            <table border="0">

                                <tr>
                                    <td class="lbl" align="right">FROM <% Response.Write(Util.get_setting("SingularBugLabel", "bug")); %>
                                    :
                                        <td align="left" valign="bottom">
                                            <input type="text" class="txt" id="from_bug" runat="server" size="8" />
                                            <span class="stat" id="static_from_bug" runat="server" style='display: none;'></span>
                                            <br>
                                            <span class="stat" id="static_from_desc" runat="server" style='display: none;'></span>

                                            <tr>
                                                <td colspan="2"><span class="err" id="from_err" runat="server">&nbsp;</span>
                                                <tr>
                                                    <td class="lbl" align="right">INTO <% Response.Write(Util.get_setting("SingularBugLabel", "bug")); %>
                                                    :
                                                    <td align="left" valign="bottom">
                                                        <input type="text" class="txt" id="into_bug" runat="server" size="8" />
                                                        <span class="stat" id="static_into_bug" runat="server" style='display: none;'></span>
                                                        <br>
                                                        <span class="stat" id="static_into_desc" runat="server" style='display: none;'></span>

                                                        <tr>
                                                            <td colspan="2"><span class="err" id="into_err" runat="server">&nbsp;</span>
                                                        </tr>


                                <tr>
                                    <td colspan="2" align="center">
                                        <br>
                                        <input class="btn" type="submit" runat="server" id="submit" value="Merge" />
                            </table>

                            <input type="hidden" id="confirm" runat="server" />
                            <input type="hidden" id="prev_from_bug" runat="server" />
                            <input type="hidden" id="prev_into_bug" runat="server" />
                            <input type="hidden" id="orig_id" runat="server" />
                        </form>

                        <p>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>


