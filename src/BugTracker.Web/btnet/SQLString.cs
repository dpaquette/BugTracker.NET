using System;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace btnet
{
    public class SQLString
    {
        private string _value;
        private List<SqlParameter> _parameters;

        public SQLString(string value)
        {
            _value = value;
            _parameters = new List<SqlParameter>();
        }

        public override string ToString()
        {
            return _value;
        }

        public SQLString Replace(string parameter, string value)
        {
            var cleanParameter = parameter.Replace("$","");
            _parameters.Add(new SqlParameter { ParameterName = cleanParameter, Value = value });
            _value =  _value.Replace(parameter, "@" + cleanParameter);
            return this;
        }

        public SqlParameter[] GetParameters()
        {
            return _parameters.ToArray();
        }
    }
}