using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Npgsql;

namespace Overthink.PolitiScout.Controllers
{
    [Route("ws")]
    [ApiController]
    public class OccupiedElectedOfficeWSController : ControllerBase
    {

        private ILogger<OccupiedElectedOfficeWSController> logger;
        private IConfiguration configuration;

        public OccupiedElectedOfficeWSController(IConfiguration configuration, ILogger<OccupiedElectedOfficeWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }


    }
}
