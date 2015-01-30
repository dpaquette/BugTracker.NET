<%@ Page Language="C#" CodeBehind="edit_org.aspx.cs" Inherits="btnet.edit_org" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <form class="frm" runat="server">
        <div class="align">

            <table border="0">
                <tr>
                    <td>
                        <a href="orgs.aspx">back to organizations</a>

                        <table border="0">

                            <tr>
                                <td class="lbl">Organization Name:</td>
                                <td>
                                    <input runat="server" type="text" class="txt" id="og_name" maxlength="30" size="30"></td>
                                <td runat="server" class="err" id="name_err">&nbsp;</td>
                            </tr>

                            <tr>
                                <td class="lbl">
                                Domain (like, "example.com"):
	<td>
        <input runat="server" type="text" class="txt" id="og_domain" maxlength="80" size="30"></td>
                                <td runat="server" class="err" id="domain_err">&nbsp;</td>
                            </tr>
                        <td class="lbl">Active:</td>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="og_active" /></td>
                    <td>&nbsp</td>
                </tr>

                <tr>
                    <td colspan="3">
                        <br>
                        <br>
                        <div class="smallnote" style="width: 400px;">
                            Can members of this organization view/edit bugs associated with other organizations?<br>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td class="lbl" colspan="3">Permission level for bugs associated with other (or no) organizations<br>
                        <asp:RadioButtonList RepeatDirection="Horizontal" ID="other_orgs" runat="server">
                            <asp:ListItem Text="none" Value="0" ID="other_orgs0" runat="server" />
                            <asp:ListItem Text="view only" Value="1" ID="other_orgs1" runat="server" />
                            <asp:ListItem Text="edit" Value="2" ID="other_orgs2" runat="server" />
                        </asp:RadioButtonList>
                    <tr>
                        <td colspan="3">&nbsp;
                        </td>
                    </tr>

            </table>
            <table border="0">


                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_search" />
                    <td class="lbl">Can search</td>
                    <td>&nbsp</td>
                </tr>

                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="external_user" /></td>
                    <td class="lbl">External users&nbsp;&nbsp; <span class="smallnote">(External users cannot view posts marked "Visible for internal users only")</span></td>
                    <td>&nbsp</td>
                </tr>

                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_only_see_own_reported" /></td>
                    <td class="lbl">Can see only own reported</td>
                    <td>&nbsp</td>
                </tr>

                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_be_assigned_to" /></td>
                    <td class="lbl">Members of this org appear in "assigned to" dropdown in edit bug page</td>
                    <td>&nbsp</td>
                </tr>

                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="non_admins_can_use" /></td>
                    <td class="lbl">Non-admin with permission to add users can add users to this org</td>
                    <td>&nbsp</td>
                </tr>

            </table>

            <table border="0">


                <tr>
                    <td colspan="3">
                        <br>
                        <br>
                        <div class="smallnote" style="width: 400px;">
                            Field level permissions<br>
                        </div>
                    </td>
                </tr>


                <tr>
                    <td colspan="3">&nbsp;
                    </td>
                </tr>

                <tr>
                    <td>
                    "Project" field permission
		<td colspan="2">
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="project_field" runat="server">
                <asp:ListItem Text="none" Value="0" ID="project0" runat="server" />
                <asp:ListItem Text="view only" Value="1" ID="project1" runat="server" />
                <asp:ListItem Text="edit" Value="2" ID="project2" runat="server" />
            </asp:RadioButtonList>
                    <tr>
                        <td>
                        "Organization" field permission
		<td colspan="2">
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="org_field" runat="server">
                <asp:ListItem Text="none" Value="0" ID="org0" runat="server" />
                <asp:ListItem Text="view only" Value="1" ID="org1" runat="server" />
                <asp:ListItem Text="edit" Value="2" ID="org2" runat="server" />
            </asp:RadioButtonList>
                        <tr>
                            <td>
                            "Category" field permission
		<td colspan="2">
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="category_field" runat="server">
                <asp:ListItem Text="none" Value="0" ID="category0" runat="server" />
                <asp:ListItem Text="view only" Value="1" ID="category1" runat="server" />
                <asp:ListItem Text="edit" Value="2" ID="category2" runat="server" />
            </asp:RadioButtonList>
                            <tr>
                                <td>
                                "Priority" field permission
		<td colspan="2">
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="priority_field" runat="server">
                <asp:ListItem Text="none" Value="0" ID="priority0" runat="server" />
                <asp:ListItem Text="view only" Value="1" ID="priority1" runat="server" />
                <asp:ListItem Text="edit" Value="2" ID="priority2" runat="server" />
            </asp:RadioButtonList>
                                <tr>
                                    <td>
                                    "Status" field permission
		<td colspan="2">
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="status_field" runat="server">
                <asp:ListItem Text="none" Value="0" ID="status0" runat="server" />
                <asp:ListItem Text="view only" Value="1" ID="status1" runat="server" />
                <asp:ListItem Text="edit" Value="2" ID="status2" runat="server" />
            </asp:RadioButtonList>
                                    <tr>
                                        <td>
                                        "Assigned To" field permission
		<td colspan="2">
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="assigned_to_field" runat="server">
                <asp:ListItem Text="none" Value="0" ID="assigned_to0" runat="server" />
                <asp:ListItem Text="view only" Value="1" ID="assigned_to1" runat="server" />
                <asp:ListItem Text="edit" Value="2" ID="assigned_to2" runat="server" />
            </asp:RadioButtonList>
                                        <tr>
                                            <td>
                                            User Defined Attribute field permission
		<td colspan="2">
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="udf_field" runat="server">
                <asp:ListItem Text="none" Value="0" ID="udf0" runat="server" />
                <asp:ListItem Text="view only" Value="1" ID="udf1" runat="server" />
                <asp:ListItem Text="edit" Value="2" ID="udf2" runat="server" />
            </asp:RadioButtonList>
                                            <tr>
                                                <td>
                                                "Tags" field permission
		<td colspan="2">
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="tags_field" runat="server">
                <asp:ListItem Text="none" Value="0" ID="tags0" runat="server" />
                <asp:ListItem Text="view only" Value="1" ID="tags1" runat="server" />
                <asp:ListItem Text="edit" Value="2" ID="tags2" runat="server" />
            </asp:RadioButtonList>
            </table>
            <table border="0">

                <tr>
                    <td colspan="3">
                        <br>
                        <br>
                        <div class="smallnote" style="width: 400px;">
                            Use the following settings to control permissions for non-admins.<br>
                            Admins have all permissions regardless of these settings.<br>
                        </div>
                    </td>
                </tr>


                <tr>
                    <td colspan="3">&nbsp;
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_edit_sql" /></td>
                    <td class="lbl">Can edit sql and create/edit queries for everybody</td>
                    <td>&nbsp</td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_delete_bug" /></td>
                    <td class="lbl">Can delete bugs</td>
                    <td>&nbsp</td>
                </tr>

                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_edit_and_delete_posts" /></td>
                    <td class="lbl">Can edit and delete comments and attachments</td>
                    <td>&nbsp</td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_merge_bugs" /></td>
                    <td class="lbl">Can merge two bugs into one</td>
                    <td>&nbsp</td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_mass_edit_bugs" /></td>
                    <td class="lbl">Can mass edit bugs on search page</td>
                    <td>&nbsp</td>
                </tr>

                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_use_reports" /></td>
                    <td class="lbl">Can use reports</td>
                    <td>&nbsp</td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_edit_reports" /></td>
                    <td class="lbl">Can create/edit reports</td>
                    <td>&nbsp</td>
                </tr>

                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_view_tasks" /></td>
                    <td class="lbl">Can view tasks/time</td>
                    <td>&nbsp</td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_edit_tasks" /></td>
                    <td class="lbl">Can edit tasks/time</td>
                    <td>&nbsp</td>
                </tr>

                <tr>
                    <td>
                        <asp:CheckBox runat="server" class="cb" ID="can_assign_to_internal_users" /></td>
                    <td class="lbl">Can assign to internal users (even if external org)</td>
                    <td>&nbsp</td>
                </tr>



            </table>

            <table border="0">


                <tr>
                    <td colspan="2" align="left">
                        <span runat="server" class="err" id="msg">&nbsp;</span>
                    </td>
                </tr>

                <tr>
                    <td colspan="2" align="center">
                        <input runat="server" class="btn" type="submit" id="sub" value="Create or Edit">
                        <td>&nbsp</td>
                    </td>
                </tr>
                </td>
            </tr>

            </table>


            </td>
        </tr>
        </table>

        </div>
    </form>
</asp:Content>

