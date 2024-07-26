using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVerification.codes.mrz
{
    using FieldName = DocumentFieldKey;
    using IndexCountPair = Tuple<int, int>;

    public abstract class BaseMRZParser
    {
        protected BaseMRZParser(Dictionary<FieldName, IndexCountPair> mrzFIM,
            Dictionary<string, string> docTypeDescrDict,
            int mrzExpLength)
        {
            _mrzFieldIdxMap = mrzFIM;
            _documentTypeDescriptionDict = docTypeDescrDict;
            _mrzDataExpectedLength = mrzExpLength;
        }
        public virtual Customer Parse(string mrz)
        {
            var validationMessage = MRZValidationMessage(mrz);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                return new Customer
                {
                    IsValid = false,
                    ValidationMessage = validationMessage
                };
            }
            var output = new Customer
            {
                DocumentType = DocumentType(mrz),
                Gender = Gender(mrz),
                ExpireDate = ExpireDate(mrz),
                IssuingCountryIso = IssuingCountryIso(mrz),
                FirstName = FirstName(mrz),
                LastName = LastName(mrz),
                DocumentNumber = DocumentNumber(mrz),
                NationalityIso = NationalityIso(mrz),
                DateOfBirth = DateOfBirth(mrz)
            };
            output.DocumentTypeDescription = DocumentTypeDescription(
                output.DocumentType);
            output.IssuingCountryName = IssuingCountryName(
                output.IssuingCountryIso);
            output.FullName = (
                output.FirstName + " " + output.LastName)
                .Replace("  ", " ").Trim();
            output.NationalityName = NationalityName(
                output.NationalityIso);
            output.Age = (int)(DateTime.Now.Subtract(
                output.DateOfBirth).TotalDays / 365);
            return output;
        }
        public abstract string CreateMRZ(Customer customer);
        protected string CheckDigit(string icaoPassportNumber)
        {
            //http://www.highprogrammer.com/alan/numbers/mrp.html#checkdigit
            //
            icaoPassportNumber = icaoPassportNumber.ToUpper();
            var inputArray = icaoPassportNumber.Trim().ToCharArray();
            var multiplier = 7;
            var total = 0;
            foreach (var dig in inputArray)
            {
                total = total + _checkDigitArray[dig] * multiplier;
                if (multiplier == 7) multiplier = 3;
                else if (multiplier == 3) multiplier = 1;
                else if (multiplier == 1) multiplier = 7;
            }
            long result;
            Math.DivRem(total, 10, out result);
            return result.ToString();
        }
        protected int GetCheckDigitIndex(FieldName fn)
        {
            return _mrzFieldIdxMap[fn].Item1 +
                _mrzFieldIdxMap[fn].Item2;
        }
        protected string DocumentTypeDescription(string docType)
            => _documentTypeDescriptionDict[docType];
        protected virtual string DocumentType(string mrz)
        {
            return mrz.Substring(
               _mrzFieldIdxMap[FieldName.DOCUMENT_TYPE].Item1,
               _mrzFieldIdxMap[FieldName.DOCUMENT_TYPE].Item2);
        }
        protected virtual string IssuingCountryIso(string mrz)
        {
            return mrz.Substring(
                _mrzFieldIdxMap[FieldName.ISSUING_COUNTRY_ISO].Item1,
                _mrzFieldIdxMap[FieldName.ISSUING_COUNTRY_ISO].Item2);
        }
        protected string IssuingCountryName(string issIso)
        {
            var natItem = _nationalities.NationalitybyCode(issIso);
            return natItem != null ? natItem.Name : string.Empty;

        }
        protected virtual string FirstName(string mrz)
        {
            var nameArraySplit = mrz.Substring(
                _mrzFieldIdxMap[FieldName.FIRST_NAME].Item1)
                .Split(new[] { "<<" }, StringSplitOptions.RemoveEmptyEntries);
            return nameArraySplit.Length >= 2
                ? nameArraySplit[1].Replace("<", " ")
                : nameArraySplit[0].Replace("<", " ");

        }
        protected virtual string LastName(string mrz)
        {

            var nameArraySplit = mrz.Substring(
                _mrzFieldIdxMap[FieldName.LAST_NAME].Item1)
                .Split(new[] { "<<" }, StringSplitOptions.RemoveEmptyEntries);
            return nameArraySplit.Length >= 2
                ? nameArraySplit[0].Replace("<", " ")
                : string.Empty;

        }
        protected virtual string DocumentNumber(string mrz)
        {

            return mrz.Substring(
                _mrzFieldIdxMap[FieldName.DOCUMENT_NUMBER].Item1,
                _mrzFieldIdxMap[FieldName.DOCUMENT_NUMBER].Item2)
                .Replace("<", string.Empty);

        }
        protected virtual string NationalityIso(string mrz)
        {
            return mrz.Substring(
                _mrzFieldIdxMap[FieldName.NATIONALITY_ISO].Item1,
                _mrzFieldIdxMap[FieldName.NATIONALITY_ISO].Item2);
        }
        protected string NationalityName(string natIso)
        {
            var natItem = _nationalities.NationalitybyCode(natIso);
            return natItem != null ? natItem.Name : string.Empty;

        }
        protected virtual DateTime DateOfBirth(string mrz)
        {
            var dob = new DateTime(
                int.Parse(
                    DateTime.Now.Year.ToString().Substring(0, 2)
                    + mrz.Substring( //YY
                    _mrzFieldIdxMap[FieldName.DATE_OF_BIRTH].Item1, 2)),
                int.Parse(
                    mrz.Substring( //MM
                    _mrzFieldIdxMap[FieldName.DATE_OF_BIRTH].Item1 + 2, 2)),
                int.Parse(
                    mrz.Substring( //DD
                    _mrzFieldIdxMap[FieldName.DATE_OF_BIRTH].Item1 + 4, 2)));

            if (dob < DateTime.Now)
                return dob;

            return dob.AddYears(-100); //Subtract a century

        }
        protected virtual string Gender(string mrz)
        {

            return mrz.Substring(
                _mrzFieldIdxMap[FieldName.GENDER].Item1, 1);

        }
        protected virtual DateTime ExpireDate(string mrz)
        {
            //I am assuming all passports will certainly expire this century
            return new DateTime(
                int.Parse(DateTime.Now.Year.ToString().Substring(0, 2)
                + mrz.Substring(
                    _mrzFieldIdxMap[FieldName.EXPIRE_DATE].Item1, 2)),
                int.Parse(mrz.Substring(
                    _mrzFieldIdxMap[FieldName.EXPIRE_DATE].Item1 + 2, 2)),
                int.Parse(mrz.Substring(
                    _mrzFieldIdxMap[FieldName.EXPIRE_DATE].Item1 + 4, 2)));

        }
        protected virtual List<string> OptionalData(string mrz)
        {
            var optData = mrz.Substring(
                _mrzFieldIdxMap[FieldName.OPTIONAL_DATA].Item1,
                 _mrzFieldIdxMap[FieldName.OPTIONAL_DATA].Item2);
            var parts = optData.Split('<');
            var result = new List<string>();
            foreach (var p in parts)
            {
                if (!String.Equals(" ", p)
                    && !String.Equals(string.Empty, p))
                {
                    result.Add(p);
                }
            }
            return result;
        }
        //Look up table for mapping document fields with 
        //indices of raw MRZ string.
        protected readonly Dictionary<FieldName, IndexCountPair> _mrzFieldIdxMap;
        protected readonly int _mrzDataExpectedLength;
        protected readonly Nationalities _nationalities = new Nationalities();
        protected readonly Dictionary<string, string> _documentTypeDescriptionDict;
        //values that are used for calculating check-digit number.
        protected readonly Dictionary<char, int> _checkDigitArray =
             new Dictionary<char, int>
             {
                { '<', 0 }, { '0', 0 }, {'1', 1}, {'2', 2},
                {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6},
                { '7', 7}, {'8', 8}, {'9', 9 }, {'A', 10},
                {'B', 11}, {'C', 12}, {'D', 13}, {'E', 14},
                {'F', 15}, {'G', 16}, {'H', 17}, {'I', 18},
                {'J', 19}, {'K', 20}, {'L', 21}, {'M', 22},
                {'N', 23}, {'O', 24}, {'P', 25}, {'Q', 26},
                {'R', 27}, {'S', 28}, {'T', 29}, {'U', 30},
                {'V', 31}, {'W', 32}, {'X', 33}, {'Y', 34},
                {'Z', 35}
             };
        protected static Dictionary<string, string> GetDefaultDocTypeDescriptionDict()
            => new Dictionary<string, string>()
        {
                {"P", "Passport"},
                {"C", "Card"},
                {"I", "Identity"},
                {"V", "Visa"}
        };
        protected virtual string MRZValidationMessage(string mrz)
        {
            if (string.IsNullOrEmpty(mrz)) return "Empty MRZ";
            string dType = "";
            if (!_documentTypeDescriptionDict.TryGetValue(
                DocumentType(mrz), out dType))
            {
                return "Invalid document type";
            }
            else if (mrz.Length < _mrzDataExpectedLength)
            {
                return $"MRZ length is not valid should be" +
                    $" {_mrzDataExpectedLength} but it is {mrz.Length}";
            }
            return string.Empty;
        }
    }
}
