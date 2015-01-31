<%@ Page Language="C#" CodeBehind="edit_bug.aspx.cs" Inherits="btnet.edit_bug" ValidateRequest="false" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>
<%@ Import Namespace="btnet.Security" %>

<asp:Content ContentPlaceHolderID="headerScripts" runat="server">
    <script type="text/javascript" src="edit_bug.js"></script>
    <%  if (User.Identity.GetUseFCKEditor())
        { %>
    <script type="text/javascript" src="scripts/ckeditor/ckeditor.js"></script>
    <% } %>
    <script>
        var this_bugid = <%=Convert.ToString(id)%>;

        $(document).ready(do_doc_ready);

        function do_doc_ready() {
            date_format = '<%=btnet.Util.get_setting("DatepickerDateFormat", "yy-mm-dd")%>';
            $(".date").datepicker({ dateFormat: date_format, duration: 'fast' });
            $(".date").change(mark_dirty);
            $(".warn").click(warn_if_dirty);
            <% if (User.Identity.GetUseFCKEditor())
               {
                   Response.Write("CKEDITOR.replace( 'comment' )");
               }	    
            %>
            on_body_load();
            $(document).on("unload", "body", function () { on_body_unload(); });

        }
    </script>
    <link rel="StyleSheet" href="custom/btnet_edit_bug.css" type="text/css">
</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server" ClientIDMode="Static">

    <div class="container">

        <div class="row">


            <div id="edit_bug_menu" class="custom-collapse <%=id > 0 ? "col-sm-12 col-md-3 col-lg-2" : "" %>">
                <div class="btn btn-default text-left visible-xs-block visible-sm-block"  style="text-align: left; width: 175px" data-toggle="collapse" data-target="#side_menu_collapse">
                    <i class="glyphicon glyphicon-tasks"></i><span> Bug Tools </span> <i class="glyphicon glyphicon-chevron-down pull-right"></i>
                </div>
                <ul id="side_menu_collapse" class="collapse">
                    <%  if (User.Identity.GetCanAddBugs() && id > 0)
                        { %>
                    <li>
                        <a href="edit_bug.aspx?id=0" class="btn btn-primary warn"><span class="glyphicon glyphicon-plus"></span>&nbsp;New <%=btnet.Util.get_setting("SingularBugLabel", "bug")%></a>
                    </li>
                    <% } %>

                    <li class="dropdown-toggle" id="clone" runat="server" />
                    <li class="dropdown-toggle" id="print" runat="server" />
                    <li class="dropdown-toggle" id="merge_bug" runat="server" />
                    <li class="dropdown-toggle" id="delete_bug" runat="server" />
                    <li class="dropdown-toggle" id="svn_revisions" runat="server" />
                    <li class="dropdown-toggle" id="git_commits" runat="server" />
                    <li class="dropdown-toggle" id="hg_revisions" runat="server" />
                    <li class="dropdown-toggle" id="subscribers" runat="server" />
                    <li class="dropdown-toggle" id="subscriptions" runat="server" />
                    <li class="dropdown-toggle" id="relationships" runat="server" />
                    <li class="dropdown-toggle" id="tasks" runat="server" />
                    <li class="dropdown-toggle" id="send_email" runat="server" />
                    <li class="dropdown-toggle" id="attachment" runat="server" />
                    <li class="dropdown-toggle" id="custom" runat="server" />
                </ul>
            </div>

            <div id="bugform_div" class="col-sm-12 col-md-9 col-lg-10">


                <form runat="server" class="form-horizontal">
                    <fieldset>

                        <!-- Form Name -->
                        <legend><%=btnet.Util.get_setting("SingularBugLabel", "bug")%>  <span runat="server" class="bugid" id="bugid"></span>
                            <div class="pull-right" id="prev_next" runat="server" style="font-size: 12px; display: inline-block">
                            </div>
                        </legend>
                        <% if (id == 0 || permission_level == btnet.Security.PermissionLevel.All)
                           { %>


                        <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10">
                                Presets <a title="Use previously saved settings for project, category, priority, etc..."
                                    href="javascript:get_presets()">use</a>&nbsp;/&nbsp;
                                            <a title="Save current settings for project, category, priority, etc., so that you can reuse later."
                                                href="javascript:set_presets()">save</a>

                            </div>
                        </div>
                        <% } %>

                        <% if (btnet.Util.get_setting("DisplayAnotherButtonInEditBugPage", "0") == "1")
                           { %>
                        <div>
                            <span runat="server" class="err" id="custom_field_msg2">&nbsp;</span>
                            <span runat="server" class="err" id="msg2">&nbsp;</span>
                        </div>
                        <div style="text-align: center;">
                            <input
                                runat="server"
                                class="btn"
                                type="submit"
                                id="submit_button2"
                                onclick="on_user_hit_submit()"
                                value="Update" />
                        </div>
                        <% } %>

                        <div class="form-group">
                            <label class="col-sm-2 control-label" for="short_desc">Description</label>
                            <div class="col-sm-10">

                                <span class="short_desc_static" id="static_short_desc" runat="server" style='display: none;'></span>
                                <input runat="server" type="text" class="form-control" id="short_desc" maxlength="200"
                                    onkeydown="count_chars('short_desc',200)" onkeyup="count_chars('short_desc',200)" />

                                <span runat="server" class="err" id="short_desc_err"></span>

                                <p class="help-block" id="short_desc_cnt">&nbsp;</p>
                                <span runat="server" id="reported_by"></span>
                            </div>
                        </div>

                        <div class="form-group">
                            <label runat="server" id="tags_label" class="col-sm-2 control-label" for="tags">Tags</label>
                            <div class="col-sm-10">
                                <span class="stat" id="static_tags" runat="server"></span>
                                <input runat="server" type="text" class="form-control" id="tags" size="70" maxlength="80" onkeydown="mark_dirty()" onkeyup="mark_dirty()" />
                                <span id="tags_link" runat="server">&nbsp;&nbsp;<a href='javascript:show_tags()'>tags</a></span>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-2 control-label" for="project" id="project_label" runat="server">Project</label>
                            <div class="col-sm-10">
                                <span class="stat" id="static_project" runat="server"></span>
                                <asp:DropDownList ID="project" runat="server" class="form-control"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="org" class="col-sm-2 control-label" id="org_label" runat="server">Organization</label>
                            <div class="col-sm-10">
                                <span class="stat" id="static_org" runat="server"></span>
                                <asp:DropDownList ID="org" class="form-control" runat="server"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="category" class="control-label col-sm-2" id="category_label" runat="server">Category</label>
                            <div class="col-sm-10">
                                <span class="stat" id="static_category" runat="server"></span>
                                <asp:DropDownList ID="category" class="form-control" runat="server"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="priority" class="control-label col-sm-2" id="priority_label" runat="server">Priority</label>
                            <div class="col-sm-10">
                                <span class="stat" id="static_priority" runat="server"></span>
                                <asp:DropDownList class="form-control" ID="priority" runat="server"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <label for="assigned_to" class="control-label col-sm-2" id="assigned_to_label" runat="server">Assigned to</label>
                            <div class="col-sm-10">
                                <span class="stat" id="static_assigned_to" runat="server"></span>
                                <asp:DropDownList class="form-control" ID="assigned_to" runat="server"></asp:DropDownList>
                                <span runat="server" class="err" id="assigned_to_err"></span>

                            </div>
                        </div>

                        <div class="form-group">
                            <label for="status" class="control-label col-sm-2" id="status_label" runat="server">Status</label>
                            <div class="col-sm-10">
                                <span class="stat" id="static_status" runat="server"></span>
                                <asp:DropDownList class="form-control" ID="status" runat="server"></asp:DropDownList>

                            </div>
                        </div>

                        <% if (btnet.Util.get_setting("ShowUserDefinedBugAttribute", "1") == "1")
                           { %>
                        <div class="form-group">
                            <label for="udf" class="control-label col-sm-2" id="udf_label" runat="server">
                                <%=btnet.Util.get_setting("UserDefinedBugAttributeName", "YOUR ATTRIBUTE")%>:&nbsp;</label>
                            <div class="col-sm-10">
                                <span class="stat" id="static_udf" runat="server"></span>
                                <asp:DropDownList class="form-control" ID="udf" runat="server">
                                </asp:DropDownList>

                            </div>
                        </div>
                        <% } %>

                        <div class="form-group">
                            <label for="comment" class="control-label col-sm-2" id="comment_label" runat="server">Comment</label>
                            <div class="col-sm-10">
                                <textarea id="comment" rows="10" cols="100" runat="server" class="form-control" onkeydown="mark_dirty()" onkeyup="mark_dirty()"></textarea>
                                <p class="help-block">
                                    <% 
                                        if (permission_level != PermissionLevel.ReadOnly)
                                        {
                                            Response.Write("Entering \""
                                                + btnet.Util.get_setting("BugLinkMarker", "bugid#")
                                                + "999\" in comment creates link to id 999");
                                        } 
                                    %>
                                </p>

                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10">
                                <label class="checkbox-inline">

                                    <asp:CheckBox runat="server" ID="internal_only" />
                                    <span runat="server" id="internal_only_label">Comment visible to internal users only</span>

                                </label>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10">
                                <div runat="server" class="err" id="custom_field_msg"></div>
                                <div runat="server" class="err" id="custom_validation_err_msg"></div>
                                <div runat="server" class="err" id="msg"></div>

                                <input runat="server" class="btn btn-primary" type="submit" id="submit_button"
                                    onclick="on_user_hit_submit()"
                                    value="Update" />

                            </div>
                        </div>

                        <table border="0" cellpadding="0" cellspacing="4">
                            <%
                                                                                
                                display_project_specific_custom_fields();    
                            %>
                        </table>


                        <input type="hidden" id="new_id" runat="server" value="0" />
                        <input type="hidden" id="prev_short_desc" runat="server" />
                        <input type="hidden" id="prev_tags" runat="server" />
                        <input type="hidden" id="prev_project" runat="server" />
                        <input type="hidden" id="prev_project_name" runat="server" />
                        <input type="hidden" id="prev_org" runat="server" />
                        <input type="hidden" id="prev_org_name" runat="server" />
                        <input type="hidden" id="prev_category" runat="server" />
                        <input type="hidden" id="prev_priority" runat="server" />
                        <input type="hidden" id="prev_assigned_to" runat="server" />
                        <input type="hidden" id="prev_assigned_to_username" runat="server" />
                        <input type="hidden" id="prev_status" runat="server" />
                        <input type="hidden" id="prev_udf" runat="server" />
                        <input type="hidden" id="prev_pcd1" runat="server" />
                        <input type="hidden" id="prev_pcd2" runat="server" />
                        <input type="hidden" id="prev_pcd3" runat="server" />
                        <input type="hidden" id="snapshot_timestamp" runat="server" />
                        <input type="hidden" id="clone_ignore_bugid" runat="server" value="0" />
                        <input type="hidden" id="user_hit_submit" name="user_hit_submit" value="0" />

                        <%  
                            if (id != 0)
                            {
                                display_bug_relationships();
                            }
                        %>
                    </fieldset>
                </form>
            </div>
            <!-- bug form div -->
        </div>

        <br>
        <span id="toggle_images" runat="server"></span>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <span id="toggle_history" runat="server"></span>
        <br>
        <br>

        <div id="posts">

            <%
                // COMMENTS
                if (id != 0)
                {
                    btnet.PrintBug.write_posts(
                        ds_posts,
                        Response,
                        id,
                        permission_level,
                        true, // write links
                        images_inline,
                        history_inline,
                        true, User.Identity);
                }

            %>
        </div>
        <!-- posts -->
    </div>
</asp:Content>
