<%@ Page Language="C#" CodeBehind="add_attachment.aspx.cs" Inherits="btnet.add_attachment" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<%@ Import Namespace="System.IO" %>


<asp:Content ContentPlaceHolderID="headerScripts" runat="server">
    <script>

        function set_msg(s) {
            document.getElementById("msg").innerHTML = s
            document.getElementById("file_input").innerHTML
                = '<input type=file class=txt name="attached_file" id="attached_file" maxlength=255 size=60>'
        }

        function waiting() {
            document.getElementById("msg").innerHTML = "Uploading..."
            return true
        }

</script>

</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <iframe name="hiddenframe" style="display: none">x</iframe>

    <div class="align">
        Add attachment to <% Response.Write(Convert.ToString(bugid)); %>
        <p>
            <table border="0">
                <tr>
                    <td>
                        <form target="hiddenframe" class="frm" runat="server" enctype="multipart/form-data" onsubmit="return waiting()">
                            <table border="0">

                                <tr>
                                    <td class="lbl">Description:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="desc" maxlength="80" size="80"></td>
                                    <td runat="server" class="err" id="desc_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td class="lbl">File:</td>
                                    <td>
                                        <div id="file_input">
                                            <input runat="server" type="file" class="txt" id="attached_file" maxlength="255" size="60">
                                        </div>
                                    </td>
                                    <td runat="server" class="err" id="attached_file_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <asp:CheckBox runat="server" class="cb" ID="internal_only" />
                                        <span runat="server" id="internal_only_label">Visible to internal users only</span>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="3" align="left">
                                        <span runat="server" class="err" id="msg">&nbsp;</span>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="2" align="center">
                                        <input runat="server" class="btn" type="submit" id="sub" value="Upload">
                                    </td>
                                </tr>
                            </table>
                        </form>
                    </td>
                </tr>
            </table>
    </div>
</asp:Content>
