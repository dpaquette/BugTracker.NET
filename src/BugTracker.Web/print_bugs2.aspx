<%@ Page Language="C#" CodeBehind="print_bugs2.aspx.cs" Inherits="btnet.print_bugs2" AutoEventWireup="True" %>

<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


    SQLString sql;

    DataSet ds = null;
    DataView dv = null;
    bool images_inline;
    bool history_inline;

    ///////////////////////////////////////////////////////////////////////
    void Page_Load(Object sender, EventArgs e)
    {

        Util.do_not_cache(Response);


        titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
            + "print " + Util.get_setting("PluralBugLabel", "bugs");


        // are we doing the query to get the bugs or are we using the cached dataview?
        string qu_id_string = Request.QueryString["qu_id"];

        if (qu_id_string != null)
        {

            // use sql specified in query string
            int qu_id = Convert.ToInt32(qu_id_string);
            sql = new SQLString(@"select qu_sql from queries where qu_id = @quid");
            sql = sql.AddParameterWithValue("quid", qu_id_string);
            var bug_sql = new SQLString((string)btnet.DbUtil.execute_scalar(sql));

            // replace magic variables
            bug_sql = bug_sql.AddParameterWithValue("ME", Convert.ToString(User.Identity.GetUserId()));
            bug_sql = Util.alter_sql_per_project_permissions(bug_sql, User.Identity);

            // all we really need is the bugid, but let's do the same query as print_bugs.aspx
            ds = btnet.DbUtil.get_dataset(bug_sql);
        }
        else
        {
            dv = (DataView)Session["bugs"];
        }

        HttpCookie cookie = Request.Cookies["images_inline"];
        if (cookie == null || cookie.Value == "0")
        {
            images_inline = false;
        }
        else
        {
            images_inline = true;
        }

        cookie = Request.Cookies["history_inline"];
        if (cookie == null || cookie.Value == "0")
        {
            history_inline = false;
        }
        else
        {
            history_inline = true;
        }

    }


</script>

<html>
<head>
    <title id="titl" runat="server">btnet print bugs detail</title>
    <link rel="StyleSheet" href="btnet.css" type="text/css">
    <style>
        a {
            text-decoration: underline;
        }

            a:visited {
                text-decoration: underline;
            }

            a:hover {
                text-decoration: underline;
            }
    </style>
</head>

<%

    bool firstrow = true;

    if (dv != null)
    {
        foreach (DataRowView drv in dv)
        {
            if (!firstrow)
            {
                Response.Write("<hr STYLE='page-break-before: always'>");
            }
            else
            {
                firstrow = false;
            }

            DataRow dr = btnet.Bug.get_bug_datarow(
                (int)drv[1],
                User.Identity);

            PrintBug.print_bug(Response, dr, User.Identity,
                false /* include style */,
                images_inline,
                history_inline,
                true /*internal_posts */); ;
        }
    }
    else
    {
        if (ds != null)
        {
            foreach (DataRow dr2 in ds.Tables[0].Rows)
            {
                if (!firstrow)
                {
                    Response.Write("<hr STYLE='page-break-before: always'>");
                }
                else
                {
                    firstrow = false;
                }

                DataRow dr = btnet.Bug.get_bug_datarow(
                    (int)dr2[1],
                    User.Identity);

                PrintBug.print_bug(Response, dr, User.Identity,
                    false, // include style
                    images_inline,
                    history_inline,
                    true); // internal_posts
            }
        }
        else
        {
            Response.Write("Please recreate the list before trying to print...");
            Response.End();
        }
    }

%>
</html>


