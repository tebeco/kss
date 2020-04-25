using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kss.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kss.ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RespositoryScanController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<RepositoryScan> Get()
        {
            return Array.Empty<RepositoryScan>();
        }
    }
}
