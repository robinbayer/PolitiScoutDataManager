using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class Territory
    {
        public int territoryId { get; set; }
        public string fullName { get; set; }
        public string shortName { get; set; }
        public int territoryLevelId { get; set; }
    }
}
