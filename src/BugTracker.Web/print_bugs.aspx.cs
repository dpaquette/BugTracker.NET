using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using btnet.Models;

namespace btnet
{
    public partial class print_bugs : BasePage
    {
        SQLString sql;

        DataSet ds;
        DataView dv;

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            if (Request["format"] != "excel")
            {
                Util.do_not_cache(Response);
            }



            // fetch the sql
            int queryId = Convert.ToInt32(Request["queryId"]);
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string sortBy = Request["sortBy"];
            string sortOrder = Request["sortOrder"];
            BugQueryFilter[] filters = BuildFilter(Request.Params);
            Query query;
            using (Context context = new Context())
            {
                query = context.Queries.Find(queryId);
            }

            BugQueryExecutor executor = new BugQueryExecutor(query);

            BugQueryResult result = executor.ExecuteQuery(User.Identity, start, length, sortBy, sortOrder, filters);
            

            dv = new DataView(result.Data);
            
            string format = Request["format"];
            if (format != null && format == "excel")
            {
                Util.print_as_excel(Response, dv);
            }
            else
            {
                print_as_html();
            }

        }

        private BugQueryFilter[] BuildFilter(NameValueCollection queryParams)
        {
            List<BugQueryFilter> filters = new List<BugQueryFilter>();
            int arrayIndex = 0;
            while (!string.IsNullOrEmpty(queryParams[string.Format("filters[{0}][Column]", arrayIndex)]))
            {
                BugQueryFilter filter = new BugQueryFilter();
                filter.Column = queryParams[string.Format("filters[{0}][Column]", arrayIndex)];
                filter.Value = queryParams[string.Format("filters[{0}][Value]", arrayIndex)];
                filters.Add(filter);
                arrayIndex++;
            }
            return filters.ToArray();
        }

        ///////////////////////////////////////////////////////////////////////
        void print_as_html()
        {

            Response.Write("<html><head><link rel='StyleSheet' href='btnet.css' type='text/css'></head><body>");

            Response.Write("<table class=bugt border=1>");
            int col;

            for (col = 1; col < dv.Table.Columns.Count; col++)
            {

                Response.Write("<td class=bugh>\n");
                if (dv.Table.Columns[col].ColumnName == "$FLAG")
                {
                    Response.Write("flag");
                }
                else if (dv.Table.Columns[col].ColumnName == "$SEEN")
                {
                    Response.Write("new");
                }
                else
                {
                    Response.Write(dv.Table.Columns[col].ColumnName);
                }
                Response.Write("</td>");
            }

            foreach (DataRowView drv in dv)
            {
                Response.Write("<tr>");
                for (col = 1; col < dv.Table.Columns.Count; col++)
                {
                    if (dv.Table.Columns[col].ColumnName == "$FLAG")
                    {
                        int flag = (int)drv[col];
                        string cls = "wht";
                        if (flag == 1) cls = "red";
                        else if (flag == 2) cls = "grn";

                        Response.Write("<td class=datad><span class=" + cls + ">&nbsp;</span>");

                    }
                    else if (dv.Table.Columns[col].ColumnName == "$SEEN")
                    {
                        int seen = (int)drv[col];
                        string cls = "old";
                        if (seen == 0)
                        {
                            cls = "new";
                        }
                        else
                        {
                            cls = "old";
                        }
                        Response.Write("<td class=datad><span class=" + cls + ">&nbsp;</span>");

                    }
                    else
                    {
                        Type datatype = dv.Table.Columns[col].DataType;

                        if (Util.is_numeric_datatype(datatype))
                        {
                            Response.Write("<td class=bugd align=right>");
                        }
                        else
                        {
                            Response.Write("<td class=bugd>");
                        }

                        // write the data
                        if (drv[col].ToString() == "")
                        {
                            Response.Write("&nbsp;");
                        }
                        else
                        {
                            Response.Write(Server.HtmlEncode(drv[col].ToString()).Replace("\n", "<br>"));
                        }
                    }
                    Response.Write("</td>");
                }
                Response.Write("</tr>");
            }

            Response.Write("</table></body></html>");
        }


    }
}
