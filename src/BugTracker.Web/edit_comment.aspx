<%@ Page Language="C#" CodeBehind="edit_comment.aspx.cs" Inherits="btnet.edit_comment" ValidateRequest="false" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ Import Namespace="btnet.Security" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content ContentPlaceHolderID="headerScripts" runat="server">
    <%  if (User.Identity.GetUseFCKEditor())
        { %>
    <script type="text/javascript" src="scripts/ckeditor/ckeditor.js"></script>
    <% } %>

    <script>

        $(document).ready(do_doc_ready);

        function do_doc_ready() {
    <% 
            if (use_fckeditor)
            {
                Response.Write("CKEDITOR.replace( 'comment' )");
            }
            else
            {
                Response.Write("$('textarea').resizable()");
            }
    %>

        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <a href="edit_bug.aspx?id=<% Response.Write(Convert.ToString(bugid));%>">back to <% Response.Write(btnet.Util.get_setting("SingularBugLabel","bug")); %></a>
    <form class="frm" runat="server">

        <table border="0">
            <tr>
                <td colspan="3">
                    <textarea rows="16" cols="80" runat="server" class="txt resizable" id="comment"></textarea>
                <tr>
                    <td colspan="3">
                        <asp:CheckBox runat="server" class="cb" ID="internal_only" />
                        <span runat="server" id="internal_only_label">Visible to internal users only</span>
                    </td>
                </tr>

            <tr>
                <td colspan="3" align="left">
                    <span runat="server" class="err" id="msg">&nbsp;</span>
                <tr>
                    <td colspan="2" align="center">
                        <input runat="server" class="btn" type="submit" id="sub" value="Update">
        </table>
    </form>


</asp:Content>
