using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IBscanUltimate;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using eVerification.codes;
using eVerification.codes.mrz;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Drawing.Text;
using ElySCardDotNet;
using System.Text.RegularExpressions;
using DarrenLee.Media;


namespace eVerification
{
    public partial class CustomerDetails : Form
    {
     
        private TabControl tabControl;
        Camera myCamera = new Camera();
        private string lastCapturedImageFilename = string.Empty;
        private int currentOrientation = 0; // 0 for normal, 1 for 90 degrees, 2 for 180 degrees, 3 for 270 degrees


        private TravelEntryRequest travelEntryRequest;
        private TravelEntryRequest tempTravelEntryRequest;


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

        public CustomerDetails(string firstName, string givenName, string nationality, string fullNames, string passportNo, string dateOfBirth, Image photo, string applicationNumber, Dictionary<string, Image> fingerImages)
        {
            InitializeComponent();
            //ElyObject = new ElyMRTDDotNet.ElyMRTDDotNet();
            tabControl = new TabControl();

            //Initializing Camera
            GetInfo();
            myCamera.OnFrameArrived += myCamera_OnFrameArrived;

            pbRightThumb.Image = fingerImages["SFF_Right_Thumb"];
            pbRightIndex.Image = fingerImages["SFF_Right_Index"];
            pbRightMiddle.Image = fingerImages["SFF_Right_Middle"];
            pbRightRing.Image = fingerImages["SFF_Right_Ring"];
            pbRightLittle.Image = fingerImages["SFF_Right_Little"];
            pbLeftThumb.Image = fingerImages["SFF_Left_Thumb"];
            pbLeftIndex.Image = fingerImages["SFF_Left_Index"];
            pbLeftMiddle.Image = fingerImages["SFF_Left_Middle"];
            pbLeftRing.Image = fingerImages["SFF_Left_Ring"];
            pbLeftLittle.Image = fingerImages["SFF_Left_Little"];

            comboBox8.SelectedIndex = 0;
            cbFlight.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            tbFirstname.Text = firstName;
            tbSecondname.Text = givenName;
            tbNationality.Text = nationality;
            tbPassportNo.Text = passportNo;
            tbDateOfBirth.Text = dateOfBirth;
            namesGrid.Text = fullNames;
            tbNationality.Text = nationality;
            tbApplicationNumber.Text = applicationNumber;
            if (DateTime.TryParse(dateOfBirth, out DateTime parsedDate))
            {
                dateTimePicker2.Value = parsedDate;
            }
            else
            {
                // Handle the case when the date string is not valid
                MessageBox.Show("Invalid date format");
            }
           
            if (photo != null)
            {
                pbPhoto.Image = photo;
            }

            // Initialize the ComboBox with countries
            try
            {
                DataTable countries = codes.Country.AllCountries();
                if (countries.Rows.Count > 0)
                {
                    cbCountry.DataSource = countries;
                    cbCountry.DisplayMember = "Name";
                    cbCountry.ValueMember = "Iso";
                    cbCountry.SelectedIndex = 0;
                    //cbCountry.Enabled = true;
                    tbIssuingCountry.DataSource = countries;
                    tbIssuingCountry.DisplayMember = "Name";
                    tbIssuingCountry.ValueMember = "Iso";
                    tbIssuingCountry.SelectedIndex = 0;
                    //tbIssuingCountry.Enabled = true;
                }
                else
                {
                    MessageBox.Show("No countries found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing countries: {ex.Message}");
            }

            try
            {
                DataTable countries = codes.Country.AllCountries();
                if (countries.Rows.Count > 0)
                {
                    tbIssuingCountry.DataSource = countries;
                    tbIssuingCountry.DisplayMember = "Name";
                    tbIssuingCountry.ValueMember = "Iso";
                    tbIssuingCountry.SelectedIndex = 0;
                    tbIssuingCountry.Enabled = true;
                }
                else
                {
                    MessageBox.Show("No countries found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing countries: {ex.Message}");
            }

            try
            {
                DataTable events = TravelEventType.AllEvents();
                if (events.Rows.Count > 0)
                {
                    cbEvent.DataSource = events;
                    cbEvent.DisplayMember = "name";
                    cbEvent.ValueMember = "value";
                    cbEvent.SelectedIndex = 0;
                    cbEvent.Enabled = true;
                }
                else
                {
                    MessageBox.Show("No travel events found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing travel events: {ex.Message}");
            }

            try
            {
                DataTable flightEvents = FlightEventType.AllFlightEvents();
                if (flightEvents.Rows.Count > 0)
                {
                    cbFlight.DataSource = flightEvents;
                    cbFlight.DisplayMember = "name";
                    cbFlight.ValueMember = "value";
                    cbFlight.SelectedIndex = 0;
                    cbFlight.Enabled = true;
                }
                else
                {
                    MessageBox.Show("No flight events found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing flight events: {ex.Message}");
            }



            try
            {
                DataTable docs = codes.DocumentType.AllDocuments();
                if(docs.Rows.Count > 0)
                {
                    cbDocType.DataSource = docs;
                    cbDocType.DisplayMember = "name";
                    cbDocType.ValueMember = "value";
                    cbDocType.SelectedIndex = 0;
                    cbCountry.Enabled = true;
                }
            }catch(Exception ex)
            {
                MessageBox.Show($"Error initializing documents: {ex.Message}");
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

        private void myCamera_OnFrameArrived(object sender, FrameArrivedEventArgs e)
        {
            try
            {
                // Get the frame from the camera
                Image frame = e.GetFrame();

                // Clone the frame to avoid altering the original frame
                Image adjustedFrame = (Image)frame.Clone();

                // Adjust the image orientation based on the current orientation
                switch (currentOrientation)
                {
                    case 0:
                        adjustedFrame.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                        break;
                    case 1:
                        adjustedFrame.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 2:
                        adjustedFrame.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 3:
                        adjustedFrame.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }

                // Display the image or process it further as needed
                pbCamera.Image = adjustedFrame;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine(ex.Message);
            }
        }

       

        //getting camera info
        private void GetInfo()
        {
            var cameraDevices = myCamera.GetCameraSources();
            var cameraResolution = myCamera.GetSupportedResolutions();

            foreach(var device in cameraDevices)
            {
                cbDevice.Items.Add(device);
            }

            foreach (var res in cameraResolution)
            {
                cbResolution.Items.Add(res);
            }
            cbDevice.SelectedIndex = 0;
            cbResolution.SelectedIndex = 0;
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox6.SelectedIndex != comboBox8.SelectedIndex)
            {
                comboBox8.SelectedIndex = comboBox6.SelectedIndex;
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox8.SelectedIndex != comboBox6.SelectedIndex)
            {
                comboBox6.SelectedIndex = comboBox8.SelectedIndex;
            }
        }

        static Dictionary<string, string> BuildPortNameHash(string[] portsToMap)
        {
            Dictionary<string, string> oReturnTable = new Dictionary<string, string>();
            MineRegistryForPortName("SYSTEM\\CurrentControlSet\\Enum", oReturnTable, portsToMap);
            return oReturnTable;
        }

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

        private string set_port()
        {
            string sPortName = string.Empty;
            foreach (KeyValuePair<string, string> kvp in friendlyPorts)
                if (kvp.Value.ToLower().Contains("ely"))
                    sPortName = kvp.Key;
            return sPortName;
        }

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
            try
            {
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
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary
                string message = String.Format("Exception occurred during device initialization: {0}", ex.Message);
                OnMsg_UpdateStatusMessage(message);
            }
            finally
            {
                m_bInitializing = false;
                OnMsg_UpdateDeviceList();
            }
        }

        private void DeactivateDevice()
        {
            if (m_nDevHandle != -1)
            {
                // Stop the device if necessary
                DLL._IBSU_CancelCaptureImage(m_nDevHandle);

                // Destroy the client window
                DLL._IBSU_DestroyClientWindow(m_nDevHandle, true);

                // Release callback functions
                DLL._IBSU_ReleaseCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_COMMUNICATION_BREAK);
                DLL._IBSU_ReleaseCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_PREVIEW_IMAGE);
                DLL._IBSU_ReleaseCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_TAKING_ACQUISITION);
                DLL._IBSU_ReleaseCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_COMPLETE_ACQUISITION);
                DLL._IBSU_ReleaseCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_RESULT_IMAGE_EX);
                DLL._IBSU_ReleaseCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_OPTIONAL_EVENT_FINGER_COUNT);
                DLL._IBSU_ReleaseCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_OPTIONAL_EVENT_FINGER_QUALITY);
                DLL._IBSU_ReleaseCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_OPTIONAL_EVENT_CLEAR_PLATEN_AT_CAPTURE);
                DLL._IBSU_ReleaseCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_KEYBUTTON);

                // Close the device
                DLL._IBSU_CloseDevice(m_nDevHandle);
                m_nDevHandle = -1;
            }
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
           // DisplayImageOnPictureBox(fingerName, strFileName);
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
                CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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

            lock (CustomerDetails.m_sync)
            {
                CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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

            lock (CustomerDetails.m_sync)
            {
                CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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

            lock (CustomerDetails.m_sync)
            {
                CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
                int[] qualityArray = new int[4];
                Marshal.Copy(pQualityArray, qualityArray, 0, qualityArrayCount);
                pDlg.m_FingerQuality[0] = (DLL.IBSU_FingerQualityState)qualityArray[0];
                pDlg.m_FingerQuality[1] = (DLL.IBSU_FingerQualityState)qualityArray[1];
                pDlg.m_FingerQuality[2] = (DLL.IBSU_FingerQualityState)qualityArray[2];
                pDlg.m_FingerQuality[3] = (DLL.IBSU_FingerQualityState)qualityArray[3];

            //    pDlg.m_picScanner.Invalidate(false);
            }
        }

        private static void OnEvent_DeviceCount(
            int detectedDevices,
            IntPtr pContext
            )
        {
            if (pContext == null)
                return;

            lock (CustomerDetails.m_sync)
            {
                CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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
                CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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
                CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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
                    CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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

        private static void OnEvent_ResultImageEx(int deviceHandle, IntPtr pContext, int imageStatus, DLL.IBSU_ImageData image, DLL.IBSU_ImageType imageType, int detectedFingerCount, int segmentImageArrayCount, IntPtr pSegmentImageArray, IntPtr pSegmentPositionArray)
        {
            try
            {
                lock (m_sync)
                {
                    CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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
                                //pDlg._SaveJP2Image(ref imageArray[i], segmentName);
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
                    CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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

                   // pDlg.m_picScanner.Invalidate(false);
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
                    CustomerDetails pDlg = (CustomerDetails)CustomerDetails.FromHandle(pContext);
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
                DeactivateDevice();
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
                    DeactivateDevice();
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
                DeactivateDevice();

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

                if (MessageBox.Show(strValue, "e-Verification System", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

        private void CustomerDetails_Form(object sender, EventArgs e)
        {
            try
            {

                m_nDevHandle = -1;
                m_nCurrentCaptureStep = -1;
                m_bInitializing = false;
                m_strImageMessage = "";
                m_bNeedClearPlaten = false;
                m_bSaveWarningOfClearPlaten = false;
                m_btnCaptureStop.Enabled = false;
                m_btnCaptureStart.Enabled = false;
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

            }
            catch (Exception ex)
            {
                string fx;
                string caller;
                Logger.ErrorFunctions(new StackTrace(), new StackFrame(), out fx, out caller);
                Logger.LogError(string.Format("Error {0} >> {1} \n", DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss"), ExceptionExtensions.ToLogString(ex, Environment.StackTrace)), fx, caller);

            }
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Form7_Load(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        

        private void m_btnCaptureStop_Click(object sender, EventArgs e)
        {
            OnMsg_CaptureStopClick();
            clearScren();
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

        private void CustomerDetailsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DeactivateDevice();
                OnMsg_CaptureStopClick();
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
            string message = "e-Verification System";
            e.Graphics.DrawString(message, new Font("Times New Roman", 28), Brushes.White, 30, 10);
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
                m_ImgSubFolder = "";
                pbPhoto.Image = eVerification.Properties.Resources.image;

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

        private void button17_Click(object sender, EventArgs e)
        {
            SDKMainForm sdk = new SDKMainForm();
            sdk.Show();
            this.Hide();
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            var travelEventType = cbEvent.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(travelEventType) || travelEventType == "~ Select ~")
            {
                MessageBox.Show("Please select a travel event.");
                return;
            }

            var documentNumber = tbPassportNo.Text; // Assuming you have a text box for this
            if (string.IsNullOrEmpty(documentNumber) || documentNumber == "~ Select ~")
            {
                MessageBox.Show("Please enter document number.");
                return;
            }

            var issuingCountry = tbIssuingCountry.SelectedValue?.ToString(); 
            if (string.IsNullOrEmpty(issuingCountry) || issuingCountry == "~ Select ~")
            {
                MessageBox.Show("Please select Issuing Country");
                return;
            }
            var documentType = cbDocType.SelectedValue.ToString();
            if (string.IsNullOrEmpty(documentType) || documentType == "~ Select ~")
            {
                MessageBox.Show("Please select Document type");
                return;
            }
            var surname = tbFirstname.Text; // Assuming you have a text box for this
            if (string.IsNullOrEmpty(surname) || surname == "~ Select ~")
            {
                MessageBox.Show("Please enter Surname");
                return;
            }
            var givenNames = tbSecondname.Text; // Assuming you have a text box for this
            if (string.IsNullOrEmpty(givenNames) || givenNames == "~ Select ~")
            {
                MessageBox.Show("Please enter Given Names");
                return;
            }
            var applicationNumber = tbApplicationNumber.Text; // Assuming you have a text box for this
            if (string.IsNullOrEmpty(applicationNumber) || applicationNumber == "~ Select ~")
            {
                MessageBox.Show("Please enter Application Number");
                return;
            }
            var passportNumber = tbPassportNo.Text;
            if (string.IsNullOrEmpty(passportNumber) || passportNumber == "~ Select ~")
            {
                MessageBox.Show("Please enter Document Number");
                return;
            }
            var nationality = tbNationality.Text;
            if (string.IsNullOrEmpty(nationality) || nationality == "~ Select ~")
            {
                MessageBox.Show("Please enter Nationality");
                return;
            }
            var dateOfBirth = tbDateOfBirth.Text;
            if (string.IsNullOrEmpty(dateOfBirth) || dateOfBirth == "~ Select ~")
            {
                MessageBox.Show("Please enter Date of Birth");
                return;
            }
            var placeOfBirth = tbPlaceOfBirth.Text; // Assuming you have a text box for this
            if (string.IsNullOrEmpty(placeOfBirth) || placeOfBirth == "~ Select ~")
            {
                MessageBox.Show("Please enter Place of Birth");
                return;
            }
            var countryOfResidence = cbCountry.SelectedValue.ToString();
            if (string.IsNullOrEmpty(countryOfResidence) || countryOfResidence == "~ Select ~")
            {
                MessageBox.Show("Please enter Country of Residence");
                return;
            }
            var flightNumber = cbFlight.SelectedValue.ToString();
            if (string.IsNullOrEmpty(flightNumber) || flightNumber == "~ Select ~")
            {
                MessageBox.Show("Please enter Flight Number");
                return;
            }
            var origination = tbOrigination.Text; // Assuming you have a text box for this
            if (string.IsNullOrEmpty(origination) || origination == "~ Select ~")
            {
                MessageBox.Show("Please enter Document Number");
                return;
            }
            var destination = tbDestination.Text; // Assuming you have a text box for this
            if (string.IsNullOrEmpty(destination) || destination == "~ Select ~")
            {
                MessageBox.Show("Please enter Destination");
                return;
            }
            var lengthOfStay = int.Parse(tbLengthOfStay.Text); // Assuming you have a text box for this
            if (!int.TryParse(tbLengthOfStay.Text, out lengthOfStay) || lengthOfStay <= 0)
            {
                MessageBox.Show("Please enter a valid Length of Stay");
                return;
            }
            var profession = tbProfession.Text; // Assuming you have a text box for this
            if (string.IsNullOrEmpty(profession) || profession == "~ Select ~")
            {
                MessageBox.Show("Please enter Profession");
                return;
            }
            var reasonForTravel = tbReasonForTravel.Text; // Assuming you have a text box for this
            if (string.IsNullOrEmpty(reasonForTravel) || reasonForTravel == "~ Select ~")
            {
                MessageBox.Show("Please enter Reason for Travel");
                return;
            }
            var travelDateTime = DateTime.Parse(tbTravelDateTime.Text); // Assuming you have a text box for this
            if (!DateTime.TryParse(tbTravelDateTime.Text, out travelDateTime) || travelDateTime == DateTime.MinValue)
            {
                MessageBox.Show("Please enter a valid Date of Event");
                return;
            }

            // Create a TravelEntryRequest object and store it temporarily
            tempTravelEntryRequest = new TravelEntryRequest
            {
                travelEventType = travelEventType,
                documentNumber = documentNumber,
                issuingCountry = issuingCountry,
                documentType = documentType,
                surname = surname,
                givenNames = givenNames,
                applicationNumber = applicationNumber,
                passportNumber = passportNumber,
                nationality = nationality,
                dateOfBirth = dateOfBirth,
                placeOfBirth = placeOfBirth,
                countryOfResidence = countryOfResidence,
                flightNumber = flightNumber,
                origination = origination,
                destination = destination,
                lengthOfStay = lengthOfStay,
                profession = profession,
                reasonForTravel = reasonForTravel,
                travelDateTime = travelDateTime
            };

            // Redirect to the images tab
            RedirectToImagesTab();
        }

        private void RedirectToImagesTab()
        {
            //redirecting to image tab
            Images.Show();
            Images.BringToFront();
        }

        private void m_btnCaptureStart_Click(object sender, EventArgs e)
        {
            OnMsg_CaptureStartClick();
        }

        private void cbDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            myCamera.ChangeCamera(cbDevice.SelectedIndex);
        }

        private void cbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            myCamera.Start(cbResolution.SelectedIndex);
        }

        private void captureBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string directoryPath = @"C:\Users\thispc\Desktop\charlie\images";

                // Ensure the directory exists
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    MessageBox.Show("Image Folder Created");
                }

                // If the button text is "Recapture", delete the previous image and restart the camera
                if (captureBtn.Text == "Recapture" && !string.IsNullOrEmpty(lastCapturedImageFilename))
                {
                    if (File.Exists(lastCapturedImageFilename))
                    {
                        File.Delete(lastCapturedImageFilename);
                    }
                    // Restart the camera for capturing a new image
                    myCamera.Start();
                    captureBtn.Text = "Capture"; // Change text back to "Capture" until new image is saved
                    return;
                }

                // Generate a unique filename to avoid overwriting
                string filename = Path.Combine(directoryPath, "image_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg");

                // Ensure the image is not null
                if (pbCamera.Image != null)
                {
                    pbCamera.Image.Save(filename, ImageFormat.Jpeg);
                    lastCapturedImageFilename = filename; // Update the last captured image filename

                    // Change the button text to "Recapture"
                    captureBtn.Text = "Recapture";

                    // Convert the captured image to a base64 string
                    string imageData = Convert.ToBase64String(File.ReadAllBytes(filename));
                    tempTravelEntryRequest.photo = imageData;                    
                }
                else
                {
                    // Handle the case when the image is null
                    Console.WriteLine("No image to save.");
                }

                myCamera.Stop();
            }
            catch (Exception ex)
            {
                // Handle exceptions that occur during the save operation
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Method to post the travel entry with the image
        private void PostTravelEntryWithImage()
        {
            try
            {
                APIInterface api = new APIInterface { url = "http://gto-srv-train01:8586/api/eVisa/" };
                var response = api.post_travel_entry(tempTravelEntryRequest);

                // Handle the response
                if (response != null)
                {
                    MessageBox.Show("Post Travel Entry successful: " + response.message);
                }
                else
                {
                    Logger.LogError("Post Travel Entry failed. Response was null or invalid.");
                    MessageBox.Show("Post Travel Entry failed.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception in PostTravelEntryWithImage: " + ex.Message);
                MessageBox.Show("Post Travel Entry failed due to an exception.");
            }
        }




        private void allNextBtn_Click(object sender, EventArgs e)
        {
            if (tempTravelEntryRequest == null)
            {
                MessageBox.Show("Travel Entry Request is not initialized.");
                return;
            }
            // Collect fingerprint images and add them to the request
            tempTravelEntryRequest.fingerImages = new List<FingerPrintEntry>
             {
                 new FingerPrintEntry { fingerType = "Right Thumb", fingerImage = ImageToBase64(pbRightThumb.Image), fingerPrintQuality = 0 },
                 new FingerPrintEntry { fingerType = "Right Index", fingerImage = ImageToBase64(pbRightIndex.Image), fingerPrintQuality = 0 },
                 new FingerPrintEntry { fingerType = "Right Middle", fingerImage = ImageToBase64(pbRightMiddle.Image), fingerPrintQuality = 0 },
                 new FingerPrintEntry { fingerType = "Right Ring", fingerImage = ImageToBase64(pbRightRing.Image), fingerPrintQuality = 0 },
                 new FingerPrintEntry { fingerType = "Right Little", fingerImage = ImageToBase64(pbRightLittle.Image), fingerPrintQuality = 0 },
                 new FingerPrintEntry { fingerType = "Left Thumb", fingerImage = ImageToBase64(pbLeftThumb.Image), fingerPrintQuality = 0 },
                 new FingerPrintEntry { fingerType = "Left Index", fingerImage = ImageToBase64(pbLeftIndex.Image), fingerPrintQuality = 0 },
                 new FingerPrintEntry { fingerType = "Left Middle", fingerImage = ImageToBase64(pbLeftMiddle.Image), fingerPrintQuality = 0 },
                 new FingerPrintEntry { fingerType = "Left Ring", fingerImage = ImageToBase64(pbLeftRing.Image), fingerPrintQuality = 0 },
                 new FingerPrintEntry { fingerType = "Left Little", fingerImage = ImageToBase64(pbLeftLittle.Image), fingerPrintQuality = 0 }
             }; 

             // Post the complete data to the API
            PostTravelEntryWithImage(); 
        }
        private string ImageToBase64(Image image)
        {
            if (image == null)
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }


        private void imgNextBtn_Click(object sender, EventArgs e)
        {
            Fingerprints.Show();
            Fingerprints.BringToFront();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Images.Show();
            Images.BringToFront();
        }

        private void orientBtn_Click(object sender, EventArgs e)
        {
            currentOrientation = (currentOrientation + 1) % 4;
            AdjustImageOrientation();
        }
        private void AdjustImageOrientation()
        {
            if (pbCamera.Image == null)
            {
                return;
            }

            Image img = (Image)pbCamera.Image.Clone();

            switch (currentOrientation)
            {
                case 0:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    break;
                case 1:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            pbCamera.Image = img;
        }
    }
}
