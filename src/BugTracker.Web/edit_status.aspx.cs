using System;
using System.Data;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class edit_status : BasePage
    {

        int id;
        SQLString sql;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);
            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit status";

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

                    sql = new SQLString(@"select st_name, st_sort_seq, isnull(st_style,'') [st_style], st_default from statuses where st_id = @id");
                    sql = sql.AddParameterWithValue("id", id);
                    DataRow dr = btnet.DbUtil.get_datarow(sql);

                    // Fill in this form
                    name.Value = (string)dr["st_name"];
                    sort_seq.Value = Convert.ToString((int)dr["st_sort_seq"]);
                    style.Value = (string)dr["st_style"];
                    default_selection.Checked = Convert.ToBoolean((int)dr["st_default"]);
                }
            }
            else
            {
                on_update();
            }

        }


        ///////////////////////////////////////////////////////////////////////
        Boolean validate()
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
        void on_update()
        {

            Boolean good = validate();

            if (good)
            {
                if (id == 0)  // insert new
                {
                    sql = new SQLString("insert into statuses (st_name, st_sort_seq, st_style, st_default) values (@na, @ss, @st, @df)");
                }
                else // edit existing
                {

                    sql = new SQLString(@"update statuses set
				st_name = @na,
				st_sort_seq = @ss,
				st_style = @st,
				st_default = @df
				where st_id = @id");

                    sql = sql.AddParameterWithValue("id", id);

                }
                sql = sql.AddParameterWithValue("na", name.Value);
                sql = sql.AddParameterWithValue("ss", sort_seq.Value);
                sql = sql.AddParameterWithValue("st", style.Value);
                sql = sql.AddParameterWithValue("df", Util.bool_to_string(default_selection.Checked));
                btnet.DbUtil.execute_nonquery(sql);
                Server.Transfer("statuses.aspx");

            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText = "Status was not created.";
                }
                else // edit existing
                {
                    msg.InnerText = "Status was not updated.";
                }

            }

        }

    }
}
