<%@ Page Language="C#" CodeBehind="edit_report.aspx.cs" Inherits="btnet.edit_report" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="headerScripts" runat="server">
    <script type="text/javascript" src="edit_area/edit_area_full.js"></script>

    <script>
        editAreaLoader.init({
            id: "sql_text"	// id of the textarea to transform
            , start_highlight: true	// if start with highlight
            , toolbar: "search, go_to_line, undo, redo, help"
            , browsers: "all"
            , language: "en"
            , syntax: "sql"
            , allow_toggle: false
            , min_height: 300
            , min_width: 400
        });
    </script>

</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class='align'>
        <a href='reports.aspx'>back to reports</a>
        <br>
        <br>
        <table border='0'>
            <tr>
                <td>
                    <form class='frm' runat="server">
                        <table border='0'>
                            <tr>
                                <td class='lbl'>Description:</td>
                                <td>
                                    <input runat="server" type='text' class='txt' id="desc" maxlength='80' size='80'></td>
                                <td runat="server" class='err' id="desc_err">&nbsp;</td>
                            </tr>
                            <tr>
                                <td class='lbl'>Chart Type:</td>
                                <td>
                                    <asp:RadioButton Text="Table" runat="server" GroupName="chart_type" ID="table" />
                                    &nbsp;&nbsp;&nbsp;
							<asp:RadioButton Text="Pie" runat="server" GroupName="chart_type" ID="pie" />
                                    &nbsp;&nbsp;&nbsp;
							<asp:RadioButton Text="Bar" runat="server" GroupName="chart_type" ID="bar" />
                                    &nbsp;&nbsp;&nbsp;
							<asp:RadioButton Text="Line" runat="server" GroupName="chart_type" ID="line" />
                                    &nbsp;&nbsp;&nbsp;
                                </td>
                                <td runat="server" class='err' id="chart_type_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td colspan="3">&nbsp;
                                </td>
                            </tr>

                            <tr>
                                <td class='lbl'>SQL:</td>
                                <td colspan='2'>
                                    <textarea rows='10' cols='70' runat="server" class='txt' name="sql_text" id="sql_text"></textarea>
                                </td>
                            </tr>
                            <tr>
                                <td colspan='3' align='center'>
                                    <span runat="server" class='err' id="msg">&nbsp;</span>
                                </td>
                            </tr>
                            <tr>
                                <td colspan='2' align='center'>
                                    <input runat="server" class='btn' type='submit' id="sub" value="Create or Edit">
                                </td>
                                <td>&nbsp</td>
                            </tr>
                            <tr>
                                <td>&nbsp</td>
                                <td colspan='2' class='cmt'>To use "Pie", "Bar", or "Line", your SQL statement should have two columns
                                <br>
                                    where the first column is the label and the second column contains the value.
							<br>
                                    <br>
                                    You can use the pseudo-variable $ME in your report which will be replaced by your user ID.
							<br>
                                    For example:
							<ul>
                                select .... from ....<br>
                                where bg_assigned_to_user = $ME
                            </ul>
                                </td>
                            </tr>
                        </table>
                    </form>
                </td>
            </tr>
        </table>
</asp:Content>


