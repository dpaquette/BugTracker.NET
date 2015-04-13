<%@ Page Language="C#" CodeBehind="delete_udf.aspx.cs" Inherits="btnet.delete_udf" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content runat="server" ContentPlaceHolderID="body">

    <div class="align">
        <p>&nbsp</p>
        <a href="udfs.aspx">back to user defined attribute values</a>

        <p>
            or
        </p>

        <form runat="server" id="frm">
            <a id="confirm_href" runat="server" data-action="submit"></a>
            <input type="hidden" id="row_id" runat="server" />
        </form>

    </div>

</asp:Content>


