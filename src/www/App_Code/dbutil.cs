/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace btnet
{
    public class DbUtil
    {
        ///////////////////////////////////////////////////////////////////////
        public static object execute_scalar(string sql)
        {
            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                Util.write_to_log("sql=\n" + sql);
            }

            using (SqlConnection conn = get_sqlconnection())
            {
                object returnValue;
                SqlCommand cmd = new SqlCommand(sql, conn);
                returnValue = cmd.ExecuteScalar();
                conn.Close(); // redundant, but just to be clear
                return returnValue;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static void execute_nonquery_without_logging(string sql)
        {
            using (SqlConnection conn = get_sqlconnection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close(); // redundant, but just to be clear
            }

        }

        ///////////////////////////////////////////////////////////////////////
        public static void execute_nonquery(string sql)
        {

            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                Util.write_to_log("sql=\n" + sql);
            }

            using (SqlConnection conn = get_sqlconnection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close(); // redundant, but just to be clear
            } 
        }

        ///////////////////////////////////////////////////////////////////////
        public static void execute_nonquery(SqlCommand cmd)
        {
            log_command(cmd);

            using (SqlConnection conn = get_sqlconnection())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    conn.Close(); // redundant, but just to be clear
                }
                finally
                {
                    conn.Close(); // redundant, but just to be clear
                    cmd.Connection = null;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static SqlDataReader execute_reader(string sql, CommandBehavior behavior)
        {
            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                Util.write_to_log("sql=\n" + sql);
            }

            SqlConnection conn = get_sqlconnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    return cmd.ExecuteReader(behavior | CommandBehavior.CloseConnection);
                }
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static SqlDataReader execute_reader(SqlCommand cmd, CommandBehavior behavior)
        {
            log_command(cmd);

            SqlConnection conn = get_sqlconnection();
            try
            {
                cmd.Connection = conn;
                return cmd.ExecuteReader(behavior | CommandBehavior.CloseConnection);
            }
            catch
            {
                conn.Close();
                throw;
            }
            finally
            {
                cmd.Connection = null;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static DataSet get_dataset(string sql)
        {

            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                Util.write_to_log("sql=\n" + sql);
            }

            DataSet ds = new DataSet();
            using (SqlConnection conn = get_sqlconnection())
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
               	{
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    da.Fill(ds);
                    stopwatch.Stop();
                    log_stopwatch_time(stopwatch);
                    conn.Close(); // redundant, but just to be clear
                	return ds;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static void log_stopwatch_time(System.Diagnostics.Stopwatch stopwatch)
        {
            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                Util.write_to_log("elapsed milliseconds:" + stopwatch.ElapsedMilliseconds.ToString("0000"));
            }
        }


        ///////////////////////////////////////////////////////////////////////
        public static DataView get_dataview(string sql)
        {
            DataSet ds = get_dataset(sql);
            return new DataView(ds.Tables[0]);
        }


        ///////////////////////////////////////////////////////////////////////
        public static DataRow get_datarow(string sql)
        {
            DataSet ds = get_dataset(sql);
            if (ds.Tables[0].Rows.Count != 1)
            {
                return null;
            }
            else
            {
                return ds.Tables[0].Rows[0];
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static SqlConnection get_sqlconnection()
        {

            string connection_string = Util.get_setting("ConnectionString", "MISSING CONNECTION STRING");
            SqlConnection conn = new SqlConnection(connection_string);
            conn.Open();
            return conn;
        }

        ///////////////////////////////////////////////////////////////////////
        private static void log_command(SqlCommand cmd)
        {
            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("sql=\n" + cmd.CommandText);
                foreach (SqlParameter param in cmd.Parameters)
                {
                    sb.Append("\n  ");
                    sb.Append(param.ParameterName);
                    sb.Append("=");
                    if (param.Value == null || Convert.IsDBNull(param.Value))
                    {
                        sb.Append("null");
                    }
                    else if (param.SqlDbType == SqlDbType.Text || param.SqlDbType == SqlDbType.Image)
                    {
                        sb.Append("...");
                    }
                    else
                    {
                        sb.Append("\"");
                        sb.Append(param.Value);
                        sb.Append("\"");
                    }
                }
                Util.write_to_log(sb.ToString());
            }
        }

    } // end DbUtil

} // end namespace