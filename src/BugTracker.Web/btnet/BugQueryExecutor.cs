using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Web;
using btnet.Models;
using btnet.Security;
using System.IO;

namespace btnet
{
    public class BugQueryExecutor
    {
        private readonly Query _query;
        private readonly string[] _columnNames;

        public BugQueryExecutor(Query query)
        {
            _query = query;
            _columnNames = query.ColumnNames;           
        }

        public BugQueryResult ExecuteQuery(IIdentity identity, int start, int length, string orderBy, string sortDirection, BugQueryFilter[] filters = null)
        {
            if (!string.IsNullOrEmpty(orderBy) && !_columnNames.Contains(orderBy))
            {
                throw new ArgumentException("Invalid order by column specified: {0}", orderBy);
            }
           
            var initialSql = string.Format("SELECT t.* FROM ({0}) t", GetInnerSql(identity));
            SQLString sqlString = new SQLString(initialSql);
            var initialCountSql = string.Format("SELECT COUNT(*) FROM ({0}) t", GetInnerSql(identity));
            SQLString countSqlString = new SQLString(initialCountSql);
            SQLString countUnfilteredSqlString = new SQLString(initialCountSql);
            int userId = identity.GetUserId();
            sqlString.AddParameterWithValue("@ME", userId);
            countSqlString.AddParameterWithValue("@ME", userId);
            countUnfilteredSqlString.AddParameterWithValue("@ME", userId);

            
            if (filters != null && filters.Any())
            {
                ApplyWhereClause(sqlString, filters);
                ApplyWhereClause(countSqlString, filters);
            }
           
            sqlString.Append(" ORDER BY ");

            sqlString.Append(BuildDynamicOrderByClause());

            sqlString.AddParameterWithValue("ORDER_BY", orderBy ?? _columnNames.First());
            sqlString.AddParameterWithValue("SORT_DIRECTION", sortDirection);

         
            sqlString.Append(" OFFSET @offset ROWS FETCH NEXT @page_size ROWS ONLY");
            sqlString.AddParameterWithValue("page_size", length);
            sqlString.AddParameterWithValue("offset", start);


            return new BugQueryResult
            {
                CountUnfiltered = Convert.ToInt32(DbUtil.execute_scalar(countUnfilteredSqlString)),
                CountFiltered = Convert.ToInt32(DbUtil.execute_scalar(countSqlString)),
                Data = DbUtil.get_dataset(sqlString).Tables[0]
            };

        }

        private string BuildDynamicOrderByClause()
        {
            return string.Join(", ",
                _columnNames.Select(column => string.Format(
                    @" CASE WHEN @ORDER_BY = '{0}' AND @SORT_DIRECTION = 'DESC' THEN [{0}] END DESC, 
  CASE WHEN @ORDER_BY = '{0}' AND @SORT_DIRECTION = 'ASC' THEN [{0}] END ASC", column)).ToArray());
        }

        private void ApplyWhereClause(SQLString sqlString, BugQueryFilter[] filters)
        {
            sqlString.Append(" WHERE ");
            List<string> conditions = new List<string>();
            foreach (var filter in filters)
            {
                if (!_columnNames.Contains(filter.Column))
                {
                    throw new ArgumentException("Invalid filter column: {0}", filter.Column);
                }
                string parameterName = GetCleanParameterName(filter.Column);
                conditions.Add(string.Format("[{0}] = @{1}", filter.Column, parameterName));
                sqlString.AddParameterWithValue(parameterName, filter.Value);
            }
            sqlString.Append(string.Join(" AND ", conditions));
        }

        private string GetInnerSql(IIdentity identity)
        {
            SQLString innerSql = new SQLString(_query.SQL);
            return Util.alter_sql_per_project_permissions(innerSql, identity).ToString();
        }

        private string GetCleanParameterName(string columnName)
        {
            var parameterName = columnName;
            //TODO: Add other invalid special characters
            parameterName = parameterName.Replace(" ", "_");            
            return parameterName;
        }


    }
}