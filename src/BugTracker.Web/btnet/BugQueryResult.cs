using System.Data;

namespace btnet
{
    public class BugQueryResult
    {
        public int CountUnfiltered { get; set; }

        public int CountFiltered { get; set; }

        public DataTable Data { get; set; }
    }
}