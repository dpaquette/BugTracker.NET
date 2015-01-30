using System;
using System.Linq;
using System.Data;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class delete_customfield : BasePage
    {

        SQLString sql;


        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "delete custom field";

            if (IsPostBack)
            {
                // do delete here

                sql = new SQLString(@"select sc.name [column_name], df.name [default_constraint_name]
			from syscolumns sc
			inner join sysobjects so on sc.id = so.id
			left outer join sysobjects df on df.id = sc.cdefault
			where so.name = 'bugs'
			and sc.colorder = @id");

                sql = sql.AddParameterWithValue("@id", Util.sanitize_integer(row_id.Value));
                DataRow dr = DbUtil.get_datarow(sql);

                // if there is a default, delete it
                if (dr["default_constraint_name"].ToString() != "")
                {
                    sql = new SQLString(@"alter table bugs drop constraint @df");
                    sql = sql.AddParameterWithValue("@df", (string)dr["default_constraint_name"]);
                    DbUtil.execute_nonquery(sql);
                }


                // delete column itself
                sql = new SQLString(@"
if exists(select * from sys.columns 
            where Name = @orgcolumn and Object_ID = Object_ID(N'orgs'))
begin
    exec('alter table orgs drop column ' + @orgcolumn);
end
if exists(select * from sys.columns 
            where Name = @nm and Object_ID = Object_ID(N'bugs'))
begin
    exec('alter table bugs drop column ' +  @nm)
end");


                sql.AddParameterWithValue("@orgcolumn", "og_" + dr["column_name"] + "_field_permission_level");
                sql = sql.AddParameterWithValue("@nm", (string)dr["column_name"]);
                DbUtil.execute_nonquery(sql);


                //delete row from custom column table
                sql = new SQLString(@"delete from custom_col_metadata
		where ccm_colorder = @num");
                sql = sql.AddParameterWithValue("@num", Util.sanitize_integer(row_id.Value));

                Application["custom_columns_dataset"] = null;
                DbUtil.execute_nonquery(sql);

                Response.Redirect("customfields.aspx");


            }
            else
            {
                string id = Util.sanitize_integer(Request["id"]);

                sql = new SQLString(@"select sc.name
			from syscolumns sc
			inner join sysobjects so on sc.id = so.id
			left outer join sysobjects df on df.id = sc.cdefault
			where so.name = 'bugs'
			and sc.colorder = @id");

                sql = sql.AddParameterWithValue("@id", id);
                DataRow dr = DbUtil.get_datarow(sql);

                confirm_href.InnerText = "confirm delete of \""
                    + Convert.ToString(dr["name"])
                    + "\"";

                row_id.Value = id;
            }

        }
    }
}
