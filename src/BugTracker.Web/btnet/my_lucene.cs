/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Web;
using System.Data;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using Lucene.Net;

#pragma warning disable 618

namespace btnet
{
	public class MyLucene
	{
        public static string index_path = btnet.Util.get_lucene_index_folder();

        public static Lucene.Net.Analysis.Standard.StandardAnalyzer anal = new Lucene.Net.Analysis.Standard.StandardAnalyzer();
        public static Lucene.Net.QueryParsers.QueryParser parser = new Lucene.Net.QueryParsers.QueryParser("text", anal);
        public static Lucene.Net.Highlight.Formatter formatter = new Lucene.Net.Highlight.SimpleHTMLFormatter(
                    "<span class='highlighted'>",
                    "</span>");
        
        public static Lucene.Net.Highlight.SimpleFragmenter fragmenter = new Lucene.Net.Highlight.SimpleFragmenter(400);
        protected static Lucene.Net.Search.Searcher searcher = null;

        public static object my_lock = new object(); // for a lock

        ///////////////////////////////////////////////////////////////////////
        static Lucene.Net.Documents.Document create_doc(int bug_id, int post_id, string source, string text)
        {
           // btnet.Util.write_to_log("indexing " + Convert.ToString(bug_id));

            Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
            
            //Fields f = new Lucene.Net.Documents.Field(
                            
            doc.Add(new Lucene.Net.Documents.Field(
                "bg_id",
                Convert.ToString(bug_id),
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.UN_TOKENIZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "bp_id",
                Convert.ToString(post_id),
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.UN_TOKENIZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "src",
                source,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.UN_TOKENIZED));

            // For the highlighter, store the raw text
            doc.Add(new Lucene.Net.Documents.Field(
                "raw_text",
                text,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.UN_TOKENIZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "text",
                new System.IO.StringReader(text)));

            return doc;
        }

        ///////////////////////////////////////////////////////////////////////
		static DataSet get_text_custom_cols()
		{
			DataSet ds_custom_fields  = btnet.DbUtil.get_dataset(@"
/* get searchable cols */					
select sc.name
from syscolumns sc
inner join systypes st on st.xusertype = sc.xusertype
inner join sysobjects so on sc.id = so.id
where so.name = 'bugs'
and st.[name] <> 'sysname'
and sc.name not in ('rowguid',
'bg_id',
'bg_short_desc',
'bg_reported_user',
'bg_reported_date',
'bg_project',
'bg_org',
'bg_category',
'bg_priority',
'bg_status',
'bg_assigned_to_user',
'bg_last_updated_user',
'bg_last_updated_date',
'bg_user_defined_attribute',
'bg_project_custom_dropdown_value1',
'bg_project_custom_dropdown_value2',
'bg_project_custom_dropdown_value3',
'bg_tags')
and st.[name] in ('nvarchar','varchar')
and sc.length > 30");					
				
			return ds_custom_fields;				
		}

        ///////////////////////////////////////////////////////////////////////
		static string get_text_custom_cols_names(DataSet ds_custom_fields)
		{
		
			string custom_cols = "";
			foreach (DataRow dr in ds_custom_fields.Tables[0].Rows)
			{
				custom_cols += "[" + (string) dr["name"] + "],";
			}		
			return custom_cols;
		
		}

        ///////////////////////////////////////////////////////////////////////
        // create a new index
        static void threadproc_build(object obj)
		{
            lock (my_lock)
            {
                try
                {
                    System.Web.HttpApplicationState app = (System.Web.HttpApplicationState)obj;

                    btnet.Util.write_to_log("started creating Lucene index using folder " + MyLucene.index_path);
                    Lucene.Net.Index.IndexWriter writer = new Lucene.Net.Index.IndexWriter(index_path, anal, true);



					
					string sql = @"
select bg_id, 	
$custom_cols
isnull(bg_tags,'') bg_tags,
bg_short_desc
from bugs";
					DataSet ds_text_custom_cols = get_text_custom_cols();
					
					sql = sql.Replace("$custom_cols", get_text_custom_cols_names(ds_text_custom_cols));

                    // index the bugs
                    DataSet ds = btnet.DbUtil.get_dataset(sql);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
						// desc
						writer.AddDocument(MyLucene.create_doc(
							(int)dr["bg_id"],
							0,
							"desc",
							(string)dr["bg_short_desc"]));

						// tags
						string tags = (string) dr["bg_tags"];
                        if (tags != "")
                        {
							writer.AddDocument(MyLucene.create_doc(
								(int)dr["bg_id"],
								0,
								"tags",
								tags));
						}
                            
						// custom text fields
						foreach (DataRow dr_custom_col in ds_text_custom_cols.Tables[0].Rows)                            
						{
							string name = (string) dr_custom_col["name"];
							string val = Convert.ToString(dr[name]);
							if (val != "")
							{
								writer.AddDocument(MyLucene.create_doc(
									(int)dr["bg_id"],
									0,
									name.Replace("'","''"),
									val));
							}
						}
                    }

                    // index the bug posts
                    ds = btnet.DbUtil.get_dataset(@"
select bp_bug, bp_id, 
isnull(bp_comment_search,bp_comment) [text] 
from bug_posts 
where bp_type <> 'update'
and bp_hidden_from_external_users = 0");

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        writer.AddDocument(MyLucene.create_doc(
                            (int)dr["bp_bug"],
                            (int)dr["bp_id"],
                            "post",
                            (string)dr["text"]));
                    }

                    writer.Optimize();
                    writer.Close();
                    btnet.Util.write_to_log("done creating Lucene index");
                }
                catch (Exception e)
                {
                    btnet.Util.write_to_log("exception building Lucene index: " + e.Message);
                    btnet.Util.write_to_log(e.StackTrace);
                }
            }
		}

        public static Lucene.Net.Search.Hits search(Lucene.Net.Search.Query query)
        {
            Lucene.Net.Search.Hits hits = null;
            lock (my_lock) // prevent contention between searches and writing?
            {
                if (searcher == null)
                {
                    searcher = new Lucene.Net.Search.IndexSearcher(MyLucene.index_path);
                }
                hits = searcher.Search(query);
            }
            return hits;
        }

        // update an existing index
        static void threadproc_update(object obj)
        {
            // just to be safe, make the worker threads wait for each other
            //System.Console.Beep(540, 20);
            lock (my_lock) // prevent contention between searching and writing?
            {
                //System.Console.Beep(840, 20);
                try
                {
                    if (searcher != null)
                    {
                        try
                        {
                            searcher.Close();
                        }
                        catch (Exception e)
                        {
                            btnet.Util.write_to_log("Exception closing lucene searcher:" + e.Message);
                            btnet.Util.write_to_log(e.StackTrace);
                        }
                        searcher = null;
                    }

                    Lucene.Net.Index.IndexModifier modifier = new Lucene.Net.Index.IndexModifier(index_path, anal, false);

                    // same as buid, but uses "modifier" instead of write.
                    // uses additional "where" clause for bugid

                    int bug_id = (int)obj;

                    btnet.Util.write_to_log("started updating Lucene index using folder " + MyLucene.index_path);

                    modifier.DeleteDocuments(new Lucene.Net.Index.Term("bg_id", Convert.ToString(bug_id)));

					string sql = @"
select bg_id, 
$custom_cols
isnull(bg_tags,'') bg_tags,
bg_short_desc    
from bugs where bg_id = $bugid";

					sql = sql.Replace("$bugid",Convert.ToString(bug_id));

					DataSet ds_text_custom_cols = get_text_custom_cols();
					
					sql = sql.Replace("$custom_cols", get_text_custom_cols_names(ds_text_custom_cols));                   

                    // index the bugs
                    DataRow dr = btnet.DbUtil.get_datarow(sql);	

					modifier.AddDocument(MyLucene.create_doc(
						(int)dr["bg_id"],
						0,
						"desc",
						(string)dr["bg_short_desc"]));

					// tags
					string tags = (string) dr["bg_tags"];
					if (tags != "")
					{
						modifier.AddDocument(MyLucene.create_doc(
							(int)dr["bg_id"],
							0,
							"tags",
							tags));
					}
                            
					// custom text fields
					foreach (DataRow dr_custom_col in ds_text_custom_cols.Tables[0].Rows)                            
					{
						string name = (string) dr_custom_col["name"];
						string val = Convert.ToString(dr[name]);
						if (val != "")
						{
							modifier.AddDocument(MyLucene.create_doc(
								(int)dr["bg_id"],
								0,
								name.Replace("'","''"),
								val));
						}
					}


                    // index the bug posts
                    DataSet ds = btnet.DbUtil.get_dataset(@"
select bp_bug, bp_id, 
isnull(bp_comment_search,bp_comment) [text] 
from bug_posts 
where bp_type <> 'update'
and bp_hidden_from_external_users = 0
and bp_bug = " + Convert.ToString(bug_id));

                    foreach (DataRow dr2 in ds.Tables[0].Rows)
                    {
                        modifier.AddDocument(MyLucene.create_doc(
                            (int)dr2["bp_bug"],
                            (int)dr2["bp_id"],
                            "post",
                            (string)dr2["text"]));
                    }

                    modifier.Flush();
                    modifier.Close();
                    btnet.Util.write_to_log("done updating Lucene index");
                }
                catch (Exception e)
                {
                    btnet.Util.write_to_log("exception updating Lucene index: " + e.Message);
                    btnet.Util.write_to_log(e.StackTrace);
                }
            }
        }

		public static void build_lucene_index(System.Web.HttpApplicationState app)
		{
			System.Threading.Thread thread = new System.Threading.Thread(threadproc_build);
			thread.Start(app);
		}


        public static void update_lucene_index(int bug_id)
        {
            System.Threading.Thread thread = new System.Threading.Thread(threadproc_update);
            thread.Start(bug_id);
        }

	}

}


