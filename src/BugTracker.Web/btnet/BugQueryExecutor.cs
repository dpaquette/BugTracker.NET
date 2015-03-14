using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Web;
using btnet.Models;
using btnet.Security;

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
            if (query.Columns == "ColumnsNeeded")
            {
                var initialSql = string.Format("SELECT TOP 1 t.* FROM ({0}) t", GetInnerSql());
                SQLString sqlString = new SQLString(initialSql);
                sqlString.AddParameterWithValue("@ME", -1);
                var dataSet = DbUtil.get_dataset(sqlString);
                var columns = new List<string>();
                foreach (DataColumn column in dataSet.Tables[0].Columns)
                {
                 columns.Add(column.ColumnName);
                }
                _columnNames = columns.ToArray();
            }
        }

        public DataTable ExecuteQuery(IIdentity identity, int pageNumber, string orderBy, string sortDirection)
        {
            if (!string.IsNullOrEmpty(orderBy) && !_columnNames.Contains(orderBy))
            {
                throw new ArgumentException("Invalid order by column specified: {0}", orderBy);
            }

            int pageSize = identity.GetBugsPerPage();

            //TODO: Add Util.alter_sql_per_project_permissions(bug_sql, User.Identity);
            var initialSql = string.Format("SELECT t.* FROM ({0}) t", GetInnerSql());
            SQLString sqlString = new SQLString(initialSql);
            sqlString.AddParameterWithValue("@ME", identity.GetUserId());

            sqlString.Append(" ORDER BY ");

            sqlString.Append(BuildDynamicOrderByClause());

            sqlString.AddParameterWithValue("ORDER_BY", orderBy ?? _columnNames.First());
            sqlString.AddParameterWithValue("SORT_DIRECTION", sortDirection);
            sqlString.Append(" OFFSET @offset ROWS FETCH NEXT @page_size ROWS ONLY");
            sqlString.AddParameterWithValue("page_size", pageSize);
            sqlString.AddParameterWithValue("offset", pageSize*pageNumber);

            return btnet.DbUtil.get_dataset(sqlString).Tables[0];

        }

        private string BuildDynamicOrderByClause()
        {
            return string.Join(", ",
                _columnNames.Select(column => string.Format(
                    @"CASE WHEN @ORDER_BY = '{0}' AND @SORT_DIRECTION = 'DESC' THEN [{0}] END DESC, 
  CASE WHEN @ORDER_BY = '{0}' AND @SORT_DIRECTION = 'ASC' THEN [{0}] END ASC
                            ", column)).ToArray());
        }

        private string GetInnerSql()
        {
            var innerSql = _query.SQL;
            var indexOfOrderBy = innerSql.IndexOf("order by", StringComparison.InvariantCultureIgnoreCase);
            if (indexOfOrderBy > 0)
            {
                innerSql = innerSql.Substring(0, indexOfOrderBy);
            }
            return innerSql.Replace("isnull(pr_background_color,'#ffffff')", "isnull(pr_background_color,'#ffffff') [$COLOR]");

        }
    }
}