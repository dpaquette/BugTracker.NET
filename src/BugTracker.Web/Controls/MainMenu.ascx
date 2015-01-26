<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainMenu.ascx.cs" Inherits="btnet.Controls.MainMenu" %>
<%@ Import Namespace="btnet" %>
<%@ Import Namespace="btnet.Security" %>
<%= Util.context.Application["custom_header"]%>
<nav class="navbar navbar-default">
    <div class="container-fluid">
        <div class="navbar-header">
            <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
            <a class="navbar-brand" href="/bugs.aspx">
                <%= Util.context.Application["custom_logo"] %>
            </a>
        </div>
        <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
            <ul class="nav navbar-nav">
                <li><a href="bugs.aspx"><%:btnet.Util.get_setting("PluralBugLabel", "bugs") %></a></li>
                <% if (Page.User.Identity.GetCanSearch())
                   { %>
                <li><a href="search.aspx">search</a> </li>
                <% } %>
                <% if (Util.get_setting("EnableWhatsNewPage", "0") == "1")
                   { %>
                <li><a href="view_whatsnew.aspx">news</a> </li>
                <% } %>

                <% if (!Page.User.IsInRole(BtnetRoles.Guest))
                   { %>
                <li>
                    <a href="queries.aspx">queries</a>
                </li>
                <% } %>

                <% if (Page.User.IsInRole(BtnetRoles.Admin) || Page.User.Identity.GetCanUseReports() || Page.User.Identity.GetCanEditReports())
                   { %>
                <li>
                    <a href="reports.aspx">reports</a>
                </li>
                <% } %>


                <% if (Util.get_setting("CustomMenuLinkLabel", "") != "")
                   { %>
                <li>
                    <a href="<%:Util.get_setting("CustomMenuLinkUrl", "") %>"><%:Util.get_setting("CustomMenuLinkLabel", "") %></a>
                </li>
                <% } %>
                <% if (Page.User.IsInRole(BtnetRoles.Admin))
                   { %>
                <li>
                    <a href="admin.aspx">admin</a>
                </li>
                <% }
                   else if (Page.User.IsInRole(BtnetRoles.ProjectAdmin))
                   {%>
                <li>
                    <a href="users.aspx">users</a>
                </li>
                <% }    %>
            </ul>
            <form class="navbar-form navbar-left" action="edit_bug.aspx" method="get">
                <div class="form-group">
                    <input class="form-control" size="4" type="text" name="id" accesskey="g" placeholder="bug id">
                </div>
                <button type="submit" class="btn btn-default">go</button>
            </form>

            <% if (Util.get_setting("EnableSearch", "1") == "1" && Page.User.Identity.GetCanSearch())
               { %>

            <form class="navbar-form navbar-left" role="search" action="search_text.aspx" method="get" onsubmit='return on_submit_search()'>
                <div class="form-group">
                    <input type="text" class="form-control" id='lucene_input' name="query" accesskey="s" placeholder="Search">
                </div>
                <button type="submit" class="btn btn-default"><span class="glyphicon glyphicon-search"></span></button>
                <a href='lucene_syntax.html' target='_blank'>?</a>
            </form>
            <% } %>
            <ul class="nav navbar-nav navbar-right">

                <li class="dropdown">
                    <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false"><span class="glyphicon glyphicon-user"></span> <%:Page.User.Identity.Name %> <span class="caret"></span></a>
                    <ul class="dropdown-menu" role="menu">
                        <% if (!Page.User.IsInRole(BtnetRoles.Guest))
                           { %>
                        <li>
                            <a href="edit_self.aspx">settings</a>
                        </li>
                        <% } %>
                        <li>
                            <a href="logoff.aspx">logoff</a>
                        </li>

                    </ul>
                </li>
            </ul>
        </div>
        <!-- /.navbar-collapse -->
    </div>
</nav>
<span id="debug" style='position: absolute; top: 0; left: 0;'></span>
<script type='text/javascript' src='scripts/require.js'></script>
<script>
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
    $(function () {
        $('a').filter(function () { return this.hostname && this.hostname !== location.hostname; }).addClass('external-link');

        $('li:has(a:contains("<%:SelectedItem%>"))').addClass("active");
    });
</script>
