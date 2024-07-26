using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace eVerification.codes
{
    public static class Deserializer
    {
        public static T desialize<T>(string input)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(input))
                return (T)serializer.Deserialize(reader);
        }
    }
    public class APIInterface
    {
        public string url { get; set; }
        public VerifyResponse verify_with_fingerprint(VerifyRequest req)
        {
            var _url = new System.Text.StringBuilder();
            _url.Append(url != null ? url.TrimEnd('/') : "").Append("/validate_finger_print");

            string _req = JsonConvert.SerializeObject(req);
            Logger.LogError("Method:verify_with_fingerprint  Request: " + _req);
            var res = this.httpRequest(_url.ToString(), _req);
            Logger.LogError("Method:verify_with_fingerprint  Response: " + res);
            return !string.IsNullOrEmpty(res) ? JsonConvert.DeserializeObject<VerifyResponse>(res) : null;
        }

        public VerifyResponse verify_document(VerifyRequest req)
        {
            try
            {
                var _url = new System.Text.StringBuilder();
                _url.Append(url != null ? url.TrimEnd('/') : "").Append("/validateDocumentNumber");
                _url.Append("?documentNumber=").Append(req.documentNumber);
                _url.Append("&issuingCountry=").Append(req.issuingcountry);
                _url.Append("&documentType=").Append(req.documentType);

                Logger.LogError("Generated URL: " + _url.ToString());

                var res = this.httpRequest(_url.ToString(), null);
                Logger.LogError("Method:validateDocumentNumber  Response: " + res);

                return !string.IsNullOrEmpty(res) ? JsonConvert.DeserializeObject<VerifyResponse>(res) : null;
            }
            catch (WebException ex)
            {
                Logger.LogError("WebException in verify_document: " + ex.Message);
                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        Logger.LogError("Response: " + reader.ReadToEnd());
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception in verify_document: " + ex.Message);
                throw;
            }
        }



        public VerifyResponse verify_fingerprint_only(VerifyRequest req)
        {
            var _url = new System.Text.StringBuilder();
            _url.Append(url != null ? url.TrimEnd('/') : "").Append("/check_individual_by_finger_print");

            string _req = JsonConvert.SerializeObject(req);
            Logger.LogError("Method:verify_fingerprint_only  Request: " + _req);
            var res = this.httpRequest(_url.ToString(), _req);
            Logger.LogError("Method:verify_fingerprint_only  Response: " + res);
            return !string.IsNullOrEmpty(res) ? JsonConvert.DeserializeObject<VerifyResponse>(res) : null;
        }

        public TravelEntryResponse post_travel_entry(TravelEntryRequest req)
        {
            try
            {
                var _url = new System.Text.StringBuilder();
                _url.Append(url != null ? url.TrimEnd('/') : "").Append("/postTravelEntry");

                Logger.LogError("Generated URL: " + _url.ToString());

                // Serialize the request object to JSON
                string jsonData = JsonConvert.SerializeObject(req);
                Logger.LogError("JSON Data: " + jsonData);

                // Send the request
                var res = this.httpRequest(_url.ToString(), jsonData);
                Logger.LogError("Method:postTravelEntry Response: " + res);

                // Check if the response starts with '{' or '[' indicating a JSON response
                if (!string.IsNullOrEmpty(res))
                {
                    if (res.Trim().StartsWith("{") || res.Trim().StartsWith("["))
                    {
                        return JsonConvert.DeserializeObject<TravelEntryResponse>(res);
                    }
                    else
                    {
                        Logger.LogError("Unexpected response format: " + res);
                    }
                }
                else
                {
                    Logger.LogError("Empty response from API.");
                }

                return null;
            }
            catch (WebException ex)
            {
                Logger.LogError("WebException in post_travel_entry: " + ex.Message);
                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        Logger.LogError("Response: " + reader.ReadToEnd());
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception in post_travel_entry: " + ex.Message);
                throw;
            }
        }




        public SearchResponse search(SearchRequest req)
        {
            var _url = new System.Text.StringBuilder();
            _url.Append(url != null ? url.TrimEnd('/') : "").Append("/search_individual_by_names");

            string _req = JsonConvert.SerializeObject(req);
            Logger.LogError("Method:search  Request: " + _req);
            var res = this.httpRequest(_url.ToString(), _req);
            Logger.LogError("Method:search  Response: " + res);
            return !string.IsNullOrEmpty(res) ? JsonConvert.DeserializeObject<SearchResponse>(res) : null;
        }

        public LoginResponse login(LoginRequest req)
        {
            var _url = new System.Text.StringBuilder();
            _url.Append(url != null ? url.TrimEnd('/') : "").Append("/validate_user");

            string _req = JsonConvert.SerializeObject(req);
            Logger.LogError("Method:login  Request: " + _req);
            var res = this.httpRequest(_url.ToString(), _req);
            Logger.LogError("Method:login  Response: " + res);
            return !string.IsNullOrEmpty(res) ? JsonConvert.DeserializeObject<LoginResponse>(res) : null;
        }

        public string httpRequest(string url, string data)
        {
            string str = string.Empty;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }
            catch (Exception ex)
            {
                string fx, caller;
                Logger.ErrorFunctions(new StackTrace(ex), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);
                throw ex;
            }

            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;

            if (data == null)
            {
                // Perform a GET request
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
            }
            else
            {
                // Perform a POST request
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                httpWebRequest.ContentLength = (long)buffer.Length;
                httpWebRequest.ServicePoint.Expect100Continue = false;

                using (Stream requestStream = httpWebRequest.GetRequestStream())
                    requestStream.Write(buffer, 0, buffer.Length);
            }

            try
            {
                using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
                    str = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    str = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    str = ex.Message;
                }
                string fx, caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);
                //throw ex;
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);
                //throw ex;
            }
            return str;
        }

    }

    public class VerifyRequest
    {
        public string documentType { get; set; }
        public string documentNumber { get; set; }
        public string issuingcountry { get; set; }
        public Finger[] fingers { get; set; }
    }

    public class Finger
    {
        public string fingerType { get; set; }
        public byte[] fingerPrint { get; set; }
    }



    public class VerifyResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string fullName { get; set; }
        public string surName { get; set; }
        public string givenNames { get; set; }
        public string applicationNumber { get; set; }
        public string passportNumber { get; set; }
        public string nationality { get; set; }
        public string dateOfBirth { get; set; }
        public string photo { get; set; }
        public string applicationtype { get; set; }
        public string applicationStatus { get; set; }
        public string visaExpirationDate { get; set; }
        public string message { get; set; }
        public string placeOfBirth { get; set; }
        public string email { get; set; }
        public string documentPlaceOfIssue { get; set; }
        public string matchingScore { get; set; }
        public string providedFingerPrint { get; set; }
        public string fingerPrintFromDatabase { get; set; }
        public string fingerType { get; set; }
        public Visaapplicationhistory[] visaApplicationHistory { get; set; }
    }

     

    public class Visaapplicationhistory
    {
        public string ID { get; set; }
        public string VISALON_APP_ID { get; set; }
        public string APPLICATION_STATUS { get; set; }
        public string APPLY_TYPE { get; set; }
        public string CATEGORY { get; set; }
        public string CHIP_SN { get; set; }
        public DateTime CREATION_DATE { get; set; }
        public DateTime DATE_OF_EXPIRY { get; set; }
        public string FINGERPRINTS_MATCH_RESULT { get; set; }
        public string ORGANIZATION_CODE { get; set; }
        public string PREVIOUS_PERMIT_ID { get; set; }
        public string REASONS { get; set; }
        public DateTime RESOLUTION_DATE { get; set; }
        public string SUBCATEGORY { get; set; }
        public string VISALON_APP_TYPE { get; set; }
        public int DOCUMENT_RECORD_ID { get; set; }
        public string ORGANIZATION_ID { get; set; }
        public string PERSON_RECORD_ID { get; set; }
        public string REFERRED_TO { get; set; }
        public string VISALON_PERSON_DATA_ID { get; set; }
        public string EPAYMENT_STATUS { get; set; }
        public string REASON_FOR_CANCELLING { get; set; }
        public string REASON_FOR_REJECTION { get; set; }
        public string DEFER_AND_HOLD { get; set; }
        public string APPLICATION_FINAL { get; set; }
        public string PERMIT_GROUP_ID { get; set; }
        public string EXPIRATION_NOTIFIED { get; set; }
        public string REASON_FOR_CANCEL_BY_USER { get; set; }
        public string REPLACEMENT_PRINT_OPTION_ID { get; set; }
    }


    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }


    public class LoginResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }



    public class SearchRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }


    public class SearchResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string application_id { get; set; }
        public string application_status { get; set; }
        public DateTime BIRTHDAY { get; set; }
        public string EMAIL { get; set; }
        public string PASSPORT_NUMBER { get; set; }
        public string PLACE_OF_BIRTH { get; set; }
        public string PLACE_OF_ISSUE { get; set; }
        public object SPOUSE_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string VISALON_APP_TYPE { get; set; }
        public string document_type { get; set; }
        public string issuing_country { get; set; }
        public string nationality { get; set; }
    }

    public class TravelEntryRequest
    {
        public string travelEventType { get; set; }
        public string documentNumber { get; set; }
        public string issuingCountry { get; set; }
        public string documentType { get; set; }
        public string surname { get; set; }
        public string givenNames { get; set; }
        public string applicationNumber { get; set; }
        public string passportNumber { get; set; }
        public string nationality { get; set; }
        public string dateOfBirth { get; set; }
        public string placeOfBirth { get; set; }
        public string countryOfResidence { get; set; }
        public string flightNumber { get; set; }
        public string origination { get; set; }
        public string destination { get; set; }
        public int lengthOfStay { get; set; }
        public string profession { get; set; }
        public string reasonForTravel { get; set; }
        public DateTime travelDateTime { get; set; }
        public string photo { get; set; }
        public List<FingerPrintEntry> fingerImages { get; set; }
    }

    public class FingerPrintEntry
    {
        public string fingerType { get; set; }
        public string fingerImage { get; set; }
        public int fingerPrintQuality { get; set; }
    }

    public class TravelEntryResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }


}

