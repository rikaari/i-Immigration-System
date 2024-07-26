using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using IBscanUltimate;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using eVerification.codes;
using System.Diagnostics;
using ElySCardDotNet;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO.Ports;
using eVerification.codes.mrz;
using Newtonsoft.Json;

namespace eVerification
{
    public partial class SDKMainForm : Form
    {
        private struct CaptureInfo
        {
            public string PreCaptureMessage;		// to display on fingerprint window
            public string PostCaptuerMessage;		// to display on fingerprint window
            public DLL.IBSU_ImageType ImageType;				// capture mode
            public int NumberOfFinger;			// number of finger count
            public string fingerName;				// finger name (e.g left thumbs, left index ... )
            public DLL.IBSM_FingerPosition fingerPosition;		// Finger position. e.g Right Thumb, Right Index finger
        }

        private struct ThreadParam
        {
            public IntPtr pParentHandle;
            public IntPtr pFrameImageHandle;
            public int devIndex;
        }


        // Capture sequence definitions
        private const string CAPTURE_SEQ_FLAT_SINGLE_FINGER = "Single flat finger";
        private const string CAPTURE_SEQ_ROLL_SINGLE_FINGER = "Single rolled finger";
        private const string CAPTURE_SEQ_2_FLAT_FINGERS = "2 flat fingers";
        private const string CAPTURE_SEQ_10_SINGLE_FLAT_FINGERS = "10 single flat fingers";
        private const string CAPTURE_SEQ_10_SINGLE_ROLLED_FINGERS = "10 single rolled fingers";
        private const string CAPTURE_SEQ_4_FLAT_FINGERS = "4 flat fingers";
        private const string CAPTURE_SEQ_10_FLAT_WITH_4_FINGER_SCANNER = "10 flat fingers with 4-finger scanner";

        // Beep definitions
        private const int __BEEP_FAIL__ = 0;
        private const int __BEEP_SUCCESS__ = 1;
        private const int __BEEP_OK__ = 2;
        private const int __BEEP_DEVICE_COMMUNICATION_BREAK__ = 3;

        // LED color definitions
        private const int __LED_COLOR_NONE__ = 0;
        private const int __LED_COLOR_GREEN__ = 1;
        private const int __LED_COLOR_RED__ = 2;
        private const int __LED_COLOR_YELLOW__ = 3;

        // Key button definitions
        private const int __LEFT_KEY_BUTTON__ = 1;
        private const int __RIGHT_KEY_BUTTON__ = 2;

        static Thread m_initThread;
        static Object m_sync = new Object();

        private DLL.IBSU_Callback m_callbackDeviceCommunicationBreak = null;
        private DLL.IBSU_CallbackPreviewImage m_callbackPreviewImage = null;
        private DLL.IBSU_CallbackFingerCount m_callbackFingerCount = null;
        private DLL.IBSU_CallbackFingerQuality m_callbackFingerQuality = null;
        private DLL.IBSU_CallbackDeviceCount m_callbackDeviceCount = null;
        private DLL.IBSU_CallbackInitProgress m_callbackInitProgress = null;
        private DLL.IBSU_CallbackTakingAcquisition m_callbackTakingAcquisition = null;
        private DLL.IBSU_CallbackCompleteAcquisition m_callbackCompleteAcquisition = null;
        private DLL.IBSU_CallbackClearPlatenAtCapture m_callbackClearPlaten = null;
        private DLL.IBSU_CallbackResultImageEx m_callbackResultImageEx = null;
        private DLL.IBSU_CallbackKeyButtons m_callbackPressedKeyButtons = null;


        private DLL.IBSU_SdkVersion m_verInfo;
        private int m_nDevHandle;								///< Device handle
        public bool m_bSelectDev;
        public bool m_bInitializing;							///< Device initialization is in progress
        private int m_nCurrentCaptureStep;
        private string m_ImgSaveFolder;				            ///< Base folder for image saving
        private string m_ImgSubFolder;								///< Sub Folder for image sequence
        private string m_strImageMessage;
        private bool m_bNeedClearPlaten;
        private bool m_bBlank;
        private bool m_bSaveWarningOfClearPlaten;
        private int m_nOvImageTextHandle;
        private int m_nOvClearPlatenHandle;
        private int[] m_nOvSegmentHandle = new int[DLL.IBSU_MAX_SEGMENT_COUNT];
        private DLL.IBSU_LedType m_LedType;

        public static codes.VerifyResponse verifyResponse;
        public static codes.SearchResponse searchResponse;
        public static int SelectedIdex;
        private string strFolder = "";

        private List<CaptureInfo> m_vecCaptureSeq = new List<CaptureInfo>();
        private DLL.IBSU_FingerQualityState[] m_FingerQuality = new DLL.IBSU_FingerQualityState[DLL.IBSU_MAX_SEGMENT_COUNT];


        #region MRZ

        Dictionary<string, string> friendlyPorts = BuildPortNameHash(SerialPort.GetPortNames());
        private volatile List<string> dg1 = new List<string>();
        private string mrz_info;
        //Variables Scanner
        private Scanner m_scanner = null;
        private DelegateReadMRZ m_delegateReadMRZ = null;
        private MRZ mrz_infos;
        //private ElyMRTDDotNet.ElyMRTDDotNet ElyObject;
        private Customer customer;
        string mPortName = "";


        #endregion

        public SDKMainForm()
        {
            InitializeComponent();

            //ElyObject = new ElyMRTDDotNet.ElyMRTDDotNet();

            // Associate the function with the EnabledChanged event
            m_btnCaptureStart.EnabledChanged += (sender, e) => UpdateButtonColor();
            try
            {

                m_delegateReadMRZ = new DelegateReadMRZ(readMRZ);
                m_scanner = new Scanner(m_delegateReadMRZ);
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

                MessageBox.Show("Please connect an ID Box Reader", "ELY TRAVEL DOC", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


            try
            {
                mPortName = set_port();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

                //throw ex;
            }
        }

        private void connectMRZ()
        {
            try
            {
                string device_details = "";
                this.m_scanner.Disconnect();

                if (this.mPortName != string.Empty)
                {
                    try
                    {
                        string message;
                        message = "Connecting to MRZ Reader...";
                        _SetStatusBarMessage(message);
                        m_scanner.Connect(mPortName);
                        if (m_scanner.IsConnected())
                        {
                            message = "MRZ Reader Connected";
                            _SetStatusBarMessage(message);
                            try
                            {
                                string version = m_scanner.GetVersion();

                                if (version != "")
                                {
                                    device_details += "Version: " + version + "\n";
                                    if ((Convert.ToDouble(version.Substring(0, 4).Replace(".", ",")) > 2.53))
                                    {
                                        try
                                        {
                                            string serial = m_scanner.GetSerialNumber();

                                            if (serial != "")
                                            {
                                                device_details += "S/N " + serial + "\n";
                                            }

                                        }
                                        catch (Exception exc)
                                        {
                                            string fx;
                                            string caller;
                                            Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                                            Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(exc, Environment.StackTrace)), fx, caller);

                                            try
                                            {
                                                //ElyObject.disconnect();
                                            }
                                            catch (Exception excep)
                                            {
                                                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                                                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(excep, Environment.StackTrace)), fx, caller);
                                            }
                                            return;
                                        }

                                        try
                                        {
                                            string prod_info = m_scanner.GetProductInformation();

                                            if (prod_info != "")
                                            {
                                                device_details += "P/N " + prod_info + "\n";
                                            }

                                        }
                                        catch (Exception exc)
                                        {
                                            string fx;
                                            string caller;
                                            Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                                            Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(exc, Environment.StackTrace)), fx, caller);

                                            try
                                            {
                                                //ElyObject.disconnect();
                                            }
                                            catch (Exception excep)
                                            {
                                                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                                                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(excep, Environment.StackTrace)), fx, caller);

                                            }
                                            return;
                                        }
                                    }
                                }

                            }
                            catch (Exception exc)
                            {
                                string fx;
                                string caller;
                                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(exc, Environment.StackTrace)), fx, caller);

                                try
                                {
                                    //ElyObject.disconnect();
                                }
                                catch (Exception excep)
                                {
                                    Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                                    Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(excep, Environment.StackTrace)), fx, caller);
                                }
                                return;
                            }



                            try
                            {
                                string nnaVersion = m_scanner.GetNnaVersion();

                                if (nnaVersion != "")
                                {
                                    device_details += "FW NNA " + nnaVersion[0] + "." + nnaVersion[1] + "\n";
                                }
                            }
                            catch (Exception ex)
                            {
                                string fx;
                                string caller;
                                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

                            }

                            //m_scanner.EnableReading = this.formOptions.checkBoxAutoDetect.Checked;
                        }

                    }
                    catch (Exception ex)
                    {
                        string fx;
                        string caller;
                        Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                        Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

                    }
                }

                Logger.LogError("connect MRZ-> " + (string.IsNullOrEmpty(device_details) ? "Failed...." : device_details), "", "");
            }
            catch (Exception exc)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(exc, Environment.StackTrace)), fx, caller);

            }
        }

        private void Read()
        {
            try
            {
                Logger.LogError("Reading MRZ...", "", "");
                if (m_scanner.IsConnected())
                {
                    Logger.LogError("Scanner(MRZ) Connected", "", "");
                    string message;
                    message = "Reading MRZ Data ...";
                    _SetStatusBarMessage(message);
                    m_scanner.Inquire();
                    //ElyRDRControl.RDR_CloseComm();
                    return;
                }
            }
            catch (Exception exc)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(exc, Environment.StackTrace)), fx, caller);

            }
        }


        private void readMRZ(string mrz)
        {
            Logger.LogError("readMRZ -> Called [ " + mrz + " ]", "", "");
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(m_delegateReadMRZ, new Object[] { mrz });
                }
                catch (Exception ex)
                {
                    string fx;
                    string caller;
                    Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                    Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

                }
                return;
            }

            try
            {
                mrz_infos = new MRZ(mrz);

                String mrzBlock = mrz.Replace(System.Environment.NewLine, string.Empty);
                if (mrzBlock.Length == 88)
                {
                    TD3MRZParser mrzParser = new TD3MRZParser();
                    customer = mrzParser.Parse(mrzBlock);
                }
                else
                {
                    TD1MRZParser mrzParser = new TD1MRZParser();
                    customer = mrzParser.Parse(mrzBlock);
                }
                this.mrz_info = mrz_infos.MRZ_info;


                readDocument();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }
        private void readDocument()
        {
            try
            {
                Logger.LogError("Parsing MRZ...", "", "");
                if (this.mrz_infos.MRZ_info != null)
                {
                    string message;
                    message = "Parsing MRZ data...";
                    _SetStatusBarMessage(message);
                    MRZ_ParserDotNet.MRZ_ParserDotNet mrz_object = this.mrz_infos.Line3.Length > 5 ?
                        new MRZ_ParserDotNet.MRZ_ParserDotNet(true, new string[] {
                            mrz_infos.Line1, mrz_infos.Line2, mrz_infos.Line3 }) :
                        new MRZ_ParserDotNet.MRZ_ParserDotNet(false, new string[] { mrz_infos.Line1, mrz_infos.Line2 });

                    string doctype = mrz_infos.Line1.Substring(0, 1).Trim();
                    string docNo = mrz_object.getDocumentNumber().Trim();
                    string country = mrz_object.getCountryName(mrz_object.getNationality()).Trim();
                    Logger.LogError("\ndoctype: " + doctype + " docNo: " + docNo + " country: " + country, "", "");

                    if (string.IsNullOrEmpty(doctype) || string.IsNullOrEmpty(docNo) || string.IsNullOrEmpty(country))
                    {
                        MessageBox.Show("MRZ Reader failed to read all details. Please try again.");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(doctype))
                        {
                            if (doctype.Equals("E") || doctype.Equals("P") || doctype.Equals("S") || doctype.Equals("D") || doctype.Equals("C"))
                            {
                                cbDocType.SelectedIndex = cbDocType.FindStringExact("PASSPORT");
                            }
                            else if (doctype.Equals("V"))
                            {
                                cbDocType.SelectedIndex = cbDocType.FindStringExact("VISA");
                            }
                        }

                        if (!string.IsNullOrEmpty(docNo))
                        {
                            tbDocumentNo.Text = RemoveSpecialCharacters(docNo);
                        }

                        if (!string.IsNullOrEmpty(country))
                        {
                            cbCountry.SelectedIndex = cbCountry.FindStringExact(country);
                        }

                        if (validate())
                            if (cbDocumentOnly.Checked || cbFingerPrinterLater.Checked)
                                m_btnVerify_Click(m_btnVerify, null);
                    }

                }
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

                MRZ_ParserDotNet.MRZ_ParserDotNet mrz_object = this.mrz_infos.Line3.Length > 5 ?
                    new MRZ_ParserDotNet.MRZ_ParserDotNet(true, new string[] { mrz_infos.Line1, mrz_infos.Line2, mrz_infos.Line3 }) :
                    new MRZ_ParserDotNet.MRZ_ParserDotNet(false, new string[] { mrz_infos.Line1, mrz_infos.Line2 });


                string doctype = mrz_infos.Line1.Substring(0, 1).Trim();
                string docNo = mrz_object.getDocumentNumber().Trim();
                string country = mrz_object.getCountryName(mrz_object.getIssuer()).Trim();
                Logger.LogError("\ndoctype: " + doctype + " docNo: " + docNo + " country: " + country, "", "");

                if (string.IsNullOrEmpty(doctype))
                {


                    cbDocType.SelectedIndex = (new string[] { "P", "E", "S" }).Contains(doctype) ? 2 : 4;// cbDocType.FindStringExact();
                }

                if (!string.IsNullOrEmpty(docNo))
                {
                    tbDocumentNo.Text = RemoveSpecialCharacters(docNo);
                }
                if (!string.IsNullOrEmpty(country))
                {
                    cbCountry.SelectedIndex = cbCountry.FindStringExact(country);
                }

            }

        }

        public static string RemoveSpecialCharacters(string input)
        {
            Regex r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, String.Empty);
        }

        #region COM Port Management
        /// <summary>
        /// Begins recursive registry enumeration
        /// </summary>
        /// <param name="portsToMap">array of port names (i.e. COM1, COM2, etc)</param>
        /// <returns>a hashtable mapping Friendly names to non-friendly port values</returns>
        static Dictionary<string, string> BuildPortNameHash(string[] portsToMap)
        {
            Dictionary<string, string> oReturnTable = new Dictionary<string, string>();
            MineRegistryForPortName("SYSTEM\\CurrentControlSet\\Enum", oReturnTable, portsToMap);
            return oReturnTable;
        }
        /// <summary>
        /// Recursively enumerates registry subkeys starting with startKeyPath looking for 
        /// "Device Parameters" subkey. If key is present, friendly port name is extracted.
        /// </summary>
        /// <param name="startKeyPath">the start key from which to begin the enumeration</param>
        /// <param name="targetMap">dictionary that will get populated with 
        /// nonfriendly-to-friendly port names</param>
        /// <param name="portsToMap">array of port names (i.e. COM1, COM2, etc)</param>
        static void MineRegistryForPortName(string startKeyPath, Dictionary<string, string> targetMap,
            string[] portsToMap)
        {
            if (targetMap.Count >= portsToMap.Length)
                return;
            using (RegistryKey currentKey = Registry.LocalMachine)
            {
                try
                {
                    using (RegistryKey currentSubKey = currentKey.OpenSubKey(startKeyPath))
                    {
                        string[] currentSubkeys = currentSubKey.GetSubKeyNames();
                        if (currentSubkeys.Contains("Device Parameters") &&
                            startKeyPath != "SYSTEM\\CurrentControlSet\\Enum")
                        {
                            object portName = Registry.GetValue("HKEY_LOCAL_MACHINE\\" +
                                startKeyPath + "\\Device Parameters", "PortName", null);
                            if (portName == null ||
                                portsToMap.Contains(portName.ToString()) == false)
                                return;
                            object friendlyPortName = Registry.GetValue("HKEY_LOCAL_MACHINE\\" +
                                startKeyPath, "FriendlyName", null);
                            string friendlyName = "N/A";
                            if (friendlyPortName != null)
                                friendlyName = friendlyPortName.ToString();
                            if (friendlyName.Contains(portName.ToString()) == false)
                                friendlyName = string.Format("{0} ({1})", friendlyName, portName);
                            targetMap[portName.ToString()] = friendlyName;
                        }
                        else
                        {
                            foreach (string strSubKey in currentSubkeys)
                                MineRegistryForPortName(startKeyPath + "\\" + strSubKey, targetMap, portsToMap);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error accessing key '{0}'.. Skipping..", startKeyPath);
                }
            }
        }
        #endregion

        private string set_port()
        {
            string sPortName = string.Empty;
            foreach (KeyValuePair<string, string> kvp in friendlyPorts)
                if (kvp.Value.ToLower().Contains("ely"))
                    sPortName = kvp.Key;
            return sPortName;
        }




        ////////////////////////////////////////////////////////////////////////////////////////////
        // User defined functions
        private bool _ChkFolder(string sPath)
        {
            DirectoryInfo di = new DirectoryInfo(sPath);
            return di.Exists;
        }

        private void _InitializeDeviceThreadCallback(object pParam)
        {
            if (pParam == null)
                return;

            ThreadParam param = (ThreadParam)pParam;
            IntPtr formHandle = param.pParentHandle;
            IntPtr frameImageHandle = param.pFrameImageHandle;
            int devIndex = param.devIndex;
            int devHandle = -1;
            int nRc = DLL.IBSU_STATUS_OK;
            int ledCount = 0;
            uint operableLEDs = 0;

            m_bInitializing = true;
            nRc = DLL._IBSU_OpenDevice(devIndex, ref devHandle);
            m_bInitializing = false;

            if (nRc >= DLL.IBSU_STATUS_OK)
            {
                m_nDevHandle = devHandle;

                DLL._IBSU_GetOperableLEDs(devHandle, ref m_LedType, ref ledCount, ref operableLEDs);

                DLL.IBSU_RECT clientRect = new DLL.IBSU_RECT();
                Win32.GetClientRect(frameImageHandle, ref clientRect);

                // Create display window
                DLL._IBSU_CreateClientWindow(devHandle, frameImageHandle, clientRect.left, clientRect.top, clientRect.right, clientRect.bottom);
                DLL._IBSU_AddOverlayQuadrangle(devHandle, ref m_nOvClearPlatenHandle, 0, 0, 0, 0, 0, 0, 0, 0, 0, (uint)0);
                DLL._IBSU_AddOverlayText(devHandle, ref m_nOvImageTextHandle, "Arial", 10, true, "", 10, 10, 0);

                for (int i = 0; i < DLL.IBSU_MAX_SEGMENT_COUNT; i++)
                {
                    DLL._IBSU_AddOverlayQuadrangle(devHandle, ref m_nOvSegmentHandle[i], 0, 0, 0, 0, 0, 0, 0, 0,
                    0, (uint)0);
                }

                // register callback functions
                DLL._IBSU_RegisterCallbacks(devHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_COMMUNICATION_BREAK, m_callbackDeviceCommunicationBreak, formHandle);
                DLL._IBSU_RegisterCallbacks(devHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_PREVIEW_IMAGE, m_callbackPreviewImage, formHandle);
                DLL._IBSU_RegisterCallbacks(devHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_TAKING_ACQUISITION, m_callbackTakingAcquisition, formHandle);
                DLL._IBSU_RegisterCallbacks(devHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_COMPLETE_ACQUISITION, m_callbackCompleteAcquisition, formHandle);
                DLL._IBSU_RegisterCallbacks(devHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_RESULT_IMAGE_EX, m_callbackResultImageEx, formHandle);
                DLL._IBSU_RegisterCallbacks(devHandle, DLL.IBSU_Events.ENUM_IBSU_OPTIONAL_EVENT_FINGER_COUNT, m_callbackFingerCount, formHandle);
                DLL._IBSU_RegisterCallbacks(devHandle, DLL.IBSU_Events.ENUM_IBSU_OPTIONAL_EVENT_FINGER_QUALITY, m_callbackFingerQuality, formHandle);
                DLL._IBSU_RegisterCallbacks(devHandle, DLL.IBSU_Events.ENUM_IBSU_OPTIONAL_EVENT_CLEAR_PLATEN_AT_CAPTURE, m_callbackClearPlaten, formHandle);
                DLL._IBSU_RegisterCallbacks(devHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_KEYBUTTON, m_callbackPressedKeyButtons, formHandle);
            }

            // status notification and sequence start
            if (nRc == DLL.IBSU_STATUS_OK)
            {
                OnMsg_CaptureSeqStart();
                return;
            }

            if (nRc > DLL.IBSU_STATUS_OK)
                OnMsg_InitWarning();
            else
            {
                string message;
                switch (nRc)
                {
                    case DLL.IBSU_ERR_DEVICE_ACTIVE:
                        message = String.Format("[Error code = {0}] Device initialization failed because in use by another thread/process.", nRc);
                        OnMsg_UpdateStatusMessage(message);
                        break;
                    case DLL.IBSU_ERR_USB20_REQUIRED:
                        message = String.Format("[Error code = {0}] Device initialization failed because SDK only works with USB 2.0.", nRc);
                        OnMsg_UpdateStatusMessage(message);
                        break;
                    default:
                        message = String.Format("[Error code = {0}] Device initialization failed", nRc);
                        OnMsg_UpdateStatusMessage(message);
                        break;
                }
            }

            OnMsg_UpdateDeviceList();
        }

        private void _SetStatusBarMessage(string message)
        {
            m_txtStatusMessage.Text = message;
        }

        private void _SetImageMessage(string message)
        {
            int font_size = 10;
            int x = 10;
            int y = 10;
            Color cr = Color.FromArgb(0, 0, 255);

            if (m_bNeedClearPlaten)
            {
                cr = Color.FromArgb(255, 0, 0);
            }

            DLL._IBSU_ModifyOverlayText(m_nDevHandle, m_nOvImageTextHandle, "Arial", font_size, true, message, x, y, (uint)(ColorTranslator.ToWin32(cr)));
            //DLL._IBSU_SetClientWindowOverlayText(m_nDevHandle, "Arial", font_size, true, message, x, y, (uint)ColorTranslator.ToWin32(cr));
        }

        private void _UpdateCaptureSequences()
        {
            OnMsg_UpdateDisplayResources();
        }

        private int _ReleaseDevice()
        {
            int nRc = DLL.IBSU_STATUS_OK;

            if (m_nDevHandle != -1)
                nRc = DLL._IBSU_CloseDevice(m_nDevHandle);

            if (nRc >= DLL.IBSU_STATUS_OK)
            {
                m_nDevHandle = -1;
                m_nCurrentCaptureStep = -1;
                m_bInitializing = false;
            }

            return nRc;
        }

        private void _BeepFail()
        {
            DLL.IBSU_BeeperType beeperType = new DLL.IBSU_BeeperType();
            if (DLL._IBSU_GetOperableBeeper(m_nDevHandle, ref beeperType) != DLL.IBSU_STATUS_OK)
            {
                Win32.Beep(3500, 300);
                Thread.Sleep(150);
                Win32.Beep(3500, 150);
                Thread.Sleep(150);
                Win32.Beep(3500, 150);
                Thread.Sleep(150);
                Win32.Beep(3500, 150);
            }
            else
            {
                DLL._IBSU_SetBeeper(m_nDevHandle, DLL.IBSU_BeepPattern.ENUM_IBSU_BEEP_PATTERN_GENERIC, 2/*Sol*/, 12/*300ms = 12*25ms*/, 0, 0);
                Thread.Sleep(150);
                DLL._IBSU_SetBeeper(m_nDevHandle, DLL.IBSU_BeepPattern.ENUM_IBSU_BEEP_PATTERN_GENERIC, 2/*Sol*/, 6/*150ms = 6*25ms*/, 0, 0);
                Thread.Sleep(150);
                DLL._IBSU_SetBeeper(m_nDevHandle, DLL.IBSU_BeepPattern.ENUM_IBSU_BEEP_PATTERN_GENERIC, 2/*Sol*/, 6/*150ms = 6*25ms*/, 0, 0);
                Thread.Sleep(150);
                DLL._IBSU_SetBeeper(m_nDevHandle, DLL.IBSU_BeepPattern.ENUM_IBSU_BEEP_PATTERN_GENERIC, 2/*Sol*/, 6/*150ms = 6*25ms*/, 0, 0);
            }
        }

        private void _BeepSuccess()
        {
            DLL.IBSU_BeeperType beeperType = new DLL.IBSU_BeeperType();
            if (DLL._IBSU_GetOperableBeeper(m_nDevHandle, ref beeperType) != DLL.IBSU_STATUS_OK)
            {
                Win32.Beep(3500, 100);
                Thread.Sleep(50);
                Win32.Beep(3500, 100);
            }
            else
            {
                DLL._IBSU_SetBeeper(m_nDevHandle, DLL.IBSU_BeepPattern.ENUM_IBSU_BEEP_PATTERN_GENERIC, 2/*Sol*/, 4/*100ms = 4*25ms*/, 0, 0);
                Thread.Sleep(150);
                DLL._IBSU_SetBeeper(m_nDevHandle, DLL.IBSU_BeepPattern.ENUM_IBSU_BEEP_PATTERN_GENERIC, 2/*Sol*/, 4/*100ms = 4*25ms*/, 0, 0);
            }
        }

        private void _BeepOk()
        {
            DLL.IBSU_BeeperType beeperType = new DLL.IBSU_BeeperType();
            if (DLL._IBSU_GetOperableBeeper(m_nDevHandle, ref beeperType) != DLL.IBSU_STATUS_OK)
            {
                Win32.Beep(3500, 100);
            }
            else
            {
                DLL._IBSU_SetBeeper(m_nDevHandle, DLL.IBSU_BeepPattern.ENUM_IBSU_BEEP_PATTERN_GENERIC, 2/*Sol*/, 4/*100ms = 4*25ms*/, 0, 0);
            }
        }

        private void _BeepDeviceCommunicationBreak()
        {
            for (int i = 0; i < 8; i++)
            {
                Win32.Beep(3500, 100);
                Thread.Sleep(100);
            }
        }

        private void _SaveBitmapImage(ref DLL.IBSU_ImageData image, string fingerName)
        {
            if ((m_ImgSaveFolder == null) || (m_ImgSubFolder == null) ||
                (m_ImgSaveFolder.Length == 0) || (m_ImgSubFolder.Length == 0))
            {
                return;
            }

            string strFolder;
            strFolder = String.Format("{0}\\{1}", m_ImgSaveFolder, m_ImgSubFolder);
            System.IO.Directory.CreateDirectory(strFolder);

            string strFileName;
            strFileName = String.Format("{0}\\Image_{1}_{2}.bmp", strFolder, m_nCurrentCaptureStep, fingerName);

            if (DLL._IBSU_SaveBitmapImage(strFileName, image.Buffer,
                                      image.Width, image.Height, image.Pitch,
                                      image.ResolutionX, image.ResolutionY) != DLL.IBSU_STATUS_OK)
            {
                MessageBox.Show("Failed to save bitmap image!");
            }
            // Display image on the corresponding PictureBox
            DisplayImageOnPictureBox(fingerName, strFileName);
        }

        private void _SaveWsqImage(ref DLL.IBSU_ImageData image, string fingerName)
        {
            if ((m_ImgSaveFolder == null) || (m_ImgSubFolder == null) ||
                (m_ImgSaveFolder.Length == 0) || (m_ImgSubFolder.Length == 0))
            {
                return;
            }

            string strFolder;
            strFolder = String.Format("{0}\\{1}", m_ImgSaveFolder, m_ImgSubFolder);
            System.IO.Directory.CreateDirectory(strFolder);

            string strFileName;
            strFileName = String.Format("{0}\\Image_{1}_{2}.wsq", strFolder, m_nCurrentCaptureStep, fingerName);

            // Bug Fixed, WSQ image was flipped vertically.
            // Pitch parameter is required to fix bug.
            if (DLL._IBSU_WSQEncodeToFile(strFileName, image.Buffer,
                                      (int)image.Width, (int)image.Height, image.Pitch, image.BitsPerPixel,
                                      (int)image.ResolutionX, 0.75, "") != DLL.IBSU_STATUS_OK)
            {
                MessageBox.Show("Failed to save bitmap image!");
            }

            /***********************************************************
             * Example codes for WSQ encoding based on memory

            string filename;
            IntPtr pCompressedData = IntPtr.Zero;
            IntPtr pDecompressedData = IntPtr.Zero;
            IntPtr pDecompressedData2 = IntPtr.Zero;
	        int compressedLength=0;
	        if (DLL._IBSU_WSQEncodeMem(image.Buffer,
                                    (int)image.Width, (int)image.Height, image.Pitch, image.BitsPerPixel,
                                    (int)image.ResolutionX, (double)0.75, "",
                                    ref pCompressedData, ref compressedLength) != DLL.IBSU_STATUS_OK )
	        {
                MessageBox.Show("Failed to save WSQ_1 image!");
	        }

            byte [] compressedBuffer =new byte [compressedLength];
            Marshal.Copy(pCompressedData, compressedBuffer, 0, compressedLength);

            filename = String.Format("{0}\\Image_{1}_{2}_v1.wsq", strFolder, m_nCurrentCaptureStep, fingerName);
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(compressedBuffer, 0, compressedLength);
            w.Close();

            int width=0, height=0, pitch=0, bitsPerPixel=0, pixelPerInch=0;
            if (DLL._IBSU_WSQDecodeMem(pCompressedData, compressedLength,
                                    ref pDecompressedData, ref width, ref height,
                                    ref pitch, ref bitsPerPixel, ref pixelPerInch) != DLL.IBSU_STATUS_OK)
            {
                MessageBox.Show("Failed to Decode WSQ image!");
            }

            filename = String.Format("{0}\\Image_{1}_{2}_v1.bmp", strFolder, m_nCurrentCaptureStep, fingerName);
            if (DLL._IBSU_SaveBitmapImage(filename, pDecompressedData,
                                      (uint)width, (uint)height, pitch,
                                      pixelPerInch, pixelPerInch) != DLL.IBSU_STATUS_OK)
            {
                MessageBox.Show("Failed to save bitmap v1 image!");
            }

            if (DLL._IBSU_WSQDecodeFromFile(strFileName, ref pDecompressedData2,
                                      ref width, ref height, ref pitch,
                                      ref pixelPerInch, ref pixelPerInch) != DLL.IBSU_STATUS_OK)
            {
                MessageBox.Show("Failed to Decode WSQ image!");
            }

            filename = String.Format("{0}\\Image_{1}_{2}_v2.bmp", strFolder, m_nCurrentCaptureStep, fingerName);
            if (DLL._IBSU_SaveBitmapImage(filename, pDecompressedData2,
                                      (uint)width, (uint)height, pitch,
                                      pixelPerInch, pixelPerInch) != DLL.IBSU_STATUS_OK)
            {
                MessageBox.Show("Failed to save bitmap v2 image!");
            }

            DLL._IBSU_FreeMemory(pCompressedData);
            DLL._IBSU_FreeMemory(pDecompressedData);
            DLL._IBSU_FreeMemory(pDecompressedData2);
            ***********************************************************/
        }

        private void _SavePngImage(ref DLL.IBSU_ImageData image, string fingerName)
        {
            if ((m_ImgSaveFolder == null) || (m_ImgSubFolder == null) ||
                (m_ImgSaveFolder.Length == 0) || (m_ImgSubFolder.Length == 0))
            {
                return;
            }

            string strFolder;
            strFolder = String.Format("{0}\\{1}", m_ImgSaveFolder, m_ImgSubFolder);
            System.IO.Directory.CreateDirectory(strFolder);

            string strFileName;
            strFileName = String.Format("{0}\\{1}.png", strFolder, fingerName);

            if (DLL._IBSU_SavePngImage(strFileName, image.Buffer,
                                      image.Width, image.Height, image.Pitch,
                                      image.ResolutionX, image.ResolutionY) != DLL.IBSU_STATUS_OK)
            {
                MessageBox.Show("Failed to save png image!");
                return;
            }

            // Display image on the corresponding PictureBox
            DisplayImageOnPictureBox(fingerName, strFileName);
        }

        private void DisplayImageOnPictureBox(string fingerName, string filePath)
        {
            // Extract the base finger name by removing the segment part
            string baseFingerName = fingerName.Split('_')[0] + "_" + fingerName.Split('_')[1] + "_" + fingerName.Split('_')[2];
            // Load the image from the file
            Image originalImage = Image.FromFile(filePath);

            PictureBox targetPictureBox = null;

            switch (baseFingerName)
            {
                case "SFF_Right_Thumb":
                    targetPictureBox = pbRightThumb;
                    break;
                case "SFF_Right_Index":
                    targetPictureBox = pbRightIndex;
                    break;
                case "SFF_Right_Middle":
                    targetPictureBox = pbRightMiddle;
                    break;
                case "SFF_Right_Ring":
                    targetPictureBox = pbRightRing;
                    break;
                case "SFF_Right_Little":
                    targetPictureBox = pbRightLittle;
                    break;
                case "SFF_Left_Thumb":
                    targetPictureBox = pbLeftThumb;
                    break;
                case "SFF_Left_Index":
                    targetPictureBox = pbLeftIndex;
                    break;
                case "SFF_Left_Middle":
                    targetPictureBox = pbLeftMiddle;
                    break;
                case "SFF_Left_Ring":
                    targetPictureBox = pbLeftRing;
                    break;
                case "SFF_Left_Little":
                    targetPictureBox = pbLeftLittle;
                    break;
            }


            if (targetPictureBox == null) return; // Invalid target grid or row index

            // Resize the image to fit the cell size
            Image resizedImage = ResizeImage(originalImage, targetPictureBox.Width, targetPictureBox.Height);

            // Display the resized image on the corresponding DataGridView cell
            targetPictureBox.Image = resizedImage;
        }

        // Resize the image to fit within the specified width and height
        private Image ResizeImage(Image image, int width, int height)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Calculate the aspect ratio
            float aspectRatio = (float)originalWidth / originalHeight;

            // Determine the new width and height while maintaining the aspect ratio
            int newWidth, newHeight;
            if (width / (float)height > aspectRatio)
            {
                newWidth = (int)(height * aspectRatio);
                newHeight = height;
            }
            else
            {
                newWidth = width;
                newHeight = (int)(width / aspectRatio);
            }

            // Create a new bitmap and draw the resized image onto it
            Bitmap resizedBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedBitmap))
            {
                g.Clear(Color.White); // Optionally, set a background color
                g.DrawImage(image, (width - newWidth) / 2, (height - newHeight) / 2, newWidth, newHeight);
            }

            return resizedBitmap;
        }





        private void _SaveJP2Image(ref DLL.IBSU_ImageData image, string fingerName)
        {
            if ((m_ImgSaveFolder == null) || (m_ImgSubFolder == null) ||
                (m_ImgSaveFolder.Length == 0) || (m_ImgSubFolder.Length == 0))
            {
                return;
            }

            string strFolder;
            strFolder = String.Format("{0}\\{1}", m_ImgSaveFolder, m_ImgSubFolder);
            System.IO.Directory.CreateDirectory(strFolder);

            string strFileName;
            strFileName = String.Format("{0}\\Image_{1}_{2}.jp2", strFolder, m_nCurrentCaptureStep, fingerName);

            if (DLL._IBSU_SaveJP2Image(strFileName, image.Buffer,
                                      image.Width, image.Height, image.Pitch,
                                      image.ResolutionX, image.ResolutionY, 80) != DLL.IBSU_STATUS_OK)
            {
                MessageBox.Show("Failed to save jpeg-2000 image!");
            }
        }

        private void _DrawRoundRect(Graphics g, Brush brush, float X, float Y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLine(X + radius, Y, X + width - (radius * 2), Y);
            gp.AddArc(X + width - (radius * 2), Y, radius * 2, radius * 2, 270, 90);
            gp.AddLine(X + width, Y + radius, X + width, Y + height - (radius * 2));
            gp.AddArc(X + width - (radius * 2), Y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
            gp.AddLine(X + width - (radius * 2), Y + height, X + radius, Y + height);
            gp.AddArc(X, Y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
            gp.AddLine(X, Y + height - (radius * 2), X, Y + radius);
            gp.AddArc(X, Y, radius * 2, radius * 2, 180, 90);
            gp.CloseFigure();

            //            g.DrawPath(p, gp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(brush, gp);
            gp.Dispose();
        }

        private int _ModifyOverlayForWarningOfClearPlaten(Boolean bVisible)
        {
            if (m_nDevHandle == -1)
                return -1;

            int nRc = DLL.IBSU_STATUS_OK;
            Color cr = Color.FromArgb(255, 0, 0);
            int left, top, right, bottom;

            DLL.IBSU_RECT clientRect = new DLL.IBSU_RECT();
            Win32.GetClientRect(m_FrameImage.Handle, ref clientRect);

            left = 0; top = 0; right = clientRect.right - clientRect.left; bottom = clientRect.bottom - clientRect.top;
            if (bVisible)
            {
                nRc = DLL._IBSU_ModifyOverlayQuadrangle(m_nDevHandle, m_nOvClearPlatenHandle,
                    left, top, right, top, right, bottom, left, bottom, 20, (uint)(ColorTranslator.ToWin32(cr)));
            }
            else
            {
                nRc = DLL._IBSU_ModifyOverlayQuadrangle(m_nDevHandle, m_nOvClearPlatenHandle,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, (uint)0);
            }

            return nRc;
        }

        private void _SetLEDs(int deviceHandle, CaptureInfo info, int ledColor, bool bBlink)
        {
            uint setLEDs = 0;

            if (m_LedType == DLL.IBSU_LedType.ENUM_IBSU_LED_TYPE_FSCAN)
            {
                if (bBlink)
                {
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_BLINK_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_BLINK_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_BLINK_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_BLINK_RED;
                    }
                }

                if (info.ImageType == DLL.IBSU_ImageType.ENUM_IBSU_ROLL_SINGLE_FINGER)
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_ROLL;
                }

                if (info.fingerName == "SFF_Right_Thumb" ||
                    info.fingerName == "SRF_Right_Thumb")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_TWO_THUMB;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_THUMB_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_THUMB_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_THUMB_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_THUMB_RED;
                    }
                }
                else if (info.fingerName == "SFF_Left_Thumb" ||
                         info.fingerName == "SRF_Left_Thumb")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_TWO_THUMB;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_THUMB_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_THUMB_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_THUMB_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_THUMB_RED;
                    }
                }
                else if (info.fingerName == "TFF_2_Thumbs")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_TWO_THUMB;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_THUMB_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_THUMB_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_THUMB_RED;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_THUMB_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_THUMB_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_THUMB_RED;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_THUMB_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_THUMB_RED;
                    }
                }
                ///////////////////LEFT HAND////////////////////
                else if (info.fingerName == "SFF_Left_Index" ||
                         info.fingerName == "SRF_Left_Index")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_LEFT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_INDEX_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_INDEX_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_INDEX_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_INDEX_RED;
                    }
                }
                else if (info.fingerName == "SFF_Left_Middle" ||
                         info.fingerName == "SRF_Left_Middle")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_LEFT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_MIDDLE_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_MIDDLE_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_MIDDLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_MIDDLE_RED;
                    }
                }
                else if (info.fingerName == "SFF_Left_Ring" ||
                         info.fingerName == "SRF_Left_Ring")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_LEFT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_RING_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_RING_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_RING_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_RING_RED;
                    }
                }
                else if (info.fingerName == "SFF_Left_Little" ||
                         info.fingerName == "SRF_Left_Little")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_LEFT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_LITTLE_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_LITTLE_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_LITTLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_LITTLE_RED;
                    }
                }
                else if (info.fingerName == "4FF_Left_4_Fingers")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_LEFT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_INDEX_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_MIDDLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_RING_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_LITTLE_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_INDEX_RED;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_MIDDLE_RED;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_RING_RED;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_LITTLE_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_LEFT_INDEX_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_MIDDLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_RING_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_LITTLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_INDEX_RED;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_MIDDLE_RED;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_RING_RED;
                        setLEDs |= DLL.IBSU_LED_F_LEFT_LITTLE_RED;
                    }
                }
                ///////////RIGHT HAND /////////////////////////
                else if (info.fingerName == "SFF_Right_Index" ||
                         info.fingerName == "SRF_Right_Index")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_RIGHT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_INDEX_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_INDEX_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_INDEX_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_INDEX_RED;
                    }
                }
                else if (info.fingerName == "SFF_Right_Middle" ||
                         info.fingerName == "SRF_Right_Middle")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_RIGHT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_MIDDLE_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_MIDDLE_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_MIDDLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_MIDDLE_RED;
                    }
                }
                else if (info.fingerName == "SFF_Right_Ring" ||
                         info.fingerName == "SRF_Right_Ring")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_RIGHT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_RING_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_RING_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_RING_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_RING_RED;
                    }
                }
                else if (info.fingerName == "SFF_Right_Little" ||
                         info.fingerName == "SRF_Right_Little")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_RIGHT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_LITTLE_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_LITTLE_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_LITTLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_LITTLE_RED;
                    }
                }
                else if (info.fingerName == "4FF_Right_4_Fingers")
                {
                    setLEDs |= DLL.IBSU_LED_F_PROGRESS_RIGHT_HAND;
                    if (ledColor == __LED_COLOR_GREEN__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_INDEX_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_MIDDLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_RING_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_LITTLE_GREEN;
                    }
                    else if (ledColor == __LED_COLOR_RED__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_INDEX_RED;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_MIDDLE_RED;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_RING_RED;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_LITTLE_RED;
                    }
                    else if (ledColor == __LED_COLOR_YELLOW__)
                    {
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_INDEX_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_MIDDLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_RING_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_LITTLE_GREEN;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_INDEX_RED;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_MIDDLE_RED;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_RING_RED;
                        setLEDs |= DLL.IBSU_LED_F_RIGHT_LITTLE_RED;
                    }
                }

                if (ledColor == __LED_COLOR_NONE__)
                {
                    setLEDs = 0;
                }

                DLL._IBSU_SetLEDs(deviceHandle, setLEDs);
            }
        }

        private static void OnEvent_DeviceCommunicationBreak(
            int deviceIndex,
            IntPtr pContext
            )
        {
            if (pContext == null)
                return;

            lock (m_sync)
            {
                SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                pDlg.OnMsg_DeviceCommunicationBreak();
            }
        }

        private static void OnEvent_PreviewImage(
            int deviceHandle,
            IntPtr pContext,
            DLL.IBSU_ImageData image
            )
        {
            if (pContext == null)
                return;

            lock (SDKMainForm.m_sync)
            {
                SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
            }
        }

        private static void OnEvent_FingerCount(
            int deviceHandle,
            IntPtr pContext,
            DLL.IBSU_FingerCountState fingerCountState
            )
        {
            if (pContext == null)
                return;

            lock (SDKMainForm.m_sync)
            {
                SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                if (deviceHandle != pDlg.m_nDevHandle)
                    return;
                /*
                                string fingerState;
                                if (fingerCountState == DLL.IBSU_FingerCountState.ENUM_IBSU_FINGER_COUNT_OK)
                                    fingerState = "FINGER_COUNT_OK";
                                else if (fingerCountState == DLL.IBSU_FingerCountState.ENUM_IBSU_TOO_MANY_FINGERS)
                                    fingerState = "TOO_MANY_FINGERS";
                                else if (fingerCountState == DLL.IBSU_FingerCountState.ENUM_IBSU_TOO_FEW_FINGERS)
                                    fingerState = "TOO_FEW_FINGERS";
                                else if (fingerCountState == DLL.IBSU_FingerCountState.ENUM_IBSU_NON_FINGER)
                                    fingerState = "NON-FINGER";
                                else
                                    fingerState = "UNKNOWN";
                */
                CaptureInfo info = pDlg.m_vecCaptureSeq[pDlg.m_nCurrentCaptureStep];

                if (fingerCountState == DLL.IBSU_FingerCountState.ENUM_IBSU_NON_FINGER)
                {
                    pDlg._SetLEDs(deviceHandle, info, __LED_COLOR_RED__, true);
                }
                else
                {
                    pDlg._SetLEDs(deviceHandle, info, __LED_COLOR_YELLOW__, true);
                }
            }
        }

        private static void OnEvent_FingerQuality(
            int deviceHandle,
            IntPtr pContext,
            IntPtr pQualityArray,
            int qualityArrayCount
            )
        {
            if (pContext == null)
                return;

            lock (SDKMainForm.m_sync)
            {
                SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                int[] qualityArray = new int[4];
                Marshal.Copy(pQualityArray, qualityArray, 0, qualityArrayCount);
                pDlg.m_FingerQuality[0] = (DLL.IBSU_FingerQualityState)qualityArray[0];
                pDlg.m_FingerQuality[1] = (DLL.IBSU_FingerQualityState)qualityArray[1];
                pDlg.m_FingerQuality[2] = (DLL.IBSU_FingerQualityState)qualityArray[2];
                pDlg.m_FingerQuality[3] = (DLL.IBSU_FingerQualityState)qualityArray[3];

                pDlg.m_picScanner.Invalidate(false);
            }
        }

        private static void OnEvent_DeviceCount(
            int detectedDevices,
            IntPtr pContext
            )
        {
            if (pContext == null)
                return;

            lock (SDKMainForm.m_sync)
            {
                SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                pDlg.OnMsg_UpdateDeviceList();
            }
        }

        private static void OnEvent_InitProgress(
            int deviceIndex,
            IntPtr pContext,
            int progressValue
            )
        {
            if (pContext == null)
                return;

            lock (m_sync)
            {
                SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                string message;
                message = String.Format("Initializing device... {0}%", progressValue);
                pDlg.OnMsg_UpdateStatusMessage(message);
            }
        }

        private static void OnEvent_TakingAcquisition(
            int deviceHandle,
            IntPtr pContext,
            DLL.IBSU_ImageType imageType
            )
        {
            if (pContext == null)
                return;

            lock (m_sync)
            {
                SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                if (imageType == DLL.IBSU_ImageType.ENUM_IBSU_ROLL_SINGLE_FINGER)
                {
                    pDlg.OnMsg_Beep(__BEEP_OK__);
                    pDlg.m_strImageMessage = "When done remove finger from sensor";
                    pDlg.OnMsg_UpdateImageMessage(pDlg.m_strImageMessage);
                    pDlg.OnMsg_UpdateStatusMessage(pDlg.m_strImageMessage);
                }
            }
        }

        private static void OnEvent_CompleteAcquisition(
            int deviceHandle,
            IntPtr pContext,
            DLL.IBSU_ImageType imageType
            )
        {
            if (pContext == null)
                return;

            lock (m_sync)
            {
                try
                {
                    SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                    if (imageType == DLL.IBSU_ImageType.ENUM_IBSU_ROLL_SINGLE_FINGER)
                    {
                        pDlg.OnMsg_Beep(__BEEP_OK__);
                    }
                    else
                    {
                        pDlg.OnMsg_Beep(__BEEP_SUCCESS__);
                        pDlg.m_strImageMessage = "Remove fingers from sensor";
                        pDlg.OnMsg_UpdateImageMessage(pDlg.m_strImageMessage);
                        pDlg.m_strImageMessage = "Acquisition completed, postprocessing..";
                        pDlg.OnMsg_UpdateStatusMessage(pDlg.m_strImageMessage);
                        DLL._IBSU_RedrawClientWindow(deviceHandle);
                    }
                }
                catch (Exception ex)
                {
                    string fx;
                    string caller;
                    Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                    Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

                }
            }

        }

        /****
		 ** This IBSU_CallbackResultImage callback was deprecated since 1.7.0
		 ** Please use IBSU_CallbackResultImageEx instead
		*/
        private static void OnEvent_ResultImageEx(int deviceHandle, IntPtr pContext, int imageStatus, DLL.IBSU_ImageData image, DLL.IBSU_ImageType imageType, int detectedFingerCount, int segmentImageArrayCount, IntPtr pSegmentImageArray, IntPtr pSegmentPositionArray)
        {
            try
            {
                lock (m_sync)
                {
                    SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                    if (deviceHandle != pDlg.m_nDevHandle)
                    {
                        return;
                    }

                    if (imageStatus >= DLL.IBSU_STATUS_OK)
                    {
                        if (imageType == DLL.IBSU_ImageType.ENUM_IBSU_ROLL_SINGLE_FINGER)
                        {
                            pDlg.OnMsg_Beep(__BEEP_SUCCESS__);
                        }
                    }

                    // Added 2012-11-30
                    if (pDlg.m_bNeedClearPlaten)
                    {
                        pDlg.m_bNeedClearPlaten = false;
                        pDlg.m_picScanner.Invalidate();
                    }

                    // Image acquisition successful
                    string imgTypeName = null;

                    switch (imageType)
                    {
                        case DLL.IBSU_ImageType.ENUM_IBSU_ROLL_SINGLE_FINGER:
                            imgTypeName = "-- Rolling single finger --";
                            break;
                        case DLL.IBSU_ImageType.ENUM_IBSU_FLAT_SINGLE_FINGER:
                            imgTypeName = "-- Flat single finger --";
                            break;
                        case DLL.IBSU_ImageType.ENUM_IBSU_FLAT_TWO_FINGERS:
                            imgTypeName = "-- Flat two fingers --";
                            break;
                        case DLL.IBSU_ImageType.ENUM_IBSU_FLAT_FOUR_FINGERS:
                            imgTypeName = "-- Flat 4 fingers --";
                            break;
                        default:
                            imgTypeName = "-- Unknown --";
                            break;
                    }

                    int i = 0;

                    if (imageStatus >= DLL.IBSU_STATUS_OK)
                    {
                        CaptureInfo info = pDlg.m_vecCaptureSeq[pDlg.m_nCurrentCaptureStep];
                        pDlg._SetLEDs(deviceHandle, info, __LED_COLOR_GREEN__, false);


                        info = pDlg.m_vecCaptureSeq[pDlg.m_nCurrentCaptureStep];
                        //pDlg._SaveBitmapImage(ref image, info.fingerName);
                        //pDlg._SaveWsqImage(ref image, info.fingerName);
                        //pDlg._SavePngImage(ref image, info.fingerName);
                        //pDlg._SaveJP2Image(ref image, info.fingerName);
                        if (segmentImageArrayCount > 0)
                        {
                            DLL.IBSU_ImageData[] imageArray = new DLL.IBSU_ImageData[segmentImageArrayCount];
                            string segmentName = null;
                            IntPtr ptrRunner = pSegmentImageArray;
                            for (i = 0; i < segmentImageArrayCount; i++)
                            {
                                segmentName = String.Format("{0}_Segement_{1}", info.fingerName, i);
                                imageArray[i] = (DLL.IBSU_ImageData)Marshal.PtrToStructure(ptrRunner, typeof(DLL.IBSU_ImageData));
                                pDlg._SaveBitmapImage(ref imageArray[i], segmentName);
                                //pDlg._SaveWsqImage(ref imageArray[i], segmentName);
                                //pDlg._SavePngImage(ref imageArray[i], segmentName);
                                //pDlg._SaveJP2Image(ref imageArray[i], segmentName);
                                ptrRunner = (IntPtr)((long)ptrRunner + Marshal.SizeOf(imageArray[0]));
                            }
                        }

                        StringBuilder propertyValue = new StringBuilder();
                        double scaleFactor = 0;
                        int leftMargin = 0;
                        int TopMargin = 0;

                        DLL._IBSU_GetClientWindowProperty(deviceHandle, DLL.IBSU_ClientWindowPropertyId.ENUM_IBSU_WINDOW_PROPERTY_SCALE_FACTOR, propertyValue);
                        scaleFactor = double.Parse(propertyValue.ToString());
                        DLL._IBSU_GetClientWindowProperty(deviceHandle, DLL.IBSU_ClientWindowPropertyId.ENUM_IBSU_WINDOW_PROPERTY_LEFT_MARGIN, propertyValue);
                        leftMargin = int.Parse(propertyValue.ToString());
                        DLL._IBSU_GetClientWindowProperty(deviceHandle, DLL.IBSU_ClientWindowPropertyId.ENUM_IBSU_WINDOW_PROPERTY_TOP_MARGIN, propertyValue);
                        TopMargin = int.Parse(propertyValue.ToString());

                        DLL.IBSU_SegmentPosition[] segmentArray = new DLL.IBSU_SegmentPosition[DLL.IBSU_MAX_SEGMENT_COUNT];
                        IntPtr ptrRunner_segmentArray = pSegmentPositionArray;

                        for (i = 0; i < segmentImageArrayCount; i++)
                        {
                            Color cr = Color.FromArgb(0, 128, 0);
                            int x1 = 0;
                            int x2 = 0;
                            int x3 = 0;
                            int x4 = 0;

                            int y1 = 0;
                            int y2 = 0;
                            int y3 = 0;
                            int y4 = 0;

                            segmentArray[i] = (DLL.IBSU_SegmentPosition)Marshal.PtrToStructure(ptrRunner_segmentArray, typeof(DLL.IBSU_SegmentPosition));

                            x1 = leftMargin + (int)(segmentArray[i].x1 * scaleFactor);
                            x2 = leftMargin + (int)(segmentArray[i].x2 * scaleFactor);
                            x3 = leftMargin + (int)(segmentArray[i].x3 * scaleFactor);
                            x4 = leftMargin + (int)(segmentArray[i].x4 * scaleFactor);

                            y1 = TopMargin + (int)(segmentArray[i].y1 * scaleFactor);
                            y2 = TopMargin + (int)(segmentArray[i].y2 * scaleFactor);
                            y3 = TopMargin + (int)(segmentArray[i].y3 * scaleFactor);
                            y4 = TopMargin + (int)(segmentArray[i].y4 * scaleFactor);

                            DLL._IBSU_ModifyOverlayQuadrangle(deviceHandle, pDlg.m_nOvSegmentHandle[i], x1, y1, x2, y2, x3, y3, x4, y4, 1, Convert.ToUInt32(ColorTranslator.ToWin32(cr)));
                            ptrRunner_segmentArray = (IntPtr)((long)ptrRunner_segmentArray + Marshal.SizeOf(segmentArray[0]));


                        }
                        //}


                        string strValue1 = null;
                        if (imageStatus == DLL.IBSU_STATUS_OK)
                        {
                            strValue1 = String.Format("{0} acquisition completed successfully", imgTypeName);
                            pDlg._SetImageMessage(strValue1);
                            pDlg.OnMsg_UpdateStatusMessage(strValue1);
                        }
                        else
                        {
                            strValue1 = String.Format("{0} acquisition Waring (Warning code = {1})", imgTypeName, imageStatus);
                            pDlg._SetImageMessage(strValue1);
                            pDlg.OnMsg_UpdateStatusMessage(strValue1);

                            pDlg.OnMsg_AskRecapture(imageStatus);
                            return;
                        }
                    }
                    else
                    {
                        pDlg.m_strImageMessage = String.Format("{0} acquisition fail (Error code = {1})", imgTypeName, imageStatus);
                        pDlg._SetImageMessage(pDlg.m_strImageMessage);
                        pDlg.OnMsg_UpdateStatusMessage(pDlg.m_strImageMessage);
                        pDlg.m_nCurrentCaptureStep = pDlg.m_vecCaptureSeq.Count;

                    }
                    pDlg.OnMsg_CaptureSeqNext();
                }
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }



        private static void OnEvent_ClearPlatenAtCapture(
            int deviceIndex,
            IntPtr pContext,
            DLL.IBSU_PlatenState platenState
            )
        {

            try
            {
                if (pContext == null)
                    return;

                lock (m_sync)
                {
                    SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                    if (platenState == DLL.IBSU_PlatenState.ENUM_IBSU_PLATEN_HAS_FINGERS)
                        pDlg.m_bNeedClearPlaten = true;
                    else
                        pDlg.m_bNeedClearPlaten = false;

                    if (pDlg.m_bNeedClearPlaten)
                    {
                        pDlg.m_strImageMessage = "Please remove your fingers on the platen first!";
                        pDlg._SetImageMessage(pDlg.m_strImageMessage);
                        pDlg.OnMsg_UpdateStatusMessage(pDlg.m_strImageMessage);
                    }
                    else
                    {
                        CaptureInfo info = pDlg.m_vecCaptureSeq[pDlg.m_nCurrentCaptureStep];

                        // Display message for image acuisition again
                        string strMessage;
                        strMessage = String.Format("{0}", info.PreCaptureMessage);

                        pDlg.OnMsg_UpdateStatusMessage(strMessage);
                        pDlg._SetImageMessage(strMessage);
                        pDlg.m_strImageMessage = strMessage;
                    }

                    pDlg.m_picScanner.Invalidate(false);
                }
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }

        private static void OnEvent_PressedKeyButtons(
            int deviceIndex,
            IntPtr pContext,
            int pressedKeyButtons
            )
        {
            try
            {
                if (pContext == null)
                    return;

                lock (m_sync)
                {
                    SDKMainForm pDlg = (SDKMainForm)SDKMainForm.FromHandle(pContext);
                    string message;
                    message = String.Format("OnEvent_PressedKeyButtons = {0}", pressedKeyButtons);
                    pDlg.OnMsg_UpdateStatusMessage(message);

                    pDlg.OnMsg_GetSelectDevice();
                    bool idle = !pDlg.m_bInitializing && (pDlg.m_nCurrentCaptureStep == -1);
                    bool active = !pDlg.m_bInitializing && (pDlg.m_nCurrentCaptureStep != -1);

                    if (pressedKeyButtons == __LEFT_KEY_BUTTON__)
                    {
                        if (pDlg.m_bSelectDev && idle)
                        {
                            DLL._IBSU_SetBeeper(pDlg.m_nDevHandle, DLL.IBSU_BeepPattern.ENUM_IBSU_BEEP_PATTERN_GENERIC, 2/*Sol*/, 4/*100ms = 4*25ms*/, 0, 0);
                            pDlg.OnMsg_CaptureStartClick();
                        }
                    }
                    else if (pressedKeyButtons == __RIGHT_KEY_BUTTON__)
                    {
                        if ((active))
                        {
                            DLL._IBSU_SetBeeper(pDlg.m_nDevHandle, DLL.IBSU_BeepPattern.ENUM_IBSU_BEEP_PATTERN_GENERIC, 2/*Sol*/, 4/*100ms = 4*25ms*/, 0, 0);
                            pDlg.OnMsg_CaptureStopClick();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }




        private delegate void OnMsg_GetSelectDeviceDelegate();
        private void OnMsg_GetSelectDevice()
        {
            // Check if we need to call beginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false
                this.Invoke(new OnMsg_GetSelectDeviceDelegate(OnMsg_GetSelectDevice));

                return;
            }


            return;
        }



        private delegate void OnMsg_CaptureStartClickDelegate();
        private void OnMsg_CaptureStartClick()
        {
            try
            {
                // Check if we need to call beginInvoke.
                if (this.InvokeRequired)
                {
                    // Pass the same function to BeginInvoke,
                    // but the call would come on the correct
                    // thread and InvokeRequired will be false
                    this.BeginInvoke(new OnMsg_CaptureStartClickDelegate(OnMsg_CaptureStartClick));

                    return;
                }

                if (m_bInitializing)
                    return;


                m_ImgSaveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image-Temp");
                if (!Directory.Exists(m_ImgSaveFolder))
                    Directory.CreateDirectory(m_ImgSaveFolder);

                if (DLL._IBSU_IsWritableDirectory(m_ImgSaveFolder, true) != DLL.IBSU_STATUS_OK)
                    System.Windows.Forms.MessageBox.Show("You don't have writing permission on this folder\r\nPlease select another folder (e.g. desktop folder)");

                if (m_nCurrentCaptureStep != -1)
                {
                    bool IsActive = false;
                    int nRc;
                    nRc = DLL._IBSU_IsCaptureActive(m_nDevHandle, ref IsActive);
                    if (nRc == DLL.IBSU_STATUS_OK && IsActive)
                    {
                        DLL._IBSU_TakeResultImageManually(m_nDevHandle);
                    }

                    return;
                }

                if (m_nDevHandle == -1)
                {
                    m_bInitializing = true;
                    m_initThread = new Thread(new ParameterizedThreadStart(_InitializeDeviceThreadCallback));
                    ThreadParam param = new ThreadParam();
                    param.devIndex = 0;
                    param.pParentHandle = this.Handle;
                    param.pFrameImageHandle = m_FrameImage.Handle;
                    m_initThread.Start(param);
                }
                else
                {
                    // device already initialized
                    OnMsg_CaptureSeqStart();
                }

                OnMsg_UpdateDisplayResources();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }



        private delegate void OnMsg_CaptureStopClickDelegate();
        private void OnMsg_CaptureStopClick()
        {
            try
            {
                // Check if we need to call beginInvoke.
                if (this.InvokeRequired)
                {
                    // Pass the same function to BeginInvoke,
                    // but the call would come on the correct
                    // thread and InvokeRequired will be false
                    this.BeginInvoke(new OnMsg_CaptureStopClickDelegate(OnMsg_CaptureStopClick));

                    return;
                }

                if (m_nDevHandle == -1)
                    return;

                DLL._IBSU_CancelCaptureImage(m_nDevHandle);
                CaptureInfo m_tmpInfo = new CaptureInfo();
                _SetLEDs(m_nDevHandle, m_tmpInfo, __LED_COLOR_NONE__, false);
                m_nCurrentCaptureStep = -1;
                m_bNeedClearPlaten = false;

                string message;
                message = String.Format("Capture Sequence aborted");
                _SetStatusBarMessage(message);
                m_strImageMessage = "";
                _SetImageMessage("");
                OnMsg_UpdateDisplayResources();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }



        private delegate void OnMsg_CaptureSeqStartDelegate();
        private void OnMsg_CaptureSeqStart()
        {
            try
            {
                // Check if we need to call beginInvoke.
                if (this.InvokeRequired)
                {
                    // Pass the same function to BeginInvoke,
                    // but the call would come on the correct
                    // thread and InvokeRequired will be false
                    this.BeginInvoke(new OnMsg_CaptureSeqStartDelegate(OnMsg_CaptureSeqStart));
                    return;
                }

                if (m_nDevHandle == -1)
                {
                    OnMsg_UpdateDisplayResources();
                    return;
                }

                string strCaptureSeq = CAPTURE_SEQ_10_SINGLE_FLAT_FINGERS;

                m_vecCaptureSeq.Clear();
                CaptureInfo info = new CaptureInfo();

                // Depending on the capture sequence, set the capture info
                // The following are examples for different capture sequences

                if (strCaptureSeq == CAPTURE_SEQ_FLAT_SINGLE_FINGER)
                {
                    info.PreCaptureMessage = "Please put a single finger on the sensor!";
                    info.PostCaptuerMessage = "Keep finger on the sensor!";
                    info.ImageType = DLL.IBSU_ImageType.ENUM_IBSU_FLAT_SINGLE_FINGER;
                    info.NumberOfFinger = 1;
                    info.fingerName = "SFF_Unknown";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_UNKNOWN;
                    m_vecCaptureSeq.Add(info);
                }

                if (strCaptureSeq == CAPTURE_SEQ_ROLL_SINGLE_FINGER)
                {
                    info.PreCaptureMessage = "Please put a single finger on the sensor!";
                    info.PostCaptuerMessage = "Roll finger!";
                    info.ImageType = DLL.IBSU_ImageType.ENUM_IBSU_ROLL_SINGLE_FINGER;
                    info.NumberOfFinger = 1;
                    info.fingerName = "SRF_Unknown";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_UNKNOWN;
                    m_vecCaptureSeq.Add(info);
                }

                if (strCaptureSeq == CAPTURE_SEQ_2_FLAT_FINGERS)
                {
                    info.PreCaptureMessage = "Please put two fingers on the sensor!";
                    info.PostCaptuerMessage = "Keep fingers on the sensor!";
                    info.ImageType = DLL.IBSU_ImageType.ENUM_IBSU_FLAT_TWO_FINGERS;
                    info.NumberOfFinger = 2;
                    info.fingerName = "TFF_Unknown";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_UNKNOWN;
                    m_vecCaptureSeq.Add(info);
                }

                if (strCaptureSeq == CAPTURE_SEQ_10_SINGLE_FLAT_FINGERS)
                {
                    info.PreCaptureMessage = "Please put right thumb on the sensor!";
                    info.fingerName = "SFF_Right_Thumb";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_THUMB;
                    info.PostCaptuerMessage = "Keep fingers on the sensor!";
                    info.ImageType = DLL.IBSU_ImageType.ENUM_IBSU_FLAT_SINGLE_FINGER;
                    info.NumberOfFinger = 1;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put right index on the sensor!";
                    info.fingerName = "SFF_Right_Index";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_INDEX_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put right middle on the sensor!";
                    info.fingerName = "SFF_Right_Middle";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_MIDDLE_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put right ring on the sensor!";
                    info.fingerName = "SFF_Right_Ring";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_RING_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put right little on the sensor!";
                    info.fingerName = "SFF_Right_Little";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_LITTLE_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left thumb on the sensor!";
                    info.fingerName = "SFF_Left_Thumb";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_THUMB;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left index on the sensor!";
                    info.fingerName = "SFF_Left_Index";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_INDEX_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left middle on the sensor!";
                    info.fingerName = "SFF_Left_Middle";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_MIDDLE_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left ring on the sensor!";
                    info.fingerName = "SFF_Left_Ring";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_RING_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left little on the sensor!";
                    info.fingerName = "SFF_Left_Little";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_LITTLE_FINGER;
                    m_vecCaptureSeq.Add(info);
                }

                if (strCaptureSeq == CAPTURE_SEQ_10_SINGLE_ROLLED_FINGERS)
                {
                    info.PreCaptureMessage = "Please put right thumb on the sensor!";
                    info.PostCaptuerMessage = "Roll finger!";
                    info.ImageType = DLL.IBSU_ImageType.ENUM_IBSU_ROLL_SINGLE_FINGER;
                    info.NumberOfFinger = 1;
                    info.fingerName = "SRF_Right_Thumb";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_THUMB;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put right index on the sensor!";
                    info.fingerName = "SRF_Right_Index";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_INDEX_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put right middle on the sensor!";
                    info.fingerName = "SRF_Right_Middle";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_MIDDLE_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put right ring on the sensor!";
                    info.fingerName = "SRF_Right_Ring";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_RING_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put right little on the sensor!";
                    info.fingerName = "SRF_Right_Little";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_RIGHT_LITTLE_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left thumb on the sensor!";
                    info.fingerName = "SRF_Left_Thumb";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_THUMB;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left index on the sensor!";
                    info.fingerName = "SRF_Left_Index";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_INDEX_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left middle on the sensor!";
                    info.fingerName = "SRF_Left_Middle";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_MIDDLE_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left ring on the sensor!";
                    info.fingerName = "SRF_Left_Ring";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_RING_FINGER;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left little on the sensor!";
                    info.fingerName = "SRF_Left_Little";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_LEFT_LITTLE_FINGER;
                    m_vecCaptureSeq.Add(info);
                }

                if (strCaptureSeq == CAPTURE_SEQ_4_FLAT_FINGERS)
                {
                    info.PreCaptureMessage = "Please put 4 fingers on the sensor!";
                    info.PostCaptuerMessage = "Keep fingers on the sensor!";
                    info.ImageType = DLL.IBSU_ImageType.ENUM_IBSU_FLAT_FOUR_FINGERS;
                    info.NumberOfFinger = 4;
                    info.fingerName = "4FF_Unknown";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_UNKNOWN;
                    m_vecCaptureSeq.Add(info);
                }

                if (strCaptureSeq == CAPTURE_SEQ_10_FLAT_WITH_4_FINGER_SCANNER)
                {
                    info.PreCaptureMessage = "Please put right 4-fingers on the sensor!";
                    info.fingerName = "Right_Fingers";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_PLAIN_RIGHT_FOUR_FINGERS;
                    info.PostCaptuerMessage = "Keep fingers on the sensor!";
                    info.ImageType = DLL.IBSU_ImageType.ENUM_IBSU_FLAT_FOUR_FINGERS;
                    info.NumberOfFinger = 4;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put left 4-fingers on the sensor!";
                    info.fingerName = "Left_Fingers";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_PLAIN_LEFT_FOUR_FINGERS;
                    m_vecCaptureSeq.Add(info);

                    info.PreCaptureMessage = "Please put 2-thumbs on the sensor!";
                    info.fingerName = "Thumbs";
                    info.fingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_PLAIN_THUMBS;
                    info.ImageType = DLL.IBSU_ImageType.ENUM_IBSU_FLAT_TWO_FINGERS;
                    info.NumberOfFinger = 2;
                    m_vecCaptureSeq.Add(info);
                }

                // Make subfolder name
                m_ImgSubFolder = DateTime.Now.ToString("yyyy-MM-dd HHmmss");

                OnMsg_CaptureSeqNext();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);
            }
            finally
            {
                // Ensure the device is released
                if (m_nDevHandle != -1)
                {
                    // Call the function to release the device
                    m_bInitializing = false;
                }
            }
        }


        private delegate void OnMsg_CaptureSeqNextDelegate();
        private void OnMsg_CaptureSeqNext()
        {

            try
            {
                // Check if we need to call beginInvoke.
                if (this.InvokeRequired)
                {
                    // Pass the same function to BeginInvoke,
                    // but the call would come on the correct
                    // thread and InvokeRequired will be false
                    this.BeginInvoke(new OnMsg_CaptureSeqNextDelegate(OnMsg_CaptureSeqNext));

                    return;
                }

                int nRc;

                if (m_nDevHandle == -1)
                    return;

                m_bBlank = false;
                m_FingerQuality[0] = DLL.IBSU_FingerQualityState.ENUM_IBSU_FINGER_NOT_PRESENT;
                m_FingerQuality[1] = DLL.IBSU_FingerQualityState.ENUM_IBSU_FINGER_NOT_PRESENT;
                m_FingerQuality[2] = DLL.IBSU_FingerQualityState.ENUM_IBSU_FINGER_NOT_PRESENT;
                m_FingerQuality[3] = DLL.IBSU_FingerQualityState.ENUM_IBSU_FINGER_NOT_PRESENT;


                m_nCurrentCaptureStep++;
                if (m_nCurrentCaptureStep >= m_vecCaptureSeq.Count)
                {
                    // All of capture sequence completely
                    CaptureInfo m_tmpInfo = new CaptureInfo();
                    _SetLEDs(m_nDevHandle, m_tmpInfo, __LED_COLOR_NONE__, false);
                    m_nCurrentCaptureStep = -1;
                    m_btnCaptureStart.Enabled = false;
                    m_btnVerify.Enabled = true;
                    Win32.SetFocus(m_btnVerify.Handle);

                    return;
                }

                DLL._IBSU_SetClientDisplayProperty(m_nDevHandle, DLL.IBSU_ClientWindowPropertyId.ENUM_IBSU_WINDOW_PROPERTY_DISP_INVALID_AREA, new StringBuilder("TRUE"));


                StringBuilder propertyValue = new StringBuilder("1");
                DLL._IBSU_SetProperty(m_nDevHandle, DLL.IBSU_PropertyId.ENUM_IBSU_PROPERTY_ROLL_MODE, propertyValue);
                StringBuilder strValue = new StringBuilder();
                strValue.AppendFormat("%d", 1);
                DLL._IBSU_SetProperty(m_nDevHandle, DLL.IBSU_PropertyId.ENUM_IBSU_PROPERTY_ROLL_LEVEL, strValue);


                for (int i = 0; i < DLL.IBSU_MAX_SEGMENT_COUNT; i++)
                {
                    DLL._IBSU_ModifyOverlayQuadrangle(m_nDevHandle, m_nOvSegmentHandle[i], 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                }

                // Make capture delay for display result image on multi capture mode (500 ms)
                if (m_nCurrentCaptureStep > 0)
                {
                    Thread.Sleep(500);
                    m_strImageMessage = "";
                }

                CaptureInfo info = m_vecCaptureSeq[m_nCurrentCaptureStep];

                DLL.IBSU_ImageResolution imgRes = DLL.IBSU_ImageResolution.ENUM_IBSU_IMAGE_RESOLUTION_500;
                bool bAvailable = false;
                nRc = DLL._IBSU_IsCaptureAvailable(m_nDevHandle, info.ImageType, imgRes, ref bAvailable);
                if (nRc != DLL.IBSU_STATUS_OK || !bAvailable)
                {
                    string message;
                    message = String.Format("The capture mode {0} is not available", info.ImageType);
                    _SetStatusBarMessage(message);
                    m_nCurrentCaptureStep = -1;
                    OnMsg_UpdateDisplayResources();
                    return;
                }

                // Start capture
                uint captureOptions = DLL.IBSU_OPTION_AUTO_CONTRAST | DLL.IBSU_OPTION_AUTO_CAPTURE | DLL.IBSU_OPTION_IGNORE_FINGER_COUNT;


                nRc = DLL._IBSU_BeginCaptureImage(m_nDevHandle, info.ImageType, imgRes, captureOptions);
                if (nRc >= DLL.IBSU_STATUS_OK)
                {
                    // Display message for image acuisition
                    string strMessage;
                    strMessage = String.Format("{0}", info.PreCaptureMessage);
                    _SetStatusBarMessage(strMessage);
                    _SetImageMessage(strMessage);
                    m_strImageMessage = strMessage;

                    _SetLEDs(m_nDevHandle, info, __LED_COLOR_RED__, true);
                }
                else
                {
                    string strMessage;
                    strMessage = String.Format("Failed to execute IBSU_BeginCaptureImage()");
                    _SetStatusBarMessage(strMessage);
                }

                OnMsg_UpdateDisplayResources();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }

        private delegate void OnMsg_DeviceCommunicationBreakDelegate();
        private void OnMsg_DeviceCommunicationBreak()
        {
            // Check if we need to call beginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false
                this.BeginInvoke(new OnMsg_DeviceCommunicationBreakDelegate(OnMsg_DeviceCommunicationBreak));

                return;
            }

        }

        private delegate void OnMsg_InitWarningDelegate();
        private void OnMsg_InitWarning()
        {
            // Check if we need to call beginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false
                this.BeginInvoke(new OnMsg_InitWarningDelegate(OnMsg_InitWarning));

                return;
            }

        }

        private delegate void OnMsg_DrawClientWindowDelegate();
        private void OnMsg_DrawClientWindow()
        {
            // Check if we need to call beginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false
                this.BeginInvoke(new OnMsg_DrawClientWindowDelegate(OnMsg_DrawClientWindow));

                return;
            }

        }

        private delegate void OnMsg_UpdateDeviceListDelegate();
        private void OnMsg_UpdateDeviceList()
        {
            try
            {
                // Check if we need to call beginInvoke.
                if (this.InvokeRequired)
                {
                    // Pass the same function to BeginInvoke,
                    // but the call would come on the correct
                    // thread and InvokeRequired will be false
                    this.BeginInvoke(new OnMsg_UpdateDeviceListDelegate(OnMsg_UpdateDeviceList));

                    return;
                }

                bool idle = !m_bInitializing && (m_nCurrentCaptureStep == -1);
                if (idle)
                {
                    m_btnCaptureStop.Enabled = false;
                    m_btnCaptureStart.Enabled = false;
                    m_btnVerify.Enabled = false;
                }

                // store currently selected device 

                // populate combo box
                int devices = 0;
                DLL._IBSU_GetDeviceCount(ref devices);

                if (idle)
                {
                    _UpdateCaptureSequences();
                }
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }

        private delegate void OnMsg_UpdateDisplayResourcesDelegate();
        private void OnMsg_UpdateDisplayResources()
        {
            try
            {
                // Check if we need to call beginInvoke.
                if (this.InvokeRequired)
                {
                    // Pass the same function to BeginInvoke,
                    // but the call would come on the correct
                    // thread and InvokeRequired will be false
                    this.BeginInvoke(new OnMsg_UpdateDisplayResourcesDelegate(OnMsg_UpdateDisplayResources));

                    return;
                }

                bool idle = !m_bInitializing && (m_nCurrentCaptureStep == -1);
                bool active = !m_bInitializing && (m_nCurrentCaptureStep != -1);



                m_btnCaptureStart.Enabled = true;
                m_btnCaptureStop.Enabled = active;
                m_btnVerify.Enabled = false;

                string strCaption = "";
                if (active)
                    strCaption = "Take Result Image";
                else if (!active && !m_bInitializing)
                    strCaption = "CAPTURE";

                m_btnCaptureStart.Text = strCaption;
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }

        private delegate void OnMsg_UpdateStatusMessageDelegate(string message);
        private void OnMsg_UpdateStatusMessage(string message)
        {
            // Check if we need to call beginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false
                this.BeginInvoke(new OnMsg_UpdateStatusMessageDelegate(OnMsg_UpdateStatusMessage),
                    new object[] { message });

                return;
            }

            _SetStatusBarMessage(message);
        }

        private delegate void OnMsg_UpdateImageMessageDelegate(string message);
        private void OnMsg_UpdateImageMessage(string message)
        {
            // Check if we need to call beginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false
                this.BeginInvoke(new OnMsg_UpdateImageMessageDelegate(OnMsg_UpdateImageMessage),
                    new object[] { message });

                return;
            }

            _SetImageMessage(message);
        }

        private delegate void OnMsg_BeepDelegate(int beepType);
        private void OnMsg_Beep(int beepType)
        {
            try
            {
                // Check if we need to call beginInvoke.
                if (this.InvokeRequired)
                {
                    // Pass the same function to BeginInvoke,
                    // but the call would come on the correct
                    // thread and InvokeRequired will be false
                    this.BeginInvoke(new OnMsg_BeepDelegate(OnMsg_Beep),
                        new object[] { beepType });

                    return;
                }

                if (beepType == __BEEP_FAIL__)
                    _BeepFail();
                else if (beepType == __BEEP_SUCCESS__)
                    _BeepSuccess();
                else if (beepType == __BEEP_OK__)
                    _BeepOk();
                else if (beepType == __BEEP_DEVICE_COMMUNICATION_BREAK__)
                    _BeepDeviceCommunicationBreak();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }

        private delegate void OnMsg_UpdateNFIQScoreDelegate(string score);
        private void OnMsg_UpdateNFIQScore(string score)
        {
            // Check if we need to call beginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false
                this.BeginInvoke(new OnMsg_UpdateNFIQScoreDelegate(OnMsg_UpdateNFIQScore),
                    new object[] { score });

                return;
            }

        }

        private delegate void OnMsg_AskRecaptureDelegate(int imageStatus);
        private void OnMsg_AskRecapture(int imageStatus)
        {
            try
            {
                // Check if we need to call beginInvoke.
                if (this.InvokeRequired)
                {
                    // Pass the same function to BeginInvoke,
                    // but the call would come on the correct
                    // thread and InvokeRequired will be false
                    this.BeginInvoke(new OnMsg_AskRecaptureDelegate(OnMsg_AskRecapture),
                        new object[] { imageStatus });

                    return;
                }

                String strValue;

                strValue = String.Format("[Warning = {0}] Do you want a recapture?", imageStatus);

                if (MessageBox.Show(strValue, "i-Immigration System", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    m_nCurrentCaptureStep = m_nCurrentCaptureStep - 1;
                }

                OnMsg_CaptureSeqNext();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        // Form event
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SDKMainForm_Load(object sender, EventArgs e)
        {
            try
            {

                m_nDevHandle = -1;
                m_nCurrentCaptureStep = -1;
                m_bInitializing = false;
                m_strImageMessage = "";
                m_bNeedClearPlaten = false;
                m_bSaveWarningOfClearPlaten = false;
                cbCountry.DataSource = codes.Country.AllCountries();
                cbCountry.SelectedIndex = 0;
                cbDocType.DataSource = codes.DocumentType.AllDocuments();
                cbDocType.SelectedIndex = 0;
                tbDocumentNo.Enabled = cbCountry.Enabled = true;
                m_btnCaptureStop.Enabled = false;
                m_btnCaptureStart.Enabled = false;
                m_btnVerify.Enabled = false;
                m_verInfo = new DLL.IBSU_SdkVersion();
                DLL._IBSU_GetSDKVersion(ref m_verInfo);
                string titleName = "i-Immigration System";
                this.Text = titleName;

                m_callbackDeviceCommunicationBreak = new DLL.IBSU_Callback(OnEvent_DeviceCommunicationBreak);
                m_callbackPreviewImage = new DLL.IBSU_CallbackPreviewImage(OnEvent_PreviewImage);
                m_callbackFingerCount = new DLL.IBSU_CallbackFingerCount(OnEvent_FingerCount);
                m_callbackFingerQuality = new DLL.IBSU_CallbackFingerQuality(OnEvent_FingerQuality);
                m_callbackDeviceCount = new DLL.IBSU_CallbackDeviceCount(OnEvent_DeviceCount);
                m_callbackInitProgress = new DLL.IBSU_CallbackInitProgress(OnEvent_InitProgress);
                m_callbackTakingAcquisition = new DLL.IBSU_CallbackTakingAcquisition(OnEvent_TakingAcquisition);
                m_callbackCompleteAcquisition = new DLL.IBSU_CallbackCompleteAcquisition(OnEvent_CompleteAcquisition);
                m_callbackClearPlaten = new DLL.IBSU_CallbackClearPlatenAtCapture(OnEvent_ClearPlatenAtCapture);
                m_callbackResultImageEx = new DLL.IBSU_CallbackResultImageEx(OnEvent_ResultImageEx);
                m_callbackPressedKeyButtons = new DLL.IBSU_CallbackKeyButtons(OnEvent_PressedKeyButtons);

                DLL._IBSU_RegisterCallbacks(0, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_DEVICE_COUNT, m_callbackDeviceCount, this.Handle);
                DLL._IBSU_RegisterCallbacks(0, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_INIT_PROGRESS, m_callbackInitProgress, this.Handle);


                OnMsg_UpdateDeviceList();

                try
                {
                    connectMRZ();
                    Read();
                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }


        bool validateSearch()
        {
            if (string.IsNullOrEmpty(tbFirstName.Text) && string.IsNullOrEmpty(tbLastName.Text))
            {
                tbFirstName.Focus();
                errorProvider1.SetError(tbFirstName, "Please Enter atleast one name.");
                return false;
            }
            else
                errorProvider1.SetError(tbFirstName, "");
            return true;
        }




        bool validate()
        {
            if (cbDocType.SelectedIndex < 1)
            {
                cbDocType.Focus();
                errorProvider1.SetError(cbDocType, "Please Select Document Type");
                return false;
            }
            else
                errorProvider1.SetError(cbDocType, "");

            if (cbDocType.SelectedIndex != 4)
            {

                if (string.IsNullOrEmpty(tbDocumentNo.Text))
                {
                    tbDocumentNo.Focus();
                    errorProvider1.SetError(tbDocumentNo, "Please Enter Document No.");
                    return false;
                }
                else
                    errorProvider1.SetError(tbDocumentNo, "");

                if (cbCountry.SelectedIndex < 1)
                {
                    cbCountry.Focus();
                    errorProvider1.SetError(cbCountry, "Please Select Country");
                    return false;
                }
                else
                    errorProvider1.SetError(cbCountry, "");
            }
            else
            {
                if (string.IsNullOrEmpty(tbDocumentNo.Text))
                {
                    tbDocumentNo.Focus();
                    errorProvider1.SetError(tbDocumentNo, "Please Enter Document No.");
                    return false;
                }
                else
                    errorProvider1.SetError(tbDocumentNo, "");
            }

            return true;
        }

        private void Timer_StatusFingerQuality_Tick(object sender, EventArgs e)
        {
            try
            {
                Boolean idle = !m_bInitializing && (m_nCurrentCaptureStep == -1);

                if (!idle)
                {
                    if (m_bNeedClearPlaten && m_bBlank)
                    {
                        if (m_bSaveWarningOfClearPlaten == false)
                        {
                            _ModifyOverlayForWarningOfClearPlaten(true);
                            m_bSaveWarningOfClearPlaten = true;
                        }
                    }
                    else
                    {
                        if (m_bSaveWarningOfClearPlaten == true)
                        {
                            _ModifyOverlayForWarningOfClearPlaten(false);
                            m_bSaveWarningOfClearPlaten = false;
                        }
                    }
                }

                m_picScanner.Invalidate(false);
                if (m_bNeedClearPlaten)
                    m_bBlank = !m_bBlank;
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }

        private void SDKMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _ReleaseDevice();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }


        private void m_cboCaptureSeq_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                OnMsg_UpdateDisplayResources();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }

        private void m_picIBLogo_Paint(object sender, PaintEventArgs e)
        {
            string message = "i-Immigration System";
            Font font = new Font("Times New Roman", 20);

            // Specify the x and y coordinates for positioning the text
            float x = 30;  // Specify the x-coordinate
            float y = 0;  // Specify the y-coordinate

            e.Graphics.DrawString(message, font, Brushes.White, x, y);
        }


        private void m_picScanner_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                SolidBrush hbr_finger;
                //            SolidBrush hbr_touch;

                for (int i = 0; i < 4; i++)
                {
                    if (m_bNeedClearPlaten)
                    {
                        if (m_bBlank)
                            hbr_finger = new SolidBrush(Color.Red);
                        else
                            hbr_finger = new SolidBrush(Color.FromArgb(78, 78, 78));
                    }
                    else
                    {
                        switch (m_FingerQuality[i])
                        {
                            case DLL.IBSU_FingerQualityState.ENUM_IBSU_QUALITY_GOOD:
                                hbr_finger = new SolidBrush(Color.Green);
                                break;
                            case DLL.IBSU_FingerQualityState.ENUM_IBSU_QUALITY_FAIR:
                                hbr_finger = new SolidBrush(Color.Orange);
                                break;
                            case DLL.IBSU_FingerQualityState.ENUM_IBSU_QUALITY_POOR:
                                hbr_finger = new SolidBrush(Color.Red);
                                break;
                            case DLL.IBSU_FingerQualityState.ENUM_IBSU_FINGER_NOT_PRESENT:
                                hbr_finger = new SolidBrush(Color.FromArgb(78, 78, 78));
                                break;
                            case DLL.IBSU_FingerQualityState.ENUM_IBSU_QUALITY_INVALID_AREA_TOP:
                                hbr_finger = new SolidBrush(Color.Red);
                                break;
                            case DLL.IBSU_FingerQualityState.ENUM_IBSU_QUALITY_INVALID_AREA_BOTTOM:
                                hbr_finger = new SolidBrush(Color.Red);
                                break;
                            case DLL.IBSU_FingerQualityState.ENUM_IBSU_QUALITY_INVALID_AREA_LEFT:
                                hbr_finger = new SolidBrush(Color.Red);
                                break;
                            case DLL.IBSU_FingerQualityState.ENUM_IBSU_QUALITY_INVALID_AREA_RIGHT:
                                hbr_finger = new SolidBrush(Color.Red);
                                break;
                            default:
                                return;
                        }
                    }

                    _DrawRoundRect(e.Graphics, hbr_finger, 15 + i * 22, 30, 19, 70, 9);
                }
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }



        private void m_btnVerify_Click(object sender, EventArgs e)
        {
            try
            {
                if (NetworkStatus.CheckNet() == true)
                {
                    if (NetworkStatus.IsConnectedToInternet())
                    {
                        var _url = eVerification.Properties.Settings.Default.finger_verif_url;
                        if (string.IsNullOrEmpty(_url))
                            _url = "http://gto-srv-train01:8586/api/eVisa/";

                        codes.APIInterface api = new codes.APIInterface { url = _url };

                        if (cbFingerPrintOnly.Checked)
                        {
                            strFolder = String.Format("{0}\\{1}", m_ImgSaveFolder, m_ImgSubFolder);
                            if (Directory.Exists(strFolder) && !strFolder.Equals(@"\"))
                            {
                                Logger.LogError("fingerprint-> Captured Ok", "", "");
                                string[] files = Directory.GetFiles(strFolder, "*Thumbs*.png");
                                codes.VerifyRequest req = new codes.VerifyRequest();
                                req.fingers = (from f in files.AsQueryable()
                                               select new codes.Finger
                                               {
                                                   fingerType = FingerType.getFinger(new FileInfo(f).Name.Replace(".png", "")),
                                                   fingerPrint = File.ReadAllBytes(new FileInfo(f).FullName)
                                               }).ToArray();

                                string message = "Contacting eVisa platform for Verification ...";
                                _SetStatusBarMessage(message);
                                m_btnVerify.Enabled = false;
                                Logger.LogError("Verify-> Request Sending...", "", "");
                                verifyResponse = api.verify_fingerprint_only(req);
                                if (verifyResponse != null)
                                {
                                    Logger.LogError("Verify-> Response Recieved", "", "");
                                    if (verifyResponse.data != null)
                                    {
                                        message = "Successfully Retrived Verification Data";
                                        _SetStatusBarMessage(message);
                                        Logger.LogError("Verify-> Response Ok", "", "");
                                        setScreen();
                                    }
                                    else
                                    {
                                        message = verifyResponse.message;
                                        _SetStatusBarMessage(message);
                                        Form popup = new PopupForm();
                                        DialogResult dialogresult = popup.ShowDialog();
                                    }
                                }
                                else
                                {
                                    message = "No Response from Server";
                                    _SetStatusBarMessage(message);
                                }
                            }
                        }
                        else
                        {
                            if (!validate())
                            {
                                connectMRZ();
                                Read();
                            }
                            else
                            {
                                Logger.LogError("validate-> Successful", "", "");

                                codes.VerifyRequest req = new codes.VerifyRequest
                                {
                                    documentType = ((DataRowView)cbDocType.SelectedItem).Row.ItemArray[0].ToString(),
                                    documentNumber = tbDocumentNo.Text,
                                    issuingcountry = ((DataRowView)cbCountry.SelectedItem).Row.ItemArray[0].ToString(),
                                };

                                if (!cbDocumentOnly.Checked && !cbFingerPrinterLater.Checked && !cbFingerPrintOnly.Checked)
                                {
                                    strFolder = String.Format("{0}\\{1}", m_ImgSaveFolder, m_ImgSubFolder);
                                    if (Directory.Exists(strFolder) && !strFolder.Equals(@"\"))
                                    {
                                        Logger.LogError("fingerprint-> Captured Ok", "", "");
                                        string[] files = Directory.GetFiles(strFolder, "*Thumbs*.png");
                                        req.fingers = (from f in files.AsQueryable()
                                                       select new codes.Finger
                                                       {
                                                           fingerType = FingerType.getFinger(new FileInfo(f).Name.Replace(".png", "")),
                                                           fingerPrint = File.ReadAllBytes(new FileInfo(f).FullName)
                                                       }).ToArray();

                                        string message = "Contacting eVisa platform for Verification ...";
                                        _SetStatusBarMessage(message);
                                        m_btnVerify.Enabled = false;
                                        Logger.LogError("Verify-> Request Sending...", "", "");
                                        verifyResponse = api.verify_with_fingerprint(req);
                                        if (verifyResponse != null)
                                        {
                                            Logger.LogError("Verify-> Response Recieved", "", "");
                                            if (verifyResponse.data != null)
                                            {
                                                message = "Successfully Retrived Verification Data";
                                                _SetStatusBarMessage(message);
                                                Logger.LogError("Verify-> Response Ok", "", "");
                                                setScreen();
                                            }
                                            else
                                            {
                                                message = verifyResponse.message;
                                                _SetStatusBarMessage(message);
                                                Form popup = new PopupForm();
                                                DialogResult dialogresult = popup.ShowDialog();
                                            }
                                        }
                                        else
                                        {
                                            message = "No Response from Server";
                                            _SetStatusBarMessage(message);
                                        }
                                    }
                                }
                                else if (cbFingerPrinterLater.Checked || cbDocumentOnly.Checked)
                                {
                                    string message = "Contactings eVisa platform for Verification ...";
                                    _SetStatusBarMessage(message);
                                    m_btnVerify.Enabled = false;
                                    Logger.LogError("Verify-> Request Sending...", "", "");
                                    verifyResponse = api.verify_document(req);
                                    if (verifyResponse != null)
                                    {
                                        Logger.LogError("Verify-> Response Recieved", "", "");
                                        if (verifyResponse.data != null)
                                        {
                                            message = "Successfully Retrived Verification Data";
                                            _SetStatusBarMessage(message);
                                            Logger.LogError("Verify-> Response Ok", "", "");
                                            setScreen();
                                        }
                                        else
                                        {
                                            message = verifyResponse.message;
                                            _SetStatusBarMessage(message);
                                            Form popup = new PopupForm();
                                            DialogResult dialogresult = popup.ShowDialog();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        SDKMainForm.verifyResponse = new VerifyResponse
                        {
                            code = 100,
                            data = new Data
                            {
                                message = "VPN Connection failed, Ensure you have logged into your VPN account"
                            }
                        };

                        Form popup = new PopupForm();
                        DialogResult dialogresult = popup.ShowDialog();
                    }
                }
                else
                {
                    SDKMainForm.verifyResponse = new VerifyResponse
                    {
                        code = 100,
                        data = new Data
                        {
                            message = "Internet Connection failed, Ensure you connected to Internet"
                        }
                    };
                    Form popup = new PopupForm();
                    DialogResult dialogresult = popup.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clearScren();
                _SetStatusBarMessage("eVisa connection failed. Contact system Administrator");
                Logger.LogError($"Error {DateTime.Now:dd-MMM-yyyy HH:mm:ss} >> {ex.ToString()} \n");
            }
        }


        void setScreen()
        {
            tbFullname.Text = $"{SDKMainForm.verifyResponse.data.surName} {SDKMainForm.verifyResponse.data.givenNames}";
            tbNationality.Text = SDKMainForm.verifyResponse.data.nationality;
            tbPassportNo.Text = SDKMainForm.verifyResponse.data.passportNumber;
            tbDateOfBirth.Text = SDKMainForm.verifyResponse.data.dateOfBirth;
            tbAppNo.Text = SDKMainForm.verifyResponse.data.applicationNumber;
            tbAppType.Text = SDKMainForm.verifyResponse.data.applicationtype;
            tbAppStatus.Text = SDKMainForm.verifyResponse.data.applicationStatus;
            tbAppExpDate.Text = SDKMainForm.verifyResponse.data.visaExpirationDate;
            tbBirthPlace.Text = SDKMainForm.verifyResponse.data.placeOfBirth;
            tbEmail.Text = SDKMainForm.verifyResponse.data.email;
            tbIssuePlace.Text = SDKMainForm.verifyResponse.data.documentPlaceOfIssue;
            textBox1.Text = SDKMainForm.verifyResponse.data.dateOfBirth;
            textBox2.Text = SDKMainForm.verifyResponse.data.nationality;


            try
            {
                if (SDKMainForm.verifyResponse.data.photo != null)
                {
                    byte[] bytes = Convert.FromBase64String(SDKMainForm.verifyResponse.data.photo);
                    Image image;
                    MemoryStream ms = new MemoryStream(bytes);
                    image = Image.FromStream(ms, true, false);
                    pbPhoto.Image = image;
                }
            }
            catch (Exception ex)
            {

            }

            if (cbFingerPrinterLater.Checked)
            {
                cbFingerPrinterLater.Checked = false;
                OnMsg_CaptureStartClick();
            }
            else
            {
                Form popup = new PopupForm();
                DialogResult dialogresult = popup.ShowDialog();
                if (dialogresult == DialogResult.OK)
                {
                    m_btnCaptureStop.Enabled = true;
                }
            }
        }

        void clearScren()
        {
            try
            {
                OnMsg_UpdateDisplayResources();
                Win32.SetFocus(m_btnCaptureStart.Handle);

                CaptureInfo m_tmpInfo = new CaptureInfo();
                _SetLEDs(m_nDevHandle, m_tmpInfo, __LED_COLOR_NONE__, false);
                m_nCurrentCaptureStep = -1;
                m_btnCaptureStart.Enabled = true;
                m_btnVerify.Enabled = false;
                cbDocumentOnly.Checked = cbFingerPrinterLater.Checked = cbFingerPrintOnly.Checked = false;
                m_ImgSubFolder = "";
                tbDocumentNo.Text = tbFullname.Text = tbNationality.Text = tbPassportNo.Text = tbDateOfBirth.Text = tbAppNo.Text = tbAppType.Text = tbAppStatus.Text = tbAppExpDate.Text = tbBirthPlace.Text = tbEmail.Text = tbIssuePlace.Text = tbFirstName.Text = tbLastName.Text = "";
                pbPhoto.Image = eVerification.Properties.Resources.image;
                cbCountry.SelectedIndex = cbDocType.SelectedIndex = 0;
                m_strImageMessage = "";
                m_btnCaptureStart.Text = "START";

                try
                {
                    if (Directory.Exists(strFolder))
                        System.IO.Directory.Delete(strFolder);
                    m_ImgSubFolder = strFolder = "";
                }
                catch (Exception ex)
                {
                    //string fx;
                    //string caller;
                    //Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                    //Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

                }
            }
            catch (Exception ex)
            {
                //string fx;
                //string caller;
                //Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                //Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }

        private void cbDocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbDocumentNo.Text = "";
            cbCountry.SelectedIndex = 0;

            if (cbDocType.SelectedIndex == 4)
            {
                tbDocumentNo.Enabled = true;
                cbCountry.Enabled = false;
            }
            else
            {
                tbDocumentNo.Enabled = cbCountry.Enabled = true;
            }
        }

        private void cbDocumentOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDocumentOnly.Checked)
            {
                cbFingerPrintOnly.Checked = cbFingerPrinterLater.Checked = false;
                m_btnCaptureStart.Enabled = false;
                m_btnCaptureStop.Enabled = false;
                m_btnVerify.Enabled = true;
                m_btnCaptureStart.Text = "START";
            }
            else
            {
                 m_btnCaptureStart.Enabled = false;
                 m_btnCaptureStart.Text = "START";
                 m_btnVerify.Enabled = m_btnCaptureStop.Enabled = false;
            }

        }

        private void cbFingerPrinterLater_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFingerPrinterLater.Checked)
            {
                cbDocumentOnly.Checked = cbFingerPrintOnly.Checked = false;
                m_btnCaptureStart.Enabled = true;
                m_btnCaptureStop.Enabled = false;
                m_btnVerify.Enabled = true;
                m_btnCaptureStart.Text = "START";
            }
            else
            {
                m_btnCaptureStart.Enabled = true;
                m_btnCaptureStart.Text = "START";
                m_btnVerify.Enabled = m_btnCaptureStop.Enabled = false;
            }
        }

        private void cbFingerPrintOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFingerPrintOnly.Checked)
            {
                cbDocumentOnly.Checked = cbFingerPrinterLater.Checked = false;
                m_btnCaptureStart.Text = "SEARCH";
            }
            else { }
                m_btnCaptureStart.Text = "START";
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {


        }

        public void loadSelection(int index)
        {
            var selected = searchResponse.data[index];
            if (selected != null)
            {
                cbDocumentOnly.Checked = true;
                cbDocType.SelectedIndex = cbDocType.FindStringExact(selected.document_type);
                cbCountry.SelectedIndex = cbCountry.FindStringExact(selected.issuing_country.Trim());
                tbDocumentNo.Text = selected.PASSPORT_NUMBER;
                m_btnVerify_Click(m_btnVerify, null);
            }

        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            string firstName = SDKMainForm.verifyResponse.data.surName;
            string givenName = SDKMainForm.verifyResponse.data.givenNames;
            string nationality = SDKMainForm.verifyResponse.data.nationality;
            string passportNo = SDKMainForm.verifyResponse.data.passportNumber;
            string dateOfBirth = SDKMainForm.verifyResponse.data.dateOfBirth;
            string fullNames = $"{SDKMainForm.verifyResponse.data.surName} {SDKMainForm.verifyResponse.data.givenNames}";
            string applicationNumber = SDKMainForm.verifyResponse.data.applicationNumber;
            Image photo = null;
            
            try
            {
                if (SDKMainForm.verifyResponse.data.photo != null)
                {
                    byte[] bytes = Convert.FromBase64String(SDKMainForm.verifyResponse.data.photo);
                    MemoryStream ms = new MemoryStream(bytes);
                    photo = Image.FromStream(ms, true, false);
                }
            }
            catch (Exception ex)
            {
                // Handle exception if necessary
            }
            // Create a dictionary to store the PictureBoxes and their images
            Dictionary<string, Image> fingerImages = new Dictionary<string, Image>()
            {
                { "SFF_Right_Thumb", pbRightThumb.Image },
                { "SFF_Right_Index", pbRightIndex.Image },
                { "SFF_Right_Middle", pbRightMiddle.Image },
                { "SFF_Right_Ring", pbRightRing.Image },
                { "SFF_Right_Little", pbRightLittle.Image },
                { "SFF_Left_Thumb", pbLeftThumb.Image },
                { "SFF_Left_Index", pbLeftIndex.Image },
                { "SFF_Left_Middle", pbLeftMiddle.Image },
                { "SFF_Left_Ring", pbLeftRing.Image },
                { "SFF_Left_Little", pbLeftLittle.Image }
            };  
            CustomerDetails dets = new CustomerDetails(firstName, givenName, nationality, fullNames, passportNo, dateOfBirth, photo, applicationNumber, fingerImages);
            dets.Show();
            this.Hide();
        }

        private void m_btnCaptureStop_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if we need to call beginInvoke.
                if (this.InvokeRequired)
                {
                    // Pass the same function to BeginInvoke,
                    // but the call would come on the correct
                    // thread and InvokeRequired will be false
                    this.BeginInvoke(new OnMsg_CaptureStopClickDelegate(OnMsg_CaptureStopClick));

                    return;
                }

                if (m_nDevHandle == -1)
                    return;

                DLL._IBSU_CancelCaptureImage(m_nDevHandle);
                CaptureInfo m_tmpInfo = new CaptureInfo();
                _SetLEDs(m_nDevHandle, m_tmpInfo, __LED_COLOR_NONE__, false);
                m_nCurrentCaptureStep = -1;
                m_bNeedClearPlaten = false;

                string message;
                message = String.Format("Capture Sequence aborted");
                _SetStatusBarMessage(message);
                m_strImageMessage = "";
                _SetImageMessage("");
                OnMsg_UpdateDisplayResources();
            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }

        private void m_btnCaptureStart_Click(object sender, EventArgs e)
        {
            OnMsg_CaptureStartClick();
        }

        private void UpdateButtonColor()
        {
            if (m_btnCaptureStart.Enabled)
            {
                m_btnCaptureStart.BackColor = Color.Green;
            }
            else
            {
                m_btnCaptureStart.BackColor = Color.Gray;
            }
        }
    }
}
