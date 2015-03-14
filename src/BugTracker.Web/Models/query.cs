using System;

namespace btnet.Models
{
    public partial class Query
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string SQL { get; set; }
        public Nullable<int> Default { get; set; }
        public Nullable<int> User { get; set; }
        public Nullable<int> Org { get; set; }

        public string Columns { get; set; }

        public string[] ColumnNames
        {
            get { return Columns.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries); }
        }


    }
}
