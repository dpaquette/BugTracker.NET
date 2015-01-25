<%@ Page Language="C#" CodeBehind="edit_customfield.aspx.cs" Inherits="btnet.edit_customfield" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <table border="0">
            <tr>
                <td>
                    <a href="customfields.aspx">back to custom fields</a><p>
                        <form class="frm" runat="server">
                            <table border="0">

                                <tr>
                                    <td colspan="3">Field Name:&nbsp;<span class="smallnote" style="font-size: 12pt; font-weight: bold;" id="name" runat="server">
                                    </span>
                                    </td>
                                </tr>

                                <% if (dropdown_type.Value == "normal")
                                   { %>

                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <span class="smallnote" id="vals_explanation" runat="server">Use the following if you want the custom field to be a "normal" dropdown.
	<br>
                                            Create a pipe seperated list of values as shown below.
	<br>
                                            No individiual value should be longer than the length of your custom field.
	<br>
                                            Don't use commas, &gt;, &lt;, or quotes in the list of values.
	<br>
                                            Line breaks for your readability are ok.
	<br>
                                            Here are some examples:
	<br>
                                            "1.0|1.1|1.2"
	<br>
                                            "red|blue|green"
	<br>
                                            It's ok to have one of the values be blank:<br>
                                            "|red|blue|green"
                                        </span>
                                    </td>
                                </tr>

                                <% } %>

                                <tr>
                                    <td colspan="3">
                                        <br>
                                        <div class="lbl" id="vals_label" runat="server">Normal Dropdown Values:</div>
                                        <p>
                                            <textarea runat="server" class="txt" id="vals" rows="6" cols="60"></textarea>
                                            <br>
                                            <span runat="server" class="err" id="vals_err">&nbsp;</span>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="2" class="lbl">Default:
	&nbsp;&nbsp;<input runat="server" type="text" class="txt" id="default_value" maxlength="50" size="50"></td>
                                    <input runat="server" type="hidden" id="hidden_default_value">
                                    <input runat="server" type="hidden" id="hidden_default_name">
                                    <td runat="server" class="err" id="default_value_error">&nbsp;</td>
                                </tr>


                                <tr>
                                    <td colspan="3">
                                        <span class="smallnote">Controls what order the custom fields display on the page.
                                        </span>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="2" class="lbl">Sort Sequence:
	&nbsp;&nbsp;<input runat="server" type="text" class="txt" id="sort_seq" maxlength="2" size="2"></td>
                                    <td runat="server" class="err" id="sort_seq_err">&nbsp;</td>
                                </tr>


                                <tr>
                                    <td colspan="3" align="left">
                                        <span runat="server" class="err" id="msg">&nbsp;</span>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="2" align="center">
                                        <input runat="server" class="btn" type="submit" id="sub" value="Update">
                                    </td>
                                    <td>&nbsp;</td>
                                </tr>

                            </table>

                            <input type="hidden" id="dropdown_type" runat="server">
                        </form>

                </td>
            </tr>
        </table>
    </div>
</asp:Content>
