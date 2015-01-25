<%@ Page Language="C#" CodeBehind="edit_styles.aspx.cs" Inherits="btnet.edit_styles" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <div class="align">

        <div class="lbl" style="width: 600;">
            The query "demo use of css classes" has as its first column a CSS class name that is
 composed of the priority's CSS class name concatenated with the status's CSS
 class name.  The SQL looks like this:
        </div>
        <p>
            <div style="font-family: courier; font-weight: bold;">
                select <span style="color: red;">isnull(pr_style + st_style,'datad')</span>, bg_id [id], bg_short_desc [desc], .... etc
            </div>
            <p>
                <div class="lbl" style="width: 600;">
                    Note that in the sql, where there isn't both a priority CSS class and a status CSS class
 available, the default CSS class name of "datad" is used.    The following list lets you see
 how all the different priority/status combinations will look.   Click on a link to edit
 a priority or a status.

                </div>

                <%

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        btnet.SortableHtmlTable.create_from_dataset(
                            Response, ds, "", "", false);

                    }
                    else
                    {
                        Response.Write("No priority/status combos in the database.");
                    }

                %>

                <div cls="lbl">Relevant lines from btnet_custom.css:</div>
                <div class="frm" style="width: 600px;" id="relevant_lines" runat="server">
                </div>

    </div>
</asp:Content>
