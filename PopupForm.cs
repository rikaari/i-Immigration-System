using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eVerification
{
    public partial class PopupForm : Form
    {
        public PopupForm()
        {
            InitializeComponent();
        }

        private void PopupForm_Load(object sender, EventArgs e)
        {
            m_labelResults.Text = SDKMainForm.verifyResponse.data != null ? SDKMainForm.verifyResponse.data.message : SDKMainForm.verifyResponse.message;
            img_status.Image = SDKMainForm.verifyResponse.code == 0 ? eVerification.Properties.Resources.success : eVerification.Properties.Resources.failure;
        }
    }
}
