using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class ElectionForTerritory
    {
        public int electionForTerritoryId { get; set; }
        public int territoryId { get; set; }
        public DateTime electionDate { get; set; }
        public string electionTypeDescription { get; set; }

    }
}
