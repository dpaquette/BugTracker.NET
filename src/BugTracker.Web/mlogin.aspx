<%@ Page Language="C#" CodeBehind="mlogin.aspx.cs" Inherits="btnet.mlogin" ValidateRequest="false" AutoEventWireup="True" %>

<%@ Import Namespace="System.Data.SqlClient" %>
<!--
Copyright 2002-2013 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

    string sql;

    ///////////////////////////////////////////////////////////////////////
    void Page_Load(Object sender, EventArgs e)
    {

        Util.set_context(HttpContext.Current);
        Util.do_not_cache(Response);

        if (btnet.Util.get_setting("EnableMobile", "0") == "0")
        {
            Response.Write("BugTracker.NET EnableMobile is not set to 1 in Web.config");
            Response.End();
        }

        msg.InnerText = "";

        titl.InnerText = btnet.Util.get_setting("AppTitle", "BugTracker.NET") + " - Logon";
        my_header.InnerText = titl.InnerText;

        // fill in the username first time in
        if (IsPostBack)
        {
            on_logon();
        }

    }

    ///////////////////////////////////////////////////////////////////////
    void on_logon()
    {

        bool authenticated = btnet.Authenticate.check_password(user.Value, pw.Value);

        if (authenticated)
        {
            var sql = new SQLString("select us_id from users where us_username = @us");
            sql = sql.AddParameterWithValue("us", user.Value);
            DataRow dr = btnet.DbUtil.get_datarow(sql);
            if (dr != null)
            {
                int us_id = (int)dr["us_id"];

                btnet.Security.create_session(
                    Request,
                    Response,
                    us_id,
                    user.Value,
                    "0");

                btnet.Util.redirect(Request, Response);
            }
        }
        else
        {
            msg.InnerText = "Invalid User or Password.";
        }

    }

</script>

<html>
<head>
    <title id="titl" runat="server">Title</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="Content/style/jquery.mobile.custom.structure.css" />
    <link rel="stylesheet" href="Content/style/jquery.mobile.custom.theme.css" />
    
    <link rel="stylesheet" href="mbtnet_base.css" />

    <script src="scripts/jquery-1.11.1.min.js"></script>
    <script src="scripts/jquery.mobile-1.4.4.min.js"></script>
</head>
<body>

    <div data-role="page">

        <div data-role="header">
            <h1 id="my_header" runat="server">Header</h1>
        </div>
        <!-- /header -->

        <div data-role="content">

            <form data-ajax="false" id="Form1" class="frm" runat="server">
                <div class="err" runat="server" id="msg">&nbsp;</div>
                <label>User:</label>
                <input runat="server" type="text" id="user">
                <label>Password:</label>
                <input runat="server" type="password" id="pw">
                <input data-role="button" id="Submit1" type="submit" value="Logon" runat="server">
                <input type="hidden" id="mobile" name="mobile" value="1" />
            </form>

        </div>
        <!-- /content -->

    </div>
    <!-- /page -->

</body>
</html>
