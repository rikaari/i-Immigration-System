
namespace eVerification
{
    partial class AppListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gvSearchResults = new System.Windows.Forms.DataGridView();
            this.datumBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lASTNAMEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fIRSTNAMEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bIRTHDAYDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pASSPORTNUMBERDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pLACEOFBIRTHDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pLACEOFISSUEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.issuingcountryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nationalityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.documenttypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.applicationidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.applicationstatusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vISALONAPPTYPEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvSearchResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datumBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.gvSearchResults);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1027, 524);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Results";
            // 
            // gvSearchResults
            // 
            this.gvSearchResults.AllowUserToAddRows = false;
            this.gvSearchResults.AllowUserToDeleteRows = false;
            this.gvSearchResults.AutoGenerateColumns = false;
            this.gvSearchResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSearchResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.lASTNAMEDataGridViewTextBoxColumn,
            this.fIRSTNAMEDataGridViewTextBoxColumn,
            this.bIRTHDAYDataGridViewTextBoxColumn,
            this.pASSPORTNUMBERDataGridViewTextBoxColumn,
            this.pLACEOFBIRTHDataGridViewTextBoxColumn,
            this.pLACEOFISSUEDataGridViewTextBoxColumn,
            this.issuingcountryDataGridViewTextBoxColumn,
            this.nationalityDataGridViewTextBoxColumn,
            this.documenttypeDataGridViewTextBoxColumn,
            this.applicationidDataGridViewTextBoxColumn,
            this.applicationstatusDataGridViewTextBoxColumn,
            this.vISALONAPPTYPEDataGridViewTextBoxColumn});
            this.gvSearchResults.DataSource = this.datumBindingSource;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gvSearchResults.DefaultCellStyle = dataGridViewCellStyle1;
            this.gvSearchResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvSearchResults.Location = new System.Drawing.Point(3, 16);
            this.gvSearchResults.Name = "gvSearchResults";
            this.gvSearchResults.ReadOnly = true;
            this.gvSearchResults.Size = new System.Drawing.Size(1021, 505);
            this.gvSearchResults.TabIndex = 0;
            this.gvSearchResults.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvSearchResults_CellContentClick);
            // 
            // datumBindingSource
            // 
            this.datumBindingSource.DataSource = typeof(eVerification.codes.Datum);
            // 
            // lASTNAMEDataGridViewTextBoxColumn
            // 
            this.lASTNAMEDataGridViewTextBoxColumn.DataPropertyName = "LAST_NAME";
            this.lASTNAMEDataGridViewTextBoxColumn.HeaderText = "Surname";
            this.lASTNAMEDataGridViewTextBoxColumn.Name = "lASTNAMEDataGridViewTextBoxColumn";
            this.lASTNAMEDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // fIRSTNAMEDataGridViewTextBoxColumn
            // 
            this.fIRSTNAMEDataGridViewTextBoxColumn.DataPropertyName = "FIRST_NAME";
            this.fIRSTNAMEDataGridViewTextBoxColumn.HeaderText = "Given.Name";
            this.fIRSTNAMEDataGridViewTextBoxColumn.Name = "fIRSTNAMEDataGridViewTextBoxColumn";
            this.fIRSTNAMEDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bIRTHDAYDataGridViewTextBoxColumn
            // 
            this.bIRTHDAYDataGridViewTextBoxColumn.DataPropertyName = "BIRTHDAY";
            this.bIRTHDAYDataGridViewTextBoxColumn.HeaderText = "BirthDay";
            this.bIRTHDAYDataGridViewTextBoxColumn.Name = "bIRTHDAYDataGridViewTextBoxColumn";
            this.bIRTHDAYDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // pASSPORTNUMBERDataGridViewTextBoxColumn
            // 
            this.pASSPORTNUMBERDataGridViewTextBoxColumn.DataPropertyName = "PASSPORT_NUMBER";
            this.pASSPORTNUMBERDataGridViewTextBoxColumn.HeaderText = "Passport.No";
            this.pASSPORTNUMBERDataGridViewTextBoxColumn.Name = "pASSPORTNUMBERDataGridViewTextBoxColumn";
            this.pASSPORTNUMBERDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // pLACEOFBIRTHDataGridViewTextBoxColumn
            // 
            this.pLACEOFBIRTHDataGridViewTextBoxColumn.DataPropertyName = "PLACE_OF_BIRTH";
            this.pLACEOFBIRTHDataGridViewTextBoxColumn.HeaderText = "Birth.Place";
            this.pLACEOFBIRTHDataGridViewTextBoxColumn.Name = "pLACEOFBIRTHDataGridViewTextBoxColumn";
            this.pLACEOFBIRTHDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // pLACEOFISSUEDataGridViewTextBoxColumn
            // 
            this.pLACEOFISSUEDataGridViewTextBoxColumn.DataPropertyName = "PLACE_OF_ISSUE";
            this.pLACEOFISSUEDataGridViewTextBoxColumn.HeaderText = "Issue.Place";
            this.pLACEOFISSUEDataGridViewTextBoxColumn.Name = "pLACEOFISSUEDataGridViewTextBoxColumn";
            this.pLACEOFISSUEDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // issuingcountryDataGridViewTextBoxColumn
            // 
            this.issuingcountryDataGridViewTextBoxColumn.DataPropertyName = "issuing_country";
            this.issuingcountryDataGridViewTextBoxColumn.HeaderText = "Issuie.Country";
            this.issuingcountryDataGridViewTextBoxColumn.Name = "issuingcountryDataGridViewTextBoxColumn";
            this.issuingcountryDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nationalityDataGridViewTextBoxColumn
            // 
            this.nationalityDataGridViewTextBoxColumn.DataPropertyName = "nationality";
            this.nationalityDataGridViewTextBoxColumn.HeaderText = "Nationality";
            this.nationalityDataGridViewTextBoxColumn.Name = "nationalityDataGridViewTextBoxColumn";
            this.nationalityDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // documenttypeDataGridViewTextBoxColumn
            // 
            this.documenttypeDataGridViewTextBoxColumn.DataPropertyName = "document_type";
            this.documenttypeDataGridViewTextBoxColumn.HeaderText = "Document.Type";
            this.documenttypeDataGridViewTextBoxColumn.Name = "documenttypeDataGridViewTextBoxColumn";
            this.documenttypeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // applicationidDataGridViewTextBoxColumn
            // 
            this.applicationidDataGridViewTextBoxColumn.DataPropertyName = "application_id";
            this.applicationidDataGridViewTextBoxColumn.HeaderText = "Application.ID";
            this.applicationidDataGridViewTextBoxColumn.Name = "applicationidDataGridViewTextBoxColumn";
            this.applicationidDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // applicationstatusDataGridViewTextBoxColumn
            // 
            this.applicationstatusDataGridViewTextBoxColumn.DataPropertyName = "application_status";
            this.applicationstatusDataGridViewTextBoxColumn.HeaderText = "Application.Status";
            this.applicationstatusDataGridViewTextBoxColumn.Name = "applicationstatusDataGridViewTextBoxColumn";
            this.applicationstatusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // vISALONAPPTYPEDataGridViewTextBoxColumn
            // 
            this.vISALONAPPTYPEDataGridViewTextBoxColumn.DataPropertyName = "VISALON_APP_TYPE";
            this.vISALONAPPTYPEDataGridViewTextBoxColumn.HeaderText = "VISA.APP.TYPE";
            this.vISALONAPPTYPEDataGridViewTextBoxColumn.Name = "vISALONAPPTYPEDataGridViewTextBoxColumn";
            this.vISALONAPPTYPEDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // AppListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 555);
            this.Controls.Add(this.groupBox2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppListForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Results";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.AppListForm_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvSearchResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datumBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView gvSearchResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn lASTNAMEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fIRSTNAMEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn bIRTHDAYDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pASSPORTNUMBERDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pLACEOFBIRTHDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pLACEOFISSUEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn issuingcountryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nationalityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn documenttypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn applicationidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn applicationstatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vISALONAPPTYPEDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource datumBindingSource;
    }
}