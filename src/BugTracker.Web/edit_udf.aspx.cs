using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class edit_udf : BasePage
    {
        private int id;
        private SQLString sql;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit user defined attribute value";

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

                    sql = new SQLString(@"select udf_name, udf_sort_seq, udf_default from user_defined_attribute where udf_id = @udfid");
                    sql = sql.AddParameterWithValue("udfid", Convert.ToString(id));
                    DataRow dr = btnet.DbUtil.get_datarow(sql);

                    // Fill in this form
                    name.Value = (string)dr[0];
                    sort_seq.Value = Convert.ToString((int)dr[1]);
                    default_selection.Checked = Convert.ToBoolean((int)dr["udf_default"]);
                }
            }
            else
            {
                on_update();
            }

        }


        ///////////////////////////////////////////////////////////////////////
        private Boolean validate()
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
        private void on_update()
        {

            Boolean good = validate();

            if (good)
            {
                if (id == 0)  // insert new
                {
                    sql = new SQLString("insert into user_defined_attribute (udf_name, udf_sort_seq, udf_default) values (@na, @ss, @df)");
                }
                else // edit existing
                {

                    sql = new SQLString(@"update user_defined_attribute set
				udf_name = @na,
				udf_sort_seq = @ss,
				udf_default = @df
				where udf_id = @id");

                    sql = sql.AddParameterWithValue("id", Convert.ToString(id));

                }
                sql = sql.AddParameterWithValue("na", name.Value);
                sql = sql.AddParameterWithValue("ss", sort_seq.Value);
                sql = sql.AddParameterWithValue("df", Util.bool_to_string(default_selection.Checked));
                btnet.DbUtil.execute_nonquery(sql);
                Server.Transfer("udfs.aspx");

            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText = "User defined attribute value was not created.";
                }
                else // edit existing
                {
                    msg.InnerText = "User defined attribute value was not updated.";
                }

            }

        }

    }
}
