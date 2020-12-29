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
    public class CandidateForElectionWSController : ControllerBase
    {

        private ILogger<CandidateForElectionWSController> logger;
        private IConfiguration configuration;

        public CandidateForElectionWSController(IConfiguration configuration, ILogger<CandidateForElectionWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

    }
}
