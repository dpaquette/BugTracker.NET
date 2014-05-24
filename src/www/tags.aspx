<%@ Page language="C#"%>
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

Security security;

class TagLabel : IComparable<TagLabel>
{
	public int count;
	public string label;
	public int CompareTo(TagLabel other)
	{
		if (this.count > other.count)
			return -1;
		else if (this.count < other.count)
			return 1;
		else
			return 0;
	}
}


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
    
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

}

void print_tags()
{

    if (security.user.category_field_permission_level == Security.PERMISSION_NONE)
    {
		return;
	}

    SortedDictionary<string, List<int>> tags =
        (SortedDictionary<string, List<int>>)Application["tags"];

    List<TagLabel> tags_by_count = new List<TagLabel>();

	Dictionary<string, string> fonts = new Dictionary<string, string>();

    foreach (string s in tags.Keys)
    {
		TagLabel tl = new TagLabel();
		tl.count = tags[s].Count;
		tl.label = s;
		tags_by_count.Add(tl);
    }

	tags_by_count.Sort(); // sort in descending count order

	float total = tags.Count;
	float so_far = 0.0F;
	int previous_count = -1;
	string previous_font = "";

	foreach (TagLabel tl in tags_by_count)
	{
		so_far++;

		if (tl.count == previous_count)
			fonts[tl.label] = previous_font; // if same count, then same font
		else if (so_far/total < .1)
			fonts[tl.label] = "24pt";
		else if (so_far/total < .2)
			fonts[tl.label] = "22pt";
		else if (so_far/total < .3)
			fonts[tl.label] = "20pt";
		else if (so_far/total < .4)
			fonts[tl.label] = "18pt";
		else if (so_far/total < .5)
			fonts[tl.label] = "16pt";
		else if (so_far/total < .6)
			fonts[tl.label] = "14pt";
		else if (so_far/total < .7)
			fonts[tl.label] = "12pt";
		else if (so_far/total < .8)
			fonts[tl.label] = "10pt";
		else
			fonts[tl.label] = "8pt";

		previous_font = fonts[tl.label];
		previous_count = tl.count;
	}


    foreach (string s in tags.Keys)
    {


        Response.Write("\n<a style='font-size:");
		Response.Write(fonts[s]);
		Response.Write(";' href='javascript:opener.append_tag(\"");

		Response.Write(s.Replace("'","%27"));

        Response.Write("\")'>");

        Response.Write(s);

		Response.Write("(");
		Response.Write(tags[s].Count);
        Response.Write(")</a>&nbsp;&nbsp; ");

    }

}

</script>
<html>
<head>
<title>Tags</title>
<script>
function do_unload()
{
    opener.done_selecting_tags()
}
</script>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body onunload="do_unload()">

<p>
<% print_tags(); %>

</body>
</html>