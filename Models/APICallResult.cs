using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Models
{
    public class APICallResult
    {

        public const int RESULT_CODE_SUCCESS = 0;

        public APICallResult()
        {
            errors = new List<string>();
        }

        public int resultCode { get; set; }
        public string successKeyValue { get; set; }
        public List<string> errors { get; set; }

    }
}
