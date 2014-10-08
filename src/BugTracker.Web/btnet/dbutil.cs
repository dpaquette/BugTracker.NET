/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;

namespace btnet
{
    public class DbUtil
    {
        ///////////////////////////////////////////////////////////////////////
        public static object execute_scalar(SQLString sql)
        {
            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                Util.write_to_log("sql=\n" + sql);
            }

            using (SqlConnection conn = GetConnection())
            {
                object returnValue;
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                cmd.Parameters.AddRange(sql.GetParameters().ToArray());
                returnValue = cmd.ExecuteScalar();
                conn.Close(); // redundant, but just to be clear
                return returnValue;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static void execute_nonquery_without_logging(SQLString sql)
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                cmd.Parameters.AddRange(sql.GetParameters().ToArray());
                cmd.ExecuteNonQuery();
                conn.Close(); // redundant, but just to be clear
            }

        }

        ///////////////////////////////////////////////////////////////////////
        public static void execute_nonquery(SQLString sql)
        {

            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                Util.write_to_log("sql=\n" + sql);
            }

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                cmd.Parameters.AddRange(sql.GetParameters().ToArray());
                cmd.ExecuteNonQuery();
                conn.Close(); // redundant, but just to be clear
            } 
        }

        ///////////////////////////////////////////////////////////////////////
        public static void execute_nonquery(SqlCommand cmd)
        {
            log_command(cmd);

            using (SqlConnection conn = GetConnection())
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
        public static SqlDataReader execute_reader(SQLString sql, CommandBehavior behavior)
        {
            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                Util.write_to_log("sql=\n" + sql);
            }

            SqlConnection conn = GetConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql.ToString(), conn))
                {
                    cmd.Parameters.AddRange(sql.GetParameters().ToArray());
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

            SqlConnection conn = GetConnection();
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
        public static DataSet get_dataset(SQLString sql)
        {

            if (Util.get_setting("LogSqlEnabled", "1") == "1")
            {
                Util.write_to_log("sql=\n" + sql);
            }

            DataSet ds = new DataSet();
            using (SqlConnection conn = GetConnection())
            {
                using (SqlDataAdapter da = new SqlDataAdapter( sql.ToString(), conn))
               	{
                    da.SelectCommand = new SqlCommand(sql.ToString());
                    da.SelectCommand.Parameters.AddRange(sql.GetParameters().ToArray());
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
        public static DataView get_dataview(SQLString sql)
        {
            DataSet ds = get_dataset(sql);
            return new DataView(ds.Tables[0]);
        }


        ///////////////////////////////////////////////////////////////////////
        public static DataRow get_datarow(SQLString sql)
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
        public static SqlConnection GetConnection()
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