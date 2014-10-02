using System;
using System.Configuration;
using Nest;

namespace btnet.Search
{
    public static class BugSearchFactory
    {
        public static IBugSearch CreateBugSearch()
        {
            var uriString = Util.get_setting("SearchServerURI", string.Empty);
            if (string.IsNullOrEmpty(uriString))
            {
                throw new ConfigurationErrorsException("SearchServerURI is missing. This application setting is required in order to use search");
            }

            var node = new Uri( uriString);

            var settings = new ConnectionSettings(
                node,
                defaultIndex: "btnet"
            );
            var client = new ElasticClient(settings);
            return new BugSearch(client);
        }
    }
}