using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace btnet.Controllers
{
    [Authorize]
    public class BugFromEmailController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post()
        {            
            return Ok();
        }
    }
}
