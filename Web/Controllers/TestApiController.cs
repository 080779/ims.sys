using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    [RoutePrefix("api/Test")]
    public class TestApiController : ApiController
    {
        [HttpGet, Route("Get")]
        public string Get()
        {
            return "ok";
        }
    }
}
