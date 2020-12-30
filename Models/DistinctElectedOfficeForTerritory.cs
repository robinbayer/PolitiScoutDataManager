using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class DistinctElectedOfficeForTerritory
    {
        public int distinctElectedOfficeForTerritoryId { get; set; }
        public int electedOfficeForTerritoryId { get; set; }
        public int territoryId { get; set; }
        public string electedOfficeReferenceName { get; set; }
        public string distinctOfficeDesignator { get; set; }
    }
}
