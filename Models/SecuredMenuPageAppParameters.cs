using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class SecuredMenuPageAppParameters : SinglePageAppParameters
    {
        public bool ShowMenuOptionAffiliatedSalesResourceLookupMenuItem { get; set; }
        public bool ShowMenuOptionViewMasterCustomerInformation { get; set; }
        public bool ShowMenuOptionMaintainMasterSubordinateCustomerMenuItem { get; set; }
        public bool ShowMenuOptionSubordinateExistingCustomerMenuItem { get; set; }
        public bool ShowMenuOptionRemapToDifferentCorporateEndCustomer { get; set; }
        public bool ShowMenuOptionPromoteSubordinateCustomer { get; set; }
        public bool ShowMenuOptionUpdateMasterCustomerInformation { get; set; }

        // PROGRAMMER'S NOTE:  Will add properties to support new menu options
        //public bool allowMenuOption2 { get; set; }

        public SecuredMenuPageAppParameters(string baseUrl, string systemVerion, string deployedEnvironmentName,
                                            string loggedInUserId, string loggedInUserReferenceName) : base(baseUrl, systemVerion, deployedEnvironmentName, loggedInUserId,
                                                                                                            loggedInUserReferenceName)
        {
        }

    }
}
