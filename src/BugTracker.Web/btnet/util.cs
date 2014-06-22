/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Web;
using System.Data;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace btnet
{

	///////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////
	// Util
	///////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////
	public class Util {

		public static HttpContext context = null;
		private static HttpRequest Request = null;
		//private static HttpResponse Response = null;
		//private static HttpServerUtility Server = null;

		static object dummy = new object();

		public static Regex reCommas = new Regex(",");
		public static Regex rePipes = new Regex("\\|");
		static Regex reEmail = new Regex("^([a-zA-Z0-9_\\-\\'\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})$"); 


		public static bool validate_email(string s)
		{
			return reEmail.IsMatch(s);
		}

		///////////////////////////////////////////////////////////////////////
		public static void set_context(HttpContext asp_net_context)
		{
			context = asp_net_context;
			
			try
			{
				Request = context.Request;
			}
			catch(Exception e)
			{
				Util.write_to_log ("caught exception in util.set_context:" + e.Message);
			}

		}

		///////////////////////////////////////////////////////////////////////
		public static string get_form_name()
		{
			return get_setting("AspNetFormId","ctl00");
		}

		///////////////////////////////////////////////////////////////////////
		public static string get_log_file_path()
		{

			// determine log file name
			string log_file_folder = Util.get_log_folder();

			DateTime now = DateTime.Now;
			string now_string =
				(now.Year).ToString()
				+ "_" +
				(now.Month).ToString("0#")
				+ "_" +
				(now.Day).ToString("0#");

			string path = log_file_folder
				+ "\\"
				+ "btnet_log_"
				+ now_string
				+ ".txt";

			return path;

		}

		///////////////////////////////////////////////////////////////////////
		public static void write_to_log(string s)
		{

			if (Util.get_setting("LogEnabled","1") == "0")
			{
				return;
			}

			string path = get_log_file_path();

			lock(dummy)
			{
				System.IO.StreamWriter w = System.IO.File.AppendText(path);

				// write to it


				string url = "";
				
				try // To workaround problem with IIS integrated mode
				{
					if (HttpContext.Current != null)
					{
						if (HttpContext.Current.Request != null)
						{
							url = HttpContext.Current.Request.Url.ToString();
						}
					}
				}
				catch 
				{
					// do nothing
				}

				w.WriteLine(DateTime.Now.ToString("yyy-MM-dd HH:mm:ss")
					+ " "
					+ url
					+ " "
					+ s);

				w.Close();
			}
		}

		///////////////////////////////////////////////////////////////////////
		public static void write_to_memory_log(string s)
		{

			if (HttpContext.Current == null)
			{
				return;
			}

			if (Util.get_setting("MemoryLogEnabled","0") == "0")
			{
				return;
			}

			string url = "";
			if (HttpContext.Current.Request != null)
			{
				url = HttpContext.Current.Request.Url.ToString();
			}

			string line = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss:fff")
				+ " "
				+ url
				+ " "
				+ s;


			List<string> list = (List<string>) HttpContext.Current.Application["log"];

			if (list == null)
			{
				list = new List<string>();
				HttpContext.Current.Application["log"] = list;
			}

			list.Add(line);

		}

		///////////////////////////////////////////////////////////////////////
		public static void do_not_cache(HttpResponse Response)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader ("Pragma", "no-cache");
			Response.Expires = -1;
		}

		///////////////////////////////////////////////////////////////////////
		public static string get_setting(string name, string default_value)
		{

			NameValueCollection name_values
                = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("appSettings");
			if (string.IsNullOrEmpty(name_values[name]))
			{
				return default_value;
			}
			else
			{
				return name_values[name];
			}
		}


		///////////////////////////////////////////////////////////////////////
		public static bool is_int(string maybe_int)
		{
			try
			{
				int i = Int32.Parse(maybe_int);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		///////////////////////////////////////////////////////////////////////
		public static string is_valid_decimal(string name, string val, int left_of_decimal, int right_of_decimal)
		{
			System.Globalization.CultureInfo ci = btnet.Util.get_culture_info();
			decimal x;
			if (!Decimal.TryParse(val, System.Globalization.NumberStyles.Float, ci, out x))
			{
				return name + " is not in a valid decimal format";
			}

			string[] vals = val.Split(new string[] { ci.NumberFormat.NumberDecimalSeparator }, StringSplitOptions.None);

			if (vals.Length > 0)
			{
				if (vals[0].Length > left_of_decimal)
				{
					return name  +  " has too many digits to the left of the decimal point";
				}
			}
			else if (vals.Length > 1)
			{
				if (vals[1].Length > right_of_decimal)
				{
					return name + " has too many digits to the right of the decimal point";
				}
			}

			return "";

		}

		///////////////////////////////////////////////////////////////////////
		public static bool is_datetime(string maybe_datetime)
		{
			DateTime d;

			try
			{
				d = DateTime.Parse(maybe_datetime,get_culture_info());
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		///////////////////////////////////////////////////////////////////////
		public static string bool_to_string(bool b)
		{
			return (b ? "1" : "0");
		}


		///////////////////////////////////////////////////////////////////////
        public static string strip_html(string text_with_tags) {

            if (Util.get_setting("StripHtmlTagsFromSearchableText","1") == "1")
            {
            	return HttpUtility.HtmlDecode(Regex.Replace(text_with_tags, @"<(.|\n)*?>", string.Empty));
			}
			else
			{
				return text_with_tags;
			}

        }

		///////////////////////////////////////////////////////////////////////
		public static string strip_dangerous_tags(string text_with_tags)
		{
		    string s = Regex.Replace(text_with_tags,
		                         @"<script", "<scrSAFEipt", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"</script", "</scrSAFEipt", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"<object", "<obSAFEject", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"</object", "</obSAFEject", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"<embed", "<emSAFEbed", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"</embed", "</emSAFEbed", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onabort", "onSAFEabort", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onblur", "onSAFEblur", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onchange", "onSAFEchange", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onclick", "onSAFEclick", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"ondblclick", "onSAFEdblclick", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onerror", "onSAFEerror", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onfocus", "onSAFEfocus", RegexOptions.IgnoreCase);

		    s = Regex.Replace(s, @"onkeydown", "onSAFEkeydown", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onkeypress", "onSAFEkeypress", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onkeyup", "onSAFEkeyup", RegexOptions.IgnoreCase);

		    s = Regex.Replace(s, @"onload", "onSAFEload", RegexOptions.IgnoreCase);

		    s = Regex.Replace(s, @"onmousedown", "onSAFEmousedown", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onmousemove", "onSAFEmousemove", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onmouseout", "onSAFEmouseout", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onmouseup", "onSAFEmouseup", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onmouseup", "onSAFEmouseup", RegexOptions.IgnoreCase);

		    s = Regex.Replace(s, @"onreset", "onSAFEresetK", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onresize", "onSAFEresize", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onselect", "onSAFEselect", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onsubmit", "onSAFEsubmit", RegexOptions.IgnoreCase);
		    s = Regex.Replace(s, @"onunload", "onSAFEunload", RegexOptions.IgnoreCase);

			return s;
		}
		

		///////////////////////////////////////////////////////////////////////
		public static System.Globalization.CultureInfo get_culture_info()
		{
			// Create a basic culture object to provide also all input parsing
			return new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
		}

		///////////////////////////////////////////////////////////////////////
		public static string format_db_date_and_time(object date)
		{

			if (date.GetType() == typeof(System.DBNull))
			{
				return "";
			}
			// not sure when this case happens, but it's a workaround for a bug
			// somebody reported, 1257368
			else if (date.GetType() == typeof(System.String))
			{
                return date.ToString();
			}
			else
			{

				// We don't know whether time is significant or not,
				// but we can guess.  Probably, not for sure, but probably
				// if the time is 12:00 AM, the time is just debris.

                DateTime dt = (DateTime)date;

                if (dt.Year == 1900)
                {
                    return "";
                }
                else
                {
                    string date_time_format = "";
                    if ((dt.Hour == 0 || dt.Hour == 12) && dt.Minute == 0 && dt.Second == 0)
                    {
                        date_time_format = get_setting("JustDateFormat", "g");
                    }
                    else
                    {
                        date_time_format = get_setting("DateTimeFormat", "g");
                    }
	                
	                int hours_offset = Convert.ToInt32(btnet.Util.get_setting("DisplayTimeOffsetInHours","0"));
	                
	                if (hours_offset != 0)
	                {
	                	dt = dt.AddHours(hours_offset);
	                }
	                return dt.ToString(date_time_format,get_culture_info());
                }
			}
		}


		//modified by CJU on jan 9 2008
		///////////////////////////////////////////////////////////////////////
		public static string format_db_value( Decimal val ) {

			return val.ToString( get_culture_info( ) );

		}

		///////////////////////////////////////////////////////////////////////
		public static string format_db_value( DateTime val ) {

			return format_db_date_and_time( val );

		}

		///////////////////////////////////////////////////////////////////////
		public static string format_db_value( object val ) {

			if( val is Decimal )
				return format_db_value( (Decimal)val );
			else if( val is DateTime )
				return format_db_value( (DateTime)val );
            else 
			    return Convert.ToString( val );

		}
		//end modified by CJU on jan 9 2008

		///////////////////////////////////////////////////////////////////////
		public static string format_local_date_into_db_format(string date)
		{


			// seems to already be in the right format
			DateTime d;
			try
			{
				d = DateTime.Parse(date,get_culture_info());
			}
			catch (FormatException)
			{
				// Can not translate this
				return "";
			}
			// Note that yyyyMMdd HH:mm:ss is a universal SQL dateformat for strings.
			return d.ToString(get_setting("SQLServerDateFormat","yyyyMMdd HH:mm:ss"));

		}

		///////////////////////////////////////////////////////////////////////
		public static string format_local_decimal_into_db_format( string val )
		{
			decimal x = decimal.Parse(val, get_culture_info());

			return x.ToString( System.Globalization.CultureInfo.InvariantCulture );
		}

		///////////////////////////////////////////////////////////////////////
		public static string alter_sql_per_project_permissions(string sql, Security security)
		{

			string project_permissions_sql;

			string dpl = Util.get_setting("DefaultPermissionLevel","2");

			if (dpl == "0")
			{
				project_permissions_sql = @" (bugs.bg_project in (
					select pu_project
					from project_user_xref
					where pu_user = $user
					and pu_permission_level > 0)) ";
			}
			else
			{
				project_permissions_sql = @" (bugs.bg_project not in (
					select pu_project
					from project_user_xref
					where pu_user = $user
					and pu_permission_level = 0)) ";
			}

            if (security.user.can_only_see_own_reported)
            {
                project_permissions_sql += @"
					    and bugs.bg_reported_user = $user ";

            }
            else
            {
                if (security.user.other_orgs_permission_level == 0)
                {
                    project_permissions_sql += @"
					    and bugs.bg_org = $user.org ";

                }
            }

			project_permissions_sql
				= project_permissions_sql.Replace("$user.org",Convert.ToString(security.user.org));

			project_permissions_sql
				= project_permissions_sql.Replace("$user",Convert.ToString(security.user.usid));


			// Figure out where to alter sql for project permissions
            // I've tried lots of different schemes over the years....

            int alter_here_pos = sql.IndexOf("$ALTER_HERE"); // places - can be multiple - are explicitly marked
            if (alter_here_pos != -1)
            {
                return sql.Replace("$ALTER_HERE", "/* ALTER_HERE */ " + project_permissions_sql);
            }
            else
            {
                string bug_sql;

                int where_pos = sql.IndexOf("WhErE"); // first look for a "special" where, case sensitive, in case there are multiple where's to choose from
                if (where_pos == -1)
                    where_pos = sql.ToUpper().IndexOf("WHERE");

                int order_pos = sql.IndexOf("/*ENDWHR*/"); // marker for end of the where statement

                if (order_pos == -1)
                    order_pos = sql.ToUpper().LastIndexOf("ORDER BY");

                if (order_pos < where_pos)
                    order_pos = -1; // ignore an order by that occurs in a subquery, for example

                if (where_pos != -1 && order_pos != -1)
                {
                    // both WHERE and ORDER BY clauses
                    bug_sql = sql.Substring(0, where_pos + 5)
                        + " /* altered - both  */ ( "
                        + sql.Substring(where_pos + 5, order_pos - (where_pos + 5))
                        + " ) AND ( "
                        + project_permissions_sql
                        + " ) "
                        + sql.Substring(order_pos);
                }
                else if (order_pos == -1 && where_pos == -1)
                {
                    // Neither
                    bug_sql = sql + " /* altered - neither */ WHERE " + project_permissions_sql;
                }
                else if (order_pos == -1)
                {
                    // WHERE, without order
                    bug_sql = sql.Substring(0, where_pos + 5)
                        + " /* altered - just where */ ( "
                        + sql.Substring(where_pos + 5)
                        + " ) AND ( "
                        + project_permissions_sql + " )";
                }
                else
                {
                    // ORDER BY, without WHERE
                    bug_sql = sql.Substring(0, order_pos)
                        + " /* altered - just order by  */ WHERE "
                        + project_permissions_sql
                        + sql.Substring(order_pos);
                }

                return bug_sql;
            }

		}


		///////////////////////////////////////////////////////////////////////
		public static string encrypt_string_using_MD5(string s)
		{

			byte[] byte_array = System.Text.Encoding.Default.GetBytes(s);

			System.Security.Cryptography.HashAlgorithm alg =
				System.Security.Cryptography.HashAlgorithm.Create("MD5");

			byte[] byte_array2 = alg.ComputeHash(byte_array);

			System.Text.StringBuilder sb
				= new System.Text.StringBuilder(byte_array2.Length);

			foreach(byte b in byte_array2)
			{
				sb.AppendFormat("{0:X2}", b);
			}

			return sb.ToString();
		}

        ///////////////////////////////////////////////////////////////////////
        public static void update_user_password(int us_id, string unencypted)
        {
            Random random = new Random();
            int salt = random.Next(10000, 99999);

            string encrypted = Util.encrypt_string_using_MD5(unencypted + Convert.ToString(salt));

            string sql = "update users set us_password = N'$en', us_salt = $salt where us_id = $id";

            sql = sql.Replace("$en", encrypted);
            sql = sql.Replace("$salt", Convert.ToString(salt));
            sql = sql.Replace("$id", Convert.ToString(us_id));

            btnet.DbUtil.execute_nonquery(sql);
        }

		///////////////////////////////////////////////////////////////////////
		public static string capitalize_first_letter(string s)
		{
			if (s.Length > 0 && Util.get_setting("NoCapitalization","0") == "0")
			{
				return s.Substring(0,1).ToUpper() + s.Substring(1,s.Length-1);
			}
			return s;

		}


		///////////////////////////////////////////////////////////////////////
		public static string sanitize_integer(string s)
		{
			int n;
			string s2;
			try
			{
				n = Convert.ToInt32(s);
				s2 = Convert.ToString(n);
			}
			catch
			{
				throw (new Exception("Expected integer.  Possible SQL injection attempt?"));

			}

			return s;
		}


		///////////////////////////////////////////////////////////////////////
		public static bool is_numeric_datatype(Type datatype)
		{

			if (datatype == typeof(System.Int32)
			|| datatype == typeof(System.Decimal)
			|| datatype == typeof(System.Double)
			|| datatype == typeof(System.Single)
			|| datatype == typeof(System.UInt32)
			|| datatype == typeof(System.Int64)
			|| datatype == typeof(System.UInt64)
			|| datatype == typeof(System.Int16)
			|| datatype == typeof(System.UInt16))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		///////////////////////////////////////////////////////////////////////
		protected static string get_absolute_or_relative_folder(string folder)
		{

			if (folder.IndexOf(":") == 1
			|| folder.StartsWith("\\\\"))
			{
				// leave as is
				return folder;
			}
			else
			{
                string map_path = (string)HttpRuntime.Cache["MapPath"];
                return map_path + "\\" + folder;
			}

		}

        ///////////////////////////////////////////////////////////////////////
        public static string get_folder(string name, string dflt)
        {
            String folder = Util.get_setting(name, "");
            if (folder == "")
                return dflt;

            folder = get_absolute_or_relative_folder(folder);
            if (!System.IO.Directory.Exists(folder))
            {
                throw (new Exception(name + " specified in Web.config, \""
                + folder
                + "\", not found.  Edit Web.config."));
            }

            return folder;

        }


   		///////////////////////////////////////////////////////////////////////
        public static string get_lucene_index_folder()
        {
            string map_path = (string)HttpRuntime.Cache["MapPath"];
            return get_folder("LuceneIndexFolder", map_path + "\\App_Data\\lucene_index");
        }

		///////////////////////////////////////////////////////////////////////
		public static string get_upload_folder()
		{
            string map_path = (string)HttpRuntime.Cache["MapPath"];
            return get_folder("UploadFolder", map_path + "\\App_Data\\uploads");
		}

		///////////////////////////////////////////////////////////////////////
		public static string get_log_folder()
		{
            string map_path = (string)HttpRuntime.Cache["MapPath"];
            return get_folder("LogFileFolder", map_path + "\\App_Data\\logs");
        }

		///////////////////////////////////////////////////////////////////////
		public static string[] split_string_using_commas(string s)
		{
			return reCommas.Split(s);
		}


		
		///////////////////////////////////////////////////////////////////////
		public static string[] split_dropdown_vals(string s)
		{
			string[] array = rePipes.Split(s);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim().Replace("\r","").Replace("\n","");
			}
			return array;
		}

		///////////////////////////////////////////////////////////////////////
		// common to add/edit custom files, project
		public static string validate_dropdown_values(string vals)
		{
			if (vals.Contains("'")
			|| vals.Contains("\"")
			|| vals.Contains("<")
			|| vals.Contains(">")
			|| vals.Contains("\t"))
			{
				return "Special characters like <, >, or quotes not allowed.";
			}
			else
			{
				return "";
			}
		}

		///////////////////////////////////////////////////////////////////////
		public static string how_long_ago(int seconds)
		{
			return how_long_ago(new TimeSpan(0,0,seconds));
		}

		///////////////////////////////////////////////////////////////////////
		public static string how_long_ago(TimeSpan ts)
        {

            if (ts.Days > 0)
            {
                if (ts.Days == 1)
                {
                    if (ts.Hours > 2)
                    {
                        return "1 day and " + ts.Hours + " hours ago";
                    }
                    else
                    {
                        return "1 day ago";
                    }
                }
                else
                {
                    return ts.Days + " days ago";
                }
            }
            else if (ts.Hours > 0)
            {
                if (ts.Hours == 1)
                {
                    if (ts.Minutes > 5)
                    {
                        return "1 hour and " + ts.Minutes + " minutes ago";
                    }
                    else
                    {
                        return "1 hour ago";
                    }
                }
                else
                {
                    return ts.Hours + " hours ago";
                }
            }
            else if (ts.Minutes > 0)
            {
                if (ts.Minutes == 1)
                {
                    return "1 minute ago";
                }
                else
                {
                    return ts.Minutes + " minutes ago";
                }
            }
            else
            {
                return ts.Seconds + " seconds ago";
            }

		}


		///////////////////////////////////////////////////////////////////////
		public static DataTable get_related_users(Security security, bool force_full_names)
		{
			string sql = "";

			if (Util.get_setting("DefaultPermissionLevel","2") == "0")
			{
				// only show users who have explicit permission
				// for projects that this user has permissions for

				sql = @"
/* get related users 1 */

select us_id,
case when $fullnames then
    case when len(isnull(us_firstname,'') + ' ' + isnull(us_lastname,'')) > 1
	then isnull(us_firstname,'') + ' ' + isnull(us_lastname,'')
    else us_username end
else us_username end us_username,
isnull(us_email,'') us_email,
us_org,
og_external_user
into #temp
from users
inner join orgs on us_org = og_id
where us_id in
	(select pu1.pu_user from project_user_xref pu1
	where pu1.pu_project in
		(select pu2.pu_project from project_user_xref pu2
		where pu2.pu_user = $user.usid
		and pu2.pu_permission_level <> 0
		)
	and pu1.pu_permission_level <> 0
	)

if $og_external_user = 1 -- external
and $og_other_orgs_permission_level = 0 -- other orgs
begin
	delete from #temp where og_external_user = 1 and us_org <> $user.org 
end

$limit_users

select us_id, us_username, us_email from #temp order by us_username

drop table #temp";



			}
			else
			{
				// show users UNLESS they have been explicitly excluded
				// from all the projects the viewer is able to view

				// the cartesian join in the first select is intentional

				sql= @"
/* get related users 2 */
select  pj_id, us_id,
case when $fullnames then
    case when len(isnull(us_firstname,'') + ' ' + isnull(us_lastname,'')) > 1
	then isnull(us_firstname,'') + ' ' + isnull(us_lastname,'')
    else us_username end
else us_username end us_username,
isnull(us_email,'') us_email
into #temp
from projects, users
where pj_id not in
(
	select pu_project from project_user_xref
	where pu_permission_level = 0 and pu_user = $user.usid
)


$limit_users


if $og_external_user = 1 -- external
and $og_other_orgs_permission_level = 0 -- other orgs
begin
	select distinct a.us_id, a.us_username, a.us_email
	from #temp a
	inner join users b on a.us_id = b.us_id
	inner join orgs on b.us_org = og_id
	where og_external_user = 0 or b.us_org = $user.org
	order by a.us_username
end
else
begin

	select distinct us_id, us_username, us_email
		from #temp
		left outer join project_user_xref on pj_id = pu_project
		and us_id = pu_user
		where isnull(pu_permission_level,2) <> 0
		order by us_username
end

drop table #temp";

			}

			if (Util.get_setting("LimitUsernameDropdownsInSearch","0") == "1")
			{
				string sql_limit_user_names = @"

select isnull(bg_assigned_to_user,0) keep_me
into #temp2
from bugs
union
select isnull(bg_reported_user,0) from bugs

delete from #temp
where us_id not in (select keep_me from #temp2)
drop table #temp2";

				sql = sql.Replace("$limit_users",sql_limit_user_names);
			}
			else
			{
				sql = sql.Replace("$limit_users","");
			}



            if (force_full_names || Util.get_setting("UseFullNames", "0") == "1")
			{
                // true condition
                sql = sql.Replace("$fullnames", "1 = 1");
            }
			else
			{
                // false condition
                sql = sql.Replace("$fullnames", "0 = 1");
			}

			sql = sql.Replace("$user.usid",Convert.ToString(security.user.usid));
			sql = sql.Replace("$user.org",Convert.ToString(security.user.org));
			sql = sql.Replace("$og_external_user",Convert.ToString(security.user.external_user ? 1 : 0));
			sql = sql.Replace("$og_other_orgs_permission_level",Convert.ToString(security.user.other_orgs_permission_level));

			return btnet.DbUtil.get_dataset(sql).Tables[0];

		}


		///////////////////////////////////////////////////////////////////////
		public static int get_default_user(int projectid)
		{

			if (projectid == 0) {return 0;}

			string sql = @"select isnull(pj_default_user,0)
					from projects
					where pj_id = $pj";

			sql = sql.Replace("$pj", Convert.ToString(projectid));
			object obj = btnet.DbUtil.execute_scalar(sql);

			if (obj != null)
			{
				return (int) obj;
			}
			else
			{
				return 0;
			}

		}

        
        ///////////////////////////////////////////////////////////////////////
        public static DataSet get_custom_columns()
        {

			DataSet ds = (DataSet) context.Application["custom_columns_dataset"];
            
            if (ds != null)
            {
            	return ds;	
            }
            else
            {
            	ds = btnet.DbUtil.get_dataset(@"
/* custom columns */ select sc.name, st.[name] [datatype], 
case when st.[name] = 'nvarchar' or st.[name] = 'nchar' then sc.length/2 else sc.length end as [length], 
sc.xprec, sc.xscale, sc.isnullable,
mm.text [default value], 
dflts.name [default name], 
isnull(ccm_dropdown_type,'') [dropdown type],
isnull(ccm_dropdown_vals,'') [vals],
isnull(ccm_sort_seq, sc.colorder) [column order],
sc.colorder
from syscolumns sc
inner join systypes st on st.xusertype = sc.xusertype
inner join sysobjects so on sc.id = so.id
left outer join syscomments mm on sc.cdefault = mm.id
left outer join custom_col_metadata on ccm_colorder = sc.colorder
left outer join sysobjects dflts on dflts.id = mm.id
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
order by sc.id, isnull(ccm_sort_seq,sc.colorder)");
				
				context.Application["custom_columns_dataset"]  = ds;
				return ds;
			}

        }


		///////////////////////////////////////////////////////////////////////
		public static bool check_password_strength(string pw)
		{
			if (Util.get_setting("RequireStrongPasswords","0") == "0")
			{
				return true;
			}

			if (pw.Length < 8) return false;
			if (pw.IndexOf("password") > -1) return false;
			if (pw.IndexOf("123") > -1) return false;
			if (pw.IndexOf("asdf") > -1) return false;
			if (pw.IndexOf("qwer") > -1) return false;
			if (pw.IndexOf("test") > -1) return false;

			int lowercase = 0;
			int uppercase = 0;
			int digits = 0;
			int special_chars = 0;

			for (int i = 0; i < pw.Length; i++)
			{
				char c = pw[i];
				if (c >= 'a' && c <= 'z') lowercase = 1;
				else if (c >= 'A' && c <= 'Z') uppercase = 1;
				else if (c >= '0' && c <= '9') digits = 1;
				else special_chars = 1;
			}

			if (lowercase + uppercase + digits + special_chars < 2)
			{
				return false;
			}

			return true;
		}

   		///////////////////////////////////////////////////////////////////////
        public static string filename_to_content_type(string filename)
        {
            string ext = System.IO.Path.GetExtension(filename).ToLower();

            if (ext == ".jpg"
	        || ext == ".jpeg")
	        {
		        return "image/jpeg";
	        }
	        else if (ext == ".gif")
	        {
		        return "image/gif";
	        }
	        else if (ext == ".bmp")
	        {
		        return "image/bmp";
	        }
            else if (ext == ".tiff")
            {
                return "image/tiff";
            }
            else if (ext == ".txt" || ext == ".ini" || ext == ".bat" || ext == ".js")
	        {
		        return "text/plain";
	        }
	        else if (ext == ".doc" || ext == ".docx")
	        {
		        return "application/msword";
	        }
	        else if (ext == ".xls")
	        {
		        return "application/excel";
	        }
	        else if (ext == ".zip")
	        {
		        return "application/zip";
	        }
	        else if (ext == ".htm"
	        || ext == ".html"
	        || ext == ".asp"
	        || ext == ".aspx"
	        || ext == ".php")
	        {
		        return "text/html";
	        }
            else if (ext == ".xml")
            {
                return "text/xml";
            }
            else
	        {
		        return "";
	        }
        
        }
        
        public static string request_to_string_for_sql(string val, string datatype)
        {

			if (val == null || val.Length == 0)
			{
				if (datatype == "varchar"
				|| datatype == "nvarchar"
				|| datatype == "char"
				|| datatype == "nchar")
				{
					return "N''";
				}
				else
				{
					return "null";
				}
			}

			val = val.Replace("'","''");

			if (datatype == "datetime")
				return "'" + btnet.Util.format_local_date_into_db_format(val) + "'";
			else if (datatype == "decimal")
				return btnet.Util.format_local_decimal_into_db_format( val );
			else if (datatype == "int")
				return val;
			else 
				return "N'" + val + "'";
       
        }

        ///////////////////////////////////////////////////////////////////////
        public static void redirect(HttpRequest Request, HttpResponse Response)
        {

            // redirect to the page the user was going to or start off with bugs.aspx
            string url = Request.QueryString["url"];
            string qs = Request.QueryString["qs"];

            if (String.IsNullOrEmpty(url))
            {
                string mobile = Request["mobile"];
                if (String.IsNullOrEmpty(mobile))
                {
                    Response.Redirect("bugs.aspx");
                }
                else {
                    Response.Redirect("mbugs.aspx");
                }
            }
            else if (url == Request.ServerVariables["URL"])  // I can't remember what this code means...
            {
                Response.Redirect("bugs.aspx");
            }
            else
            {
                Response.Redirect(remove_line_breaks(url) + "?" + remove_line_breaks(HttpUtility.UrlDecode(qs)));
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static void redirect(string url, HttpRequest Request, HttpResponse Response)
        {
            //redirect to the url supplied with the original querystring
            if (url.IndexOf("?") > 0)
            {
                Response.Redirect(url + "&url=" 
                    + remove_line_breaks(Request.QueryString["url"]) 
                    + "&qs=" 
                    + remove_line_breaks(Request.QueryString["qs"]));
            }
            else
            {
                Response.Redirect(url + "?url="
                    + remove_line_breaks(Request.QueryString["url"])
                    + "&qs="
                    + remove_line_breaks(Request.QueryString["qs"]));
            }
        }
        
        ///////////////////////////////////////////////////////////////////////
        public static string remove_line_breaks(String s) {
            if (s == null)
            {
                return "";
            }
            else
            {
                return s.Replace("\n", "").Replace("\r", "");
            }
        }
        
        ///////////////////////////////////////////////////////////////////////
		public static void update_most_recent_login_datetime(int us_id)
		{
			string sql = @"update users set us_most_recent_login_datetime = getdate() where us_id = $us";
			sql = sql.Replace("$us", Convert.ToString(us_id));
			DbUtil.execute_nonquery(sql);
		}

        /////////////////////////////////////////////////////////////////////////
        //public static void print_as_excel(HttpResponse Response, DataView dv)
        //{ 
        //    Response.AddHeader("content-disposition", "attachment;filename=bugs.xls");
        //    Response.Write("<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/></head><body>");
        //    Response.Write("<table border=1>");
        //    int startCol = 0;
        //    int col;
            
        //    // column names first_column = true;
        //    for (col = startCol; col < dv.Table.Columns.Count;col++)
        //    { 
        //        if (dv.Table.Columns[col].ColumnName == "$FLAG")
        //            continue;
        //        if (dv.Table.Columns[col].ColumnName == "$SEEN")
        //            continue;
        //        Response.Write("<td>");
        //        Response.Write(dv.Table.Columns[col].ColumnName.Replace("<br>"," "));
        //        Response.Write("</td>");

        //    } // bug rows 

        //    foreach (DataRowView drv in dv)
        //    {
        //        Response.Write("<tr>");


        //        for (col = startCol; col < dv.Table.Columns.Count; col++)
        //        {
        //            if (dv.Table.Columns[col].ColumnName == "$FLAG")
        //                continue;
        //            if (dv.Table.Columns[col].ColumnName == "$SEEN")
        //                continue;

        //            Response.Write("<td>");

        //            if (drv[col].ToString().IndexOf("\r\n") >= 0)
        //            {
        //                Response.Write("\"" + drv[col].ToString().Replace("\"", "\"\"").Replace("\r\n", "\n") + "\"");
        //            }
        //            else
        //            {
        //                Response.Write(drv[col].ToString().Replace("\n", ""));
        //            }

        //            Response.Write("</td>");
        //        }
        //        Response.Write("</tr>");

        //    } 

        //    Response.Write("</table>");
 
        //} 


        ///////////////////////////////////////////////////////////////////////
		public static void print_as_excel(HttpResponse Response, DataView dv)
		{
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=btnet_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;

            if (btnet.Util.get_setting("WriteUtf8Preamble", "1") == "1")
            {
                Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            }

			int col;
			bool first_column;
            string quote = "\"";
            string two_quotes = quote+quote;
            string line_break = "\r\n";

			// column names
			first_column = true;
			for (col = 1; col < dv.Table.Columns.Count; col++)
			{
				if (dv.Table.Columns[col].ColumnName == "$FLAG")
					continue;
				if (dv.Table.Columns[col].ColumnName == "$SEEN")
					continue;

				if (!first_column)
				{
                    Response.Write(",");
				}
                Response.Write(quote);
                Response.Write(dv.Table.Columns[col].ColumnName.Replace("<br>", " ").Replace(quote, two_quotes));
                Response.Write(quote);
				first_column = false;
			}
            Response.Write(line_break);

			// bug rows
			foreach (DataRowView drv in dv)
			{
				first_column = true;
				for (col = 1; col < dv.Table.Columns.Count; col++)
				{
                    DataColumn column = dv.Table.Columns[col];

					if (column.ColumnName == "$FLAG")
						continue;
					if (column.ColumnName == "$SEEN")
						continue;

                    if (!first_column)
                    {
                        Response.Write(",");
                    }

                    Response.Write(quote);
                    if (column.DataType == typeof(System.DateTime))
                    {
                        Response.Write(btnet.Util.format_db_date_and_time(drv[col]));
                    }
                    else
                    {
                        Response.Write(drv[col].ToString().Replace(line_break, "|").Replace("\n", "|").Replace(quote, two_quotes));
                    }
                    
                    Response.Write(quote);

					first_column = false;
				}
                Response.Write(line_break);
			}
            
            Response.End();
		}

		
		///////////////////////////////////////////////////////////////////////
		public static DataSet get_all_tasks(Security security, int bugid)
		{
            string sql = "select ";
            
            if (bugid == 0)
            {
                sql += @"
bg_id as [id], 
bg_short_desc as [description], 
pj_name as [project], 
ct_name as [category], 
bug_statuses.st_name as [status],  
bug_users.us_username as [assigned to],";
            }

            sql += "tsk_id [task<br>id], tsk_description [task<br>description] ";

			if (btnet.Util.get_setting("ShowTaskAssignedTo","1") == "1")
			{
				sql += ", task_users.us_username [task<br>assigned to]";
			}

			if (btnet.Util.get_setting("ShowTaskPlannedStartDate","1") == "1")
			{
				sql += ", tsk_planned_start_date [planned start]";
			}
			if (btnet.Util.get_setting("ShowTaskActualStartDate","1") == "1")
			{
				sql += ", tsk_actual_start_date [actual start]";
			}

			if (btnet.Util.get_setting("ShowTaskPlannedEndDate","1") == "1")
			{
				sql += ", tsk_planned_end_date [planned end]";
			}
			if (btnet.Util.get_setting("ShowTaskActualEndDate","1") == "1")
			{
				sql += ", tsk_actual_end_date [actual end]";
			}

			if (btnet.Util.get_setting("ShowTaskPlannedDuration","1") == "1")
			{
				sql += ", tsk_planned_duration [planned<br>duration]";
			}
			if (btnet.Util.get_setting("ShowTaskActualDuration","1") == "1")
			{
				sql += ", tsk_actual_duration  [actual<br>duration]";
			}


			if (btnet.Util.get_setting("ShowTaskDurationUnits","1") == "1")
			{
				sql += ", tsk_duration_units [duration<br>units]";
			}

			if (btnet.Util.get_setting("ShowTaskPercentComplete","1") == "1")
			{
				sql += ", tsk_percent_complete [percent<br>complete]";
			}

			if (btnet.Util.get_setting("ShowTaskStatus","1") == "1")
			{
				sql += ", task_statuses.st_name  [task<br>status]";
			}		

			if (btnet.Util.get_setting("ShowTaskSortSequence","1") == "1")
			{
				sql += ", tsk_sort_sequence  [seq]";
			}	

			sql += @"
from bug_tasks 
inner join bugs on tsk_bug = bg_id
left outer join projects on bg_project = pj_id
left outer join categories on bg_category = ct_id
left outer join statuses bug_statuses on bg_status = bug_statuses.st_id
left outer join statuses task_statuses on tsk_status = task_statuses.st_id
left outer join users bug_users on bg_assigned_to_user = bug_users.us_id
left outer join users task_users on tsk_assigned_to_user = task_users.us_id
where tsk_bug in 
("; 

			if (bugid == 0)
			{
				sql += btnet.Util.alter_sql_per_project_permissions("select bg_id from bugs", security);
			}
			else
			{
				sql += Convert.ToString(bugid);
			}
			sql += @"
)
order by tsk_sort_sequence, tsk_id";

			
			DataSet ds = btnet.DbUtil.get_dataset(sql);
			
			return ds;
		}

        
        ///////////////////////////////////////////////////////////////////////
        public static void display_bug_not_found(HttpResponse Response, Security security, int id)
        {
            Response.Write("<link rel=StyleSheet href=btnet.css type=text/css>");
            security.write_menu(Response, btnet.Util.get_setting("PluralBugLabel", "bugs"));
            Response.Write("<p>&nbsp;</p><div class=align>");
            Response.Write("<div class=err>");
            Response.Write(btnet.Util.capitalize_first_letter(btnet.Util.get_setting("SingularBugLabel", "bug")));
            Response.Write(" not found:&nbsp;" + Convert.ToString(id) + "</div>");
            Response.Write("<p><a href=bugs.aspx>View ");
            Response.Write(btnet.Util.get_setting("PluralBugLabel", "bug"));
            Response.Write("</a>");
            Response.End();
        }

        ///////////////////////////////////////////////////////////////////////
        public static void display_you_dont_have_permission(HttpResponse Response, Security security)
        {
            Response.Write("<link rel=StyleSheet href=btnet.css type=text/css>");
            security.write_menu(Response, btnet.Util.get_setting("PluralBugLabel", "bugs"));
            Response.Write("<p>&nbsp;</p><div class=align>");
            Response.Write("<div class=err>You are not allowed to view this "
                + btnet.Util.get_setting("SingularBugLabel", "bug")
                + "</div>");
            Response.Write("<p><a href=bugs.aspx>View "
                + btnet.Util.capitalize_first_letter(btnet.Util.get_setting
                ("PluralBugLabel", "bugs")) + "</a>");
            Response.End();
        }

		
    } // end Util
}
