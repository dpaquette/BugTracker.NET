using System.Web.Http;
using btnet.Models;

namespace btnet.Controllers
{
    [Authorize]
    public class BugQueryController : ApiController
    {
        public IHttpActionResult Get(int queryId, string sortBy, string sortOrder, int start, int length, [FromUri] BugQueryFilter[] filters)
        {
            Query query;
            using (Context context = new Context())
            {
                query = context.Queries.Find(queryId);
            }
            if (query != null)
            {
                BugQueryExecutor queryExecutor = new BugQueryExecutor(query);
                var result = queryExecutor.ExecuteQuery(User.Identity, start, length, sortBy, sortOrder, filters);
                return Ok(new
                    {
                        recordsTotal = result.CountUnfiltered,
                        recordsFiltered = result.CountFiltered,
                        data = result.Data
                    });
            }
            return NotFound();
        }
    }
}
