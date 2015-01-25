using System;
using System.Data;
using System.Web;
using btnet.Mail;
using btnet.Security;

namespace btnet
{
    [PageAllowAnonymous]
    public partial class forgot : BasePage
    {

        ///////////////////////////////////////////////////////////////////////
        public void Page_Load(Object sender, EventArgs e)
        {

            Util.set_context(HttpContext.Current);
            Util.do_not_cache(Response);

            if (Util.get_setting("ShowForgotPasswordLink", "0") == "0")
            {
                Response.Write("Sorry, Web.config ShowForgotPasswordLink is set to 0");
                Response.End();
            }

            if (!IsPostBack)
            {
                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "forgot password";
            }
            else
            {
                msg.InnerHtml = "";

                if (email.Value == "" && username.Value == "")
                {
                    msg.InnerHtml = "Enter either your Username or your Email address.";
                }
                else if (email.Value != "" && !Util.validate_email(email.Value))
                {
                    msg.InnerHtml = "Format of email address is invalid.";
                }
                else
                {

                    int user_count = 0;
                    int user_id = 0;

                    if (email.Value != "" && username.Value == "")
                    {

                        // check if email exists
                        SQLString sql = new SQLString("select count(1) from users where us_email = @email");
                        sql.AddParameterWithValue("email", email.Value);
                        user_count = (int)DbUtil.execute_scalar(sql);

                        if (user_count == 1)
                        {
                            sql = new SQLString("select us_id from users where us_email = @email");
                            sql.AddParameterWithValue("email", email.Value);
                            user_id = (int)DbUtil.execute_scalar(sql);
                        }


                    }
                    else if (email.Value == "" && username.Value != "")
                    {
                        // check if email exists
                        SQLString sql = new SQLString(
                                                    "select count(1) from users where isnull(us_email,'') != '' and  us_username = @username");
                        sql.AddParameterWithValue("username", username.Value);
                        user_count = (int)DbUtil.execute_scalar(sql);

                        if (user_count == 1)
                        {
                            sql = new SQLString("select us_id from users where us_username = @username");
                            sql.AddParameterWithValue("username", username.Value);
                            user_id = (int)DbUtil.execute_scalar(sql);
                        }
                    }
                    else if (email.Value != "" && username.Value != "")
                    {
                        // check if email exists
                        SQLString sql = new SQLString(
                                                    "select count(1) from users where us_username = @username and us_email = @email");
                        sql.AddParameterWithValue("username", username.Value);
                        sql.AddParameterWithValue("email", email.Value);
                        user_count = (int)DbUtil.execute_scalar(sql);

                        if (user_count == 1)
                        {
                            sql = new SQLString(
                                                            "select us_id from users where us_username = @username and us_email = @email");
                            sql.AddParameterWithValue("username", username.Value);
                            sql.AddParameterWithValue("email", email.Value);
                            user_id = (int)DbUtil.execute_scalar(sql);
                        }
                    }


                    if (user_count == 1)
                    {
                        string guid = Guid.NewGuid().ToString();
                        var sql = new SQLString(@"
declare @username nvarchar(255)
declare @email nvarchar(255)

select @username = us_username, @email = us_email
	from users where us_id = @user_id

insert into emailed_links
	(el_id, el_date, el_email, el_action, el_user_id)
	values (@guid, getdate(), @email, N'forgot', @user_id)

select @username us_username, @email us_email");

                        sql = sql.AddParameterWithValue("guid", guid);
                        sql = sql.AddParameterWithValue("user_id", Convert.ToString(user_id));

                        DataRow dr = DbUtil.get_datarow(sql);

                        string result = Email.send_email(
                            (string)dr["us_email"],
                            Util.get_setting("NotificationEmailFrom", ""),
                            "", // cc
                            "reset password",

                            "Click to <a href='"
                                + Util.get_setting("AbsoluteUrlPrefix", "")
                                + "change_password.aspx?id="
                                + guid
                                + "'>reset password</a> for user \""
                                + (string)dr["us_username"]
                                + "\".",

                            MailFormat.Html);

                        if (result == "")
                        {
                            msg.InnerHtml = "An email with password info has been sent to you.";
                        }
                        else
                        {
                            msg.InnerHtml = "There was a problem sending the email.";
                            msg.InnerHtml += "<br>" + result;
                        }
                    }
                    else
                    {
                        msg.InnerHtml = "Unknown username or email address.<br>Are you sure you spelled everything correctly?<br>Try just username, just email, or both.";
                    }
                }
            }
        }

    }
}
