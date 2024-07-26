using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVerification.codes.mrz
{
    using FieldName = DocumentFieldKey;
    using IndexCountPair = Tuple<int, int>;
    public class TD1MRZParser : BaseMRZParser
    {
        public TD1MRZParser() :
            base(GetFieldIdxLUT(),
                GetDefaultDocTypeDescriptionDict(), 90)
        { }
        private static Dictionary<FieldName, IndexCountPair> GetFieldIdxLUT()
        {
            var LUT = new Dictionary<FieldName, IndexCountPair>()
            {
                {FieldName.DOCUMENT_TYPE, new IndexCountPair(0, 1) },
                {FieldName.ISSUING_COUNTRY_ISO, new IndexCountPair(2, 3) },
                {FieldName.FIRST_NAME, new IndexCountPair(0 + 60, 0) },
                {FieldName.LAST_NAME, new IndexCountPair(0 + 60, 0) },
                {FieldName.DOCUMENT_NUMBER, new IndexCountPair(5 + 0, 9) },
                {FieldName.NATIONALITY_ISO, new IndexCountPair(15 + 30, 3) },
                {FieldName.DATE_OF_BIRTH, new IndexCountPair(0 + 30, 6) },
                {FieldName.GENDER, new IndexCountPair(7 + 30, 1) },
                {FieldName.EXPIRE_DATE, new IndexCountPair(8 + 30, 6) },
                {FieldName.ISSUE_DATE, new IndexCountPair(0, 0) },
                {FieldName.ISSUING_AUTHORITY, new IndexCountPair(10 + 44, 0) },
                {FieldName.PLACE_OF_BIRTH, new IndexCountPair(10 + 44, 0) },
                 {FieldName.OPTIONAL_DATA, new IndexCountPair(16 + 0, 15) }
            };
            return LUT;
        }

        public override string CreateMRZ(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
