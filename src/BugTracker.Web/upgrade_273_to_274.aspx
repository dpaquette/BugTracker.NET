<%@ Page language="C#"%>
<!--

Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License


If you are upgrading from version 2.7.3 or earlier to version 2.7.4 or later,
invoke this page.   It encrypts user passwords consistent with 2.7.4+.


-->

<% @Import Namespace="btnet" %>
<% @Import Namespace="System.Data" %>

<script language="C#" runat="server">



///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	

	Random random = new Random();

	string sql = "select us_username, us_id, us_password from users where len(us_password) < 32";


	DataSet ds = btnet.DbUtil.get_dataset(sql);
	foreach (DataRow dr in ds.Tables[0].Rows)
	{
        System.Threading.Thread.Sleep(10); // give time for the random number to seed differently;
        string us_username = (string) dr["us_username"];
		int us_id = (int) dr["us_id"];
		string us_password = (string) dr["us_password"];
		{
			Response.Write ("encrypting " + us_username + "<br>");
            btnet.Util.update_user_password(us_id, us_password);
		}
	}

	Response.Write ("done encrypting unencrypted passwords");

}



</script>