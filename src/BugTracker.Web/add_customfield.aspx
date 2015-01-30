<%@ Page Language="C#" CodeBehind="add_customfield.aspx.cs" Inherits="btnet.add_customfield" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<%@ Register Src="~/Controls/MainMenu.ascx" TagPrefix="uc1" TagName="MainMenu" %>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="align">
        <table border="0">
            <tr>
                <td>
                    <a href="customfields.aspx">back to custom fields</a>
                    <form class="frm" runat="server">
                        <table border="0">

                            <tr>
                                <td colspan="3">
                                    <span class="smallnote">Don't use single quotes, &gt;, or &lt; characters in the Field Name.</span>
                                </td>
                            </tr>

                            <tr>
                                <td class="lbl">Field Name:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="name" maxlength="30" size="30"></td>
                                <td runat="server" class="err" id="name_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td colspan="3">&nbsp;
                                </td>
                            </tr>

                            <tr>
                                <td colspan="3" width="350">
                                    <span class="smallnote">A dropdown type of "normal" uses the values specified in "Normal Dropdown Values"
    below. A dropdown type of "users" is filled with values from the users table. The
    same list that is used for "assigned to" will be used for a "user" dropdown.
                                    </span>
                                </td>
                            </tr>


                            <tr>
                                <td class="lbl">Dropdown Type:</td>
                                <td>
                                    <asp:DropDownList ID="dropdown_type" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td>&nbsp</td>
                            </tr>

                            <tr>
                                <td colspan="3">
                                    <span class="smallnote">For "user" dropdown, select "int"</span>
                                </td>
                            </tr>

                            <tr>
                                <td class="lbl">Datatype:</td>
                                <td>
                                    <asp:DropDownList ID="datatype" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td nowrap runat="server" class="err" id="datatype_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td colspan="3">
                                    <span class="smallnote">
                                        <br>
                                        <br>
                                        For text type fields like char, varchar, nvarchar, etc, specify max length.<br>
                                        <br>
                                        For decimal type, specify as A,B where A is the total number of digits and<br>
                                        B is the number of those digits to the right of decimal point.<br>
                                        <br>
                                    </span>
                                </td>
                            </tr>

                            <tr>
                                <td class="lbl">Length/Precision:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="length" maxlength="6" size="6"></td>
                                <td nowrap runat="server" class="err" id="length_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td colspan="3">
                                    <span class="smallnote">
                                        <br>
                                        <br>
                                        If you specify required, you must supply a default.&nbsp;&nbsp;Don't forget the parenthesis.
                                    </span>
                                </td>
                            </tr>

                            <tr>
                                <td class="lbl">Required (NULL or NOT NULL):</td>
                                <td>
                                    <asp:CheckBox runat="server" class="cb" ID="required" />
                                </td>
                                <td nowrap runat="server" class="err" id="required_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td class="lbl">Default:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="default_text" maxlength="30" size="30"></td>
                                <td nowrap runat="server" class="err" id="default_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td colspan="3">&nbsp;
                                </td>
                            </tr>

                            <tr>
                                <td colspan="3">
                                    <span class="smallnote">Use the following if you want the custom field to be a "normal" dropdown.
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


                            <tr>
                                <td colspan="3">
                                    <br>
                                    <div class="lbl">Normal Dropdown Values:</div>
                                    <p>
                                        <textarea runat="server" class="txt" id="vals" rows="6" cols="60"></textarea>
                                        <br>
                                        <span runat="server" class="err" id="vals_err">&nbsp;</span>
                            </tr>


                            <tr>
                                <td colspan="3">&nbsp;
                                </td>
                            </tr>

                            <tr>
                                <td colspan="3">
                                    <span class="smallnote">Controls what order the custom fields display on the page.
                                    </span>
                                </td>
                            </tr>

                            <tr>
                                <td class="lbl">Sort Sequence:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="sort_seq" maxlength="2" size="2"></td>
                                <td runat="server" class="err" id="sort_seq_err">&nbsp;</td>
                            </tr>



                            <tr>
                                <td colspan="3" align="left">
                                    <span runat="server" class="err" id="msg">&nbsp;</span>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2" align="center">
                                    <input runat="server" class="btn" type="submit" id="sub" value="Create">
                                    <td>&nbsp</td>
                                </td>
                            </tr>
                    </form>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>



