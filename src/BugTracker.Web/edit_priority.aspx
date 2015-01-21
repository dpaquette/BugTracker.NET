<%@ Page Language="C#" CodeBehind="edit_priority.aspx.cs" Inherits="btnet.edit_priority" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script type="text/javascript">

        function change_sample_color() {

            var sample = document.getElementById("sample");
            var color = document.getElementById("<%= color.ClientID%>");

            try {
                sample.style.background = color.value;
            }
            catch (e) {
            }


        }
    </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="align">
        <table border="0">
            <tr>
                <td>
                    <a href="priorities.aspx">back to priorities</a>
                    <form class="frm" runat="server">
                        <table border="0">

                            <tr>
                                <td class="lbl">Description:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="name" maxlength="20" size="20" /></td>
                                <td runat="server" class="err" id="name_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td colspan="3">
                                    <span class="smallnote">Sort Sequence controls the sort order in the dropdowns.</span>
                                </td>
                            </tr>

                            <tr>
                                <td class="lbl">Sort Sequence:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="sort_seq" maxlength="2" size="2" /></td>
                                <td runat="server" class="err" id="sort_seq_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td colspan="3">
                                    <span class="smallnote">Background Color and CSS Class can be used to control the look of lists.<br>
                                        See the example queries.</span>
                                </td>
                            </tr>

                            <tr>
                                <td class="lbl">Background Color:</td>
                                <td>
                                    <input onkeyup="change_sample_color()" runat="server" type="text" class="txt" id="color" value="#ffffff" maxlength="7" size="7" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;<span style="padding: 3px;" id="sample">&nbsp;&nbsp;Sample&nbsp;&nbsp;</span></td>
                                <td runat="server" class="err" id="color_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td class="lbl">CSS Class:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="style" value="" maxlength="10" size="10" />
                                    &nbsp;&nbsp;<a target="_blank" href="edit_styles.aspx">more CSS info...</a>
                                </td>
                                <td runat="server" class="err" id="style_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td class="lbl">Default Selection:</td>
                                <td>
                                    <asp:CheckBox runat="server" class="cb" ID="default_selection" /></td>
                                <td>&nbsp</td>
                            </tr>

                            <tr>
                                <td colspan="3" align="left">
                                    <span runat="server" class="err" id="msg">&nbsp;</span>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2" align="center">
                                    <input runat="server" class="btn" type="submit" id="sub" value="Create or Edit" />

                                </td>
                                <td>&nbsp</td>
                            </tr>

                        </table>
                    </form>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>


