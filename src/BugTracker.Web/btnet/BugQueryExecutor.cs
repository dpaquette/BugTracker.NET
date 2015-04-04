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
        private const int MaxLength = 5000;

        public BugQueryExecutor(Query query)
        {
            _query = query;
            _columnNames = query.ColumnNames;           
        }

        public BugQueryResult ExecuteQuery(IIdentity identity, int start, int length, string orderBy,
            string sortDirection, BugQueryFilter[] filters = null)
        {
            return ExecuteQuery(identity, start, length, orderBy, sortDirection, false, filters);
        }

        public BugQueryResult ExecuteQuery(IIdentity identity, int start, int length, string orderBy, string sortDirection, bool idOnly, BugQueryFilter[] filters = null)
        {
            if (!string.IsNullOrEmpty(orderBy) && !_columnNames.Contains(orderBy))
            {
                throw new ArgumentException("Invalid order by column specified: {0}", orderBy);
            }

            bool hasFilters = filters != null && filters.Any();
            string columnsToSelect = idOnly ? "id" : "*";
            var innerSql = GetInnerSql(identity);
            var countSql = string.Format("SELECT COUNT(1) FROM ({0}) t", GetInnerSql(identity));

            SQLString sqlString = new SQLString(countSql);

            sqlString.Append(";");
            if (hasFilters)
            {
                sqlString.Append(countSql);
                ApplyWhereClause(sqlString, filters);
                sqlString.Append(";");
            }

            var bugsSql = string.Format("SELECT t.{0} FROM ({1}) t",columnsToSelect, innerSql);
            sqlString.Append(bugsSql);
            

            sqlString.Append(" WHERE id IN (");
            var innerBugsSql = string.Format("SELECT t.id FROM ({0}) t", innerSql);
            sqlString.Append(innerBugsSql);

            ApplyWhereClause(sqlString, filters);
            
            
            if (hasFilters)
            {
                foreach (var filter in filters)
	            {
                    sqlString.AddParameterWithValue(GetCleanParameterName(filter.Column), filter.Value);
	            }
    
            }
           
            sqlString.Append(" ORDER BY ");

            sqlString.Append(BuildDynamicOrderByClause(orderBy, sortDirection));

            sqlString.Append(" OFFSET @offset ROWS FETCH NEXT @page_size ROWS ONLY)");
            
            
            int userId = identity.GetUserId();
            sqlString.AddParameterWithValue("@ME", userId);
            sqlString.AddParameterWithValue("page_size", length > 0 ? length : MaxLength);
            sqlString.AddParameterWithValue("offset", start);
            DataSet dataSet = DbUtil.get_dataset(sqlString);

            var countUnfiltered = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
            var countFiltered = hasFilters ? Convert.ToInt32(dataSet.Tables[1].Rows[0][0]) : countUnfiltered;
            var bugDataTableIndex = hasFilters ? 2 : 1;
            
            return new BugQueryResult
            {
                CountUnfiltered = countUnfiltered,
                CountFiltered = countFiltered,
                Data = dataSet.Tables[bugDataTableIndex]
            };

        }

        private string BuildDynamicOrderByClause(string orderBy, string sortDirection)
        {            
            return string.Format("[{0}] {1}", orderBy, sortDirection.ToUpper() == "ASC" ? "ASC" : "DESC");            
        }

        private void ApplyWhereClause(SQLString sqlString, BugQueryFilter[] filters)
        {
            if (filters != null && filters.Any())
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
                }
                sqlString.Append(string.Join(" AND ", conditions));
            }
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