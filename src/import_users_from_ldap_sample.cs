/*

Copyright 2008 Corey Trager
Distributed under the terms of the GNU General Public License

Use this as a starting point for importing users from an LDAP server (like Active Directory).

Search LDAP for users.  Iterate through the users.  For each user, generate a SQL "INSERT" statement
to insert the user into the BugTracker.NET database.   Write the SQL to stdout.

I tested this using OpenLDAP. 

- Corey Trager, Jan 2008. 
 
 */ 
using System;
using System.DirectoryServices;

namespace btnet
{
    class Program
    {
        static void Main(string[] args)
        {
            string sql = @"insert into users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query, us_org)
values (N'$uid', N'$gn', N'$sn', N'$password', 0, 1, 1);
";

            try
            {
                DirectoryEntry de = new DirectoryEntry("LDAP://127.0.0.1/dc=mycompany,dc=com");
                de.AuthenticationType = AuthenticationTypes.None;
                DirectorySearcher search = new DirectorySearcher(de);
                search.SearchScope = System.DirectoryServices.SearchScope.Subtree;
                search.Filter = "objectClass=inetOrgPerson";
                SearchResultCollection results = search.FindAll();

                foreach (SearchResult result in results)
                {
                    System.Diagnostics.Debug.Print(result.Path);

                    DirectoryEntry de2 = result.GetDirectoryEntry();

                    string uid = (string)de2.Properties["uid"].Value;
                    
                    string gn = (string)de2.Properties["gn"].Value;
                    if (gn == null) gn = "";
                    
                    string sn = (string)de2.Properties["sn"].Value;
                    if (sn == null) sn = "";
                    
                    string password = "unsafe_password";

                    string insert_sql = sql;
                    insert_sql = insert_sql.Replace("$uid",uid);
                    insert_sql = insert_sql.Replace("$gn",gn);
                    insert_sql = insert_sql.Replace("$sn",sn);
                    insert_sql = insert_sql.Replace("$password", password);
                    System.Diagnostics.Debug.Print(insert_sql);
                    System.Console.WriteLine(insert_sql);
                }
            

            }
            catch (Exception e)
            {
                string s = e.Message;
                if (e.InnerException != null)
                {
                    s += "\n";
                    s += e.InnerException.Message;
                }
                System.Diagnostics.Debug.Print(s);
            }

            System.Diagnostics.Debug.Print("bye");
            
        } // end main
    }; // end class
} // end namespace
