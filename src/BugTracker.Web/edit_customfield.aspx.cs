using System;
using System.Data;
using System.Linq;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class edit_customfield : BasePage
    {


        int id;
        SQLString sql;


        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {
            Master.Menu.SelectedItem = Util.get_setting("PluralBugLabel", "bugs");
            Util.do_not_cache(Response);


            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit custom column metadata";

            msg.InnerText = "";

            id = Convert.ToInt32(Util.sanitize_integer(Request["id"]));

            if (!IsPostBack)
            {

                // Get this entry's data from the db and fill in the form

                sql = new SQLString(@"
select sc.name,
isnull(ccm_dropdown_vals,'') [vals],
isnull(ccm_dropdown_type,'') [dropdown_type],
isnull(ccm_sort_seq, sc.colorder) [column order],
mm.text [default value], dflts.name [default name]
from syscolumns sc
inner join sysobjects so on sc.id = so.id
left outer join custom_col_metadata ccm on ccm_colorder = sc.colorder
left outer join syscomments mm on sc.cdefault = mm.id
left outer join sysobjects dflts on dflts.id = mm.id
where so.name = 'bugs'
and sc.colorder = @co");

                sql = sql.AddParameterWithValue("co", Convert.ToString(id));
                DataRow dr = DbUtil.get_datarow(sql);

                name.InnerText = (string)dr["name"];
                dropdown_type.Value = Convert.ToString(dr["dropdown_type"]);

                if (dropdown_type.Value == "normal")
                {
                    // show the dropdown vals
                }
                else
                {
                    vals.Visible = false;
                    vals_label.Visible = false;
                    //vals_explanation.Visible = false;
                }

                // Fill in this form
                vals.Value = (string)dr["vals"];
                sort_seq.Value = Convert.ToString(dr["column order"]);
                default_value.Value = Convert.ToString(dr["default value"]);
                hidden_default_value.Value = default_value.Value; // to test if it changed
                hidden_default_name.Value = Convert.ToString(dr["default name"]);

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

            sort_seq_err.InnerText = "";
            vals_err.InnerText = "";

            if (sort_seq.Value == "")
            {
                good = false;
                sort_seq_err.InnerText = "Sort Sequence is required.";
            }


            if (!Util.is_int(sort_seq.Value))
            {
                good = false;
                sort_seq_err.InnerText = "Sort Sequence must be an integer.";
            }


            if (dropdown_type.Value == "normal")
            {
                if (vals.Value == "")
                {
                    good = false;
                    vals_err.InnerText = "Dropdown values are required for dropdown type of \"normal\".";
                }
                else
                {
                    string vals_error_string = Util.validate_dropdown_values(vals.Value);
                    if (!string.IsNullOrEmpty(vals_error_string))
                    {
                        good = false;
                        vals_err.InnerText = vals_error_string;
                    }
                }
            }

            return good;
        }

        ///////////////////////////////////////////////////////////////////////
        void on_update()
        {

            Boolean good = validate();

            if (good)
            {

                sql = new SQLString(@"declare @count int
			select @count = count(1) from custom_col_metadata
			where ccm_colorder = @co

			if @count = 0
				insert into custom_col_metadata
				(ccm_colorder, ccm_dropdown_vals, ccm_sort_seq)
				values(@co, @v, @ss)
			else
				update custom_col_metadata
				set ccm_dropdown_vals = @v,
				ccm_sort_seq = @ss
				where ccm_colorder = @co");

                sql = sql.AddParameterWithValue("co", Convert.ToString(id));
                sql = sql.AddParameterWithValue("v", vals.Value);
                sql = sql.AddParameterWithValue("ss", sort_seq.Value);

                DbUtil.execute_nonquery(sql);
                Application["custom_columns_dataset"] = null;

                if (default_value.Value != hidden_default_value.Value)
                {
                    if (hidden_default_name.Value != "")
                    {
                        sql = new SQLString("alter table bugs drop constraint [" + hidden_default_name.Value.Replace("'", "''") + "]");
                        DbUtil.execute_nonquery(sql);
                        Application["custom_columns_dataset"] = null;
                    }

                    if (default_value.Value != "")
                    {
                        sql = new SQLString("alter table bugs add constraint [" + System.Guid.NewGuid().ToString() + "] default '" + default_value.Value.Replace("'", "''") + "' for [" + name.InnerText + "]");
                        DbUtil.execute_nonquery(sql);
                        Application["custom_columns_dataset"] = null;
                    }
                }

                Server.Transfer("customfields.aspx");
            }
            else
            {
                msg.InnerText = "dropdown values were not updated.";
            }

        }
    }
}
