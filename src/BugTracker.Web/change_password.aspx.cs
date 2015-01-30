using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace btnet
{
    public partial class change_password : BasePage
    {
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.set_context(HttpContext.Current);
            Util.do_not_cache(Response);

            if (!IsPostBack)
            {
                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "change password";
            }
            else
            {
                msg.InnerHtml = "";

                if (string.IsNullOrEmpty(password.Value))
                {
                    msg.InnerHtml = "Enter your password twice.";
                }
                else if (password.Value != confirm.Value)
                {
                    msg.InnerHtml = "Re-entered password doesn't match password.";
                }
                else if (!Util.check_password_strength(password.Value))
                {
                    msg.InnerHtml = "Password is not difficult enough to guess.";
                    msg.InnerHtml += "<br>Avoid common words.";
                    msg.InnerHtml += "<br>Try using a mixture of lowercase, uppercase, digits, and special characters.";
                }
                else
                {


                    string guid = Request["id"];

                    if (string.IsNullOrEmpty(guid))
                    {
                        Response.Write("no guid");
                        Response.End();
                    }

                    var sql = new SQLString(@"
declare @expiration datetime
set @expiration = dateadd(n,-$minutes,getdate())

select *,
	case when el_date < @expiration then 1 else 0 end [expired]
	from emailed_links
	where el_id = @guid

delete from emailed_links
	where el_date < dateadd(n,-240,getdate())");

                    sql = sql.AddParameterWithValue("minutes", Util.get_setting("RegistrationExpiration", "20"));
                    sql = sql.AddParameterWithValue("guid", guid);

                    DataRow dr = btnet.DbUtil.get_datarow(sql);

                    if (dr == null)
                    {
                        msg.InnerHtml = "The link you clicked on is expired or invalid.<br>Please start over again.";
                    }
                    else if ((int)dr["expired"] == 1)
                    {
                        msg.InnerHtml = "The link you clicked has expired.<br>Please start over again.";
                    }
                    else
                    {
                        Util.update_user_password((int)dr["el_user_id"], password.Value);
                        msg.InnerHtml = "Your password has been changed.";
                    }

                }
            }
        }

    }
}
