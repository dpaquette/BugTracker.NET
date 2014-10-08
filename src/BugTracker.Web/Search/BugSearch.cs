using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace btnet.Search
{
    /// <summary>
    /// A full-text bug search based on the NEST API for ElasticSearch
    /// </summary>
    public class BugSearch : IBugSearch
    {
        private readonly ElasticClient _client;

        public BugSearch(ElasticClient client)
        {
            _client = client;
        }

        ///////////////////////////////////////////////////////////////////////
        private static DataSet get_text_custom_cols()
        {
            DataSet ds_custom_fields = DbUtil.get_dataset(new SQLString(@"
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
and sc.length > 30"));

            return ds_custom_fields;
        }

        ///////////////////////////////////////////////////////////////////////
        private string get_text_custom_cols_names(DataSet ds_custom_fields)
        {

            string custom_cols = "";
            foreach (DataRow dr in ds_custom_fields.Tables[0].Rows)
            {
                custom_cols += "[" + (string)dr["name"] + "],";
            }
            return custom_cols;

        }

        /// <summary>
        /// Re-index all bugs. 
        /// Warning: This is a CPU, Database and network intensive operation
        /// </summary>
        public void IndexAll()
        {
            try
            {
                Util.write_to_log("started creating search index for all bugs");

                DataSet ds = DbUtil.get_dataset(new SQLString("select bg_id from bugs"));

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    IndexBug(Convert.ToInt32(dr["bg_id"]));
                }

                Util.write_to_log("Done recreating search index for all bugs");
            }
            catch (Exception e)
            {
                Util.write_to_log("exception building search index: " + e.Message);
                Util.write_to_log(e.StackTrace);
            }

        }

        public DataSet Search(string searchText, Security security)
        {
            ISearchResponse<object> response =
                _client.Search<object>(s => s.Types("bug")
                  .Query(q => q.QueryString(d => d.Query(searchText)))
                  .Highlight(h => h.PreTags("<span class='highlighted'>")
                                   .PostTags("</span>")
                                   .OnFields(f => f.OnField("*"))));


            DataSet results = GetSearchResultDataSet();
            DataTable resultTable = results.Tables[0];

            var filteredHits = GetHitsFilteredBySecurity(response, security);

            foreach (var hit in filteredHits)
            {
                DataRow resultRow = resultTable.NewRow();

                JObject bug = (JObject)hit.Source;
                int bugId = Convert.ToInt32(bug["bg_id"]);

                resultRow[ResultColumns.Color] = "#ffffff";

                resultRow[ResultColumns.Id] = bugId;
                resultRow[ResultColumns.Description] = bug["desc"].ToString();
                if (hit.Highlights.Any())
                {
                    var highlight = hit.Highlights.First();
                    resultRow[ResultColumns.Source] = highlight.Value.Field;
                    resultRow[ResultColumns.Text] = highlight.Value.Highlights.FirstOrDefault();
                }

                resultRow[ResultColumns.Date] = bug["bg_reported_date"].ToString();
                resultRow[ResultColumns.Status] = bug["status"].ToString();
                resultRow[ResultColumns.Score] = Convert.ToDecimal(hit.Score);

                resultTable.Rows.Add(resultRow);

            }

            return results;
        }

        private IEnumerable<IHit<object>> GetHitsFilteredBySecurity(ISearchResponse<object> response, Security security)
        {
            //NOTE: The search response will contain all bugs, but the current user might not have access to some of the bugs in the search response.
            //      This method filters the list of hits based on the list of bugs that the user has access to in the system.
            //      This is not an optimal solution but was considered the best approach given the current security filtering approach in bug tracker
            //TODO: Change this once the security approach has been redesigned.
            var sql = new SQLString(@"SELECT bg_id FROM bugs WHERE $ALTER_HERE");
            sql = Util.alter_sql_per_project_permissions(sql, security);

            DataSet ds = DbUtil.get_dataset(sql);
            HashSet<int> visibleBugIds = new HashSet<int>(
                ds.Tables[0].AsEnumerable().Select(d => Convert.ToInt32(d["bg_id"])).ToArray());

            return response.Hits.Where(h => visibleBugIds.Contains(Convert.ToInt32(h.Id)));
        }


        /// <summary>
        /// Index of re-index the bug matching the specified id
        /// </summary>
        /// <param name="bugId">The id of the bug to index</param>
        public void IndexBug(int bugId)
        {
            try
            {

                Util.write_to_log("started updating search index");

                var sql = new SQLString(@"
select bg_id, 
$custom_cols
isnull(bg_tags,'') tags,
bg_reported_date,
isnull(st_name,'') status,
bg_short_desc  as [desc]
from bugs 
left outer join statuses on st_id = bg_status
where bg_id = @bugid");

                sql = sql.Replace("bugid", Convert.ToString(bugId));

                DataSet ds_text_custom_cols = get_text_custom_cols();

                sql = sql.Replace("$custom_cols", get_text_custom_cols_names(ds_text_custom_cols));
                
                DataRow bugRow = DbUtil.get_datarow(sql);
               
                sql = new SQLString(@"
                select bp_id, 
                isnull(bp_comment_search,bp_comment) [text] ,
                bp_date
                from bug_posts 
                where bp_type <> 'update'
                and bp_hidden_from_external_users = 0
                and bp_bug = @bugId");
                sql.Replace("bugId", bugId.ToString());
                DataSet bugPosts = DbUtil.get_dataset(sql);

                IndexBug(bugRow, bugPosts.Tables[0]);

                Util.write_to_log("done updating search index");
            }
            catch (Exception e)
            {
                Util.write_to_log("exception updating search index: " + e.Message);
                Util.write_to_log(e.StackTrace);
            }
        }

        private void IndexBug(DataRow bugRow, DataTable bugPosts)
        {
            var bugJson = ToJson(bugRow, bugPosts);
            _client.Index(bugJson, i => i.Type("bug")
                                         .Id(Convert.ToInt64(bugRow["bg_id"])));

        }

        /// <summary>
        /// Conversts the specified bug and bug posts to a JSON reprenstation that is suitable for indexing
        /// </summary>
        /// <param name="bugRow">The bug</param>
        /// <param name="posts">All bug posts for the specified bug</param>
        /// <returns>A JSON representation of the bug and it's posts</returns>
        private static string ToJson(DataRow bugRow, DataTable posts)
        {        
            //NOTE: the NEST API for ElasticSearch typically works with POCOs, but BugTracker is all dataset based currently.
            //      As a workaround for now, we need to convert the dataset representation to a JSON document that is 
            //      suitable for indexing.
            IDictionary<string, object> bugDictionary = new Dictionary<string, object>();
            foreach (DataColumn column in bugRow.Table.Columns)
            {
                bugDictionary[column.ColumnName] = bugRow[column];
            }
            bugDictionary["posts"] = posts;
            string json = JsonConvert.SerializeObject(bugDictionary);
            return json;
        }

        /// <summary>
        /// Get a new dataset that will hold the results of a search
        /// </summary>
        /// <returns></returns>
        private DataSet GetSearchResultDataSet()
        {
            DataSet result = new DataSet();
            DataTable table = result.Tables.Add();
            table.Columns.Add(ResultColumns.Color, typeof(string));
            table.Columns.Add(ResultColumns.Id, typeof(int));
            table.Columns.Add(ResultColumns.Description, typeof(string));
            table.Columns.Add(ResultColumns.Source, typeof(string));
            table.Columns.Add(ResultColumns.Text, typeof(string));
            table.Columns.Add(ResultColumns.Date, typeof(DateTime));
            table.Columns.Add(ResultColumns.Status, typeof(string));
            table.Columns.Add(ResultColumns.Score, typeof(decimal));
            return result;
        }

        private static class ResultColumns
        {
            public const string Color = "Column1";
            public const string Id = "id";
            public const string Description = "desc";
            public const string Source = "search_source";
            public const string Text = "search_text";
            public const string Date = "date";
            public const string Status = "status";
            public const string Score = "#SCORE";
        }
    }
}
