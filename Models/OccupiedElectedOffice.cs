using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class OccupiedElectedOffice
    {
        public int occupiedElectedOfficeId { get; set; }
        public int territoryLevelId { get; set; }
        public string territoryLevelDescription { get; set; }
        public int territoryId { get; set; }
        public string territoryDescription { get; set; }
        public string electedOfficeDescription { get; set; }
        public int distinctElectedOfficeForTerritoryId { get; set; }
        public string distinctOfficeDesignator { get; set; }
        public int reasonForEntryId { get; set; }
        public int reasonForEntryDescription { get; set; }
        public int reasonForDepartureId { get; set; }
        public int reasonForDepartureDescription { get; set; }
    }
}
