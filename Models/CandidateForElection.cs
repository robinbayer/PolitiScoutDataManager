using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class CandidateForElection
    {
        public int candidateForElectionId { get; set; }
        public int territoryLevelId { get; set; }
        public string territoryLevelDescription { get; set; }
        public int territoryId { get; set; }
        public string territoryDescription { get; set; }
        public string electedOfficeDescription { get; set; }
        public int distinctElectedOfficeForTerritoryId { get; set; }
        public string distinctOfficeDesignator { get; set; }
        public DateTime electionDate { get; set; }
        public int politicalPartyId { get; set; }
        public string politicalPartyDescription { get; set; }
    }
}
