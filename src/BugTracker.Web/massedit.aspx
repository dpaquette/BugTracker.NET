<%@ Page Language="C#" CodeBehind="massedit.aspx.cs" Inherits="btnet.massedit" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="align">

        
            <div runat="server" id="msg" class="err">&nbsp;</div>

                <a href="search.aspx">back to search</a>

            or<p>
                <script>
                    function submit_form() {
                        var frm = document.getElementById("<%=frm.ClientID%>");
                        frm.submit();
                        return true;
                    }

                </script>
                <form runat="server" id="frm">
                    <a style="border: 1px red solid; padding: 3px;" id="confirm_href" runat="server" href="javascript: submit_form()"></a>
                    <input type="hidden" id="bug_list" runat="server" />
                    <input type="hidden" id="update_or_delete" runat="server" />
                </form>


                <p>
                    &nbsp;<p>
        <p>
            <div class="err">Email notifications are not sent when updates are made via this page.</div>
            <p>
                This SQL statement will execute when you confirm:
	<pre id="sql_text" runat="server"></pre>

    </div>

</asp:Content>