using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace eVerification.codes.mrz
{
    using FieldName = DocumentFieldKey;
    using IndexCountPair = Tuple<int, int>;

    public class TD3MRZParser : BaseMRZParser
    {
        public TD3MRZParser() :
            base(GetFieldIdxLUT(),
                GetDefaultDocTypeDescriptionDict(), 88)
        { }
        static private Dictionary<FieldName, IndexCountPair> GetFieldIdxLUT()
        {
            var LUT = new Dictionary<FieldName, IndexCountPair>()
            {
                {FieldName.DOCUMENT_TYPE, new IndexCountPair(0, 1) },
                {FieldName.ISSUING_COUNTRY_ISO, new IndexCountPair(2, 3) },
                {FieldName.FIRST_NAME, new IndexCountPair(5, 0) },
                {FieldName.LAST_NAME, new IndexCountPair(5, 0) },
                {FieldName.DOCUMENT_NUMBER, new IndexCountPair(0 + 44, 9) },
                {FieldName.NATIONALITY_ISO, new IndexCountPair(10 + 44, 3) },
                {FieldName.DATE_OF_BIRTH, new IndexCountPair(13 + 44, 6) },
                {FieldName.GENDER, new IndexCountPair(20 + 44, 1) },
                {FieldName.EXPIRE_DATE, new IndexCountPair(21 + 44, 6) },
                {FieldName.ISSUE_DATE, new IndexCountPair(0, 0) },
                {FieldName.ISSUING_AUTHORITY, new IndexCountPair(10 + 44, 0) },
                {FieldName.PLACE_OF_BIRTH, new IndexCountPair(10 + 44, 0) },
                {FieldName.OPTIONAL_DATA, new IndexCountPair(29 + 44, 13) }
            };
            return LUT;
        }
        public override string CreateMRZ(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.IssuingCountryIso)
                || string.IsNullOrEmpty(customer.LastName)
                || string.IsNullOrEmpty(customer.FirstName)
                || string.IsNullOrEmpty(customer.DocumentNumber)
                || string.IsNullOrEmpty(customer.NationalityIso)
                || customer.DateOfBirth.Year < 1901
                || string.IsNullOrEmpty(customer.Gender)
                || customer.ExpireDate.Year < 1901)
                return string.Empty;
            var line1 = "P<" + customer.IssuingCountryIso
                + (customer.LastName + "<<"
                + customer.FirstName).Replace(" ", "<");
            bool isMakeFullName = customer.LastName.Length +
                customer.FirstName.Length > _mrzDataExpectedLength / 2 - 3;
            if (isMakeFullName)
                line1 = "P<" + customer.IssuingCountryIso
                    + (customer.FirstName + "<"
                    + customer.LastName).Replace(" ", "<");
            line1 = line1.PadRight(_mrzDataExpectedLength / 2, '<')
                .Replace("-", "<");
            if (line1.Length > _mrzDataExpectedLength / 2)
                line1 = line1.Substring(0, _mrzDataExpectedLength / 2);
            var line2 = customer.DocumentNumber.PadRight(9, '<')
                + CheckDigit(customer.DocumentNumber.PadRight(9, '<'))
                + customer.NationalityIso
                + customer.DateOfBirth.ToString("yyMMdd")
                + CheckDigit(customer.DateOfBirth.ToString("yyMMdd"))
                + customer.Gender.Substring(0, 1)
                + customer.ExpireDate.ToString("yyMMdd")
                + CheckDigit(customer.ExpireDate.ToString("yyMMdd"));
            line2 = line2.PadRight(42, '<') + "0";
            var compositeCheckDigit = CheckDigit(line2.Substring(0, 10)
                + line2.Substring(13, 7) + line2.Substring(21, 22));
            line2 = line2 + compositeCheckDigit.Replace("-", "<");
            return line1 + line2;
        }
    }
}
