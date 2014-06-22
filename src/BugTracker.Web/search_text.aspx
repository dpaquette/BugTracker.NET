<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

#pragma warning disable 618

Security security;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

    
    security = new Security();
    security.check_security( HttpContext.Current, Security.ANY_USER_OK);

    Lucene.Net.Search.Query query = null;

    try
    {
        if (string.IsNullOrEmpty(Request["query"]))
        {
            throw new Exception("You forgot to enter something to search for...");
        }
        
        query = MyLucene.parser.Parse(Request["query"]);
        
    }
    catch (Exception e3)
    {
        display_exception(e3);
    }
    
    
    Lucene.Net.Highlight.QueryScorer scorer = new Lucene.Net.Highlight.QueryScorer(query);
    Lucene.Net.Highlight.Highlighter highlighter = new Lucene.Net.Highlight.Highlighter(MyLucene.formatter, scorer);
    highlighter.SetTextFragmenter(MyLucene.fragmenter); // new Lucene.Net.Highlight.SimpleFragmenter(400));

    StringBuilder sb = new StringBuilder();
    string guid = Guid.NewGuid().ToString().Replace("-", "");
    Dictionary<string, int> dict_already_seen_ids = new Dictionary<string, int>();

    sb.Append(@"
create table #$GUID
(
temp_bg_id int,
temp_bp_id int,
temp_source varchar(30),
temp_score float,
temp_text nvarchar(3000)
)
    ");

    lock (MyLucene.my_lock)
    {

        Lucene.Net.Search.Hits hits = null;
        try
        {
            hits = MyLucene.search(query);
        }
        catch (Exception e2)
        {
            display_exception(e2);
        }
        
        // insert the search results into a temp table which we will join with what's in the database
        for (int i = 0; i < hits.Length(); i++)
        {
            if (dict_already_seen_ids.Count < 100)
            {
                Lucene.Net.Documents.Document doc = hits.Doc(i);
                string bg_id = doc.Get("bg_id");
                if (!dict_already_seen_ids.ContainsKey(bg_id))
                {
                    dict_already_seen_ids[bg_id] = 1;
                    sb.Append("insert into #");
                    sb.Append(guid);
                    sb.Append(" values(");
                    sb.Append(bg_id);
                    sb.Append(",");
                    sb.Append(doc.Get("bp_id"));
                    sb.Append(",'");
                    sb.Append(doc.Get("src"));
                    sb.Append("',");
                    sb.Append(Convert.ToString((hits.Score(i))).Replace(",", "."));  // Somebody said this fixes a bug. Localization issue?
                    sb.Append(",N'");

                    string raw_text = Server.HtmlEncode(doc.Get("raw_text"));
                    Lucene.Net.Analysis.TokenStream stream = MyLucene.anal.TokenStream("", new System.IO.StringReader(raw_text));
                    string highlighted_text = highlighter.GetBestFragments(stream, raw_text, 1, "...").Replace("'", "''");
                    if (highlighted_text == "") // someties the highlighter fails to emit text...
                    {
                        highlighted_text = raw_text.Replace("'","''");
                    }
                    if (highlighted_text.Length > 3000)
                    {
						highlighted_text = highlighted_text.Substring(0,3000);
					}
                    sb.Append(highlighted_text);
                    sb.Append("'");
                    sb.Append(")\n");
                }
            }
            else
            {
                break;
            }
        }
        //searcher.Close();
    }


    sb.Append(@"
select '#ffffff', bg_id [id], bg_short_desc [desc] ,
	temp_source [search_source] ,
	temp_text [search_text],
	bg_reported_date [date],
	isnull(st_name,'') [status],
	temp_score [$SCORE]
from bugs
inner join #$GUID t on t.temp_bg_id = bg_id and t.temp_bp_id = 0
left outer join statuses on st_id = bg_status
where $ALTER_HERE

union

select '#ffffff', bg_id, bg_short_desc,
	bp_type + ',' + convert(varchar,bp_id) COLLATE DATABASE_DEFAULT,
	temp_text,
	bp_date,
	isnull(st_name,''),
	temp_score
from bugs
inner join #$GUID t on t.temp_bg_id = bg_id
inner join bug_posts on temp_bp_id = bp_id
left outer join statuses on st_id = bg_status
where $ALTER_HERE

order by t.temp_score desc, bg_id desc

drop table #$GUID

");

    string sql = sb.ToString().Replace("$GUID", guid);
    sql =  btnet.Util.alter_sql_per_project_permissions(sql, security);


    DataSet ds = btnet.DbUtil.get_dataset (sql);
    Session["bugs_unfiltered"] = ds.Tables[0];
    Session["bugs"] = new DataView(ds.Tables[0]);

    Session["just_did_text_search"] = "yes"; // switch for bugs.aspx
    Session["query"] = Request["query"]; // for util.cs, to persist the text in the search <input>
    Response.Redirect("bugs.aspx");
}

    void display_exception(Exception e)
    {
        string s = e.Message;
        if (e.InnerException != null)
        {
            s += "<br>";
            s+= e.InnerException.Message;
        }
        Response.Write(@"
<html>
<link rel=StyleSheet href=btnet.css type=text/css>
<p>&nbsp;</p>
<div class=align>
<div class=err>");
    
    
        Response.Write(s);

        Response.Write(@"
<p>
<a href='javascript:history.go(-1)'>back</a>            
</div></div>
</html>");
    
        Response.End();
        
    }    
    
</script>
