using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace btnet
{
    public partial class UpdatePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.Params.AllKeys.Contains("password_reset_key"))
            {
                passwordChangeDiv.Visible = false;
                resetKeyError.Visible = true;
            }

            if (IsPostBack)
            {
                ProcessReset();
            }
        }

        private void ProcessReset()
        {
            if ( newPassword.Value.Equals(newPasswordConfirm.Value, StringComparison.InvariantCulture))
            {
                oldPasswordError.Visible = true;
            }

            var resetKey = Request.Params["password_reset_key"];
            var username = Request.Params["user_name"];
            var salt = GetSalt(username);
            var hashedOldPassword = GetOldHashedPassword(oldPassword.Value, salt);
            if (IsValidPassword(username, hashedOldPassword, resetKey))
            {
                if (Util.check_password_strength(newPassword.Value))
                {
                    Util.update_user_password(username, newPassword.Value);
                    Response.Redirect("/default.aspx");
                }
                else
                {
                    newPasswordLacksComplexityError.Visible = true;
                }
            }
            
        }

        private string GetSalt(string username)
        {
            var sql = new SQLString("select us_salt from users where us_username = @username");
            sql.AddParameterWithValue("@username", username);
            var result = ((string)btnet.DbUtil.execute_scalar(sql));
            return result;
        }

        private string GetOldHashedPassword(string oldPassword, string salt)
        {
           var encodedPassword = System.Text.Encoding.Default.GetBytes(oldPassword + salt);

            var alg = System.Security.Cryptography.HashAlgorithm.Create("MD5");

            var hashedPassword = alg.ComputeHash(encodedPassword);

            var stringBuilder = new System.Text.StringBuilder(hashedPassword.Length);

            foreach (byte b in hashedPassword)
            {
                stringBuilder.AppendFormat("{0:X2}", b);
            }

            return stringBuilder.ToString();
        }

        private bool IsValidPassword(string username, string hashedPassword, string resetKey)
        {
            var sql = new SQLString("select count(*) from users where us_username = @username and us_password = @password and password_reset_key = @resetKey");
            sql.AddParameterWithValue("@username", username);
            sql.AddParameterWithValue("@password", hashedPassword);
            sql.AddParameterWithValue("@resetKey", resetKey);
            return ((int)btnet.DbUtil.execute_scalar(sql)) > 0;
        }
    }
}