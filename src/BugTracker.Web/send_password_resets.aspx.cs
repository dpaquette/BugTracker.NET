using System;
using System.Linq;
using System.Net.Mail;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class send_password_resets : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {          
            var sql = new SQLString("select us_id, us_email, us_username from users");
            using (var reader = DbUtil.execute_reader(sql, System.Data.CommandBehavior.Default))
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var updateQuery = new SQLString("update users set password_reset_key=@resetKey where us_id = @id");
                    updateQuery.AddParameterWithValue("@id", id);

                    var resetKey = Util.GenerateRandomString();
                    updateQuery.AddParameterWithValue("@resetKey", resetKey);

                    var emailAddress = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    var username = reader.GetString(2);
                    DbUtil.execute_nonquery(updateQuery);
                    SendMail(emailAddress, resetKey, username);
                }
            }
        }

        private void SendMail(string emailAddress, string resetKey, string username)
        {
            if (!String.IsNullOrWhiteSpace(emailAddress))
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress(emailAddress));
                message.Subject = "BugTracker Password Reset";
                message.IsBodyHtml = true;
                message.Body = String.Format("Hi, we're resetting your password. Click <a href='{0}UpdatePassword.aspx?password_reset_key={1}&user_name={2}'>here</a>", System.Configuration.ConfigurationManager.AppSettings["AbsoluteUrlPrefix"], resetKey, username);
                var client = new SmtpClient();
                client.Send(message);
            }
        }
    }
}