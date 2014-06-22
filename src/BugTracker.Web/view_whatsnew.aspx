<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


Security security;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	if (btnet.Util.get_setting("EnableWhatsNewPage","0") != "1")
	{
		Response.End();
	}

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "news?";

}

</script>


<html>
<title id="titl" runat="server">btnet news</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="jquery/jquery-1.3.2.min.js"></script>

<script>

var json = {}
var since = 0
var table_start = "<table border=1 class=datat><tr><td class=datah>when<td class=datah>id<td class=datah>desc<td class=datah>action<td class=datah>user"
var table_end = "</table>"
var seconds_in_a_day = 86400
var seconds_in_an_hour = 3600
var my_news_list = new Array()

function how_long_ago(seconds)
{

	// turn seconds ago into a friendly piece of text
	
    var days = Math.floor(seconds/seconds_in_a_day)
    seconds -= days*seconds_in_a_day;
    var hours = Math.floor(seconds/seconds_in_an_hour)
    seconds -= hours*seconds_in_an_hour;
    var minutes = Math.floor(seconds/60)
    seconds -= minutes*60;

	if (days > 0)
	{
		if (days == 1)
		{
			if (hours > 2)
			{
				return "1 day and " + hours + " hours ago";
			}
			else
			{
				return "1 day ago";
			}
		}
		else
		{
			return days + " days ago";
		}
	}
	else if (hours > 0)
	{
		if (hours == 1)
		{
			if (minutes > 5)
			{
				return "1 hour and " + minutes + " minutes ago";
			}
			else
			{
				return "1 hour ago";
			}
		}
		else
		{
			return hours + " hours ago";
		}
	}
	else if (minutes > 0)
	{
		if (minutes == 1)
		{
			return "1 minute ago";
		}
		else
		{
			return minutes + " minutes ago";
		}
	}
	else
	{
		return seconds + " seconds ago";
	}
}


function get_color(seconds_ago)
{
	if (seconds_ago < 90)
	{
		return "red"
	}
	else if (seconds_ago < 180)
	{
		return "orange"
	}
	else if (seconds_ago < 300)
	{
		return "yellow"
	}
	else
	{
		return "white"
	}
}

function process_json(json)
{

	for (i = 0; i < json.news_list.length; i++)
	{

		var news = json.news_list[i]

		my_news_list.push(news)

		if (news.seconds > since)
		{
			since = news.seconds
		}

	}

	// iterate backwards through all the news retrieved, updating the "how long ago"

	var table_rows = "" 

	for (i = my_news_list.length - 1; i > -1 ; i--)
	{

		news = my_news_list[i]

		var seconds_ago = json.now - news.seconds

		var tr = "<tr><td class=datad style='background:" 
			+ get_color(seconds_ago) + "'>" 
			+ how_long_ago(seconds_ago)
			+ "<td class=datad>" + news.bugid
			+ "<td class=datad><a href=edit_bug.aspx?id=" + news.bugid + ">" + news.desc + "</a>"  
			+ "<td class=datad>" + news.action
			+ "<td class=datad>" + news.who

		table_rows += tr

	}

	el = document.getElementById("news_table")
	el.innerHTML = table_start + table_rows + table_end

}

function get_news()
{

	$.ajax(
		{
			type: "GET",
			url: "whatsnew.aspx?since=" + since,

			cache: false,
			timeout:20000, 
			dataType: "json",

			success: function(data){
				//alert(data)
				process_json(data)
				setTimeout(get_news, 1000 * <% Response.Write(btnet.Util.get_setting("WhatsNewPageIntervalInSeconds","20")); %> ); 
				},

			error: function(XMLHttpRequest, textStatus, errorThrown) {
				setTimeout(get_news, 60000);
				}
		}
	);
};


$(document).ready(function()
{
	get_news(); 
});

</script>

<body>

<% security.write_menu(Response, "news"); %>

<table border=0 cellspacing=0 cellpadding=10>
<tr>
<td valign=top>

<div id="news_table">&nbsp</div>

</table>

</body>
</html>