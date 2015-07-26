using System.Net;
using System.Net.Http;
using System.Web.Http;
using btnet.Security;

namespace btnet.Controllers
{
    public class LoginController : ApiController
    {
        public IHttpActionResult Login(string user, string password)
        {
            LoginResult loginResult = Authenticate.AttemptLogin(Request.GetOwinContext(), user, password);

            if (loginResult.Success)
            {
                return Ok();
            }
            else
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
        }
    }
}
