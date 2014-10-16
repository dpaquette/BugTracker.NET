using System;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace btnet
{
    public class SQLString
    {
        private string _value;
        private IList<SqlParameter> _parameters;

        public SQLString(string value)
        {
            _value = value;
            _parameters = new List<SqlParameter>();
        }

        public SQLString(string value, IList<SqlParameter> parameters)
        {
            _value = value;
            _parameters = parameters;
        }

        public override string ToString()
        {
            return _value;
        }

        public SQLString Replace(string parameter, string value)
        {
            _parameters.Add(new SqlParameter { ParameterName = parameter, Value = value });
            return this;
        }

        public SQLString Replace(string parameter, int value)
        {
            _parameters.Add(new SqlParameter { ParameterName = parameter, Value = value });
            return this;
        }

        public SQLString Append(string toAppend)
        {
            _value += toAppend;
            return this;
        }

        public SQLString Append(SQLString toAppend)
        {
            _value += toAppend.ToString();
            foreach (var param in toAppend.GetParameters())
                _parameters.Add(param);
            return this;

        }
        public IList<SqlParameter> GetParameters()
        {
            return _parameters;
        }
    }
}