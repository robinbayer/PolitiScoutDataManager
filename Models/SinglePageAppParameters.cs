using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class SinglePageAppParameters
    {

        public SinglePageAppParameters(string baseUrl, string systemVersion, string deployedEnvironmentName, string loggedInUserId, string loggedInUserReferenceName)
        {
            BaseUrl = baseUrl;
            SystemVersion = systemVersion;
            DeployedEnvironmentName = deployedEnvironmentName;
            loggedInUserId = this.loggedInUserId;
            loggedInUserReferenceName = this.loggedInUserReferenceName;

            systemErrors = new List<string>();
        }

        public string BaseUrl { get; private set; }
        public string SystemVersion { get; private set; }
        public string DeployedEnvironmentName { get; set; }
        public string systemSessionExternalKey { get; set; }
        public string loggedInUserId { get; set; }
        public string loggedInUserReferenceName { get; set; }

        public List<string> systemErrors { get; set; }

    }
}
