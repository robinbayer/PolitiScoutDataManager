using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class CandidateForElection
    {
        public int candidateForElectionId { get; set; }
        public int personId { get; set; }
        public int territoryLevelId { get; set; }
        public string territoryLevelDescription { get; set; }
        public int territoryId { get; set; }
        public string territoryFullName { get; set; }
        public string electedOfficeDescription { get; set; }
        public int distinctElectedOfficeForTerritoryId { get; set; }
        public string distinctOfficeDesignator { get; set; }
        public int electionForTerritoryId { get; set; }
        public DateTime electionDate { get; set; }
        public int politicalPartyId { get; set; }
        public string politicalPartyReferenceName { get; set; }
        public int resultOfCandidacyId { get; set; }
        public string resultOfCandidacyDescription { get; set; }
    }
}
