<%@ Page Language="C#" CodeBehind="edit_dashboard.aspx.cs" Inherits="btnet.edit_dashboard" ValidateRequest="false" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="headerScripts" runat="server">
    <style>
        body {
            background: #ffffff;
        }

        .panel {
            background: #ffffff;
            border: 3px solid #cccccc;
            padding: 10px;
            margin-bottom: 10px;
        }
    </style>



    <script>

        var col = 0

        function show_select_report_page(which_col) {

            col = which_col
            popup_window = window.open('select_report.aspx')
        }

        function add_selected_report(chart_type, id) {
            var frm = document.getElementById("addform")
            frm.rp_chart_type.value = chart_type
            frm.rp_id.value = id
            frm.rp_col.value = col
            frm.submit()
        }


    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <a href="dashboard.aspx">back to dashboard</a>
    <table border="0" cellspacing="0" cellpadding="10">
        <tr>

            <td valign="top">&nbsp;<br>

                <% write_column(1); %>

                <div class="panel">
                    <a href="javascript:show_select_report_page(1)">[add report to dashboard column 1]</a>
                </div>

                <td valign="top">&nbsp;<br>

                    <% write_column(2); %>

                    <div class="panel">
                        <a href="javascript:show_select_report_page(2)">[add report to dashboard column 2]</a>
                    </div>


    </table>
    <form id="addform" method="get" action="update_dashboard.aspx">
        <input type="hidden" name="rp_id">
        <input type="hidden" name="rp_chart_type">
        <input type="hidden" name="rp_col">
        <input type="hidden" name="actn" value="add">
        <input type="hidden" name="ses" value="<% Response.Write(ses); %>">
    </form>

</asp:Content>
