<%@ Page Language="C#" CodeBehind="edit_task.aspx.cs" Inherits="btnet.edit_task" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="headerScripts">
    <script>
$(document).ready(do_doc_ready);

function do_doc_ready()
{
	date_format = '<% Response.Write(btnet.Util.get_setting("DatepickerDateFormat", "yy-mm-dd")); %>'
	$(".date").datepicker({dateFormat: date_format, duration: 'fast'})
}

    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <table border="0">
            <tr>
                <td>
                    <a href="tasks.aspx?bugid=<% Response.Write(Convert.ToString(bugid)); %>">back to tasks</a>
                    <p>
                        <form class="frm" runat="server">
                            <table border="0">

                                <tr runat="server" id="bugid_tr">
                                    <td class="lbl" id="bugid_label"></td>
                                    <td class="lbl" id="bugid_static"></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr runat="server" id="tsk_id_tr">
                                    <td class="lbl" id="tsk_id_label">Task ID:</td>
                                    <td class="lbl" id="tsk_id_static"></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr runat="server" id="desc_tr">
                                    <td class="lbl" id="desc_label">Description:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="desc" maxlength="200" size="100"></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr runat="server" id="assigned_to_tr">
                                    <td class="lbl" id="assigned_to_label">Assigned to:</td>
                                    <td>
                                        <asp:DropDownList ID="assigned_to" runat="server"></asp:DropDownList></td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr runat="server" id="planned_start_date_tr">
                                    <td class="lbl" id="planned_start_date_lbl">Planned start date:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt date" id="planned_start_date" maxlength="10" size="10">
                                        <span id="planned_start_hour_label">&nbsp;hour:</span>
                                        <asp:DropDownList ID="planned_start_hour" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="False" />
                                            <asp:ListItem Text="01" Value="01" Selected="False" />
                                            <asp:ListItem Text="02" Value="02" Selected="False" />
                                            <asp:ListItem Text="03" Value="03" Selected="False" />
                                            <asp:ListItem Text="04" Value="04" Selected="False" />
                                            <asp:ListItem Text="05" Value="05" Selected="False" />
                                            <asp:ListItem Text="06" Value="06" Selected="False" />
                                            <asp:ListItem Text="07" Value="07" Selected="False" />
                                            <asp:ListItem Text="08" Value="08" Selected="False" />
                                            <asp:ListItem Text="09" Value="09" Selected="False" />
                                            <asp:ListItem Text="10" Value="10" Selected="False" />
                                            <asp:ListItem Text="11" Value="11" Selected="False" />
                                            <asp:ListItem Text="12" Value="12" Selected="False" />
                                            <asp:ListItem Text="13" Value="13" Selected="False" />
                                            <asp:ListItem Text="14" Value="14" Selected="False" />
                                            <asp:ListItem Text="15" Value="15" Selected="False" />
                                            <asp:ListItem Text="16" Value="16" Selected="False" />
                                            <asp:ListItem Text="17" Value="17" Selected="False" />
                                            <asp:ListItem Text="18" Value="18" Selected="False" />
                                            <asp:ListItem Text="19" Value="19" Selected="False" />
                                            <asp:ListItem Text="20" Value="20" Selected="False" />
                                            <asp:ListItem Text="21" Value="21" Selected="False" />
                                            <asp:ListItem Text="22" Value="22" Selected="False" />
                                            <asp:ListItem Text="23" Value="23" Selected="False" />
                                        </asp:DropDownList>
                                        <span id="planned_start_min_label">&nbsp;min:</span>
                                        <asp:DropDownList ID="planned_start_min" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="False" />
                                            <asp:ListItem Text="15" Value="15" Selected="False" />
                                            <asp:ListItem Text="30" Value="30" Selected="False" />
                                            <asp:ListItem Text="45" Value="45" Selected="False" />
                                        </asp:DropDownList>
                                    </td>
                                    <td>&nbsp</td>
                                </tr>


                                <tr runat="server" id="actual_start_date_tr">
                                    <td class="lbl" id="actual_start_date_lbl">Actual start date:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt date" id="actual_start_date" maxlength="10" size="10">
                                        <span id="actual_start_hour_label">&nbsp;hour:</span>
                                        <asp:DropDownList ID="actual_start_hour" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="False" />
                                            <asp:ListItem Text="01" Value="01" Selected="False" />
                                            <asp:ListItem Text="02" Value="02" Selected="False" />
                                            <asp:ListItem Text="03" Value="03" Selected="False" />
                                            <asp:ListItem Text="04" Value="04" Selected="False" />
                                            <asp:ListItem Text="05" Value="05" Selected="False" />
                                            <asp:ListItem Text="06" Value="06" Selected="False" />
                                            <asp:ListItem Text="07" Value="07" Selected="False" />
                                            <asp:ListItem Text="08" Value="08" Selected="False" />
                                            <asp:ListItem Text="09" Value="09" Selected="False" />
                                            <asp:ListItem Text="10" Value="10" Selected="False" />
                                            <asp:ListItem Text="11" Value="11" Selected="False" />
                                            <asp:ListItem Text="12" Value="12" Selected="False" />
                                            <asp:ListItem Text="13" Value="13" Selected="False" />
                                            <asp:ListItem Text="14" Value="14" Selected="False" />
                                            <asp:ListItem Text="15" Value="15" Selected="False" />
                                            <asp:ListItem Text="16" Value="16" Selected="False" />
                                            <asp:ListItem Text="17" Value="17" Selected="False" />
                                            <asp:ListItem Text="18" Value="18" Selected="False" />
                                            <asp:ListItem Text="19" Value="19" Selected="False" />
                                            <asp:ListItem Text="20" Value="20" Selected="False" />
                                            <asp:ListItem Text="21" Value="21" Selected="False" />
                                            <asp:ListItem Text="22" Value="22" Selected="False" />
                                            <asp:ListItem Text="23" Value="23" Selected="False" />
                                        </asp:DropDownList>
                                        <span id="actual_start_min_label">&nbsp;min:</span>
                                        <asp:DropDownList ID="actual_start_min" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="False" />
                                            <asp:ListItem Text="15" Value="15" Selected="False" />
                                            <asp:ListItem Text="30" Value="30" Selected="False" />
                                            <asp:ListItem Text="45" Value="45" Selected="False" />
                                        </asp:DropDownList>
                                    </td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr runat="server" id="planned_end_date_tr">
                                    <td class="lbl" id="planned_end_date_lbl">Planned end date:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt date" id="planned_end_date" maxlength="10" size="10">
                                        <span id="planned_end_hour_label">&nbsp;hour:</span>
                                        <asp:DropDownList ID="planned_end_hour" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="False" />
                                            <asp:ListItem Text="01" Value="01" Selected="False" />
                                            <asp:ListItem Text="02" Value="02" Selected="False" />
                                            <asp:ListItem Text="03" Value="03" Selected="False" />
                                            <asp:ListItem Text="04" Value="04" Selected="False" />
                                            <asp:ListItem Text="05" Value="05" Selected="False" />
                                            <asp:ListItem Text="06" Value="06" Selected="False" />
                                            <asp:ListItem Text="07" Value="07" Selected="False" />
                                            <asp:ListItem Text="08" Value="08" Selected="False" />
                                            <asp:ListItem Text="09" Value="09" Selected="False" />
                                            <asp:ListItem Text="10" Value="10" Selected="False" />
                                            <asp:ListItem Text="11" Value="11" Selected="False" />
                                            <asp:ListItem Text="12" Value="12" Selected="False" />
                                            <asp:ListItem Text="13" Value="13" Selected="False" />
                                            <asp:ListItem Text="14" Value="14" Selected="False" />
                                            <asp:ListItem Text="15" Value="15" Selected="False" />
                                            <asp:ListItem Text="16" Value="16" Selected="False" />
                                            <asp:ListItem Text="17" Value="17" Selected="False" />
                                            <asp:ListItem Text="18" Value="18" Selected="False" />
                                            <asp:ListItem Text="19" Value="19" Selected="False" />
                                            <asp:ListItem Text="20" Value="20" Selected="False" />
                                            <asp:ListItem Text="21" Value="21" Selected="False" />
                                            <asp:ListItem Text="22" Value="22" Selected="False" />
                                            <asp:ListItem Text="23" Value="23" Selected="False" />

                                        </asp:DropDownList>
                                        <span id="planned_end_min_label">&nbsp;min:</span>
                                        <asp:DropDownList ID="planned_end_min" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="False" />
                                            <asp:ListItem Text="15" Value="15" Selected="False" />
                                            <asp:ListItem Text="30" Value="30" Selected="False" />
                                            <asp:ListItem Text="45" Value="45" Selected="False" />
                                        </asp:DropDownList>
                                    </td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr runat="server" id="actual_end_date_tr">
                                    <td class="lbl" id="actual_end_date_lbl">Actual end date:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt date" id="actual_end_date" maxlength="10" size="10">
                                        <span id="actual_end_hour_label">&nbsp;hour:</span>
                                        <asp:DropDownList ID="actual_end_hour" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="False" />
                                            <asp:ListItem Text="01" Value="01" Selected="False" />
                                            <asp:ListItem Text="02" Value="02" Selected="False" />
                                            <asp:ListItem Text="03" Value="03" Selected="False" />
                                            <asp:ListItem Text="04" Value="04" Selected="False" />
                                            <asp:ListItem Text="05" Value="05" Selected="False" />
                                            <asp:ListItem Text="06" Value="06" Selected="False" />
                                            <asp:ListItem Text="07" Value="07" Selected="False" />
                                            <asp:ListItem Text="08" Value="08" Selected="False" />
                                            <asp:ListItem Text="09" Value="09" Selected="False" />
                                            <asp:ListItem Text="10" Value="10" Selected="False" />
                                            <asp:ListItem Text="11" Value="11" Selected="False" />
                                            <asp:ListItem Text="12" Value="12" Selected="False" />
                                            <asp:ListItem Text="13" Value="13" Selected="False" />
                                            <asp:ListItem Text="14" Value="14" Selected="False" />
                                            <asp:ListItem Text="15" Value="15" Selected="False" />
                                            <asp:ListItem Text="16" Value="16" Selected="False" />
                                            <asp:ListItem Text="17" Value="17" Selected="False" />
                                            <asp:ListItem Text="18" Value="18" Selected="False" />
                                            <asp:ListItem Text="19" Value="19" Selected="False" />
                                            <asp:ListItem Text="20" Value="20" Selected="False" />
                                            <asp:ListItem Text="21" Value="21" Selected="False" />
                                            <asp:ListItem Text="22" Value="22" Selected="False" />
                                            <asp:ListItem Text="23" Value="23" Selected="False" />
                                        </asp:DropDownList>
                                        <span id="actual_end_min_label">&nbsp;min:</span>
                                        <asp:DropDownList ID="actual_end_min" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="False" />
                                            <asp:ListItem Text="15" Value="15" Selected="False" />
                                            <asp:ListItem Text="30" Value="30" Selected="False" />
                                            <asp:ListItem Text="45" Value="45" Selected="False" />
                                        </asp:DropDownList>
                                    </td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr runat="server" id="planned_duration_tr">
                                    <td class="lbl" id="planned_duration_lbl">Planned duration:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="planned_duration" maxlength="7" size="7"></td>
                                    <td runat="server" class="err" id="planned_duration_err">&nbsp;</td>
                                </tr>

                                <tr runat="server" id="actual_duration_tr">
                                    <td class="lbl" id="actual_duration_lbl">Actual duration:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="actual_duration" maxlength="7" size="7"></td>
                                    <td runat="server" class="err" id="actual_duration_err">&nbsp;</td>
                                </tr>

                                <tr runat="server" id="duration_units_tr">
                                    <td class="lbl" id="duration_units_lbl">Duration units:</td>
                                    <td>
                                        <asp:DropDownList ID="duration_units" runat="server">
                                            <asp:ListItem Text="minutes" Value="minutes" Selected="False" />
                                            <asp:ListItem Text="hours" Value="hours" Selected="False" />
                                            <asp:ListItem Text="days" Value="days" Selected="False" />
                                        </asp:DropDownList>
                                    </td>
                                    <td>&nbsp</td>
                                </tr>

                                <tr runat="server" id="percent_complete_tr">
                                    <td class="lbl" id="percent_complete_lbl">Percent complete:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="percent_complete" maxlength="6" size="6"></td>
                                    <td runat="server" class="err" id="percent_complete_err">&nbsp;</td>
                                </tr>

                                <tr runat="server" id="status_tr">
                                    <td class="lbl" id="status_lbl">Status:</td>
                                    <td>
                                        <asp:DropDownList ID="status" runat="server"></asp:DropDownList></td>
                                    <td>&nbsp</td>
                                </tr>


                                <tr runat="server" id="sort_sequence_tr">
                                    <td class="lbl" id="sort_sequence_lbl">Sort Sequence:</td>
                                    <td>
                                        <input runat="server" type="text" class="txt" id="sort_sequence" maxlength="3" size="3"></td>
                                    <td runat="server" class="err" id="sort_sequence_err">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td colspan="3" align="left">
                                        <span runat="server" class="err" id="msg">&nbsp;</span>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="3" align="center">
                                        <input runat="server" class="btn" type="submit" id="sub" value="Create or Edit">
                                </tr>

                            </table>
                        </form>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
