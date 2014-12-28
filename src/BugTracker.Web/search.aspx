<%@ Page Language="C#" ValidateRequest="false" CodeBehind="search.aspx.cs" Inherits="btnet.search" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ Import Namespace="btnet" %>
<%@ Import Namespace="btnet.Security" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <link rel="StyleSheet" href="Content/style/jquery-ui.min.css" type="text/css">
    <!-- use btnet_edit_bug.css to control positioning on edit_bug.asp.  use btnet_search.css to control position on search.aspx  -->
    <link rel="StyleSheet" href="custom/btnet_search.css" type="text/css">

    <script type="text/javascript" src="scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="bug_list.js"></script>
    <script type="text/javascript" src="suggest.js"></script>
    <script type="text/javascript" src="datejs/date.js"></script>

    <script type="text/javascript">

        search_suggest_min_chars = <% Response.Write(Util.get_setting("SearchSuggestMinChars","3")); %>


        // start of mass edit javascript
<% if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanMassEditBugs())
   { %>

function select_all(sel)
{
    var frm = document.getElementById("massform");
    for (var i = 0; i < frm.elements.length; i++)
    {
        var varname = frm.elements[i].name;
        if (!isNaN(parseInt(varname)))
        {
            frm.elements[i].checked = sel;
        }
    }
}

        function validate_mass()
        {

            var at_least_one_selected = false;

            // make sure at least one item is selected
            var frm = document.getElementById("massform");
            for (var i = 0; i < frm.elements.length; i++)
            {
                var varname = frm.elements[i].name;
                if (!isNaN(parseInt(varname)))
                {
                    var checkbox = frm.elements[i];
                    if (checkbox.checked == true)
                    {
                        at_least_one_selected = true;
                        break;
                    }
                }
            }

            if (!at_least_one_selected)
            {
                alert ("No items selected for mass update/delete.");
                return false;
            }

            if (frm.mass_project.selectedIndex==0
            && frm.mass_org.selectedIndex==0
            && frm.mass_category.selectedIndex==0
            && frm.mass_priority.selectedIndex==0
            && frm.mass_assigned_to.selectedIndex==0
            && frm.mass_status.selectedIndex==0
            && frm.mass_reported_by.selectedIndex==0)
            {
                if (!frm.mass_delete.checked)
                {
                    alert ("No updates were specified and delete wasn't checked.  Please specify updates or delete.");
                    return false;
                }
            }
            else
            {
                if (frm.mass_delete.checked)
                {
                    alert ("Both updates and delete were specified.   Please select one or the other.");
                    return false;
                }
            }

            return true;
        }

        function load_one_massedit_select(from_id, to_id)
        {
            var from;
            var to;
            var option;

            from = document.getElementById(from_id);
            to = document.getElementById(to_id);

            option = document.createElement('option');
            option.value = -1;
            option.text = "[do not update]";
            try {
                to.add(option, null); // standards compliant; doesn't work in IE
            }
            catch(ex) {
                to.add(option); // IE only
            }

            for (var i = 0; i < from.options.length; i++)
            {
                option = document.createElement('option');
                option.value = from.options[i].value;
                option.text = from.options[i].text;
                try {
                    to.add(option, null); // standards compliant; doesn't work in IE
                }
                catch(ex) {
                    to.add(option); // IE only
                }
            }

        }

        function load_massedit_selects()
        {

            load_one_massedit_select ("<%=project.ClientID%>","mass_project");
            load_one_massedit_select ("<%=org.ClientID%>","mass_org");
            load_one_massedit_select ("<%=category.ClientID%>","mass_category");
            load_one_massedit_select ("<%=priority.ClientID%>","mass_priority");
            load_one_massedit_select ("<%=assigned_to.ClientID%>","mass_assigned_to");
            load_one_massedit_select ("<%=status.ClientID%>","mass_status");
            load_one_massedit_select ("<%=reported_by.ClientID%>","mass_reported_by");
        }

        <% } %> // end of mass edit javascript

        function set_hit_submit_button() {
            document.getElementById('<%=hit_submit_button.ClientID%>').value = "1";
        }


        var shown = true;
        function showhide_form()
        {
            var frm =  document.getElementById("<%=searchForm.ClientID%>");
            if (shown)
            {
                frm.style.display = "none";
                shown = false;
                showhide.firstChild.nodeValue = "show form";
            }
            else
            {
                frm.style.display = "block";
                shown = true;
                showhide.firstChild.nodeValue = "hide form";
            }
        }

        function set_project_changed() {
            document.getElementById("<%=project_changed.ClientID%>").value = "1";
        }



        $(function() {
            var date_format = '<% Response.Write(btnet.Util.get_setting("DatepickerDateFormat","yy-mm-dd")); %>';
            $('.date').datepicker({
                dateFormat: date_format, 
                duration: 'fast',
                showOn: 'both',
                buttonText: "Select date"
            });
            $('.filter').click(on_invert_filter);
            $('.filter_selected').click(on_invert_filter);
        });



    </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
    
    <div id="suggest_popup" style="position: absolute; display: none; z-index: 1000;"></div>

    <div class="align">

        <% if (User.Identity.GetCanAddBugs())
   { %>
        <a href="edit_bug.aspx">
            <img src="add.png" border="0" align="top">&nbsp;add new <% Response.Write(Util.get_setting("SingularBugLabel", "bug")); %></a>
        <% } %>

        <a style='margin-left: 40px;' href='javascript:showhide_form()' id='showhide'>hide form</a>



        <table border="0">
            <tr>
                <td>
                <tr>
                    <td>
                        <div id="searchfrom">
                            <form class="frm" ID="searchForm" action="search.aspx" method="POST" runat="server" onmouseover="hide_suggest()">

                                <table border="0" cellpadding="6" cellspacing="0">
                                    <tr>
                                        <td colspan="10"><span class="smallnote">Hold down Ctrl key to select multiple items.</span></td>
                                    </tr>

                                    <tr>
                                        <td nowrap><span class="lbl">reported by:</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="reported_by" runat="server"></asp:ListBox>
                                        </td>

                                        <td nowrap><span class="lbl" id="category_label" runat="server">category:</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="category" runat="server"></asp:ListBox>
                                        </td>

                                        <td nowrap><span class="lbl" id="priority_label" runat="server">priority:</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="priority" runat="server"></asp:ListBox>
                                        </td>

                                        <td nowrap><span class="lbl" id="assigned_to_label" runat="server">assigned to:</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="assigned_to" runat="server"></asp:ListBox>
                                        </td>

                                        <td nowrap><span class="lbl" id="status_label" runat="server">status:</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="status" runat="server" ></asp:ListBox>
                                        </td>
                                    </tr>

                                </table>
                                <table border="0" cellpadding="3" cellspacing="0">
                                    <tr>

                                        <td nowrap><span class="lbl" id="org_label" runat="server">organization:</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="org" runat="server" ></asp:ListBox>
                                        </td>

                                        <td nowrap><span class="lbl" id="project_label" runat="server">project:</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="project" runat="server" onchange="set_project_changed()"
                                                AutoPostBack="true"></asp:ListBox>
                                        </td>

                                        <td nowrap><span class="lbl" id="project_custom_dropdown1_label" runat="server" style="display: none">?</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="project_custom_dropdown1" runat="server" Style="display: none" ></asp:ListBox>
                                        </td>
                                        <td nowrap><span class="lbl" id="project_custom_dropdown2_label" runat="server" style="display: none">?</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="project_custom_dropdown2" runat="server" Style="display: none" ></asp:ListBox>
                                        </td>
                                        <td nowrap><span class="lbl" id="project_custom_dropdown3_label" runat="server" style="display: none">?</span><br>
                                            <asp:ListBox Rows="6" SelectionMode="Multiple" ID="project_custom_dropdown3" runat="server" Style="display: none" ></asp:ListBox>
                                        </td>
                                    </tr>

                                </table>
                                <br>
                                <table border="0" cellpadding="3" cellspacing="0">
                                    <tr>
                                        <td><span class="lbl"><% Response.Write(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug"))); %> description contains:&nbsp;</span>
                                        <td colspan="2">
                                        <input type="text" class="txt" id="like" runat="server" onkeydown="search_criteria_onkeydown(this,event)" onkeyup="search_criteria_onkeyup(this,event)" size="50" autocomplete="off"/>


                                            <% if (show_udf)
     {
                                            %>
                                        <td nowrap rowspan="2"><span class="lbl" id="udf_label" runat="server"><% Response.Write(Util.get_setting("UserDefinedBugAttributeName", "YOUR ATTRIBUTE")); %></span><br>
                                            <asp:ListBox Rows="4" SelectionMode="Multiple" ID="udf" runat="server" ></asp:ListBox>

                                            <%
     }
                                            %>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td><span class="lbl"><% Response.Write(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug"))); %> comments contain:&nbsp;</span>
                                        <td colspan="2">
                                            <input type="text" class="txt" id="like2" runat="server" size="50" autocomplete="off"/>
                                        </td>
                                    </tr>


                                    <tr>
                                        <td nowrap><span class="lbl"><% Response.Write(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug"))); %> comments since:&nbsp;</span>
                                        <td colspan="2">
                                            <input type="text" class="txt date" id="comments_since" runat="server" size="10"/>
                                        </td>
                                    </tr>


                                    <tr>
                                        <td nowrap><span class="lbl">"Reported on" from date:&nbsp;</span>
                                        <td colspan="2">
                                            <input runat="server" type="text" class="txt date" id="from_date" maxlength="10" size="10" />
                                            
                                            &nbsp;&nbsp;&nbsp;&nbsp;
			<span class="lbl">to:&nbsp;</span>
                                            <input runat="server" type="text" class="txt date" id="to_date" maxlength="10" size="10" />
                                           
                                        </td>
                                    </tr>

                                    <tr>
                                        <td nowrap><span class="lbl">"Last updated on" from date:&nbsp;</span>
                                        <td colspan="2">
                                            <input runat="server" type="text" class="txt date" id="lu_from_date" maxlength="10" size="10"/>
                                   

                                            &nbsp;&nbsp;&nbsp;&nbsp;
			<span class="lbl">to:&nbsp;</span>
                                            <input runat="server" type="text" class="txt date" id="lu_to_date" maxlength="10" size="10" />
                                    
                                        </td>
                                    </tr>



                                    <%

                                        int minTextAreaSize = int.Parse(Util.get_setting("TextAreaThreshold", "100"));
                                        int maxTextAreaRows = int.Parse(Util.get_setting("MaxTextAreaRows", "5"));

                                    %>

                                    <tr>
                                        <td colspan="10" nowrap>Use "and" logic:<input type="radio" runat="server" name="and_or" value="and" id="and" checked/>
                                            &nbsp;&nbsp;
                                            Use "or" logic:<input type="radio" runat="server" name="and_or" value="or" id="or" />
                                        </td>
                                    </tr>

                                    <tr>
                                        <td colspan="10" align="center">
                                            <input type="hidden" runat="server" id="project_changed" value="0"/>
                                            <input type="hidden" runat="server" id="hit_submit_button" value="0"/>
                                            <input type="hidden" runat="server" id="hit_save_query_button" value="0"/>
                                            <input class="btn" type="submit" onclick="set_hit_submit_button()" value="&nbsp;&nbsp;&nbsp;Search&nbsp;&nbsp;&nbsp;" runat="server"/>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td colspan="10" align="right">
                                            
              
                                        </td>
                                    </tr>

                                </table>

                                <input type="hidden" name="new_page" id="new_page" runat="server" value="0"/>
                                <input type="hidden" name="actn" id="actn" runat="server" value=""/>
                                <input type="hidden" name="filter" id="filter" runat="server" value=""/>
                                <input type="hidden" name="sort" id="sort" runat="server" value="-1"/>
                                <input type="hidden" name="prev_sort" id="prev_sort" runat="server" value="-1"/>
                                <input type="hidden" name="prev_dir" id="prev_dir" runat="server" value="ASC"/>
                                <input type="hidden" name="tags" id="tags" value=""/>

                                <script>
                                    var enable_popups = <% Response.Write(User.Identity.GetEnablePopups() ? "1" : "0"); %>;
                                </script>

                                <div id="popup" class="buglist_popup"></div>

                                <input type="hidden" id="query" runat="server" value=""/>
                            </form>
                        </div>

                    </td>
                </tr>
        </table>
    </div>

    <%
        if (dv == null)
        {

        }
        else
        {
            if (dv.Table.Rows.Count > 0)
            {

                Response.Write("<a target=_blank href=print_bugs.aspx>print list</a>");
                Response.Write("&nbsp;&nbsp;&nbsp;<a target=_blank href=print_bugs2.aspx>print detail</a>");
                Response.Write("&nbsp;&nbsp;&nbsp;<a target=_blank href=print_bugs.aspx?format=excel>export to excel</a><br>");

                if (btnet.Util.get_setting("EnableTags", "0") == "1")
                {
                    btnet.BugList.display_buglist_tags_line(Response, User.Identity);
                }


                if (!User.IsInRole(BtnetRoles.Guest) && (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanMassEditBugs()))
                {
                    Response.Write("<form id=massform onsubmit='return validate_mass()' method=get action=massedit.aspx>");
                    display_bugs(true);
                    Response.Write("<p><table class=frm><tr><td colspan=5 class=smallnote>Update or delete all checked items");
                    Response.Write("<tr><td colspan=5>");
                    Response.Write("<a href=javascript:select_all(true)>select all</a>&nbsp;&nbsp;&nbsp;&nbsp;");
                    Response.Write("<a href=javascript:select_all(false)>deselect all</a>");
                    Response.Write("<tr>");
                    Response.Write("<td><span class=lbl>project:</span><br><select name=mass_project id=mass_project></select>");
                    Response.Write("<td><span class=lbl>organization:</span><br><select name=mass_org id=mass_org></select>");
                    Response.Write("<td><span class=lbl>category:</span><br><select name=mass_category id=mass_category></select>");
                    Response.Write("<td><span class=lbl>priority:</span><br><select name=mass_priority id=mass_priority></select>");
                    Response.Write("<td><span class=lbl>assigned to:</span><br><select name=mass_assigned_to id=mass_assigned_to></select>");
                    Response.Write("<td><span class=lbl>status:</span><br><select name=mass_status id=mass_status></select>");
                    Response.Write("<td><span class=lbl>reported by:</span><br><select name=mass_reported_by id=mass_reported_by></select>");
                    Response.Write("<tr><td colspan=5>OR DELETE:&nbsp;<input type=checkbox class=cb name=mass_delete>");
                    Response.Write("<tr><td colspan=5 align=center><input type=submit value='Update/Delete All'>");
                    Response.Write("</table></form><p><script>load_massedit_selects()</script>");
                }
                else
                {
                    // no checkboxes
                    display_bugs(false);
                }


            }
            else
            {
                Response.Write("<p>No ");
                Response.Write(Util.get_setting("PluralBugLabel", "bug"));
                Response.Write("<p>");
            }
        }
    %>

</asp:Content>