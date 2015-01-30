<%@ Page Language="C#" CodeBehind="manage_logs.aspx.cs" Inherits="btnet.manage_logs" ValidateRequest="false" EnableEventValidation="false" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="align">

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

        </form>

    </div>

</asp:Content>


