<%@ Page Language="C#" CodeBehind="edit_udf.aspx.cs" Inherits="btnet.edit_udf" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="align">
        <table border="0">
            <tr>
                <td>
                    <a href="udfs.aspx">back to user defined attribute values</a>
                    <form class="frm" runat="server">
                        <table border="0">

                            <tr>
                                <td class="lbl">Description:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="name" maxlength="20" size="20"/></td>
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
                                    <input runat="server" type="text" class="txt" id="sort_seq" maxlength="2" size="2"/></td>
                                <td runat="server" class="err" id="sort_seq_err">&nbsp;</td>
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
                                    <input runat="server" class="btn" type="submit" id="sub" value="Create or Edit"/>
                                    
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


