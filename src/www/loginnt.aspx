<%@ Page Language="C#" %>

<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">



string sql;


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	
	btnet.DbUtil.get_sqlconnection();

	Util.do_not_cache(Response);

	// Get authentication mode
	string auth_mode = Util.get_setting("WindowsAuthentication","0");


	// If manual authentication only, we shouldn't be here, so redirect to manual screen

	if (auth_mode == "0")
	{
		btnet.Util.redirect("default.aspx", Request, Response);
	}

	// Get the logon user from IIS
	string domain_windows_username = Request.ServerVariables["LOGON_USER"];

	if (domain_windows_username == "")
	{
		// If the logon user is blank, then the page is misconfigured
		// in IIS. Do nothing and let the HTML display.
	}
	else
	{

		// Extract the user name from the logon ID
		int pos = domain_windows_username.IndexOf("\\") + 1;
		string windows_username =
			domain_windows_username.Substring(pos, domain_windows_username.Length-pos);


		// Fetch the user's information from the users table
		sql = @"select us_id, us_username
			from users
			where us_username = N'$us'
			and us_active = 1";
		sql = sql.Replace("$us", windows_username.Replace("'","''"));

		DataRow dr = btnet.DbUtil.get_datarow(sql);
		if (dr != null)
		{
			// The user was found, so bake a cookie and redirect
			int userid = (int) dr["us_id"];
			btnet.Security.create_session (
				Request,
				Response,
				userid,
				(string) dr["us_username"],
				"1");

            btnet.Util.update_most_recent_login_datetime(userid);
				
			btnet.Util.redirect(Request, Response);
		}

        // Is self register enabled for users authenticated by windows?
        // If yes, then automatically insert a row in the user table
        bool enable_auto_registration = (Util.get_setting("EnableWindowsUserAutoRegistration","1") == "1");
        if (enable_auto_registration)
        {
            string template_user = Util.get_setting("WindowsUserAutoRegistrationUserTemplate", "guest");

            string first_name = windows_username;
            string last_name = windows_username;
            string signature = windows_username;
            string email = string.Empty;

            // From the browser, we only know the Windows username.  Maybe we can get the other
            // info from LDAP?
            if (Util.get_setting("EnableWindowsUserAutoRegistrationLdapSearch", "0") == "1")
            {
                using (System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry())
                {
                    de.Path = Util.get_setting("LdapDirectoryEntryPath","LDAP://127.0.0.1/DC=mycompany,DC=com");

                    de.AuthenticationType =
                    	(System.DirectoryServices.AuthenticationTypes)System.Enum.Parse(
						typeof(System.DirectoryServices.AuthenticationTypes),
                    	Util.get_setting("LdapDirectoryEntryAuthenticationType", "Anonymous"));

                    de.Username = Util.get_setting("LdapDirectoryEntryUsername", "");
                    de.Password = Util.get_setting("LdapDirectoryEntryPassword", "");

                    using (System.DirectoryServices.DirectorySearcher search =
                    	new System.DirectoryServices.DirectorySearcher(de))
                    {
                        string search_filter = Util.get_setting("LdapDirectorySearcherFilter", "(uid=$REPLACE_WITH_USERNAME$)");
                        search.Filter = search_filter.Replace("$REPLACE_WITH_USERNAME$", windows_username);
                        System.DirectoryServices.SearchResult result = null;

						try
						{
							result = search.FindOne();
                            if (result != null)
                            {
                                first_name = get_ldap_property_value(result, Util.get_setting("LdapFirstName", "gn"), first_name);
                                last_name = get_ldap_property_value(result, Util.get_setting("LdapLastName", "sn"), last_name);
                                email = get_ldap_property_value(result, Util.get_setting("LdapEmail", "mail"), email); ;
                                signature = get_ldap_property_value(result, Util.get_setting("LdapEmailSigniture", "cn"), signature); ;
                            }
                            else 
                            {
                                btnet.Util.write_to_log("LDAP search.FindOne() result = null");
                            }
						}
						catch (Exception e2)
						{
							string s = e2.Message;

							if (e2.InnerException != null)
							{
								s += "\n";
								s += e2.InnerException.Message;
							}

							// write the message to the log
							btnet.Util.write_to_log("LDAP search failed: " + s);
						}

                    }
                }
            }

            
			int new_user_id = btnet.User.copy_user(
				windows_username,
				email,
				first_name,
				last_name,
                signature,
				0, // salt
				Guid.NewGuid().ToString(), // random value for password
				template_user,
                false); 

			if (new_user_id > 0) // automatically created the user
			{
				// The user was created, so bake a cookie and redirect
				btnet.Security.create_session(
					Request,
					Response,
					new_user_id,
					windows_username.Replace("'", "''"),
					"1");
					
	            btnet.Util.update_most_recent_login_datetime(new_user_id);
					
				btnet.Util.redirect(Request, Response);
			}
        }


		// Try fetching the guest user.
		sql = @"select us_id, us_username
			from users
			where us_username = 'guest'
			and us_active = 1";

		dr = btnet.DbUtil.get_datarow(sql);
		if (dr != null)
		{
			// The Guest user was found, so bake a cookie and redirect
			int userid = (int) dr["us_id"];
			btnet.Security.create_session (
				Request,
				Response,
				userid,
				(string) dr["us_username"],
				"1");
				
			btnet.Util.update_most_recent_login_datetime(userid);				
			
			btnet.Util.redirect(Request, Response);
		}

		// If using mixed-mode authentication and we got this far,
		// then we can't sign in using integrated security. Redirect
		// to the manual screen.
		if (auth_mode != "1")
		{
			btnet.Util.redirect("default.aspx?msg=user+not+valid", Request, Response);
		}

		// If we are still here, then toss a 401 error.
		Response.StatusCode = 401;
		Response.End();
	}
}

///////////////////////////////////////////////////////////////////////
string get_ldap_property_value(System.DirectoryServices.SearchResult result, string propertyName, string defaultValue)
{
	System.DirectoryServices.ResultPropertyValueCollection values = result.Properties[propertyName];
	if (values != null && values.Count == 1 && values[0] is string)
	{
		return (string)values[0];
	}
	else
	{
		return defaultValue;
	}
}



</script>

<html>
<head>
<title>btnet logon</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
    <h1>
        Configuration Problem</h1>
    <p>
        This page has not been properly configured for Windows Integrated Authentication.
        Please contact your web administrator.</p>
    <p>
        Windows Integrated Authentication requires that this page (loginNT.aspx) does not
        permit anonymous access and Windows Integrated Security is selected as the authentication
        protocol.</p>
    <p>
        <a href="default.aspx?msg=configuration+problem">Go to logon page.</a></p>
</body>
</html>