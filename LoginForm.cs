using eVerification.codes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace eVerification
{
    public partial class LoginForm : Form
    {
        public bool UserSuccessfullyAuthenticated { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            lblError.Text = "";
        }

        bool validate()
        {
            if (string.IsNullOrEmpty(tbUsername.Text))
            {
                tbUsername.Focus();
                errorProvider1.SetError(tbUsername, "Please Enter Username.");
                return false;
            }
            else
            {
                errorProvider1.SetError(tbUsername, "");
            }

            if (string.IsNullOrEmpty(tbPassword.Text))
            {
                tbPassword.Focus();
                errorProvider1.SetError(tbPassword, "Please Enter Password.");
                return false;
            }
            else
            {
                errorProvider1.SetError(tbPassword, "");
            }

            return true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("Login button clicked.");
                var _url = eVerification.Properties.Settings.Default.finger_verif_url;

                if (string.IsNullOrEmpty(_url))
                {
                    _url = "http://gto-srv-train01:8889/interface/offsite_biometric/";
                }
                Logger.LogInfo($"Using API URL: {_url}");

                codes.APIInterface api = new codes.APIInterface { url = _url };

                if (validate())
                {
                    Logger.LogInfo("Validation passed.");

                    if (NetworkStatus.CheckNet())
                    {
                        Logger.LogInfo("Network status check passed.");

                        if (NetworkStatus.IsConnectedToInternet())
                        {
                            Logger.LogInfo("Connected to the Internet.");

                            var loginRequest = new codes.LoginRequest
                            {
                                username = tbUsername.Text.Trim(),
                                password = tbPassword.Text.Trim()
                            };

                            string requestJson = JsonConvert.SerializeObject(loginRequest);
                            Logger.LogInfo($"Login request: {requestJson}");

                            var login = api.login(loginRequest);

                            if (login != null)
                            {
                                Logger.LogInfo($"Login response: {JsonConvert.SerializeObject(login)}");

                                if (login.code == 0)
                                {
                                    Logger.LogInfo("User authenticated successfully.");
                                    UserSuccessfullyAuthenticated = true;
                                    Close();
                                }
                                else
                                {
                                    Logger.LogWarning($"Login failed: {login.message}");
                                    lblError.Text = login.message;
                                }
                            }
                            else
                            {
                                Logger.LogError("No response from server.");
                                lblError.Text = "No Response from Server";
                            }
                        }
                        else
                        {
                            Logger.LogError("Internet connection failed.");
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
                        Logger.LogError("Network check failed.");
                        SDKMainForm.verifyResponse = new VerifyResponse
                        {
                            code = 100,
                            data = new Data
                            {
                                message = "Internet Connection failed, Ensure you are connected to the Internet"
                            }
                        };

                        Form popup = new PopupForm();
                        DialogResult dialogresult = popup.ShowDialog();
                    }
                }
                else
                {
                    Logger.LogError("Validation failed.");
                    lblError.Text = "No Response from Server";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception during login process: {ex.Message}");
                lblError.Text = "No Response from Server";
            }
        }
    }
}

