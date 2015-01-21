<%@ Page Language="C#" CodeBehind="select_report.aspx.cs" Inherits="btnet.select_report" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="headerScripts" runat="server">
    <script>

        function select_report(type, id) {
            opener.add_selected_report(type, id)
            window.close()

        }

    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        </p>

        <%

            if (ds.Tables[0].Rows.Count > 0)
            {
                btnet.SortableHtmlTable.create_from_dataset(
                    Response, ds, "", "", false);

            }
            else
            {
                Response.Write("No reports in the database.");
            }

        %>
    </div>

</asp:Content>
