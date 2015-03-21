<%@ Page Language="C#" CodeBehind="bugs.aspx.cs" Inherits="btnet.bugs" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>
<%@ MasterType TypeName="btnet.LoggedIn" %>

<%@ Import Namespace="btnet" %>
<%@ Import Namespace="btnet.Security" %>
<%@ Register Src="~/Controls/BugList.ascx" TagPrefix="uc1" TagName="BugList" %>

<asp:Content ContentPlaceHolderID="headerScripts" runat="server">
    <script type="text/javascript" src="bug_list.js"></script>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server" ClientIDMode="Static">
    <div class="container-fluid">

        <form method="POST" runat="server" id="frm">
            <div class="row">
                <div class="col-md-12">
                    <label>Query</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4 col-sm-6 col-xs-12">
                    <asp:DropDownList ID="query" runat="server" class="form-control" AutoPostBack="true">
                    </asp:DropDownList>
                </div>

            </div>
            <br />
            <div class="row">
                <div class="col-md-12">
                    <% if (User.Identity.GetCanAddBugs())
                       { %>
                    <a href="edit_bug.aspx" class="btn btn-primary"><span class="glyphicon glyphicon-plus"></span>&nbsp;New <%=Util.get_setting("SingularBugLabel", "bug")%></a>

                    <% } %>
                    <div class="btn-group">
                        <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                            <span class="glyphicon glyphicon-print"></span>&nbsp;Print<span class="caret"></span>
                        </button>


                        <ul class="dropdown-menu" role="menu">
                            <li><a target="_blank" id="printbuglist" href="#">List</a></li>
                            <li><a target="_blank" href="print_bugs2.aspx">Details</a></li>
                        </ul>
                    </div>
                    <a target="_blank" class="btn btn-default" id="exportbuglist" href="#"><span class="glyphicon glyphicon-download"></span>&nbsp;Export to Excel</a>
                    <a target="_blank" class="btn btn-default pull-right" href="btnet_screen_capture.exe" title="Download Screen Capture Utility">
                        <span class="glyphicon glyphicon-camera"></span>&nbsp;Screen Capture Tool</a>
                </div>
            </div>
            <br />

            <div class="row">
                <div class="col-md-12">
                    <uc1:BugList runat="server" ID="BugList" ShowCheckbox="False" />
                </div>
            </div>
        </form>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="footerScripts" runat="server">


    <script>
        var enable_popups = <% Response.Write(User.Identity.GetEnablePopups() ? "1" : "0"); %>;
        var asp_form_id = '<%=frm.ClientID %>';
        $(function() {

            var printBugs  = function(baseUrl) {
                var queryParams = BugList.getQueryParams();
                if (queryParams != null && queryParams.queryId) {
                    queryParams.start = 0;
                    queryParams.length = -1;
                    window.open(baseUrl + $.param(queryParams), "_blank");
                }
            }
            $("#printbuglist").click(function() {
                printBugs("print_bugs.aspx?");
                return false;
            });

            $("#exportbuglist").click(function() {
                printBugs("print_bugs.aspx?format=excel&");
                return false;
            });

        });
    </script>
</asp:Content>
