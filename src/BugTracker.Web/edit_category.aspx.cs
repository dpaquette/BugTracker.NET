using System;
using System.Linq;
using System.Data;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class edit_category : BasePage
    {

        int id;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit category";

            msg.InnerText = "";

            string var = Request.QueryString["id"];
            if (var == null)
            {
                id = 0;
            }
            else
            {
                id = Convert.ToInt32(var);
            }

            if (!IsPostBack)
            {

                // add or edit?
                if (id == 0)
                {
                    sub.Value = "Create";
                }
                else
                {
                    sub.Value = "Update";

                    // Get this entry's data from the db and fill in the form

                    var sql = new SQLString(@"select ct_name, ct_sort_seq, ct_default from categories where ct_id = @categoryId");
                    sql = sql.AddParameterWithValue("categoryId", Convert.ToString(id));
                    DataRow dr = btnet.DbUtil.get_datarow(sql);

                    // Fill in this form
                    name.Value = (string)dr[0];
                    sort_seq.Value = Convert.ToString((int)dr[1]);
                    default_selection.Checked = Convert.ToBoolean((int)dr["ct_default"]);

                }
            }
            else
            {
                on_update();
            }
        }


        ///////////////////////////////////////////////////////////////////////
        protected Boolean validate()
        {

            Boolean good = true;
            if (name.Value == "")
            {
                good = false;
                name_err.InnerText = "Description is required.";
            }
            else
            {
                name_err.InnerText = "";
            }

            if (sort_seq.Value == "")
            {
                good = false;
                sort_seq_err.InnerText = "Sort Sequence is required.";
            }
            else
            {
                sort_seq_err.InnerText = "";
            }

            if (!Util.is_int(sort_seq.Value))
            {
                good = false;
                sort_seq_err.InnerText = "Sort Sequence must be an integer.";
            }
            else
            {
                sort_seq_err.InnerText = "";
            }


            return good;
        }

        ///////////////////////////////////////////////////////////////////////
        protected void on_update()
        {

            Boolean good = validate();

            if (good)
            {
                SQLString sql;
                if (id == 0)  // insert new
                {
                    sql = new SQLString("insert into categories (ct_name, ct_sort_seq, ct_default) values (@na, @ss, @df)");
                }
                else // edit existing
                {

                    sql = new SQLString(@"update categories set
				ct_name = @na,
				ct_sort_seq = @ss,
				ct_default = @df
				where ct_id = @id");

                    sql = sql.AddParameterWithValue("id", Convert.ToString(id));

                }
                sql = sql.AddParameterWithValue("na", name.Value);
                sql = sql.AddParameterWithValue("ss", sort_seq.Value);
                sql = sql.AddParameterWithValue("df", Util.bool_to_string(default_selection.Checked));
                btnet.DbUtil.execute_nonquery(sql);
                Server.Transfer("categories.aspx");

            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText = "Category was not created.";
                }
                else // edit existing
                {
                    msg.InnerText = "Category was not updated.";
                }

            }

        }

    }
}
