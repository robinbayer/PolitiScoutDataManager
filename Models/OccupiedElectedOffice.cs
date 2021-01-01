using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class OccupiedElectedOffice
    {
        public int occupiedElectedOfficeId { get; set; }
        public int personId { get; set; }
        public int territoryLevelId { get; set; }
        public string territoryLevelDescription { get; set; }
        public int territoryId { get; set; }
        public string territoryFullName { get; set; }
        public string electedOfficeDescription { get; set; }
        public int distinctElectedOfficeForTerritoryId { get; set; }
        public string distinctOfficeDesignator { get; set; }
        public int reasonForEntryId { get; set; }
        public string reasonForEntryDescription { get; set; }
        public int reasonForDepartureId { get; set; }
        public string reasonForDepartureDescription { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}
