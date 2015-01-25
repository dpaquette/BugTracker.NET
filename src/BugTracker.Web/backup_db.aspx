<%@ Page Language="C#" CodeBehind="backup_db.aspx.cs" Inherits="btnet.backup_db" ValidateRequest="false" EnableEventValidation="false" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <div class="align">
        <table border="0">
            <tr>
                <td>

                    <form runat="server">

                        <asp:DataGrid ID="MyDataGrid" runat="server" BorderColor="black" CssClass="datat"
                            CellPadding="3" AutoGenerateColumns="false" OnItemCommand="my_button_click">
                            <HeaderStyle CssClass="datah"></HeaderStyle>
                            <ItemStyle CssClass="datad"></ItemStyle>
                            <Columns>
                                <asp:BoundColumn HeaderText="File" DataField="file" />
                                <asp:HyperLinkColumn HeaderText="Download" Text="Download" DataNavigateUrlField="url"
                                    Target="_blank" />
                                <asp:ButtonColumn HeaderText="Delete" ButtonType="LinkButton" Text="Delete" CommandName="dlt" />
                            </Columns>
                        </asp:DataGrid>
                        <div class="err" id="msg" runat="server">&nbsp;</div>

                        <div>
                            <input type="submit" value="Backup Database Now" class="btn" runat="server" onserverclick="on_backup" style="width: 200px; height: 50px;"></div>

                    </form>

                    <p>&nbsp;</p>
                    <p>&nbsp;</p>
                    You can use SQL like this to restore your backup to your own server:
                    <pre>

RESTORE DATABASE your_database
   FROM DISK = 'C:\path\to\your\your_backup_file.bak'
   WITH 
      MOVE 'btnet' TO 'C:\path\to\where\you\want\your_db_data.mdf' ,
      MOVE 'btnet_log'  TO 'C:\path\to\where\you\want\your_db_log.ldf', REPLACE

</pre>


        </table>
    </div>
</asp:Content>
