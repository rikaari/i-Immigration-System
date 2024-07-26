using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVerification.codes.mrz
{
    public enum DocumentFieldKey
    {
        DOCUMENT_TYPE = 1,
        ISSUING_COUNTRY_ISO = 2,
        FIRST_NAME = 3,
        LAST_NAME = 4,
        DOCUMENT_NUMBER = 5,
        NATIONALITY_ISO = 6,
        DATE_OF_BIRTH = 7,
        GENDER = 8,
        EXPIRE_DATE = 9,
        ISSUE_DATE = 10,
        ISSUING_AUTHORITY = 11,
        PLACE_OF_BIRTH = 12,
        OPTIONAL_DATA = 13,
        OPTIONAL_DATA_2 = 14
    };
}
