using eVerification.codes.Wsq2Bmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eVerification
{
    public partial class SuccessForm : Form
    {
        public SuccessForm()
        {
            InitializeComponent();
        }


        public Image LoadImage(string data)
        {
            byte[] bytes = Convert.FromBase64String(data);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
                image = Image.FromStream(ms, true, false);
            return image;
        }

        private void SuccessForm_Load(object sender, EventArgs e)
        {
            if (SDKMainForm.verifyResponse != null)
            {
                gbFingers.Text = string.Format("Finger Prints - {0}", SDKMainForm.verifyResponse.data.fingerType);
                tbName.Text = SDKMainForm.verifyResponse.data.fullName;
                tbNationality.Text = SDKMainForm.verifyResponse.data.nationality;
                tbPassport.Text = SDKMainForm.verifyResponse.data.passportNumber;
                tbDOB.Text = SDKMainForm.verifyResponse.data.dateOfBirth;
                tbAppNo.Text = SDKMainForm.verifyResponse.data.applicationNumber;
                tbAppType.Text = SDKMainForm.verifyResponse.data.applicationtype;
                tbAPpStatus.Text = SDKMainForm.verifyResponse.data.applicationStatus;
                tbAppExpDate.Text = SDKMainForm.verifyResponse.data.visaExpirationDate;
                tbBirthPlace.Text = SDKMainForm.verifyResponse.data.placeOfBirth;
                tbEmail.Text = SDKMainForm.verifyResponse.data.email;
                tbIssuePlace.Text = SDKMainForm.verifyResponse.data.documentPlaceOfIssue;
                tbAppMesage.Text = SDKMainForm.verifyResponse.data.message;

                try
                {
                    if (SDKMainForm.verifyResponse.data.photo != null)
                    { 
                        byte[] bytes = Convert.FromBase64String(SDKMainForm.verifyResponse.data.photo);
                        Image image;
                        MemoryStream ms = new MemoryStream(bytes);
                        image = Image.FromStream(ms, true, false);
                        imgPhoto.Image = image; 
                    }
                    if (SDKMainForm.verifyResponse.data.providedFingerPrint != null)
                        picScanned.Image = LoadImage(SDKMainForm.verifyResponse.data.providedFingerPrint);
                    if (SDKMainForm.verifyResponse.data.fingerPrintFromDatabase != null)
                        picDB.Image = LoadImage(SDKMainForm.verifyResponse.data.fingerPrintFromDatabase);
                    /* {
                         byte[] bytes = Convert.FromBase64String(SDKMainForm.verifyResponse.data.fingerPrintFromDatabase);
                         WsqDecoder decoder = new WsqDecoder();
                         Bitmap bmp = decoder.Decode(bytes);
                         picDB.Image = bmp;
                         //picDB.Image = LoadImage(SDKMainForm.verifyResponse.data.fingerPrintFromDatabase);
                     }*/
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void m_picIBLogo_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString(SDKMainForm.verifyResponse.data.message, new Font("Times New Roman", 18), Brushes.White, 30, 10);
            m_picIBLogo.BackColor = SDKMainForm.verifyResponse.code == 0 ? Color.Green : Color.Red;
        }
    }
}
