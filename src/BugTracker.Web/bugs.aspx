<%@ Page Language="C#" CodeBehind="bugs.aspx.cs" Inherits="btnet.bugs" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<%@ Import Namespace="btnet" %>
<%@ Import Namespace="btnet.Security" %>


<asp:Content ContentPlaceHolderID="headerScripts" runat="server">

    <script type="text/javascript" src="bug_list.js"></script>

    <script>

        $(document).ready(function() {
            $('.filter').click(on_invert_filter);
            $('.filter_selected').click(on_invert_filter);
        });


        function on_query_changed()
        {
            var action = document.getElementById("<%=actn.ClientID%>");
            $(action).val("query");
            var frm = document.getElementById("<%=frm.ClientID%>");
            frm.submit();
        }

</script>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <form method="POST" runat="server" id="frm">
        <div class="row">
            <div class="col-md-12">
                <label>Query</label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-4 col-sm-6 col-xs-12">
                <asp:DropDownList ID="query" runat="server" class="form-control" onchange="on_query_changed()">
                </asp:DropDownList>
            </div>

        </div>
        <br />
        <div class="row">
            <div class="col-md-12">
                <% if (User.Identity.GetCanAddBugs())
                   { %>
                <a href="edit_bug.aspx" class="btn btn-primary"><span class="glyphicon glyphicon-plus"></span>&nbsp;New <%=Util.get_setting("SingularBugLabel", "bug")%></a>

                <% } %>
                <div class="btn-group">
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                        <span class="glyphicon glyphicon-print"></span>&nbsp;Print<span class="caret"></span>
                    </button>


                    <ul class="dropdown-menu" role="menu">
                        <li><a target="_blank" href="print_bugs.aspx">List</a></li>
                        <li><a target="_blank" href="print_bugs2.aspx">Details</a></li>
                    </ul>
                </div>
                <a target="_blank" class="btn btn-default" href="print_bugs.aspx?format=excel"><span class="glyphicon glyphicon-download"></span>&nbsp;Export to Excel</a>
                <a target="_blank" class="btn btn-default pull-right" href="btnet_screen_capture.exe" title="Download Screen Capture Utility">
                    <span class="glyphicon glyphicon-camera"></span>&nbsp;Screen Capture Tool</a>
            </div>
        </div>
        <br />
        


        <div class="row">
            <div class="col-md-12">

                <%
                    if (dv != null)
                    {
                        if (dv.Table.Rows.Count > 0)
                        {
                            if (btnet.Util.get_setting("EnableTags", "0") == "1")
                            {
                                btnet.BugList.display_buglist_tags_line(Response, User.Identity);
                            }
                            display_bugs(false);
                        }
                        else
                        {
                            Response.Write("<p>No ");
                            Response.Write(Util.get_setting("PluralBugLabel", "bugs"));
                            Response.Write(" yet.<p>");
                        }
                    }
                    else
                    {
                        Response.Write("<div class=err>Error in query SQL: " + sql_error + "</div>");
                    }%>
            </div>
        </div>
        <input type="hidden" name="new_page" id="new_page" runat="server" value="0" />
        <input type="hidden" name="actn" id="actn" runat="server" value="" />
        <input type="hidden" name="filter" id="filter" runat="server" value="" />
        <input type="hidden" name="sort" id="sort" runat="server" value="-1" />
        <input type="hidden" name="prev_sort" id="prev_sort" runat="server" value="-1" />
        <input type="hidden" name="prev_dir" id="prev_dir" runat="server" value="ASC" />
        <input type="hidden" name="tags" id="tags" value="" />


        <div id="popup" class="buglist_popup"></div>
    </form>
</asp:Content>

<asp:Content ContentPlaceHolderID="footerScripts" runat="server">
    <script>
        var enable_popups = <% Response.Write(User.Identity.GetEnablePopups() ? "1" : "0"); %>;
        var asp_form_id = '<% Response.Write(Util.get_form_name()); %>';
</script>
</asp:Content>
