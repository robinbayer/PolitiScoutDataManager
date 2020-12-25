using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class Person
    {
        public int personId { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string generationSuffix { get; set; }
        public string preferredFirstName { get; set; }
        public DateTime dateOfBirth { get; set; }

    }
}
