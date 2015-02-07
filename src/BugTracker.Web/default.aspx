<%@ Page Language="C#" ValidateRequest="false" CodeBehind="default.aspx.cs" Inherits="btnet.default" AutoEventWireup="True" MasterPageFile="LoggedOut.Master" %>

<%@ Import Namespace="btnet" %>

<asp:Content runat="server" ContentPlaceHolderID="body">

    <div class="row">
        <div class="col-sm-6 col-sm-offset-3 col-md-4 col-md-offset-4">
            <div class="panel panel-default">

                <div class="panel-body">
                    <form role="form" method="POST" runat="server">
                        <fieldset>
                            <div class="row">
                                <div class="center-block">
                                    <div class="profile-img">
                                        <i class="glyphicon glyphicon-user"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-10  col-md-offset-1 ">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <i class="glyphicon glyphicon-user"></i>
                                            </span>
                                            <input runat="server" type="text" class="form-control" placeholder="Username" id="user" autofocus />

                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <i class="glyphicon glyphicon-lock"></i>
                                            </span>
                                            <input runat="server" type="password" class="form-control" placeholder="Password" id="pw" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <input type="submit" class="btn btn-lg btn-primary btn-block" value="Sign in">
                                        <span runat="server" class="err" id="msg">&nbsp;</span>

                                    </div>
                                </div>
                            </div>
                        </fieldset>
                    </form>
                </div>
                <div class="panel-footer text-muted">
                    <% if (Util.get_setting("AllowGuestWithoutLogin", "0") == "1")
                       { %>
                    <div>Continue as <a href="bugs.aspx">guest</a></div>

                    <% } %>

                    <% if (Util.get_setting("AllowSelfRegistration", "0") == "1")
                       { %>
                    <div>Don't have an account? <a href="register.aspx">Register Here</a></div>

                    <% } %>

                    <% if (Util.get_setting("ShowForgotPasswordLink", "1") == "1")
                       { %>
                    <div><a href="forgot.aspx">Forgot password?</a></div>
                    <% } %>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-6 col-md-4 col-md-offset-4">
                <% Response.Write(Application["custom_welcome"]); %>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="footerScripts">
    <script>
        $(function () {
            $("<%=user.ClientID%>").focus();
        });
    </script>
</asp:Content>
