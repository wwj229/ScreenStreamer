﻿namespace ScreenStreamer.WinForms.App
{
    partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.videoSourceDetailsButton = new System.Windows.Forms.Button();
			this.videoSourceEnableCheckBox = new System.Windows.Forms.CheckBox();
			this.audioSourceEnableCheckBox = new System.Windows.Forms.CheckBox();
			this.audioSourceComboBox = new System.Windows.Forms.ComboBox();
			this.audioSourceDetailsButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.videoSourceUpdateButton = new System.Windows.Forms.Button();
			this.audioSourceUpdateButton = new System.Windows.Forms.Button();
			this.lineSeparator = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.mainControlsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this.exitButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.switchStreamingStateButton = new System.Windows.Forms.Button();
			this.infoButton = new System.Windows.Forms.Button();
			this.videoSourceSettingsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.videoSourceComboBox = new System.Windows.Forms.ComboBox();
			this.audioSourceSettingsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.networkSettingsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			this.streamNameTextBox = new System.Windows.Forms.TextBox();
			this.networkSettingsButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.networkSeparator = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.captureStatusDescriptionLabel = new System.Windows.Forms.Label();
			this.captureStatusLabel = new System.Windows.Forms.Label();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.mainControlsLayoutPanel.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.videoSourceSettingsLayoutPanel.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.audioSourceSettingsLayoutPanel.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.networkSettingsLayoutPanel.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			this.panel3.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// videoSourceDetailsButton
			// 
			this.videoSourceDetailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.videoSourceDetailsButton.AutoSize = true;
			this.videoSourceDetailsButton.BackColor = System.Drawing.SystemColors.ControlLight;
			this.videoSourceDetailsButton.FlatAppearance.BorderSize = 0;
			this.videoSourceDetailsButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this.videoSourceDetailsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.videoSourceDetailsButton.Location = new System.Drawing.Point(399, 2);
			this.videoSourceDetailsButton.Margin = new System.Windows.Forms.Padding(2);
			this.videoSourceDetailsButton.Name = "videoSourceDetailsButton";
			this.videoSourceDetailsButton.Size = new System.Drawing.Size(101, 28);
			this.videoSourceDetailsButton.TabIndex = 78;
			this.videoSourceDetailsButton.Text = "Details...";
			this.videoSourceDetailsButton.UseVisualStyleBackColor = false;
			this.videoSourceDetailsButton.Click += new System.EventHandler(this.videoSourceDetailsButton_Click);
			// 
			// videoSourceEnableCheckBox
			// 
			this.videoSourceEnableCheckBox.AutoSize = true;
			this.videoSourceEnableCheckBox.Checked = true;
			this.videoSourceEnableCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.videoSourceEnableCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
			this.videoSourceEnableCheckBox.Location = new System.Drawing.Point(2, 2);
			this.videoSourceEnableCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.videoSourceEnableCheckBox.Name = "videoSourceEnableCheckBox";
			this.videoSourceEnableCheckBox.Size = new System.Drawing.Size(159, 21);
			this.videoSourceEnableCheckBox.TabIndex = 74;
			this.videoSourceEnableCheckBox.Text = "Enable video source";
			this.videoSourceEnableCheckBox.UseVisualStyleBackColor = true;
			this.videoSourceEnableCheckBox.CheckedChanged += new System.EventHandler(this.videoEnabledCheckBox_CheckedChanged);
			// 
			// audioSourceEnableCheckBox
			// 
			this.audioSourceEnableCheckBox.AutoSize = true;
			this.audioSourceEnableCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
			this.audioSourceEnableCheckBox.Location = new System.Drawing.Point(2, 2);
			this.audioSourceEnableCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.audioSourceEnableCheckBox.Name = "audioSourceEnableCheckBox";
			this.audioSourceEnableCheckBox.Size = new System.Drawing.Size(160, 21);
			this.audioSourceEnableCheckBox.TabIndex = 86;
			this.audioSourceEnableCheckBox.Text = "Enable audio source";
			this.audioSourceEnableCheckBox.UseVisualStyleBackColor = true;
			this.audioSourceEnableCheckBox.CheckedChanged += new System.EventHandler(this.audioEnabledCheckBox_CheckedChanged);
			// 
			// audioSourceComboBox
			// 
			this.audioSourceComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.audioSourceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.audioSourceComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.audioSourceComboBox.ForeColor = System.Drawing.SystemColors.ControlText;
			this.audioSourceComboBox.FormattingEnabled = true;
			this.audioSourceComboBox.Location = new System.Drawing.Point(2, 2);
			this.audioSourceComboBox.Margin = new System.Windows.Forms.Padding(2);
			this.audioSourceComboBox.Name = "audioSourceComboBox";
			this.audioSourceComboBox.Size = new System.Drawing.Size(471, 25);
			this.audioSourceComboBox.TabIndex = 82;
			// 
			// audioSourceDetailsButton
			// 
			this.audioSourceDetailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.audioSourceDetailsButton.AutoSize = true;
			this.audioSourceDetailsButton.BackColor = System.Drawing.SystemColors.ControlLight;
			this.audioSourceDetailsButton.FlatAppearance.BorderSize = 0;
			this.audioSourceDetailsButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this.audioSourceDetailsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.audioSourceDetailsButton.Location = new System.Drawing.Point(399, 2);
			this.audioSourceDetailsButton.Margin = new System.Windows.Forms.Padding(2);
			this.audioSourceDetailsButton.Name = "audioSourceDetailsButton";
			this.audioSourceDetailsButton.Size = new System.Drawing.Size(101, 28);
			this.audioSourceDetailsButton.TabIndex = 88;
			this.audioSourceDetailsButton.Text = "Details...";
			this.audioSourceDetailsButton.UseVisualStyleBackColor = false;
			this.audioSourceDetailsButton.Click += new System.EventHandler(this.audioSourceDetailsButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label2.Location = new System.Drawing.Point(2, 0);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(148, 17);
			this.label2.TabIndex = 92;
			this.label2.Text = "Audio Source Settings";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label3.Location = new System.Drawing.Point(2, 0);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(148, 17);
			this.label3.TabIndex = 93;
			this.label3.Text = "Video Source Settings";
			// 
			// videoSourceUpdateButton
			// 
			this.videoSourceUpdateButton.AutoSize = true;
			this.videoSourceUpdateButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.videoSourceUpdateButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.videoSourceUpdateButton.Image = global::ScreenStreamer.WinForms.App.Properties.Resources.baseline_cached_black_18dp;
			this.videoSourceUpdateButton.Location = new System.Drawing.Point(476, 1);
			this.videoSourceUpdateButton.Margin = new System.Windows.Forms.Padding(1);
			this.videoSourceUpdateButton.Name = "videoSourceUpdateButton";
			this.videoSourceUpdateButton.Size = new System.Drawing.Size(24, 27);
			this.videoSourceUpdateButton.TabIndex = 72;
			this.videoSourceUpdateButton.UseVisualStyleBackColor = true;
			this.videoSourceUpdateButton.Click += new System.EventHandler(this.videoSourceUpdateButton_Click);
			// 
			// audioSourceUpdateButton
			// 
			this.audioSourceUpdateButton.AutoSize = true;
			this.audioSourceUpdateButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.audioSourceUpdateButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.audioSourceUpdateButton.Image = global::ScreenStreamer.WinForms.App.Properties.Resources.baseline_cached_black_18dp;
			this.audioSourceUpdateButton.Location = new System.Drawing.Point(476, 1);
			this.audioSourceUpdateButton.Margin = new System.Windows.Forms.Padding(1);
			this.audioSourceUpdateButton.Name = "audioSourceUpdateButton";
			this.audioSourceUpdateButton.Size = new System.Drawing.Size(24, 27);
			this.audioSourceUpdateButton.TabIndex = 84;
			this.audioSourceUpdateButton.UseVisualStyleBackColor = true;
			this.audioSourceUpdateButton.Click += new System.EventHandler(this.audioSourceUpdateButton_Click);
			// 
			// lineSeparator
			// 
			this.lineSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.mainControlsLayoutPanel.SetColumnSpan(this.lineSeparator, 2);
			this.lineSeparator.Dock = System.Windows.Forms.DockStyle.Top;
			this.lineSeparator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.lineSeparator.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lineSeparator.Location = new System.Drawing.Point(2, 0);
			this.lineSeparator.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lineSeparator.Name = "lineSeparator";
			this.lineSeparator.Size = new System.Drawing.Size(528, 2);
			this.lineSeparator.TabIndex = 105;
			// 
			// label5
			// 
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label5.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label5.Location = new System.Drawing.Point(154, 15);
			this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(353, 2);
			this.label5.TabIndex = 106;
			// 
			// label6
			// 
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label6.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label6.Location = new System.Drawing.Point(154, 15);
			this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(353, 2);
			this.label6.TabIndex = 107;
			// 
			// mainControlsLayoutPanel
			// 
			this.mainControlsLayoutPanel.ColumnCount = 2;
			this.mainControlsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainControlsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.mainControlsLayoutPanel.Controls.Add(this.flowLayoutPanel2, 1, 1);
			this.mainControlsLayoutPanel.Controls.Add(this.flowLayoutPanel1, 0, 1);
			this.mainControlsLayoutPanel.Controls.Add(this.lineSeparator, 0, 0);
			this.mainControlsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.mainControlsLayoutPanel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.mainControlsLayoutPanel.Location = new System.Drawing.Point(0, 474);
			this.mainControlsLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
			this.mainControlsLayoutPanel.Name = "mainControlsLayoutPanel";
			this.mainControlsLayoutPanel.RowCount = 2;
			this.mainControlsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainControlsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainControlsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
			this.mainControlsLayoutPanel.Size = new System.Drawing.Size(532, 58);
			this.mainControlsLayoutPanel.TabIndex = 109;
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel2.AutoSize = true;
			this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel2.Controls.Add(this.exitButton);
			this.flowLayoutPanel2.Location = new System.Drawing.Point(394, 6);
			this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(2, 4, 9, 2);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(129, 34);
			this.flowLayoutPanel2.TabIndex = 110;
			// 
			// exitButton
			// 
			this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.exitButton.AutoSize = true;
			this.exitButton.BackColor = System.Drawing.SystemColors.ControlLight;
			this.exitButton.FlatAppearance.BorderSize = 0;
			this.exitButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this.exitButton.Location = new System.Drawing.Point(2, 2);
			this.exitButton.Margin = new System.Windows.Forms.Padding(2);
			this.exitButton.Name = "exitButton";
			this.exitButton.Size = new System.Drawing.Size(125, 30);
			this.exitButton.TabIndex = 100;
			this.exitButton.Text = "Exit";
			this.exitButton.UseVisualStyleBackColor = false;
			this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this.switchStreamingStateButton);
			this.flowLayoutPanel1.Controls.Add(this.infoButton);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(18, 6);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(18, 4, 2, 2);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(288, 34);
			this.flowLayoutPanel1.TabIndex = 108;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// switchStreamingStateButton
			// 
			this.switchStreamingStateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.switchStreamingStateButton.AutoSize = true;
			this.switchStreamingStateButton.BackColor = System.Drawing.SystemColors.ControlLight;
			this.switchStreamingStateButton.FlatAppearance.BorderSize = 0;
			this.switchStreamingStateButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this.switchStreamingStateButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.switchStreamingStateButton.Location = new System.Drawing.Point(2, 2);
			this.switchStreamingStateButton.Margin = new System.Windows.Forms.Padding(2);
			this.switchStreamingStateButton.Name = "switchStreamingStateButton";
			this.switchStreamingStateButton.Size = new System.Drawing.Size(180, 30);
			this.switchStreamingStateButton.TabIndex = 92;
			this.switchStreamingStateButton.Text = "Start Streaming";
			this.switchStreamingStateButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.switchStreamingStateButton.UseVisualStyleBackColor = false;
			this.switchStreamingStateButton.Click += new System.EventHandler(this.switchStreamingStateButton_Click);
			// 
			// infoButton
			// 
			this.infoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.infoButton.AutoSize = true;
			this.infoButton.BackColor = System.Drawing.SystemColors.ControlLight;
			this.infoButton.FlatAppearance.BorderSize = 0;
			this.infoButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this.infoButton.Location = new System.Drawing.Point(186, 2);
			this.infoButton.Margin = new System.Windows.Forms.Padding(2);
			this.infoButton.Name = "infoButton";
			this.infoButton.Size = new System.Drawing.Size(100, 30);
			this.infoButton.TabIndex = 94;
			this.infoButton.Text = "Info";
			this.infoButton.UseVisualStyleBackColor = false;
			this.infoButton.Visible = false;
			this.infoButton.Click += new System.EventHandler(this.infoButton_Click);
			// 
			// videoSourceSettingsLayoutPanel
			// 
			this.videoSourceSettingsLayoutPanel.AutoSize = true;
			this.videoSourceSettingsLayoutPanel.ColumnCount = 2;
			this.videoSourceSettingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.videoSourceSettingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.videoSourceSettingsLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 1);
			this.videoSourceSettingsLayoutPanel.Controls.Add(this.label3, 0, 0);
			this.videoSourceSettingsLayoutPanel.Controls.Add(this.label6, 1, 0);
			this.videoSourceSettingsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.videoSourceSettingsLayoutPanel.Location = new System.Drawing.Point(12, 140);
			this.videoSourceSettingsLayoutPanel.Margin = new System.Windows.Forms.Padding(2, 15, 2, 2);
			this.videoSourceSettingsLayoutPanel.Name = "videoSourceSettingsLayoutPanel";
			this.videoSourceSettingsLayoutPanel.RowCount = 2;
			this.videoSourceSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.videoSourceSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.videoSourceSettingsLayoutPanel.Size = new System.Drawing.Size(509, 111);
			this.videoSourceSettingsLayoutPanel.TabIndex = 111;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.videoSourceSettingsLayoutPanel.SetColumnSpan(this.tableLayoutPanel1, 2);
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.videoSourceComboBox, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.videoSourceUpdateButton, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 25);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6, 8, 2, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(501, 84);
			this.tableLayoutPanel1.TabIndex = 113;
			// 
			// panel1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
			this.panel1.Controls.Add(this.videoSourceEnableCheckBox);
			this.panel1.Controls.Add(this.videoSourceDetailsButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 31);
			this.panel1.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(501, 51);
			this.panel1.TabIndex = 112;
			// 
			// videoSourceComboBox
			// 
			this.videoSourceComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.videoSourceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.videoSourceComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.videoSourceComboBox.ForeColor = System.Drawing.SystemColors.ControlText;
			this.videoSourceComboBox.FormattingEnabled = true;
			this.videoSourceComboBox.Location = new System.Drawing.Point(2, 2);
			this.videoSourceComboBox.Margin = new System.Windows.Forms.Padding(2);
			this.videoSourceComboBox.Name = "videoSourceComboBox";
			this.videoSourceComboBox.Size = new System.Drawing.Size(471, 25);
			this.videoSourceComboBox.TabIndex = 70;
			// 
			// audioSourceSettingsLayoutPanel
			// 
			this.audioSourceSettingsLayoutPanel.AutoSize = true;
			this.audioSourceSettingsLayoutPanel.ColumnCount = 2;
			this.audioSourceSettingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.audioSourceSettingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.audioSourceSettingsLayoutPanel.Controls.Add(this.tableLayoutPanel3, 0, 1);
			this.audioSourceSettingsLayoutPanel.Controls.Add(this.label2, 0, 0);
			this.audioSourceSettingsLayoutPanel.Controls.Add(this.label5, 1, 0);
			this.audioSourceSettingsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.audioSourceSettingsLayoutPanel.Location = new System.Drawing.Point(12, 268);
			this.audioSourceSettingsLayoutPanel.Margin = new System.Windows.Forms.Padding(2, 15, 2, 2);
			this.audioSourceSettingsLayoutPanel.Name = "audioSourceSettingsLayoutPanel";
			this.audioSourceSettingsLayoutPanel.RowCount = 2;
			this.audioSourceSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.audioSourceSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.audioSourceSettingsLayoutPanel.Size = new System.Drawing.Size(509, 104);
			this.audioSourceSettingsLayoutPanel.TabIndex = 112;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.audioSourceSettingsLayoutPanel.SetColumnSpan(this.tableLayoutPanel3, 2);
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this.panel2, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.audioSourceComboBox, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.audioSourceUpdateButton, 1, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 25);
			this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(6, 8, 2, 2);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(501, 77);
			this.tableLayoutPanel3.TabIndex = 114;
			// 
			// panel2
			// 
			this.tableLayoutPanel3.SetColumnSpan(this.panel2, 2);
			this.panel2.Controls.Add(this.audioSourceDetailsButton);
			this.panel2.Controls.Add(this.audioSourceEnableCheckBox);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 31);
			this.panel2.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(501, 44);
			this.panel2.TabIndex = 113;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.audioSourceSettingsLayoutPanel, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.networkSettingsLayoutPanel, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.videoSourceSettingsLayoutPanel, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.panel3, 0, 3);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(10, 15, 9, 0);
			this.tableLayoutPanel2.RowCount = 4;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(532, 474);
			this.tableLayoutPanel2.TabIndex = 113;
			// 
			// networkSettingsLayoutPanel
			// 
			this.networkSettingsLayoutPanel.AutoSize = true;
			this.networkSettingsLayoutPanel.ColumnCount = 2;
			this.networkSettingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.networkSettingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.networkSettingsLayoutPanel.Controls.Add(this.tableLayoutPanel5, 0, 1);
			this.networkSettingsLayoutPanel.Controls.Add(this.networkSeparator, 1, 0);
			this.networkSettingsLayoutPanel.Controls.Add(this.label8, 0, 0);
			this.networkSettingsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.networkSettingsLayoutPanel.Location = new System.Drawing.Point(12, 17);
			this.networkSettingsLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
			this.networkSettingsLayoutPanel.Name = "networkSettingsLayoutPanel";
			this.networkSettingsLayoutPanel.RowCount = 2;
			this.networkSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.networkSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.networkSettingsLayoutPanel.Size = new System.Drawing.Size(509, 106);
			this.networkSettingsLayoutPanel.TabIndex = 110;
			// 
			// tableLayoutPanel5
			// 
			this.tableLayoutPanel5.ColumnCount = 2;
			this.networkSettingsLayoutPanel.SetColumnSpan(this.tableLayoutPanel5, 2);
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.Controls.Add(this.streamNameTextBox, 1, 0);
			this.tableLayoutPanel5.Controls.Add(this.networkSettingsButton, 0, 1);
			this.tableLayoutPanel5.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel5.Location = new System.Drawing.Point(6, 25);
			this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(6, 8, 2, 2);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			this.tableLayoutPanel5.RowCount = 2;
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.Size = new System.Drawing.Size(501, 79);
			this.tableLayoutPanel5.TabIndex = 103;
			// 
			// streamNameTextBox
			// 
			this.streamNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.streamNameTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
			this.streamNameTextBox.Location = new System.Drawing.Point(104, 2);
			this.streamNameTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.streamNameTextBox.MaxLength = 100;
			this.streamNameTextBox.Name = "streamNameTextBox";
			this.streamNameTextBox.Size = new System.Drawing.Size(395, 23);
			this.streamNameTextBox.TabIndex = 66;
			this.streamNameTextBox.Text = "RAS-HOME10";
			this.streamNameTextBox.TextChanged += new System.EventHandler(this.streamNameTextBox_TextChanged);
			// 
			// networkSettingsButton
			// 
			this.networkSettingsButton.AutoSize = true;
			this.networkSettingsButton.BackColor = System.Drawing.SystemColors.ControlLight;
			this.tableLayoutPanel5.SetColumnSpan(this.networkSettingsButton, 2);
			this.networkSettingsButton.FlatAppearance.BorderSize = 0;
			this.networkSettingsButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this.networkSettingsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.networkSettingsButton.Location = new System.Drawing.Point(2, 29);
			this.networkSettingsButton.Margin = new System.Windows.Forms.Padding(2);
			this.networkSettingsButton.Name = "networkSettingsButton";
			this.networkSettingsButton.Size = new System.Drawing.Size(144, 28);
			this.networkSettingsButton.TabIndex = 68;
			this.networkSettingsButton.Text = "More settings...";
			this.networkSettingsButton.UseVisualStyleBackColor = false;
			this.networkSettingsButton.Click += new System.EventHandler(this.networkSettingsButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label1.Location = new System.Drawing.Point(2, 0);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 27);
			this.label1.TabIndex = 102;
			this.label1.Text = "Stream Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// networkSeparator
			// 
			this.networkSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.networkSeparator.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.networkSeparator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.networkSeparator.ForeColor = System.Drawing.SystemColors.ControlText;
			this.networkSeparator.Location = new System.Drawing.Point(120, 15);
			this.networkSeparator.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.networkSeparator.MaximumSize = new System.Drawing.Size(0, 2);
			this.networkSeparator.Name = "networkSeparator";
			this.networkSeparator.Size = new System.Drawing.Size(387, 2);
			this.networkSeparator.TabIndex = 109;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label8.Location = new System.Drawing.Point(2, 0);
			this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(114, 17);
			this.label8.TabIndex = 101;
			this.label8.Text = "Network Settings";
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.tableLayoutPanel4);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.panel3.Location = new System.Drawing.Point(12, 376);
			this.panel3.Margin = new System.Windows.Forms.Padding(2);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(509, 96);
			this.panel3.TabIndex = 113;
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.AutoSize = true;
			this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel4.ColumnCount = 2;
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.Controls.Add(this.captureStatusDescriptionLabel, 1, 0);
			this.tableLayoutPanel4.Controls.Add(this.captureStatusLabel, 0, 0);
			this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 67);
			this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(12, 2, 2, 2);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this.tableLayoutPanel4.RowCount = 1;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.Size = new System.Drawing.Size(509, 29);
			this.tableLayoutPanel4.TabIndex = 3;
			// 
			// captureStatusDescriptionLabel
			// 
			this.captureStatusDescriptionLabel.AutoEllipsis = true;
			this.captureStatusDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.captureStatusDescriptionLabel.Location = new System.Drawing.Point(140, 2);
			this.captureStatusDescriptionLabel.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
			this.captureStatusDescriptionLabel.Name = "captureStatusDescriptionLabel";
			this.captureStatusDescriptionLabel.Size = new System.Drawing.Size(367, 25);
			this.captureStatusDescriptionLabel.TabIndex = 0;
			this.captureStatusDescriptionLabel.Text = "_____Capture Descriptions....";
			// 
			// captureStatusLabel
			// 
			this.captureStatusLabel.AutoSize = true;
			this.captureStatusLabel.Location = new System.Drawing.Point(14, 2);
			this.captureStatusLabel.Margin = new System.Windows.Forms.Padding(12, 2, 0, 2);
			this.captureStatusLabel.Name = "captureStatusLabel";
			this.captureStatusLabel.Size = new System.Drawing.Size(126, 17);
			this.captureStatusLabel.TabIndex = 1;
			this.captureStatusLabel.Text = "__Capture Status_";
			// 
			// contextMenu
			// 
			this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.settingToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(112, 82);
			// 
			// startToolStripMenuItem
			// 
			this.startToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.startToolStripMenuItem.Name = "startToolStripMenuItem";
			this.startToolStripMenuItem.Size = new System.Drawing.Size(111, 24);
			this.startToolStripMenuItem.Text = "Start";
			this.startToolStripMenuItem.Click += new System.EventHandler(this.switchStreamingStateButton_Click);
			// 
			// settingToolStripMenuItem
			// 
			this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
			this.settingToolStripMenuItem.Size = new System.Drawing.Size(111, 24);
			this.settingToolStripMenuItem.Text = "Open";
			this.settingToolStripMenuItem.Click += new System.EventHandler(this.settingToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(108, 6);
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Name = "exitMenuItem";
			this.exitMenuItem.Size = new System.Drawing.Size(111, 24);
			this.exitMenuItem.Text = "Exit";
			this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.contextMenu;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "ScreenStreamer";
			this.notifyIcon.Visible = true;
			this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(532, 532);
			this.Controls.Add(this.tableLayoutPanel2);
			this.Controls.Add(this.mainControlsLayoutPanel);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(539, 538);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Polywall Streamer";
			this.mainControlsLayoutPanel.ResumeLayout(false);
			this.mainControlsLayoutPanel.PerformLayout();
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.videoSourceSettingsLayoutPanel.ResumeLayout(false);
			this.videoSourceSettingsLayoutPanel.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.audioSourceSettingsLayoutPanel.ResumeLayout(false);
			this.audioSourceSettingsLayoutPanel.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.networkSettingsLayoutPanel.ResumeLayout(false);
			this.networkSettingsLayoutPanel.PerformLayout();
			this.tableLayoutPanel5.ResumeLayout(false);
			this.tableLayoutPanel5.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button videoSourceDetailsButton;
        private System.Windows.Forms.Button videoSourceUpdateButton;
        private System.Windows.Forms.CheckBox videoSourceEnableCheckBox;
        private System.Windows.Forms.CheckBox audioSourceEnableCheckBox;
        private System.Windows.Forms.ComboBox audioSourceComboBox;
        private System.Windows.Forms.Button audioSourceUpdateButton;
        private System.Windows.Forms.Button audioSourceDetailsButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox videoSourceComboBox;
        private System.Windows.Forms.Label lineSeparator;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel mainControlsLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel videoSourceSettingsLayoutPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel audioSourceSettingsLayoutPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button switchStreamingStateButton;
        private System.Windows.Forms.Button infoButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label captureStatusDescriptionLabel;
        private System.Windows.Forms.Label captureStatusLabel;
        private System.Windows.Forms.TableLayoutPanel networkSettingsLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TextBox streamNameTextBox;
        private System.Windows.Forms.Button networkSettingsButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label networkSeparator;
        private System.Windows.Forms.Label label8;



		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
		private System.Windows.Forms.NotifyIcon notifyIcon;
	}
}