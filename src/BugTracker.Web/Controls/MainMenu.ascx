<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainMenu.ascx.cs" Inherits="btnet.Controls.MainMenu" %>
<%@ Import Namespace="btnet" %>
<%= Util.context.Application["custom_header"]%>

<span id="debug" style='position: absolute; top: 0; left: 0;'></span>
<script type='text/javascript' src='scripts/require.js'></script>
<script>
    function dbg(s) {
        document.getElementById('debug').innerHTML += (s + '<br>');
    }
    function on_submit_search() {
        el = document.getElementById('lucene_input');
        if (el.value == '') {
            alert('Enter the words you are search for.');
            el.focus();
            return false;
        }
        else {
            return true;
        }
    }
</script>
<script type='text/javascript'>
    require.config({
        baseUrl: 'scripts',
        paths: {
            jquery: 'jquery-1.11.1'
        }
    });
    require(['jquery'], function ($) {
        $(function () {
            $('a').filter(function () { return this.hostname && this.hostname !== location.hostname; }).addClass('external-link');

            $('td.menu_td span:contains("<%:SelectedItem%>")').addClass("selected_menu_item");
        });
    });
</script>
<table border="0" width="100%" cellpadding="0" cellspacing="0" class="menubar">
    <tr>

        <%= Util.context.Application["custom_logo"] %>
        <td width="20">&nbsp;</td>
        <td class='menu_td'>
            <a href="bugs.aspx"><span class='menu_item warn' style='margin-left: 3px;'><%:btnet.Util.get_setting("PluralBugLabel", "bugs") %></span></a>
        </td>



        <% if (Page.User.Identity.GetCanSearch())
           { %>
        <td class='menu_td'>
            <a href="search.aspx"><span class='menu_item warn' style='margin-left: 3px;'>search</span></a>
        </td>
        <% } %>


        <% if (Util.get_setting("EnableWhatsNewPage", "0") == "1")
           { %>
        <td class='menu_td'>
            <a href="view_whatsnew.aspx"><span class='menu_item warn' style='margin-left: 3px;'>news</span></a>
        </td>
        <% } %>

        <% if (!Page.User.IsInRole(BtnetRoles.Guest))
           { %>
        <td class='menu_td'>
            <a href="queries.aspx"><span class='menu_item warn' style='margin-left: 3px;'>queries</span></a>
        </td>
        <% } %>

        <% if (Page.User.IsInRole(BtnetRoles.Admin) || Page.User.Identity.GetCanUseReports() || Page.User.Identity.GetCanEditReports())
           { %>
        <td class='menu_td'>
            <a href="reports.aspx"><span class='menu_item warn' style='margin-left: 3px;'>reports</span></a>
        </td>
        <% } %>

        <% if (Util.get_setting("CustomMenuLinkLabel", "") != "")
           { %>
        <td class='menu_td'>
            <a href="<%:Util.get_setting("CustomMenuLinkUrl", "") %>"><span class='menu_item warn' style='margin-left: 3px;'><%:Util.get_setting("CustomMenuLinkLabel", "") %></span></a>
        </td>
        <% } %>
        <% if (Page.User.IsInRole(BtnetRoles.Admin))
           { %>
        <td class='menu_td'>
            <a href="admin.aspx"><span class='menu_item warn' style='margin-left: 3px;'>admin</span></a>
        </td>
        <% }
           else if (Page.User.IsInRole(BtnetRoles.ProjectAdmin))
           {%>
        <td class='menu_td'>
            <a href="users.aspx"><span class='menu_item warn' style='margin-left: 3px;'>users</span></a>
        </td>
        <% }    %>

        <td nowrap valign="middle">
            <form style='margin: 0px; padding: 0px;' action="edit_bug.aspx" method="get">
                <input class="menubtn" type="submit" value='go to ID'>
                <input class="menuinput" size="4" type="text" class="txt" name="id" accesskey="g">
            </form>
        </td>
        <% if (Util.get_setting("EnableSearch", "1") == "1" && Page.User.Identity.GetCanSearch())
           { %>
        <td nowrap valign="middle">
            <form style='margin: 0px; padding: 0px;' action="search_text.aspx" method="get" onsubmit='return on_submit_search()'>
                <input class='menubtn' type='submit' value='search text'>
                <input class='menuinput' id='lucene_input' size='24' type='text' class='txt'
                    value='<%:(string) HttpContext.Current.Session["query"] %>' name="query" accesskey="s">
                <a href='lucene_syntax.html' target='_blank' style='font-size: 7pt;'>advanced</a>
            </form>
        </td>
        <% }%>
        <td nowrap valign="middle">
            <span class="smallnote">logged in as <%:Page.User.Identity.Name %><br>
            </span></td>
        <td class='menu_td'>
            <a href="logoff.aspx"><span class='menu_item warn' style='margin-left: 3px;'>logoff</span></a>
        </td>

        <% if (!Page.User.IsInRole(BtnetRoles.Guest))
           { %>
        <td class='menu_td'>
            <a href="edit_self.aspx"><span class='menu_item warn' style='margin-left: 3px;'>settings</span></a>
        </td>
        <% } %>
        <td>
            <a target='_blank' href='about.html'><span class='menu_item' style='margin-left: 3px;'>about</span></a></td>
        <td>
            <a target="_blank" href="http://ifdefined.com/README.html"><span class='menu_item' style='margin-left: 3px;'>help</span></a></td>
    </tr>
</table>
<br>