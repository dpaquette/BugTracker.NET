<%@ Page language="C#"%>

<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

/*

The server sends back the current time and a list of the 
bugs since the "since" value.   The data is formated as JSON.

{
"now" : 9999,
"news_list" : [ 
		  	{
		  		"seconds":12,
		  		"bugid": 34,
		  		"desc": "foo",
		  		"action": "add",
		  		"who" : "ctrager",
		  	},

		  	{
		  		"seconds":12,
		  		"bugid": 34,
		  		"desc": "foo",
		  		"action": "add",
		  		"who" : "ctrager",
		  	},
		  	
		  ]

}

*/

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);

	if (Util.get_setting("EnableWhatsNewPage","0") == "0")
	{
		Response.Write("Sorry, Web.config EnableWhatsNewPage is set to 0");
		Response.End();
	}
	
	string since_string = Request["since"];
	if (string.IsNullOrEmpty(since_string))
	{
		since_string = "0";
	}
	
	long since = Convert.ToInt64(since_string);
	

	Response.ContentType = "application/json";
	
	StringBuilder json = new StringBuilder();
	
	json.Append("{");
	
	// The web server's time.  The client javascript will use this a a reference point.
	append_json_var_val(json, "now", Convert.ToString(System.DateTime.Now.Ticks/WhatsNew.ten_million));
	
	// Serialize an array of BugNews objects
	json.Append(",\"news_list\":[");

	List<BugNews> list = (List<BugNews>) Application["whatsnew"];
	
	bool first_news = true;
	if (list != null)
	{
		for (int i = 0; i < list.Count; i++)
		{
			BugNews news = list[i];
			if (news.seconds > since)
			{
				if (first_news)
				{
					first_news = false;					
				}					
				else
				{
					json.Append(",");
				}
				
				// Serialize BugNews object
				json.Append("{");
					append_json_var_val(json, "seconds", news.seconds_string);
					json.Append(",");
					append_json_var_val(json, "bugid", news.bugid);
					json.Append(",");
					append_json_var_val(json, "desc", HttpUtility.HtmlEncode(news.desc));
					json.Append(",");
					append_json_var_val(json, "action", news.action);
					json.Append(",");
					append_json_var_val(json, "who", news.who);
				json.Append("}");
			}
		}
	}

	json.Append("]}");

	Response.Write(json.ToString());

}

string escape_for_json(string s)
{

	return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}


void append_json_var_val(StringBuilder json, string var, string val)
{
	json.Append("\"");
	json.Append(var);
	json.Append("\":\"");
	json.Append(escape_for_json(val));
	json.Append("\"");
}

</script>