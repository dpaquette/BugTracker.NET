/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.Protocols;
using System.Web;
using System.Collections.Generic;

namespace btnet
{
    public class Authenticate
    {

        public static bool check_password(string username, string password)
        {

            string sql = @"
select us_username, us_id, us_password, isnull(us_salt,0) us_salt, us_active
from users
where us_username = N'$username'";

            sql = sql.Replace("$username", username.Replace("'", "''"));

            DataRow dr = btnet.DbUtil.get_datarow(sql);

            if (dr == null)
            {
                Util.write_to_log("Unknown user " + username + " attempted to login.");
                return false;
            }

            int us_active = (int)dr["us_active"];

            if (us_active == 0)
            {
                Util.write_to_log("Inactive user " + username + " attempted to login.");
                return false;
            }

            bool authenticated = false;
            LinkedList<DateTime> failed_attempts = null;

            // Too many failed attempts?
            // We'll only allow N in the last N minutes.
            failed_attempts = (LinkedList<DateTime>)HttpRuntime.Cache[username];

            if (failed_attempts != null)
            {
                // Don't count attempts older than N minutes ago.
                int minutes_ago = Convert.ToInt32(btnet.Util.get_setting("FailedLoginAttemptsMinutes", "10"));
                int failed_attempts_allowed = Convert.ToInt32(btnet.Util.get_setting("FailedLoginAttemptsAllowed", "10"));

                DateTime n_minutes_ago = DateTime.Now.AddMinutes(-1 * minutes_ago);
                while (true)
                {
                    if (failed_attempts.Count > 0)
                    {
                        if (failed_attempts.First.Value < n_minutes_ago)
                        {
                            Util.write_to_log("removing stale failed attempt for " + username);
                            failed_attempts.RemoveFirst();
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                // how many failed attempts in last N minutes?
                Util.write_to_log("failed attempt count for " + username + ":" + Convert.ToString(failed_attempts.Count));

                if (failed_attempts.Count > failed_attempts_allowed)
                {
                    Util.write_to_log("Too many failed login attempts in too short a time period: " + username);
                    return false;
                }

                // Save the list of attempts
                HttpRuntime.Cache[username] = failed_attempts;
            }

            if (btnet.Util.get_setting("AuthenticateUsingLdap", "0") == "1")
            {
                authenticated = check_password_with_ldap(username, password);
            }
            else
            {
                authenticated = check_password_with_db(username, password, dr);
            }

            if (authenticated)
            {
                // clear list of failed attempts
                if (failed_attempts != null)
                {
                    failed_attempts.Clear();
                    HttpRuntime.Cache[username] = failed_attempts;
                }

                btnet.Util.update_most_recent_login_datetime((int)dr["us_id"]);
                return true;
            }
            else
            {
                if (failed_attempts == null)
                {
                    failed_attempts = new LinkedList<DateTime>();
                }

                // Record a failed login attempt.
                failed_attempts.AddLast(DateTime.Now);
                HttpRuntime.Cache[username] = failed_attempts;

                return false;
            }
        }

        public static bool check_password_with_ldap(string username, string password)
        {
            // allow multiple, seperated by a pipe character
            string dns = btnet.Util.get_setting(
                "LdapUserDistinguishedName",
                "uid=$REPLACE_WITH_USERNAME$,ou=people,dc=mycompany,dc=com");

            string[] dn_array = dns.Split('|');

            string ldap_server = btnet.Util.get_setting(
                "LdapServer",
                "127.0.0.1");

            using (LdapConnection ldap = new LdapConnection(ldap_server))
            {

                for (int i = 0; i < dn_array.Length; i++)
                {
                    string dn = dn_array[i].Replace("$REPLACE_WITH_USERNAME$", username);

                    System.Net.NetworkCredential cred = new System.Net.NetworkCredential(dn, password);

                    ldap.AuthType = (System.DirectoryServices.Protocols.AuthType)System.Enum.Parse
                        (typeof(System.DirectoryServices.Protocols.AuthType),
                        Util.get_setting("LdapAuthType", "Basic"));

                    try
                    {
                        ldap.Bind(cred);
                        btnet.Util.write_to_log("LDAP authentication ok using " + dn + " for username: " + username);
                        return true;
                    }
                    catch (Exception e)
                    {
                        string exception_msg = e.Message;

                        if (e.InnerException != null)
                        {
                            exception_msg += "\n";
                            exception_msg += e.InnerException.Message;
                        }

                        btnet.Util.write_to_log("LDAP authentication failed using " + dn + ": " + exception_msg);
                    }
                }
            }

            return false;
        }

        public static bool check_password_with_db(string username, string password, DataRow dr)
        {

            int us_salt = (int)dr["us_salt"];

            string encrypted;

            string us_password = (string)dr["us_password"];

            if (us_password.Length < 32) // if password in db is unencrypted
            {
                encrypted = password; // in other words, unecrypted
            }
            else if (us_salt == 0)
            {
                encrypted = Util.encrypt_string_using_MD5(password);
            }
            else
            {
                encrypted = Util.encrypt_string_using_MD5(password + Convert.ToString(us_salt));
            }


            if (encrypted == us_password)
            {
                // Authenticated, but let's do a better job encrypting the password.
                // If it is not encrypted, or, if it is encrypted without salt, then
                // update it so that it is encrypted WITH salt.
                if (us_salt == 0 || us_password.Length < 32)
                {
                    btnet.Util.update_user_password((int)dr["us_id"], password);
                }
                return true;
            }
            else
            {
                Util.write_to_log("User " + username + " entered an incorrect password.");
                return false;
            }
        }
    }

}
