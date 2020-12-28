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

        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_LAST_NAME = 0;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_FIRST_NAME = 1;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_MIDDLE_NAME = 2;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_GENERATION_SUFFIX = 3;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_FIRST_NAME = 4;
        public static int PERSON_QUERY_RESULT_COLUMN_OFFSET_DATE_OF_BIRTH = 5;

        public static int PERSON_LIST_QUERY_RESULT_COLUMN_OFFSET_PERSON_ID = 0;
        public static int PERSON_LIST_QUERY_RESULT_COLUMN_OFFSET_LAST_NAME = 1;
        public static int PERSON_LIST_QUERY_RESULT_COLUMN_OFFSET_FIRST_NAME = 2;
        public static int PERSON_LIST_QUERY_RESULT_COLUMN_OFFSET_MIDDLE_NAME = 3;
        public static int PERSON_LIST_QUERY_RESULT_COLUMN_OFFSET_GENERATION_SUFFIX = 4;
        public static int PERSON_LIST_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_FIRST_NAME = 5;
        public static int PERSON_LIST_QUERY_RESULT_COLUMN_OFFSET_DATE_OF_BIRTH = 6;

        public static int TERRITORY_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_ID = 0;
        public static int TERRITORY_QUERY_RESULT_COLUMN_OFFSET_FULL_NAME = 1;
        public static int TERRITORY_QUERY_RESULT_COLUMN_OFFSET_SHORT_NAME = 2;

        public static int TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_ID = 0;
        public static int TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_ID = 1;
        public static int TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_FULL_NAME = 2;
        public static int TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_SHORT_NAME = 3;

        public static int POLITICAL_PARTY_QUERY_RESULT_COLUMN_OFFSET_REFERENCE_NAME = 0;
        public static int POLITICAL_PARTY_QUERY_RESULT_COLUMN_OFFSET_ABBREVIATION = 1;

        public static int POLITICAL_PARTY_LIST_QUERY_RESULT_COLUMN_OFFSET_POLITICAL_PARTY_ID = 0;
        public static int POLITICAL_PARTY_LIST_QUERY_RESULT_COLUMN_OFFSET_REFERENCE_NAME = 1;
        public static int POLITICAL_PARTY_LIST_QUERY_RESULT_COLUMN_OFFSET_ABBREVIATION = 2;

    }
}
