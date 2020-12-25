using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout
{
    public class ApplicationValues
    {


        public static string COOKIE_NAME_SYSTEM_SESSION_EXTERNAL_KEY = "systemSessionExternalKey";

        public static string SQL_WILDCARD_ALL = "%";


        // "SELECT p.last_name, p.first_name, p.middle_name, p.generation_suffix, p.preferred_first_name, p.date_of_birth ");
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_LAST_NAME = 0;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_FIRST_NAME = 1;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_MIDDLE_NAME = 2;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_GENERATION_SUFFIX = 3;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_FIRST_NAME = 4;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_DATE_OF_BIRTH = 5;

    }
}
