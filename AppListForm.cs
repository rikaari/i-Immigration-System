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
    public partial class AppListForm : Form
    {
        public AppListForm()
        {
            InitializeComponent();
        }

        private void AppListForm_Load(object sender, EventArgs e)
        {
            gvSearchResults.DataSource = SDKMainForm.searchResponse.data.ToList();
            //gvSearchResults.
            DataGridViewButtonColumn button = new DataGridViewButtonColumn();
            {
                button.Name = "Select";
                button.HeaderText = "Select";
                button.Text = "Select";
                button.UseColumnTextForButtonValue = true; //dont forget this line
                this.gvSearchResults.Columns.Add(button);
            }
        }

        private void gvSearchResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        { 
            if (gvSearchResults.Columns[e.ColumnIndex].Name == "Select")
            {
                SDKMainForm.SelectedIdex = e.RowIndex;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
