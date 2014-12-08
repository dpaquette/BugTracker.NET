using System.Data;
using System.Security.Principal;

namespace btnet.Search
{
    /// <summary>
    /// Provides full text based search of bugs
    /// </summary>
    public interface IBugSearch
    {
        /// <summary>
        /// Re-index all bugs. 
        /// Warning: This is a CPU, Database and network intensive operation
        /// </summary>
        void IndexAll();

        /// <summary>
        /// Index of re-index the bug matching the specified id
        /// </summary>
        /// <param name="bugId">The id of the bug to index</param>
        void IndexBug(int bugId);

        /// <summary>
        /// Search for bugs based on the specified input text and security settings
        /// </summary>
        /// <param name="searchText">The user entered search text</param>
        /// <param name="identity">The current user</param>
        /// <returns>A dataset containing the search results</returns>
        DataSet Search(string searchText, IIdentity identity);
    }
}