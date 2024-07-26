using ElySCardDotNet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;

namespace eVerification
{
    partial class SDKMainForm
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
            if( m_initThread != null )
                m_initThread.Abort();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDKMainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbFingerPrintOnly = new System.Windows.Forms.CheckBox();
            this.cbDocumentOnly = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbDocType = new System.Windows.Forms.ComboBox();
            this.cbFingerPrinterLater = new System.Windows.Forms.CheckBox();
            this.tbDocumentNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbCountry = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbFirstName = new System.Windows.Forms.TextBox();
            this.tbLastName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.m_picScanner = new System.Windows.Forms.PictureBox();
            this.m_FrameImage = new System.Windows.Forms.Label();
            this.Timer_StatusFingerQuality = new System.Windows.Forms.Timer(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.m_btnVerify = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.m_picIBLogo = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPassportNo = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tbBirthPlace = new System.Windows.Forms.TextBox();
            this.tbFullname = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tbIssuePlace = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.tbAppStatus = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tbAppType = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbAppNo = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.tbAppExpDate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbCompany = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tbDateOfBirth = new System.Windows.Forms.TextBox();
            this.pbPhoto = new System.Windows.Forms.PictureBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tbNationality = new System.Windows.Forms.TextBox();
            this.m_txtStatusMessage = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbRightLittle = new System.Windows.Forms.PictureBox();
            this.pbRightRing = new System.Windows.Forms.PictureBox();
            this.pbRightMiddle = new System.Windows.Forms.PictureBox();
            this.pbRightIndex = new System.Windows.Forms.PictureBox();
            this.pbRightThumb = new System.Windows.Forms.PictureBox();
            this.pbLeftThumb = new System.Windows.Forms.PictureBox();
            this.pbLeftLittle = new System.Windows.Forms.PictureBox();
            this.pbLeftRing = new System.Windows.Forms.PictureBox();
            this.pbLeftMiddle = new System.Windows.Forms.PictureBox();
            this.pbLeftIndex = new System.Windows.Forms.PictureBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.dataGridView5 = new System.Windows.Forms.DataGridView();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.nextBtn = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.m_btnCaptureStop = new System.Windows.Forms.Button();
            this.m_btnCaptureStart = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_picScanner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_picIBLogo)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightLittle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightRing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightMiddle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightThumb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftThumb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftLittle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftRing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftMiddle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tableLayoutPanel1);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(274, 33);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(336, 141);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Input Details";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 456F));
            this.tableLayoutPanel1.Controls.Add(this.cbFingerPrintOnly, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbDocumentOnly, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbDocType, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.cbFingerPrinterLater, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbDocumentNo, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbCountry, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tbFirstName, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.tbLastName, 4, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(21, 15);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(315, 110);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // cbFingerPrintOnly
            // 
            this.cbFingerPrintOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbFingerPrintOnly.AutoSize = true;
            this.cbFingerPrintOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbFingerPrintOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFingerPrintOnly.Location = new System.Drawing.Point(201, 8);
            this.cbFingerPrintOnly.Margin = new System.Windows.Forms.Padding(2);
            this.cbFingerPrintOnly.Name = "cbFingerPrintOnly";
            this.cbFingerPrintOnly.Size = new System.Drawing.Size(83, 17);
            this.cbFingerPrintOnly.TabIndex = 44;
            this.cbFingerPrintOnly.Text = "Names Only";
            this.cbFingerPrintOnly.UseVisualStyleBackColor = true;
            this.cbFingerPrintOnly.CheckedChanged += new System.EventHandler(this.cbFingerPrintOnly_CheckedChanged);
            // 
            // cbDocumentOnly
            // 
            this.cbDocumentOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbDocumentOnly.AutoSize = true;
            this.cbDocumentOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbDocumentOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbDocumentOnly.Location = new System.Drawing.Point(4, 8);
            this.cbDocumentOnly.Margin = new System.Windows.Forms.Padding(2);
            this.cbDocumentOnly.Name = "cbDocumentOnly";
            this.cbDocumentOnly.Size = new System.Drawing.Size(92, 17);
            this.cbDocumentOnly.TabIndex = 22;
            this.cbDocumentOnly.Text = "Documents only";
            this.cbDocumentOnly.UseVisualStyleBackColor = true;
            this.cbDocumentOnly.CheckedChanged += new System.EventHandler(this.cbDocumentOnly_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 39);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Document Type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbDocType
            // 
            this.cbDocType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbDocType.DisplayMember = "Name";
            this.cbDocType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDocType.DropDownWidth = 500;
            this.cbDocType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.cbDocType.FormattingEnabled = true;
            this.cbDocType.Location = new System.Drawing.Point(4, 54);
            this.cbDocType.Margin = new System.Windows.Forms.Padding(2);
            this.cbDocType.Name = "cbDocType";
            this.cbDocType.Size = new System.Drawing.Size(92, 25);
            this.cbDocType.TabIndex = 20;
            this.cbDocType.ValueMember = "Iso";
            this.cbDocType.SelectedIndexChanged += new System.EventHandler(this.cbDocType_SelectedIndexChanged);
            // 
            // cbFingerPrinterLater
            // 
            this.cbFingerPrinterLater.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbFingerPrinterLater.AutoSize = true;
            this.cbFingerPrinterLater.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbFingerPrinterLater.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFingerPrinterLater.Location = new System.Drawing.Point(108, 8);
            this.cbFingerPrinterLater.Margin = new System.Windows.Forms.Padding(2);
            this.cbFingerPrinterLater.Name = "cbFingerPrinterLater";
            this.cbFingerPrinterLater.Size = new System.Drawing.Size(81, 17);
            this.cbFingerPrinterLater.TabIndex = 21;
            this.cbFingerPrinterLater.Text = "FingerPrint Later";
            this.cbFingerPrinterLater.UseVisualStyleBackColor = true;
            this.cbFingerPrinterLater.CheckedChanged += new System.EventHandler(this.cbFingerPrinterLater_CheckedChanged);
            // 
            // tbDocumentNo
            // 
            this.tbDocumentNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbDocumentNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbDocumentNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbDocumentNo.Location = new System.Drawing.Point(108, 54);
            this.tbDocumentNo.Margin = new System.Windows.Forms.Padding(2);
            this.tbDocumentNo.Name = "tbDocumentNo";
            this.tbDocumentNo.Size = new System.Drawing.Size(81, 23);
            this.tbDocumentNo.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(201, 39);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Country:";
            // 
            // cbCountry
            // 
            this.cbCountry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbCountry.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbCountry.DisplayMember = "Name";
            this.cbCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCountry.DropDownWidth = 500;
            this.cbCountry.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.cbCountry.FormattingEnabled = true;
            this.cbCountry.Location = new System.Drawing.Point(201, 54);
            this.cbCountry.Margin = new System.Windows.Forms.Padding(2);
            this.cbCountry.Name = "cbCountry";
            this.cbCountry.Size = new System.Drawing.Size(98, 25);
            this.cbCountry.TabIndex = 18;
            this.cbCountry.ValueMember = "Iso";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(108, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Document No.:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(4, 93);
            this.label5.Margin = new System.Windows.Forms.Padding(2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 45;
            this.label5.Text = "Name:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbFirstName
            // 
            this.tbFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbFirstName.Location = new System.Drawing.Point(108, 83);
            this.tbFirstName.Margin = new System.Windows.Forms.Padding(2);
            this.tbFirstName.Name = "tbFirstName";
            this.tbFirstName.Size = new System.Drawing.Size(81, 23);
            this.tbFirstName.TabIndex = 46;
            // 
            // tbLastName
            // 
            this.tbLastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbLastName.Location = new System.Drawing.Point(201, 83);
            this.tbLastName.Margin = new System.Windows.Forms.Padding(2);
            this.tbLastName.Name = "tbLastName";
            this.tbLastName.Size = new System.Drawing.Size(98, 23);
            this.tbLastName.TabIndex = 47;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.m_picScanner);
            this.groupBox3.Location = new System.Drawing.Point(625, 304);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(142, 170);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Device Quality";
            // 
            // m_picScanner
            // 
            this.m_picScanner.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.m_picScanner.ErrorImage = null;
            this.m_picScanner.Image = ((System.Drawing.Image)(resources.GetObject("m_picScanner.Image")));
            this.m_picScanner.Location = new System.Drawing.Point(14, 21);
            this.m_picScanner.Margin = new System.Windows.Forms.Padding(2);
            this.m_picScanner.Name = "m_picScanner";
            this.m_picScanner.Size = new System.Drawing.Size(124, 137);
            this.m_picScanner.TabIndex = 5;
            this.m_picScanner.TabStop = false;
            this.m_picScanner.Paint += new System.Windows.Forms.PaintEventHandler(this.m_picScanner_Paint);
            // 
            // m_FrameImage
            // 
            this.m_FrameImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_FrameImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_FrameImage.Location = new System.Drawing.Point(625, 33);
            this.m_FrameImage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_FrameImage.Name = "m_FrameImage";
            this.m_FrameImage.Size = new System.Drawing.Size(234, 263);
            this.m_FrameImage.TabIndex = 2;
            // 
            // Timer_StatusFingerQuality
            // 
            this.Timer_StatusFingerQuality.Enabled = true;
            this.Timer_StatusFingerQuality.Interval = 500;
            this.Timer_StatusFingerQuality.Tick += new System.EventHandler(this.Timer_StatusFingerQuality_Tick);
            // 
            // m_btnVerify
            // 
            this.m_btnVerify.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnVerify.Location = new System.Drawing.Point(772, 391);
            this.m_btnVerify.Margin = new System.Windows.Forms.Padding(2);
            this.m_btnVerify.Name = "m_btnVerify";
            this.m_btnVerify.Size = new System.Drawing.Size(88, 30);
            this.m_btnVerify.TabIndex = 9;
            this.m_btnVerify.Text = "VERIFY";
            this.m_btnVerify.UseVisualStyleBackColor = true;
            this.m_btnVerify.Click += new System.EventHandler(this.m_btnVerify_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // m_picIBLogo
            // 
            this.m_picIBLogo.BackColor = System.Drawing.SystemColors.HotTrack;
            this.m_picIBLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.m_picIBLogo.ErrorImage = null;
            this.m_picIBLogo.InitialImage = null;
            this.m_picIBLogo.Location = new System.Drawing.Point(-3, 1);
            this.m_picIBLogo.Margin = new System.Windows.Forms.Padding(2);
            this.m_picIBLogo.Name = "m_picIBLogo";
            this.m_picIBLogo.Size = new System.Drawing.Size(966, 27);
            this.m_picIBLogo.TabIndex = 6;
            this.m_picIBLogo.TabStop = false;
            this.m_picIBLogo.Paint += new System.Windows.Forms.PaintEventHandler(this.m_picIBLogo_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(274, 178);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(336, 299);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Customer Details";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 98F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 450F));
            this.tableLayoutPanel2.Controls.Add(this.textBox2, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.textBox1, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.tbPassportNo, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label25, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.label16, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tbBirthPlace, 0, 9);
            this.tableLayoutPanel2.Controls.Add(this.tbFullname, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label24, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tbIssuePlace, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label23, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.tbEmail, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.tbAppStatus, 2, 9);
            this.tableLayoutPanel2.Controls.Add(this.label21, 2, 8);
            this.tableLayoutPanel2.Controls.Add(this.tbAppType, 2, 7);
            this.tableLayoutPanel2.Controls.Add(this.label22, 2, 6);
            this.tableLayoutPanel2.Controls.Add(this.label4, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.tbAppNo, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.label26, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.tbAppExpDate, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 4, 2);
            this.tableLayoutPanel2.Controls.Add(this.tbCompany, 4, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(16, 18);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.tableLayoutPanel2.RowCount = 10;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(316, 259);
            this.tableLayoutPanel2.TabIndex = 10;
            this.tableLayoutPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel2_Paint);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox2.Enabled = false;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.textBox2.Location = new System.Drawing.Point(2, 177);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(94, 23);
            this.textBox2.TabIndex = 35;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(2, 162);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 35;
            this.label8.Text = "Nationality:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox1.Enabled = false;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.textBox1.Location = new System.Drawing.Point(2, 127);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(94, 23);
            this.textBox1.TabIndex = 35;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(2, 112);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Date Of Birth:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbPassportNo
            // 
            this.tbPassportNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbPassportNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbPassportNo.Enabled = false;
            this.tbPassportNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbPassportNo.Location = new System.Drawing.Point(2, 77);
            this.tbPassportNo.Margin = new System.Windows.Forms.Padding(2);
            this.tbPassportNo.Name = "tbPassportNo";
            this.tbPassportNo.Size = new System.Drawing.Size(94, 23);
            this.tbPassportNo.TabIndex = 26;
            // 
            // label25
            // 
            this.label25.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(2, 212);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(61, 13);
            this.label25.TabIndex = 40;
            this.label25.Text = "Birth Place:";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(2, 62);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(68, 13);
            this.label16.TabIndex = 25;
            this.label16.Text = "Passport No:";
            // 
            // tbBirthPlace
            // 
            this.tbBirthPlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbBirthPlace.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbBirthPlace.Enabled = false;
            this.tbBirthPlace.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbBirthPlace.Location = new System.Drawing.Point(2, 234);
            this.tbBirthPlace.Margin = new System.Windows.Forms.Padding(2);
            this.tbBirthPlace.Name = "tbBirthPlace";
            this.tbBirthPlace.Size = new System.Drawing.Size(94, 23);
            this.tbBirthPlace.TabIndex = 41;
            // 
            // tbFullname
            // 
            this.tbFullname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbFullname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbFullname.Enabled = false;
            this.tbFullname.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbFullname.Location = new System.Drawing.Point(2, 27);
            this.tbFullname.Margin = new System.Windows.Forms.Padding(2);
            this.tbFullname.Name = "tbFullname";
            this.tbFullname.Size = new System.Drawing.Size(94, 23);
            this.tbFullname.TabIndex = 24;
            // 
            // label24
            // 
            this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(112, 12);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(65, 13);
            this.label24.TabIndex = 31;
            this.label24.Text = "Issue Place:";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(2, 12);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 23;
            this.label15.Text = "Full Name:";
            // 
            // tbIssuePlace
            // 
            this.tbIssuePlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbIssuePlace.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbIssuePlace.Enabled = false;
            this.tbIssuePlace.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbIssuePlace.Location = new System.Drawing.Point(112, 27);
            this.tbIssuePlace.Margin = new System.Windows.Forms.Padding(2);
            this.tbIssuePlace.Name = "tbIssuePlace";
            this.tbIssuePlace.Size = new System.Drawing.Size(88, 23);
            this.tbIssuePlace.TabIndex = 30;
            // 
            // label23
            // 
            this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(112, 62);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(35, 13);
            this.label23.TabIndex = 35;
            this.label23.Text = "Email:";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbEmail
            // 
            this.tbEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbEmail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbEmail.Enabled = false;
            this.tbEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbEmail.Location = new System.Drawing.Point(112, 77);
            this.tbEmail.Margin = new System.Windows.Forms.Padding(2);
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(88, 23);
            this.tbEmail.TabIndex = 33;
            // 
            // tbAppStatus
            // 
            this.tbAppStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbAppStatus.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbAppStatus.Enabled = false;
            this.tbAppStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbAppStatus.Location = new System.Drawing.Point(112, 234);
            this.tbAppStatus.Margin = new System.Windows.Forms.Padding(2);
            this.tbAppStatus.Name = "tbAppStatus";
            this.tbAppStatus.Size = new System.Drawing.Size(88, 23);
            this.tbAppStatus.TabIndex = 39;
            // 
            // label21
            // 
            this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(112, 200);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(59, 25);
            this.label21.TabIndex = 38;
            this.label21.Text = "Application Status:";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbAppType
            // 
            this.tbAppType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbAppType.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbAppType.Enabled = false;
            this.tbAppType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbAppType.Location = new System.Drawing.Point(112, 177);
            this.tbAppType.Margin = new System.Windows.Forms.Padding(2);
            this.tbAppType.Name = "tbAppType";
            this.tbAppType.Size = new System.Drawing.Size(88, 23);
            this.tbAppType.TabIndex = 37;
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(112, 150);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(59, 25);
            this.label22.TabIndex = 36;
            this.label22.Text = "Application Type:";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(112, 112);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 44;
            this.label4.Text = "Application No:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbAppNo
            // 
            this.tbAppNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbAppNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbAppNo.Enabled = false;
            this.tbAppNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbAppNo.Location = new System.Drawing.Point(112, 127);
            this.tbAppNo.Margin = new System.Windows.Forms.Padding(2);
            this.tbAppNo.Name = "tbAppNo";
            this.tbAppNo.Size = new System.Drawing.Size(88, 23);
            this.tbAppNo.TabIndex = 45;
            // 
            // label26
            // 
            this.label26.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(212, 12);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(87, 13);
            this.label26.TabIndex = 42;
            this.label26.Text = "Visa Expiry Date:";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbAppExpDate
            // 
            this.tbAppExpDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbAppExpDate.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbAppExpDate.Enabled = false;
            this.tbAppExpDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbAppExpDate.Location = new System.Drawing.Point(212, 27);
            this.tbAppExpDate.Margin = new System.Windows.Forms.Padding(2);
            this.tbAppExpDate.Name = "tbAppExpDate";
            this.tbAppExpDate.Size = new System.Drawing.Size(86, 23);
            this.tbAppExpDate.TabIndex = 43;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(212, 62);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 46;
            this.label6.Text = "Company:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbCompany
            // 
            this.tbCompany.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbCompany.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbCompany.Enabled = false;
            this.tbCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbCompany.Location = new System.Drawing.Point(212, 77);
            this.tbCompany.Margin = new System.Windows.Forms.Padding(2);
            this.tbCompany.Name = "tbCompany";
            this.tbCompany.Size = new System.Drawing.Size(86, 23);
            this.tbCompany.TabIndex = 47;
            // 
            // label20
            // 
            this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(16, 104);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(85, 13);
            this.label20.TabIndex = 29;
            this.label20.Text = "Date Of Birth:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbDateOfBirth
            // 
            this.tbDateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbDateOfBirth.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbDateOfBirth.Enabled = false;
            this.tbDateOfBirth.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbDateOfBirth.Location = new System.Drawing.Point(102, 98);
            this.tbDateOfBirth.Margin = new System.Windows.Forms.Padding(2);
            this.tbDateOfBirth.Name = "tbDateOfBirth";
            this.tbDateOfBirth.Size = new System.Drawing.Size(128, 23);
            this.tbDateOfBirth.TabIndex = 28;
            // 
            // pbPhoto
            // 
            this.pbPhoto.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pbPhoto.Image = ((System.Drawing.Image)(resources.GetObject("pbPhoto.Image")));
            this.pbPhoto.Location = new System.Drawing.Point(82, 15);
            this.pbPhoto.Margin = new System.Windows.Forms.Padding(2);
            this.pbPhoto.Name = "pbPhoto";
            this.pbPhoto.Size = new System.Drawing.Size(84, 77);
            this.pbPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPhoto.TabIndex = 10;
            this.pbPhoto.TabStop = false;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(16, 136);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(71, 13);
            this.label19.TabIndex = 27;
            this.label19.Text = "Nationality:";
            // 
            // tbNationality
            // 
            this.tbNationality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbNationality.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbNationality.Enabled = false;
            this.tbNationality.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbNationality.Location = new System.Drawing.Point(102, 134);
            this.tbNationality.Margin = new System.Windows.Forms.Padding(2);
            this.tbNationality.Name = "tbNationality";
            this.tbNationality.Size = new System.Drawing.Size(128, 23);
            this.tbNationality.TabIndex = 34;
            // 
            // m_txtStatusMessage
            // 
            this.m_txtStatusMessage.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.m_txtStatusMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_txtStatusMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_txtStatusMessage.Location = new System.Drawing.Point(0, 457);
            this.m_txtStatusMessage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_txtStatusMessage.Name = "m_txtStatusMessage";
            this.m_txtStatusMessage.Size = new System.Drawing.Size(904, 19);
            this.m_txtStatusMessage.TabIndex = 18;
            this.m_txtStatusMessage.Text = "Ready....";
            this.m_txtStatusMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panel1.Controls.Add(this.pbRightLittle);
            this.panel1.Controls.Add(this.pbRightRing);
            this.panel1.Controls.Add(this.pbRightMiddle);
            this.panel1.Controls.Add(this.pbRightIndex);
            this.panel1.Controls.Add(this.pbRightThumb);
            this.panel1.Controls.Add(this.pbLeftThumb);
            this.panel1.Controls.Add(this.pbLeftLittle);
            this.panel1.Controls.Add(this.pbLeftRing);
            this.panel1.Controls.Add(this.pbLeftMiddle);
            this.panel1.Controls.Add(this.pbLeftIndex);
            this.panel1.Controls.Add(this.label27);
            this.panel1.Controls.Add(this.label28);
            this.panel1.Controls.Add(this.label29);
            this.panel1.Controls.Add(this.label30);
            this.panel1.Controls.Add(this.label31);
            this.panel1.Controls.Add(this.dataGridView5);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.dataGridView4);
            this.panel1.Controls.Add(this.dataGridView3);
            this.panel1.Controls.Add(this.tbNationality);
            this.panel1.Controls.Add(this.pbPhoto);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.tbDateOfBirth);
            this.panel1.Location = new System.Drawing.Point(12, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(257, 421);
            this.panel1.TabIndex = 19;
            // 
            // pbRightLittle
            // 
            this.pbRightLittle.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbRightLittle.Location = new System.Drawing.Point(189, 342);
            this.pbRightLittle.Name = "pbRightLittle";
            this.pbRightLittle.Size = new System.Drawing.Size(51, 46);
            this.pbRightLittle.TabIndex = 64;
            this.pbRightLittle.TabStop = false;
            // 
            // pbRightRing
            // 
            this.pbRightRing.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbRightRing.Location = new System.Drawing.Point(133, 342);
            this.pbRightRing.Name = "pbRightRing";
            this.pbRightRing.Size = new System.Drawing.Size(51, 46);
            this.pbRightRing.TabIndex = 63;
            this.pbRightRing.TabStop = false;
            // 
            // pbRightMiddle
            // 
            this.pbRightMiddle.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbRightMiddle.Location = new System.Drawing.Point(76, 344);
            this.pbRightMiddle.Name = "pbRightMiddle";
            this.pbRightMiddle.Size = new System.Drawing.Size(51, 46);
            this.pbRightMiddle.TabIndex = 62;
            this.pbRightMiddle.TabStop = false;
            // 
            // pbRightIndex
            // 
            this.pbRightIndex.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbRightIndex.Location = new System.Drawing.Point(19, 344);
            this.pbRightIndex.Name = "pbRightIndex";
            this.pbRightIndex.Size = new System.Drawing.Size(51, 46);
            this.pbRightIndex.TabIndex = 61;
            this.pbRightIndex.TabStop = false;
            // 
            // pbRightThumb
            // 
            this.pbRightThumb.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbRightThumb.Location = new System.Drawing.Point(132, 266);
            this.pbRightThumb.Name = "pbRightThumb";
            this.pbRightThumb.Size = new System.Drawing.Size(51, 46);
            this.pbRightThumb.TabIndex = 60;
            this.pbRightThumb.TabStop = false;
            // 
            // pbLeftThumb
            // 
            this.pbLeftThumb.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbLeftThumb.Location = new System.Drawing.Point(62, 264);
            this.pbLeftThumb.Name = "pbLeftThumb";
            this.pbLeftThumb.Size = new System.Drawing.Size(51, 46);
            this.pbLeftThumb.TabIndex = 59;
            this.pbLeftThumb.TabStop = false;
            // 
            // pbLeftLittle
            // 
            this.pbLeftLittle.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbLeftLittle.Location = new System.Drawing.Point(184, 179);
            this.pbLeftLittle.Name = "pbLeftLittle";
            this.pbLeftLittle.Size = new System.Drawing.Size(51, 46);
            this.pbLeftLittle.TabIndex = 58;
            this.pbLeftLittle.TabStop = false;
            // 
            // pbLeftRing
            // 
            this.pbLeftRing.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbLeftRing.Location = new System.Drawing.Point(127, 179);
            this.pbLeftRing.Name = "pbLeftRing";
            this.pbLeftRing.Size = new System.Drawing.Size(51, 46);
            this.pbLeftRing.TabIndex = 57;
            this.pbLeftRing.TabStop = false;
            // 
            // pbLeftMiddle
            // 
            this.pbLeftMiddle.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbLeftMiddle.Location = new System.Drawing.Point(70, 179);
            this.pbLeftMiddle.Name = "pbLeftMiddle";
            this.pbLeftMiddle.Size = new System.Drawing.Size(51, 46);
            this.pbLeftMiddle.TabIndex = 56;
            this.pbLeftMiddle.TabStop = false;
            // 
            // pbLeftIndex
            // 
            this.pbLeftIndex.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbLeftIndex.Location = new System.Drawing.Point(13, 179);
            this.pbLeftIndex.Name = "pbLeftIndex";
            this.pbLeftIndex.Size = new System.Drawing.Size(51, 46);
            this.pbLeftIndex.TabIndex = 55;
            this.pbLeftIndex.TabStop = false;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(191, 391);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(49, 7);
            this.label27.TabIndex = 54;
            this.label27.Text = "Right little";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(137, 391);
            this.label28.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(48, 7);
            this.label28.TabIndex = 53;
            this.label28.Text = "Right Ring";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(79, 391);
            this.label29.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(57, 7);
            this.label29.TabIndex = 52;
            this.label29.Text = "Right Middle";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(21, 391);
            this.label30.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(52, 7);
            this.label30.TabIndex = 51;
            this.label30.Text = "Right Index";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(92, 328);
            this.label31.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(82, 13);
            this.label31.TabIndex = 50;
            this.label31.Text = "Right Fingers";
            // 
            // dataGridView5
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView5.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridView5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView5.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridView5.Location = new System.Drawing.Point(13, 324);
            this.dataGridView5.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView5.Name = "dataGridView5";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView5.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridView5.RowHeadersWidth = 62;
            this.dataGridView5.RowTemplate.Height = 28;
            this.dataGridView5.Size = new System.Drawing.Size(233, 79);
            this.dataGridView5.TabIndex = 45;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(130, 311);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(56, 7);
            this.label14.TabIndex = 34;
            this.label14.Text = "Right Thumb";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(187, 228);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 7);
            this.label11.TabIndex = 44;
            this.label11.Text = "Left little";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(68, 311);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(50, 7);
            this.label17.TabIndex = 33;
            this.label17.Text = "Left Thumb";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(74, 228);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 7);
            this.label10.TabIndex = 43;
            this.label10.Text = "Left Middle";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(96, 248);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(51, 13);
            this.label18.TabIndex = 32;
            this.label18.Text = "Thumbs";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(132, 228);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 7);
            this.label9.TabIndex = 42;
            this.label9.Text = "Left Ring";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(17, 228);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 7);
            this.label12.TabIndex = 41;
            this.label12.Text = "Left Index";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(92, 163);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 13);
            this.label13.TabIndex = 40;
            this.label13.Text = "Left Fingers";
            // 
            // dataGridView4
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView4.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView4.DefaultCellStyle = dataGridViewCellStyle14;
            this.dataGridView4.Location = new System.Drawing.Point(52, 245);
            this.dataGridView4.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView4.Name = "dataGridView4";
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView4.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dataGridView4.RowHeadersWidth = 62;
            this.dataGridView4.RowTemplate.Height = 28;
            this.dataGridView4.Size = new System.Drawing.Size(137, 75);
            this.dataGridView4.TabIndex = 29;
            // 
            // dataGridView3
            // 
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView3.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle16;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView3.DefaultCellStyle = dataGridViewCellStyle17;
            this.dataGridView3.Location = new System.Drawing.Point(6, 159);
            this.dataGridView3.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView3.Name = "dataGridView3";
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView3.RowHeadersDefaultCellStyle = dataGridViewCellStyle18;
            this.dataGridView3.RowHeadersWidth = 62;
            this.dataGridView3.RowTemplate.Height = 28;
            this.dataGridView3.Size = new System.Drawing.Size(234, 81);
            this.dataGridView3.TabIndex = 35;
            // 
            // nextBtn
            // 
            this.nextBtn.Location = new System.Drawing.Point(772, 423);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Size = new System.Drawing.Size(87, 29);
            this.nextBtn.TabIndex = 20;
            this.nextBtn.Text = "NEXT";
            this.nextBtn.UseVisualStyleBackColor = true;
            this.nextBtn.Click += new System.EventHandler(this.nextBtn_Click);
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Location = new System.Drawing.Point(767, 209);
            this.panel7.Margin = new System.Windows.Forms.Padding(2);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(41, 33);
            this.panel7.TabIndex = 36;
            // 
            // m_btnCaptureStop
            // 
            this.m_btnCaptureStop.BackColor = System.Drawing.Color.Red;
            this.m_btnCaptureStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCaptureStop.Location = new System.Drawing.Point(772, 357);
            this.m_btnCaptureStop.Margin = new System.Windows.Forms.Padding(2);
            this.m_btnCaptureStop.Name = "m_btnCaptureStop";
            this.m_btnCaptureStop.Size = new System.Drawing.Size(88, 30);
            this.m_btnCaptureStop.TabIndex = 65;
            this.m_btnCaptureStop.Text = "CANCEL";
            this.m_btnCaptureStop.UseVisualStyleBackColor = false;
            this.m_btnCaptureStop.Click += new System.EventHandler(this.m_btnCaptureStop_Click);
            // 
            // m_btnCaptureStart
            // 
            this.m_btnCaptureStart.BackColor = System.Drawing.SystemColors.ControlLight;
            this.m_btnCaptureStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCaptureStart.Location = new System.Drawing.Point(771, 320);
            this.m_btnCaptureStart.Margin = new System.Windows.Forms.Padding(2);
            this.m_btnCaptureStart.Name = "m_btnCaptureStart";
            this.m_btnCaptureStart.Size = new System.Drawing.Size(88, 30);
            this.m_btnCaptureStart.TabIndex = 66;
            this.m_btnCaptureStart.Text = "START";
            this.m_btnCaptureStart.UseVisualStyleBackColor = false;
            this.m_btnCaptureStart.Click += new System.EventHandler(this.m_btnCaptureStart_Click);
            // 
            // SDKMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(904, 476);
            this.Controls.Add(this.m_btnCaptureStart);
            this.Controls.Add(this.m_btnCaptureStop);
            this.Controls.Add(this.nextBtn);
            this.Controls.Add(this.m_picIBLogo);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_txtStatusMessage);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.m_btnVerify);
            this.Controls.Add(this.m_FrameImage);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.panel7);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SDKMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "i-Immigration System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SDKMainForm_FormClosing);
            this.Load += new System.EventHandler(this.SDKMainForm_Load);
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_picScanner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_picIBLogo)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightLittle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightRing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightMiddle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightThumb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftThumb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftLittle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftRing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftMiddle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
         
        private System.Windows.Forms.Label m_FrameImage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Timer Timer_StatusFingerQuality;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.PictureBox m_picScanner;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDocumentNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbCountry;
        private System.Windows.Forms.Button m_btnVerify;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.PictureBox m_picIBLogo;
        private System.Windows.Forms.ComboBox cbDocType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbFingerPrinterLater;
        private System.Windows.Forms.CheckBox cbDocumentOnly;
        private System.Windows.Forms.CheckBox cbFingerPrintOnly;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbFirstName;
        private System.Windows.Forms.TextBox tbLastName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox pbPhoto;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbFullname;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tbPassportNo;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tbNationality;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tbDateOfBirth;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox tbBirthPlace;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbIssuePlace;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbAppExpDate;
        private System.Windows.Forms.TextBox tbAppStatus;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox tbAppType;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbAppNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbCompany;
        private System.Windows.Forms.Label m_txtStatusMessage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button nextBtn;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.DataGridView dataGridView4;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.DataGridView dataGridView5;
        private System.Windows.Forms.PictureBox pbRightLittle;
        private System.Windows.Forms.PictureBox pbRightRing;
        private System.Windows.Forms.PictureBox pbRightMiddle;
        private System.Windows.Forms.PictureBox pbRightIndex;
        private System.Windows.Forms.PictureBox pbRightThumb;
        private System.Windows.Forms.PictureBox pbLeftThumb;
        private System.Windows.Forms.PictureBox pbLeftLittle;
        private System.Windows.Forms.PictureBox pbLeftRing;
        private System.Windows.Forms.PictureBox pbLeftMiddle;
        private System.Windows.Forms.PictureBox pbLeftIndex;
        private System.Windows.Forms.Button m_btnCaptureStop;
        private System.Windows.Forms.Button m_btnCaptureStart;
    }
}

