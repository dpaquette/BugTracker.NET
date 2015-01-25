using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;
using System.Data;
using System.Collections;
using System.Text;
using System.IO;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class edit_styles : BasePage
    {

        protected DataSet ds;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            ds = btnet.DbUtil.get_dataset(
                new SQLString(@"select
			'<a target=_blank href=edit_priority.aspx?id=' + convert(varchar,pr_id) + '>' + pr_name + '</a>' [priority],
			'<a target=_blank href=edit_status.aspx?id=' + convert(varchar,st_id) + '>' + st_name + '</a>' [status],
			isnull(pr_style,'') [priority CSS class],
			isnull(st_style,'') [status CSS class],
			isnull(pr_style + st_style,'datad') [combo CSS class - priority + status ],
			'<span class=''' + isnull(pr_style,'') + isnull(st_style,'')  +'''>The quick brown fox</span>' [text sample]
			from priorities, statuses /* intentioanl cartesian join */
			order by pr_sort_seq, st_sort_seq;

			select distinct isnull(pr_style + st_style,'datad')
			from priorities, statuses;"));

            ArrayList classes_list = new ArrayList();
            foreach (DataRow dr_styles in ds.Tables[1].Rows)
            {

                classes_list.Add("." + (string)dr_styles[0]);
            }

            // create path
            string path = Util.GetAbsolutePath("custom\\btnet_custom.css");

            StringBuilder relevant_css_lines = new StringBuilder();

            ArrayList lines = new ArrayList();
            if (System.IO.File.Exists(path))
            {
                string line;
                StreamReader stream = File.OpenText(path);
                while ((line = stream.ReadLine()) != null)
                {
                    for (int i = 0; i < classes_list.Count; i++)
                    {
                        if (line.IndexOf((string)classes_list[i]) > -1)
                        {
                            relevant_css_lines.Append(line);
                            relevant_css_lines.Append("<br>");
                            lines.Add(line);
                            break;
                        }
                    }
                }
                stream.Close();
            }

            relevant_lines.InnerHtml = relevant_css_lines.ToString();
        }


    }
}
