<%@ Page Language="C#" CodeBehind="print_bugs2.aspx.cs" Inherits="btnet.print_bugs2" AutoEventWireup="True" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="btnet" %>

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


