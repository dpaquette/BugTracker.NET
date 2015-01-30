<%@ Page Language="C#" CodeBehind="edit_user.aspx.cs" Inherits="btnet.edit_user" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="headerScripts" runat="server">
    <script>

        var cls = (navigator.userAgent.indexOf("MSIE") > 0) ? "className" : "class";

        function show_main_settings() {
            document.getElementById("tab1").style.display = "block";
            document.getElementById("tab2").style.display = "none";
            document.getElementById("tab3").style.display = "none";
            document.getElementById("main_btn").setAttribute(cls, 'tab_btn_pushed');
            document.getElementById("notifications_btn").setAttribute(cls, 'tab_btn');
            document.getElementById("permissions_btn").setAttribute(cls, 'tab_btn');
        }

        function show_notification_settings() {
            document.getElementById("tab1").style.display = "none";
            document.getElementById("tab2").style.display = "block";
            document.getElementById("tab3").style.display = "none";
            document.getElementById("main_btn").setAttribute(cls, 'tab_btn');
            document.getElementById("notifications_btn").setAttribute(cls, 'tab_btn_pushed');
            document.getElementById("permissions_btn").setAttribute(cls, 'tab_btn');
        }

        function show_permissions_settings() {
            document.getElementById("tab1").style.display = "none";
            document.getElementById("tab2").style.display = "none";
            document.getElementById("tab3").style.display = "block";
            document.getElementById("main_btn").setAttribute(cls, 'tab_btn');
            document.getElementById("notifications_btn").setAttribute(cls, 'tab_btn');
            document.getElementById("permissions_btn").setAttribute(cls, 'tab_btn_pushed');
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <table border="0">
            <tr>
                <td>

                    <a href="users.aspx">back to users</a>
                    &nbsp;&nbsp;&nbsp;&nbsp;


                    <form class="frm" runat="server">

                        <br>
                        <a id="main_btn" class="tab_btn_pushed" href="javascript: show_main_settings()">main settings</a>
                        <a id="notifications_btn" class="tab_btn" href="javascript: show_notification_settings()">email notification settings</a>
                        <a id="permissions_btn" class="tab_btn" href="javascript: show_permissions_settings()">permissions</a>
                        <br>
                        <br>

                        <div id="tab1" style="display: block;">
                            <table border="0" cellpadding="3">

                                <tr>
                                    <td class="lbl">Username:</td>
                                    <td colspan="2">
                                        <input runat="server" type="text" class="txt" id="username" maxlength="20" size="20"></td>
                                </tr>

                                <tr>
                                    <td>&nbsp;</td>
                                    <td colspan="2" runat="server" class="err" id="username_err">&nbsp;</td>
                                </tr>


                                <tr>
                                    <td class="lbl">Password:</td>
                                    <td colspan="2">
                                        <input runat="server" autocomplete="off" type="password" class="txt" id="pw" maxlength="20" size="20"></td>
                                </tr>

                                <tr>
                                    <td>&nbsp;</td>
                                    <td colspan="2" runat="server" class="err" id="pw_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Confirm Password:</td>
                                    <td colspan="2">
                                        <input runat="server" autocomplete="off" type="password" class="txt" id="confirm_pw" maxlength="20" size="20"></td>
                                </tr>

                                <tr>
                                    <td>&nbsp;</td>
                                    <td colspan="2" runat="server" class="err" id="confirm_pw_err">&nbsp;</td>
                                </tr>


                                <tr>
                                    <td class="lbl">Org:</td>
                                    <td colspan="2">
                                        <asp:DropDownList ID="org" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>

                                <tr>
                                    <td>&nbsp;</td>
                                    <td colspan="2" runat="server" class="err" id="org_err">&nbsp;</td>
                                </tr>


                                <tr>
                                    <td class="lbl">First Name:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="firstname" maxlength="20" size="20"></td>
                                    <td runat="server" class="err" id="firstname_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Last Name:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="lastname" maxlength="20" size="20"></td>
                                    <td runat="server" class="err" id="lastname_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Active:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="active" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl" id="admin_label" runat="server">Admin:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="admin" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl"><% Response.Write(btnet.Util.get_setting("PluralBugLabel", "Bugs")); %> Per Page:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="bugs_per_page" maxlength="3" size="3"></td>
                                    <td runat="server" class="err" id="bugs_per_page_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Enable <% Response.Write(btnet.Util.get_setting("SingularBugLabel", "bug")); %>list popups:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="enable_popups" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Edit text using colors and fonts:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="use_fckeditor" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <div class="smallnote" style="width: 400px;">
                                            Default <% Response.Write(btnet.Util.get_setting("SingularBugLabel", "bug")); %> Query is what you see when you click on the "<% Response.Write(btnet.Util.get_setting("PluralBugLabel", "bug")); %>" link
                                        </div>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Default <% Response.Write(btnet.Util.get_setting("SingularBugLabel", "bug")); %> Query:</td>
                                    <td>
                                        <asp:DropDownList ID="query" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                    <td>&nbsp;</td>
                                </tr>


                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Email:</td>
                                    <td colspan="2">
                                        <input runat="server" type="text" class="txt" id="email" maxlength="60" size="60"></td>
                                </tr>

                                <tr>
                                    <td>&nbsp;</td>
                                    <td colspan="2" runat="server" class="err" id="email_err">&nbsp;</td>
                                </tr>



                                <tr>
                                    <td class="lbl" valign="top">Outgoing Email Signature:</td>
                                    <td>
                                        <textarea class="txt" id="signature" rows="2" cols="50" runat="server"></textarea></td>
                                    <td runat="server" class="err" id="signature_err">&nbsp;</td>
                                </tr>

                            </table>
                        </div>

                        <div id="tab2" style="display: none;">
                            <table border="0" cellpadding="3">

                                <tr>
                                    <td colspan="3">
                                        <span class="smallnote">
                                            <br>
                                            <br>
                                            <div style="width: 400px;">
                                                ADMIN - SEE "NotificationEmailEnabled", "NotificationEmailFrom", "SmtpServer" settings in Web.config.<br>
                                                <br>
                                                To receive email notifications when items are added or changed, fill in your email address, enable notifications, and then select "Auto-subscribe to all items" or the other options.<br>
                                                <br>
                                            </div></td>
                                </tr>

                                <tr>
                                    <td class="lbl">Enable notifications:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="enable_notifications" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <br>
                                        <br>
                                        <div class="smallnote" style="width: 400px;">
                                            You can AUTOMATICALLY subscribe to receive notifications to items by selecting either "Auto-subscribe to all items" or by selecting the other options.<br>
                                        </div>
                                    </td>
                                </tr>


                                <tr>
                                    <td class="lbl">Auto-subscribe to all items:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="auto_subscribe" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl" id="project_auto_subscribe_label" runat="server" nowrap>Auto-subscribe per project:</td>
                                    <td>
                                        <span class="smallnote">Hold down Ctrl key to select multiple items.</span>
                                        <br>
                                        <asp:ListBox ID="project_auto_subscribe" runat="server" SelectionMode="Multiple" Rows="4"></asp:ListBox>
                                    </td>
                                    <td>&nbsp;</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Auto-subscribe to all items ASSIGNED TO you:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="auto_subscribe_own" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Auto-subscribe to all items REPORTED BY you:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="auto_subscribe_reported" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Apply subscription changes retroactively:</td>
                                    <td colspan="2">
                                        <asp:CheckBox runat="server" class="cb" ID="retroactive" />&nbsp;&nbsp;
	<span class="smallnote">Delete old subscriptions and create new ones, according to above settings.</span></td>
                                    <td>&nbsp;</td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <br>
                                        <br>
                                        <div class="smallnote" style="width: 400px;">
                                            You can REDUCE or INCREASE the amount of email you receive by selecting the following.<br>
                                        </div>
                                    </td>
                                </tr>

                                <!-- MAW -- 2006/01/27 -- Added new notification controls -->
                                <tr>
                                    <td class="lbl">Notifications for subscribed <% Response.Write(btnet.Util.get_setting("PluralBugLabel", "bugs")); %> reported by me:</td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="reported_notifications">
                                            <asp:ListItem Value="0" Text="no notifications" />
                                            <asp:ListItem Value="1" Text="when created" />
                                            <asp:ListItem Value="2" Text="when status changes" />
                                            <asp:ListItem Value="3" Text="when status or assigned-to changes" />
                                            <asp:ListItem Value="4" Text="when anything changes" />
                                        </asp:DropDownList></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Notifications for subscribed <% Response.Write(btnet.Util.get_setting("PluralBugLabel", "bugs")); %> assigned to me:</td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="assigned_notifications">
                                            <asp:ListItem Value="0" Text="no notifications" />
                                            <asp:ListItem Value="1" Text="when created" />
                                            <asp:ListItem Value="2" Text="when status changes" />
                                            <asp:ListItem Value="3" Text="when status or assigned-to changes" />
                                            <asp:ListItem Value="4" Text="when anything changes" />
                                        </asp:DropDownList></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Notifications for all other subscribed <% Response.Write(btnet.Util.get_setting("PluralBugLabel", "bugs")); %>:</td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="subscribed_notifications">
                                            <asp:ListItem Value="0" Text="no notifications" />
                                            <asp:ListItem Value="1" Text="when created" />
                                            <asp:ListItem Value="2" Text="when status changes" />
                                            <asp:ListItem Value="3" Text="when status or assigned-to changes" />
                                            <asp:ListItem Value="4" Text="when anything changes" />
                                        </asp:DropDownList></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Send notifications even for items you add or change:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="send_to_self" /></td>
                                    <td>&nbsp</td>
                                </tr>


                            </table>

                        </div>
                        <div id="tab3" style="display: none;">
                            <table border="0" cellpadding="3">

                                <tr>
                                    <td colspan="3">
                                    &nbsp;


	<tr>
        <td colspan="2" class="lbl">Force user to add new <% Response.Write(btnet.Util.get_setting("PluralBugLabel", "bugs")); %> to this project:&nbsp;&nbsp;
	<asp:DropDownList ID="forced_project" runat="server">
    </asp:DropDownList>
        </td>
    </tr>

                                <tr>
                                    <td colspan="3">
                                        <br>
                                        <br>
                                        <div class="smallnote" style="width: 400px;">
                                            If you have only given view permissions to this user, set the forced project to any of the view-only projects.<br>
                                        </div>
                                    </td>
                                </tr>


                                <tr>
                                    <td colspan="3">
                                    &nbsp;

	<tr>
        <td colspan="2">
            <asp:DataGrid ID="MyDataGrid" runat="server" BorderColor="black" CssClass="datat" CellPadding="3" AutoGenerateColumns="false">
                <HeaderStyle CssClass="datah"></HeaderStyle>
                <ItemStyle CssClass="datad"></ItemStyle>
                <Columns>


                    <asp:BoundColumn HeaderText="Project" DataField="pj_name" />

                    <asp:BoundColumn HeaderText="Project" DataField="pj_id" Visible="False" />

                    <asp:TemplateColumn HeaderText="Permissions">
                        <ItemTemplate>
                            <asp:RadioButton GroupName="permissions" Text="none" value="0" ID="none" runat="server"
                                Checked='<%# ((int)(( System.Data.DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 0 ) %>' />

                            <asp:RadioButton GroupName="permissions" Text="view only" value="1" ID="readonly" runat="server"
                                Checked='<%# ((int)(( System.Data.DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 1 ) %>' />

                            <asp:RadioButton GroupName="permissions" Text="report (add and comment only)" value="3" ID="reporter" runat="server"
                                Checked='<%# ((int)(( System.Data.DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 3 ) %>' />

                            <asp:RadioButton GroupName="permissions" Text="all (add and edit)" value="2" ID="edit" runat="server"
                                Checked='<%# ((int)(( System.Data.DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 2 ) %>' />

                        </ItemTemplate>
                    </asp:TemplateColumn>


                </Columns>
            </asp:DataGrid>
        <tr>
            <td colspan="3">
            &nbsp;

	<tr>
        <td colspan="2" class="lbl" id="project_admin_label" runat="server" nowrap>Allowed to add/delete other users for the following projects:

		<br>
            <span id="project_admin_help" runat="server" class="smallnote">Hold down Ctrl key to select multiple items.</span>
            <br>
            <br>
            <asp:ListBox ID="project_admin" runat="server" SelectionMode="Multiple" Rows="4"></asp:ListBox>
        </td>
        <td>&nbsp;</td>
    </tr>
                                <tr>
                                    <td colspan="3">
                                    &nbsp;

                            </table>
                        </div>


                        <table border="0">


                            <tr>
                                <td colspan="3" align="left">
                                    <span runat="server" class="err" id="msg">&nbsp;</span>
                                </td>
                            </tr>

                            <tr>
                                <td width="300px">
                                &nbsp;
	<td align="center">
        <input runat="server" class="btn" type="submit" id="sub" value="Create or Edit">
        <td>&nbsp</td>
    </td>
                            </tr>

                        </table>
                    </form>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>


