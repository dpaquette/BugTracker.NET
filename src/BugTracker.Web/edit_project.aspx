<%@ Page Language="C#" CodeBehind="edit_project.aspx.cs" Inherits="btnet.edit_project" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>


<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <table border="0">
            <tr>
                <td>
                    <a href="projects.aspx">back to projects</a>
                    &nbsp;&nbsp;&nbsp;&nbsp;


                    <% if (id != 0)
                       { %>
                    <a id="permissions_href" runat="server" href="" style='font-weight: bold;'>per user permissions</a>
                    <% } %>

                    <script>

                        var cls = (navigator.userAgent.indexOf("MSIE") > 0) ? "className" : "class";

                        function show_main_settings() {
                            document.getElementById("tab1").style.display = "block";
                            document.getElementById("tab2").style.display = "none";
                            document.getElementById("tab3").style.display = "none";
                            document.getElementById("main_btn").setAttribute(cls, 'tab_btn_pushed');
                            document.getElementById("custom_btn").setAttribute(cls, 'tab_btn');
                        }

                        function show_custom_field_settings() {
                            document.getElementById("tab1").style.display = "none";
                            document.getElementById("tab2").style.display = "block";
                            document.getElementById("tab3").style.display = "none";
                            document.getElementById("main_btn").setAttribute(cls, 'tab_btn');
                            document.getElementById("custom_btn").setAttribute(cls, 'tab_btn_pushed');
                        }




                    </script>
                    <br>
                    <br>
                    <form class="frm" runat="server">

                        <br>
                        <a id="main_btn" class="tab_btn_pushed" href="javascript: show_main_settings()">main settings</a>
                        <a id="custom_btn" class="tab_btn" href="javascript: show_custom_field_settings()">custom fields</a>
                        <br>
                        <br>

                        <div id="tab1" style="display: block;">
                            <table border="0" cellpadding="3">

                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Project Name:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="name" maxlength="30" size="30"></td>
                                    <td runat="server" class="err" id="name_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Active:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="active" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Default Selection in "projects" Dropdown:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="default_selection" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Default User:</td>
                                    <td>
                                        <asp:DropDownList ID="default_user" runat="server">
                                        </asp:DropDownList></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Auto-Assign New <% Response.Write(btnet.Util.get_setting("PluralBugLabel", "bug")); %>  to Default User:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="auto_assign" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <span class="smallnote">For the following, see also user page.  Make sure user's email is supplied.<br>
                                            Also see "NotificationEmailEnabled", "NotificationEmailFrom", "SmtpServer" settings in Web.config.

                                            <br>
                                        </span>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Auto-Subscribe Default User to Notifications:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="auto_subscribe" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <span class="smallnote">The following are used by btnet_pop3.exe and btnet_service_pop3.exe<br>
                                            Also see the btnet_service.exe.config file.

                                            <br>
                                        </span>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Enable Receiving <% Response.Write(btnet.Util.get_setting("PluralBugLabel", "bug")); %> via POP3 (btnet_service.exe):</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="enable_pop3" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Pop3 Username:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="pop3_username" maxlength="50" size="30"></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Pop3 Password:</td>
                                    <td>
                                        <input runat="server" type="password" class="txt" id="pop3_password" maxlength="20" size="20"></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <span class="smallnote">The following is used as the "From" email address when you respond to <% Response.Write(btnet.Util.get_setting("PluralBugLabel", "bug")); %> generated by emails
                                        </span>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">From Email Address:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="pop3_email_from" maxlength="50" size="30"></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Description:</td>
                                    <td>
                                        <textarea runat="server" class="txt" id="desc" rows="5" cols="40"></textarea></td>
                                    <td runat="server" class="err" id="desc_err">&nbsp;</td>
                                </tr>


                            </table>
                        </div>
                        <div id="tab2" style="display: none;">
                            <table border="0" cellpadding="3">

                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <b class="smallnote" style="font-size: 11pt;">Custom fields for this project only</b>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="3">&nbsp;
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <span class="smallnote">Use the following if you want to have a custom field for this project only.

                                            <br>
                                            1. Check the box to enable the field.

                                            <br>
                                            2. Fill in the label.

                                            <br>
                                            3. Create a list of values.  One value per line.

                                            <br>
                                            No individiual value should be longer than the length of your custom field.

                                            <br>
                                            Don't use commas, &gt;, &lt;, or quotes in the list of values.

                                            <br>
                                            Line breaks for your readability are ok.

                                            <br>
                                            Here are some examples:

                                            <br>
                                            "1.0|1.1|1.2"

                                            <br>
                                            "red|blue|green"

                                            <br>
                                            It's ok to have one of the values be blank:<br>
                                            "|red|blue|green"
                                        </span>
                                    </td>
                                </tr>


                                <tr>
                                    <td colspan="3">
                                        <hr>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Enable Custom Dropdown 1:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="enable_custom_dropdown1" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Custom Dropdown Label 1:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="custom_dropdown_label1" maxlength="30" size="30"></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Custom Dropdown Values 1:</td>
                                    <td>
                                        <textarea cols="40" rows="2" runat="server" type="text" class="txt" id="custom_dropdown_values1"></textarea></td>
                                    <td runat="server" class="err" id="custom_dropdown_values1_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <hr>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Enable Custom Dropdown 2:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="enable_custom_dropdown2" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Custom Dropdown Label 2:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="custom_dropdown_label2" maxlength="30" size="30"></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Custom Dropdown Values 2:</td>
                                    <td>
                                        <textarea cols="40" rows="2" runat="server" type="text" class="txt" id="custom_dropdown_values2"></textarea></td>
                                    <td runat="server" class="err" id="custom_dropdown_values2_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <hr>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="lbl">Enable Custom Dropdown 3:</td>
                                    <td>
                                        <asp:CheckBox runat="server" class="cb" ID="enable_custom_dropdown3" /></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Custom Dropdown Label 3:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="custom_dropdown_label3" maxlength="30" size="30"></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr>
                                    <td class="lbl">Custom Dropdown Values 3:</td>
                                    <td>
                                        <textarea cols="40" rows="2" runat="server" type="text" class="txt" id="custom_dropdown_values3"></textarea></td>
                                    <td runat="server" class="err" id="custom_dropdown_values3_err">&nbsp;</td>
                                </tr>

                            </table>
                        </div>

                        <table border="0" width="97%">

                            <tr>
                                <td>&nbsp;
                                </td>
                            </tr>

                            <tr>
                                <td align="left">
                                    <span runat="server" class="err" id="msg">&nbsp;
                                    </span>
                                </td>
                            </tr>

                            <tr>
                                <td align="center">
                                    <input runat="server" class="btn" type="submit" id="sub" value="Create or Edit" />
                                </td>
                            </tr>


                        </table>
                    </form>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>


