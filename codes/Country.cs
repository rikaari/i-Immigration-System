using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVerification.codes
{
    public class Country
    {
        private string iso { get; set; }
        private string name { get; set; }

        private static List<Country> list = new List<Country>
        {
            new Country{name ="~ Select ~", iso =""},
            new Country{name ="Afghanistan", iso ="AFG"},
            new Country{name ="Albania", iso ="ALB"},
            new Country{name ="Algeria", iso ="DZA"},
            new Country{name ="American Samoa", iso ="ASM"},
            new Country{name ="Andorra", iso ="AND"},
            new Country{name ="Angola", iso ="AGO"},
            new Country{name ="Anguilla", iso ="AIA"},
            new Country{name ="Antarctica", iso ="ATA"},
            new Country{name ="Antigua and Barbuda", iso ="ATG"},
            new Country{name ="Argentina", iso ="ARG"},
            new Country{name ="Armenia", iso ="ARM"},
            new Country{name ="Aruba", iso ="ABW"},
            new Country{name ="Australia", iso ="AUS"},
            new Country{name ="Austria", iso ="AUT"},
            new Country{name ="Azerbaijan", iso ="AZE"},
            new Country{name ="Bahamas", iso ="BHS"},
            new Country{name ="Bahrain", iso ="BHR"},
            new Country{name ="Bangladesh", iso ="BGD"},
            new Country{name ="Barbados", iso ="BRB"},
            new Country{name ="Belarus", iso ="BLR"},
            new Country{name ="Belgium", iso ="BEL"},
            new Country{name ="Belize", iso ="BLZ"},
            new Country{name ="Benin", iso ="BEN"},
            new Country{name ="Bermuda", iso ="BMU"},
            new Country{name ="Bhutan", iso ="BTN"},
            new Country{name ="Bolivia", iso ="BOL"},
            new Country{name ="Bonaire, Sint Eustatius and Saba", iso ="BES"},
            new Country{name ="Bosnia and Herzegovina", iso ="BIH"},
            new Country{name ="Botswana", iso ="BWA"},
            new Country{name ="Bouvet Island", iso ="BVT"},
            new Country{name ="Brazil", iso ="BRA"},
            new Country{name ="British Indian Ocean Territory", iso ="IOT"},
            new Country{name ="Brunei Darussalam", iso ="BRN"},
            new Country{name ="Bulgaria", iso ="BGR"},
            new Country{name ="Burkina Faso", iso ="BFA"},
            new Country{name ="Burundi", iso ="BDI"},
            new Country{name ="Cabo Verde", iso ="CPV"},
            new Country{name ="Cambodia", iso ="KHM"},
            new Country{name ="Cameroon", iso ="CMR"},
            new Country{name ="Canada", iso ="CAN"},
            new Country{name ="Cayman Islands", iso ="CYM"},
            new Country{name ="Central African Republic", iso ="CAF"},
            new Country{name ="Chad", iso ="TCD"},
            new Country{name ="Chile", iso ="CHL"},
            new Country{name ="China", iso ="CHN"},
            new Country{name ="Christmas Island", iso ="CXR"},
            new Country{name ="Cocos (Keeling) Islands", iso ="CCK"},
            new Country{name ="Colombia", iso ="COL"},
            new Country{name ="Comoros", iso ="COM"},
            new Country{name ="Congo", iso ="COG"},
            new Country{name ="Congo, Democratic Republic of the", iso ="COD"},
            new Country{name ="Cook Islands", iso ="COK"},
            new Country{name ="Costa Rica", iso ="CRI"},
            new Country{name ="Côte d'Ivoire", iso ="CIV"},
            new Country{name ="Croatia", iso ="HRV"},
            new Country{name ="Cuba", iso ="CUB"},
            new Country{name ="Curaçao", iso ="CUW"},
            new Country{name ="Cyprus", iso ="CYP"},
            new Country{name ="Czechia", iso ="CZE"},
            new Country{name ="Denmark", iso ="DNK"},
            new Country{name ="Djibouti", iso ="DJI"},
            new Country{name ="Dominica", iso ="DMA"},
            new Country{name ="Dominican Republic", iso ="DOM"},
            new Country{name ="Ecuador", iso ="ECU"},
            new Country{name ="Egypt", iso ="EGY"},
            new Country{name ="El Salvador", iso ="SLV"},
            new Country{name ="Equatorial Guinea", iso ="GNQ"},
            new Country{name ="Eritrea", iso ="ERI"},
            new Country{name ="Estonia", iso ="EST"},
            new Country{name ="Eswatini", iso ="SWZ"},
            new Country{name ="Ethiopia", iso ="ETH"},
            new Country{name ="Falkland Islands (Malvinas)", iso ="FLK"},
            new Country{name ="Faroe Islands", iso ="FRO"},
            new Country{name ="Fiji", iso ="FJI"},
            new Country{name ="Finland", iso ="FIN"},
            new Country{name ="France", iso ="FRA"},
            new Country{name ="French Guiana", iso ="GUF"},
            new Country{name ="French Polynesia", iso ="PYF"},
            new Country{name ="French Southern Territories", iso ="ATF"},
            new Country{name ="Gabon", iso ="GAB"},
            new Country{name ="Gambia", iso ="GMB"},
            new Country{name ="Georgia", iso ="GEO"},
            new Country{name ="Germany", iso ="DEU"},
            new Country{name ="Ghana", iso ="GHA"},
            new Country{name ="Gibraltar", iso ="GIB"},
            new Country{name ="Greece", iso ="GRC"},
            new Country{name ="Greenland", iso ="GRL"},
            new Country{name ="Grenada", iso ="GRD"},
            new Country{name ="Guadeloupe", iso ="GLP"},
            new Country{name ="Guam", iso ="GUM"},
            new Country{name ="Guatemala", iso ="GTM"},
            new Country{name ="Guernsey", iso ="GGY"},
            new Country{name ="Guinea", iso ="GIN"},
            new Country{name ="Guinea-Bissau", iso ="GNB"},
            new Country{name ="Guyana", iso ="GUY"},
            new Country{name ="Haiti", iso ="HTI"},
            new Country{name ="Heard Island and McDonald Islands", iso ="HMD"},
            new Country{name ="Holy See", iso ="VAT"},
            new Country{name ="Honduras", iso ="HND"},
            new Country{name ="Hong Kong", iso ="HKG"},
            new Country{name ="Hungary", iso ="HUN"},
            new Country{name ="Iceland", iso ="ISL"},
            new Country{name ="India", iso ="IND"},
            new Country{name ="Indonesia", iso ="IDN"},
            new Country{name ="Iran (Islamic Republic of)", iso ="IRN"},
            new Country{name ="Iraq", iso ="IRQ"},
            new Country{name ="Ireland", iso ="IRL"},
            new Country{name ="Isle of Man", iso ="IMN"},
            new Country{name ="Israel", iso ="ISR"},
            new Country{name ="Italy", iso ="ITA"},
            new Country{name ="Jamaica", iso ="JAM"},
            new Country{name ="Japan", iso ="JPN"},
            new Country{name ="Jersey", iso ="JEY"},
            new Country{name ="Jordan", iso ="JOR"},
            new Country{name ="Kazakhstan", iso ="KAZ"},
            new Country{name ="Kenya", iso ="KEN"},
            new Country{name ="Kiribati", iso ="KIR"},
            new Country{name ="Korea (Democratic People's Republic of)", iso ="PRK"},
            new Country{name ="Korea, Republic of", iso ="KOR"},
            new Country{name ="Kuwait", iso ="KWT"},
            new Country{name ="Kyrgyzstan", iso ="KGZ"},
            new Country{name ="Lao People's Democratic Republic", iso ="LAO"},
            new Country{name ="Latvia", iso ="LVA"},
            new Country{name ="Lebanon", iso ="LBN"},
            new Country{name ="Lesotho", iso ="LSO"},
            new Country{name ="Liberia", iso ="LBR"},
            new Country{name ="Libya", iso ="LBY"},
            new Country{name ="Liechtenstein", iso ="LIE"},
            new Country{name ="Lithuania", iso ="LTU"},
            new Country{name ="Luxembourg", iso ="LUX"},
            new Country{name ="Macao", iso ="MAC"},
            new Country{name ="Madagascar", iso ="MDG"},
            new Country{name ="Malawi", iso ="MWI"},
            new Country{name ="Malaysia", iso ="MYS"},
            new Country{name ="Maldives", iso ="MDV"},
            new Country{name ="Mali", iso ="MLI"},
            new Country{name ="Malta", iso ="MLT"},
            new Country{name ="Marshall Islands", iso ="MHL"},
            new Country{name ="Martinique", iso ="MTQ"},
            new Country{name ="Mauritania", iso ="MRT"},
            new Country{name ="Mauritius", iso ="MUS"},
            new Country{name ="Mayotte", iso ="MYT"},
            new Country{name ="Mexico", iso ="MEX"},
            new Country{name ="Micronesia (Federated States of)", iso ="FSM"},
            new Country{name ="Moldova, Republic of", iso ="MDA"},
            new Country{name ="Monaco", iso ="MCO"},
            new Country{name ="Mongolia", iso ="MNG"},
            new Country{name ="Montenegro", iso ="MNE"},
            new Country{name ="Montserrat", iso ="MSR"},
            new Country{name ="Morocco", iso ="MAR"},
            new Country{name ="Mozambique", iso ="MOZ"},
            new Country{name ="Myanmar", iso ="MMR"},
            new Country{name ="Namibia", iso ="NAM"},
            new Country{name ="Nauru", iso ="NRU"},
            new Country{name ="Nepal", iso ="NPL"},
            new Country{name ="Netherlands", iso ="NLD"},
            new Country{name ="New Caledonia", iso ="NCL"},
            new Country{name ="New Zealand", iso ="NZL"},
            new Country{name ="Nicaragua", iso ="NIC"},
            new Country{name ="Niger", iso ="NER"},
            new Country{name ="Nigeria", iso ="NGA"},
            new Country{name ="Niue", iso ="NIU"},
            new Country{name =" Island", iso ="NFK"},
            new Country{name ="North Macedonia", iso ="MKD"},
            new Country{name ="Northern Mariana Islands", iso ="MNP"},
            new Country{name ="Norway", iso ="NOR"},
            new Country{name ="Oman", iso ="OMN"},
            new Country{name ="Pakistan", iso ="PAK"},
            new Country{name ="Palau", iso ="PLW"},
            new Country{name ="Palestine, State of", iso ="PSE"},
            new Country{name ="Panama", iso ="PAN"},
            new Country{name ="Papua New Guinea", iso ="PNG"},
            new Country{name ="Paraguay", iso ="PRY"},
            new Country{name ="Peru", iso ="PER"},
            new Country{name ="Philippines", iso ="PHL"},
            new Country{name ="Pitcairn", iso ="PCN"},
            new Country{name ="Poland", iso ="POL"},
            new Country{name ="Portugal", iso ="PRT"},
            new Country{name ="Puerto Rico", iso ="PRI"},
            new Country{name ="Qatar", iso ="QAT"},
            new Country{name ="Réunion", iso ="REU"},
            new Country{name ="Romania", iso ="ROU"},
            new Country{name ="Russian Federation", iso ="RUS"},
            new Country{name ="Rwanda", iso ="RWA"},
            new Country{name ="Saint Barthélemy", iso ="BLM"},
            new Country{name ="Saint Helena, Ascension and Tristan da Cunha", iso ="SHN"},
            new Country{name ="Saint Kitts and Nevis", iso ="KNA"},
            new Country{name ="Saint Lucia", iso ="LCA"},
            new Country{name ="Saint Martin (French part)", iso ="MAF"},
            new Country{name ="Saint Pierre and Miquelon", iso ="SPM"},
            new Country{name ="Saint Vincent and the Grenadines", iso ="VCT"},
            new Country{name ="Samoa", iso ="WSM"},
            new Country{name ="San Marino", iso ="SMR"},
            new Country{name ="Sao Tome and Principe", iso ="STP"},
            new Country{name ="Saudi Arabia", iso ="SAU"},
            new Country{name ="Senegal", iso ="SEN"},
            new Country{name ="Serbia", iso ="SRB"},
            new Country{name ="Seychelles", iso ="SYC"},
            new Country{name ="Sierra Leone", iso ="SLE"},
            new Country{name ="Singapore", iso ="SGP"},
            new Country{name ="Sint Maarten (Dutch part)", iso ="SXM"},
            new Country{name ="Slovakia", iso ="SVK"},
            new Country{name ="Slovenia", iso ="SVN"},
            new Country{name ="Solomon Islands", iso ="SLB"},
            new Country{name ="Somalia", iso ="SOM"},
            new Country{name ="South Africa", iso ="ZAF"},
            new Country{name ="South Georgia and the South Sandwich Islands", iso ="SGS"},
            new Country{name ="South Sudan", iso ="SSD"},
            new Country{name ="Spain", iso ="ESP"},
            new Country{name ="Sri Lanka", iso ="LKA"},
            new Country{name ="Sudan", iso ="SDN"},
            new Country{name ="Suriname", iso ="SUR"},
            new Country{name ="Svalbard and Jan Mayen", iso ="SJM"},
            new Country{name ="Sweden", iso ="SWE"},
            new Country{name ="Switzerland", iso ="CHE"},
            new Country{name ="Syrian Arab Republic", iso ="SYR"},
            new Country{name ="Taiwan, Province of China", iso ="TWN"},
            new Country{name ="Tajikistan", iso ="TJK"},
            new Country{name ="Tanzania, United Republic of", iso ="TZA"},
            new Country{name ="Thailand", iso ="THA"},
            new Country{name ="Timor-Leste", iso ="TLS"},
            new Country{name ="Togo", iso ="TGO"},
            new Country{name ="Tokelau", iso ="TKL"},
            new Country{name ="Tonga", iso ="TON"},
            new Country{name ="Trinidad and Tobago", iso ="TTO"},
            new Country{name ="Tunisia", iso ="TUN"},
            new Country{name ="Turkey", iso ="TUR"},
            new Country{name ="Turkmenistan", iso ="TKM"},
            new Country{name ="Turks and Caicos Islands", iso ="TCA"},
            new Country{name ="Tuvalu", iso ="TUV"},
            new Country{name ="Uganda", iso ="UGA"},
            new Country{name ="Ukraine", iso ="UKR"},
            new Country{name ="United Arab Emirates", iso ="ARE"},
            new Country{name ="United Kingdom of Great Britain and Northern Ireland", iso ="GBR"},
            new Country{name ="United States of America", iso ="USA"},
            new Country{name ="United States Minor Outlying Islands", iso ="UMI"},
            new Country{name ="Uruguay", iso ="URY"},
            new Country{name ="Uzbekistan", iso ="UZB"},
            new Country{name ="Vanuatu", iso ="VUT"},
            new Country{name ="Venezuela (Bolivarian Republic of)", iso ="VEN"},
            new Country{name ="Viet Nam", iso ="VNM"},
            new Country{name ="Virgin Islands (British)", iso ="VGB"},
            new Country{name ="Virgin Islands (U.S.)", iso ="VIR"},
            new Country{name ="Wallis and Futuna", iso ="WLF"},
            new Country{name ="Western Sahara", iso ="ESH"},
            new Country{name ="Yemen", iso ="YEM"},
            new Country{name ="Zambia", iso ="ZMB"},
            new Country{name ="Zimbabwe", iso ="ZWE"},
            new Country{name ="United", iso ="UNO"},
            new Country{name ="United Nations Organization or one of its officials", iso ="UNO"},
            new Country{name ="Specialized agency of the United Nations or one of its officials", iso ="UNA"},
            new Country{name ="Resident of Kosovo to whom a travel document has been issued by the United Nations Interim Administration Mission in Kosovo", iso ="UNM"},
            new Country{name ="Sovereign Military Order of Malta or one of its emissaries", iso ="XOM"},
            new Country{name ="Caribbean Community or one of its emissaries", iso ="XCC"},
            new Country{name ="Stateless person as defined in Article 01 of the 1954 Convention Relating to the Status of Stateless Persons", iso ="XXA"},
            new Country{name ="Refugee other than as defined under the code", iso ="XXB"},
            new Country{name ="Common Market for Eastern and Southern Africa", iso ="XCO"},
            new Country{name ="Other", iso ="OTHER"},
            new Country{name ="Unknown country", iso ="UNKNOWN"} 
        };

        
        public static DataTable AllCountries()
        {
            DataTable d = new DataTable();
            d.Columns.Add("Iso");
            d.Columns.Add("Name");
            foreach (var i in list) d.Rows.Add(i.iso, i.name);
            return d;
        }
    }

    public class DocumentType
    {
        public string value { get; set; }
        public string name { get; set; }

        private static  List<DocumentType> list = new List<DocumentType>
        {
            new DocumentType{ name ="~ Select ~", value =""},
            new DocumentType{ name = "PASSPORT", value = "PASSPORT"},
            new DocumentType{ name = "PASSPORT DIPLOMAT", value = "PASSPORT_DIPLOMAT"},
            new DocumentType{ name = "SERVICE PASSPORT", value = "SERVICE_PASSPORT"},
            new DocumentType{ name = "VISA", value = "VISA"} 

        };
         
        public static DataTable AllDocuments()
        {
            DataTable d = new DataTable();
            d.Columns.Add("value");
            d.Columns.Add("name");
            foreach (var i in list) d.Rows.Add(i.value, i.name);
            return d;
        }

    }
    public class TravelEventType
    {
        public string value { get; set; }
        public string name { get; set; }

        private static List<TravelEventType> list = new List<TravelEventType>
    {
        new TravelEventType { name = "~ Select ~", value = "" },
        new TravelEventType { name = "Arrival", value = "Arrival" },
        new TravelEventType { name = "Departure", value = "Departure" },
        // Add other travel event types here
    };

        public static DataTable AllEvents()
        {
            DataTable d = new DataTable();
            d.Columns.Add("value");
            d.Columns.Add("name");
            foreach (var i in list) d.Rows.Add(i.value, i.name);
            return d;
        }
    }

    public class FlightEventType
    {
        public string value { get; set; }
        public string name { get; set; }

        private static List<FlightEventType> list = new List<FlightEventType>
    {
        new FlightEventType { name = "~ Select ~", value = "" },
        new FlightEventType { name = "Flight 1", value = "Flight 1" },
        new FlightEventType { name = "Flight 2", value = "Flight 2" },
    };

        public static DataTable AllFlightEvents()
        {
            DataTable d = new DataTable();
            d.Columns.Add("value");
            d.Columns.Add("name");
            foreach (var i in list) d.Rows.Add(i.value, i.name);
            return d;
        }
    }



    public class FingerType
    {
        public string name { get; set; }
        public string value { get; set; }

        private static List<FingerType> list = new List<FingerType>
        { 
            new FingerType { name ="Left_Fingers_Segement_0", value ="LEFT_LITTLE"},
            new FingerType { name ="Left_Fingers_Segement_1", value ="LEFT_RING"},
            new FingerType { name ="Left_Fingers_Segement_2", value ="LEFT_MIDDLE"},
            new FingerType { name ="Left_Fingers_Segement_3", value ="LEFT_INDEX"},
            new FingerType { name ="Thumbs_Segement_0", value ="LEFT_THUMB"},
            new FingerType { name ="Thumbs_Segement_1", value ="RIGHT_THUMB"},
            new FingerType { name ="Right_Fingers_Segement_0", value ="RIGHT_INDEX"},
            new FingerType { name ="Right_Fingers_Segement_1", value ="RIGHT_MIDDLE"},
            new FingerType { name ="Right_Fingers_Segement_2", value ="RIGHT_RING"},
            new FingerType { name ="Right_Fingers_Segement_3", value ="RIGHT_LITTLE"}
        };

        public static string getFinger(string _name)
        {
            return list.Where(x => x.name.Contains(_name)).FirstOrDefault().value;
        }
    }
     

}
