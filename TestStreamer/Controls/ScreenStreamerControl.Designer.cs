﻿namespace TestStreamer.Controls
{
    partial class ScreenStreamerControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.settingPanel = new System.Windows.Forms.Panel();
            this.encoderComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.transportComboBox = new System.Windows.Forms.ComboBox();
            this.aspectRatioCheckBox = new System.Windows.Forms.CheckBox();
            this.destSizeGroupBox = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.destWidthNumeric = new System.Windows.Forms.NumericUpDown();
            this.destHeightNumeric = new System.Windows.Forms.NumericUpDown();
            this.srcRectGroupBox = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.srcTopNumeric = new System.Windows.Forms.NumericUpDown();
            this.srcLeftNumeric = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.srcRightNumeric = new System.Windows.Forms.NumericUpDown();
            this.srcBottomNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.screensUpdateButton = new System.Windows.Forms.Button();
            this.screensComboBox = new System.Windows.Forms.ComboBox();
            this.fpsNumeric = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.showMouseCheckBox = new System.Windows.Forms.CheckBox();
            this.portNumeric = new System.Windows.Forms.NumericUpDown();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.previewButton = new System.Windows.Forms.Button();
            this.snippingToolButton = new System.Windows.Forms.Button();
            this.settingPanel.SuspendLayout();
            this.destSizeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.destWidthNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.destHeightNumeric)).BeginInit();
            this.srcRectGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srcTopNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcLeftNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcRightNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcBottomNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpsNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.portNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // settingPanel
            // 
            this.settingPanel.Controls.Add(this.snippingToolButton);
            this.settingPanel.Controls.Add(this.encoderComboBox);
            this.settingPanel.Controls.Add(this.label6);
            this.settingPanel.Controls.Add(this.label5);
            this.settingPanel.Controls.Add(this.transportComboBox);
            this.settingPanel.Controls.Add(this.aspectRatioCheckBox);
            this.settingPanel.Controls.Add(this.destSizeGroupBox);
            this.settingPanel.Controls.Add(this.srcRectGroupBox);
            this.settingPanel.Controls.Add(this.label4);
            this.settingPanel.Controls.Add(this.screensUpdateButton);
            this.settingPanel.Controls.Add(this.screensComboBox);
            this.settingPanel.Controls.Add(this.fpsNumeric);
            this.settingPanel.Controls.Add(this.label2);
            this.settingPanel.Controls.Add(this.label1);
            this.settingPanel.Controls.Add(this.label3);
            this.settingPanel.Controls.Add(this.showMouseCheckBox);
            this.settingPanel.Controls.Add(this.portNumeric);
            this.settingPanel.Controls.Add(this.addressTextBox);
            this.settingPanel.Location = new System.Drawing.Point(3, 3);
            this.settingPanel.Name = "settingPanel";
            this.settingPanel.Size = new System.Drawing.Size(473, 469);
            this.settingPanel.TabIndex = 20;
            // 
            // encoderComboBox
            // 
            this.encoderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.encoderComboBox.Enabled = false;
            this.encoderComboBox.FormattingEnabled = true;
            this.encoderComboBox.Location = new System.Drawing.Point(85, 373);
            this.encoderComboBox.Name = "encoderComboBox";
            this.encoderComboBox.Size = new System.Drawing.Size(107, 24);
            this.encoderComboBox.TabIndex = 47;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 376);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 17);
            this.label6.TabIndex = 46;
            this.label6.Text = "Encoder:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 17);
            this.label5.TabIndex = 40;
            this.label5.Text = "Transport:";
            // 
            // transportComboBox
            // 
            this.transportComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.transportComboBox.FormattingEnabled = true;
            this.transportComboBox.Location = new System.Drawing.Point(96, 122);
            this.transportComboBox.Name = "transportComboBox";
            this.transportComboBox.Size = new System.Drawing.Size(172, 24);
            this.transportComboBox.TabIndex = 39;
            // 
            // aspectRatioCheckBox
            // 
            this.aspectRatioCheckBox.AutoSize = true;
            this.aspectRatioCheckBox.Checked = true;
            this.aspectRatioCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.aspectRatioCheckBox.Location = new System.Drawing.Point(248, 285);
            this.aspectRatioCheckBox.Name = "aspectRatioCheckBox";
            this.aspectRatioCheckBox.Size = new System.Drawing.Size(114, 21);
            this.aspectRatioCheckBox.TabIndex = 38;
            this.aspectRatioCheckBox.Text = "_AspectRatio";
            this.aspectRatioCheckBox.UseVisualStyleBackColor = true;
            // 
            // destSizeGroupBox
            // 
            this.destSizeGroupBox.Controls.Add(this.label12);
            this.destSizeGroupBox.Controls.Add(this.label13);
            this.destSizeGroupBox.Controls.Add(this.destWidthNumeric);
            this.destSizeGroupBox.Controls.Add(this.destHeightNumeric);
            this.destSizeGroupBox.Location = new System.Drawing.Point(248, 180);
            this.destSizeGroupBox.Name = "destSizeGroupBox";
            this.destSizeGroupBox.Size = new System.Drawing.Size(163, 99);
            this.destSizeGroupBox.TabIndex = 37;
            this.destSizeGroupBox.TabStop = false;
            this.destSizeGroupBox.Text = "DestSize";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 31);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 17);
            this.label12.TabIndex = 28;
            this.label12.Text = "Width:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 17);
            this.label13.TabIndex = 30;
            this.label13.Text = "Height:";
            // 
            // destWidthNumeric
            // 
            this.destWidthNumeric.Location = new System.Drawing.Point(66, 29);
            this.destWidthNumeric.Maximum = new decimal(new int[] {
            8128,
            0,
            0,
            0});
            this.destWidthNumeric.Name = "destWidthNumeric";
            this.destWidthNumeric.Size = new System.Drawing.Size(78, 22);
            this.destWidthNumeric.TabIndex = 26;
            this.destWidthNumeric.Value = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            // 
            // destHeightNumeric
            // 
            this.destHeightNumeric.Location = new System.Drawing.Point(66, 57);
            this.destHeightNumeric.Maximum = new decimal(new int[] {
            8128,
            0,
            0,
            0});
            this.destHeightNumeric.Name = "destHeightNumeric";
            this.destHeightNumeric.Size = new System.Drawing.Size(78, 22);
            this.destHeightNumeric.TabIndex = 29;
            this.destHeightNumeric.Value = new decimal(new int[] {
            1080,
            0,
            0,
            0});
            // 
            // srcRectGroupBox
            // 
            this.srcRectGroupBox.Controls.Add(this.label9);
            this.srcRectGroupBox.Controls.Add(this.label10);
            this.srcRectGroupBox.Controls.Add(this.srcTopNumeric);
            this.srcRectGroupBox.Controls.Add(this.srcLeftNumeric);
            this.srcRectGroupBox.Controls.Add(this.label8);
            this.srcRectGroupBox.Controls.Add(this.label11);
            this.srcRectGroupBox.Controls.Add(this.srcRightNumeric);
            this.srcRectGroupBox.Controls.Add(this.srcBottomNumeric);
            this.srcRectGroupBox.Location = new System.Drawing.Point(19, 180);
            this.srcRectGroupBox.Name = "srcRectGroupBox";
            this.srcRectGroupBox.Size = new System.Drawing.Size(188, 141);
            this.srcRectGroupBox.TabIndex = 34;
            this.srcRectGroupBox.TabStop = false;
            this.srcRectGroupBox.Text = "SrcRect";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 17);
            this.label9.TabIndex = 32;
            this.label9.Text = "Top:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 17);
            this.label10.TabIndex = 34;
            this.label10.Text = "Left:";
            // 
            // srcTopNumeric
            // 
            this.srcTopNumeric.Enabled = false;
            this.srcTopNumeric.Location = new System.Drawing.Point(72, 21);
            this.srcTopNumeric.Maximum = new decimal(new int[] {
            8128,
            0,
            0,
            0});
            this.srcTopNumeric.Minimum = new decimal(new int[] {
            8128,
            0,
            0,
            -2147483648});
            this.srcTopNumeric.Name = "srcTopNumeric";
            this.srcTopNumeric.Size = new System.Drawing.Size(91, 22);
            this.srcTopNumeric.TabIndex = 31;
            // 
            // srcLeftNumeric
            // 
            this.srcLeftNumeric.Enabled = false;
            this.srcLeftNumeric.Location = new System.Drawing.Point(72, 49);
            this.srcLeftNumeric.Maximum = new decimal(new int[] {
            8128,
            0,
            0,
            0});
            this.srcLeftNumeric.Minimum = new decimal(new int[] {
            8128,
            0,
            0,
            -2147483648});
            this.srcLeftNumeric.Name = "srcLeftNumeric";
            this.srcLeftNumeric.Size = new System.Drawing.Size(91, 22);
            this.srcLeftNumeric.TabIndex = 33;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 17);
            this.label8.TabIndex = 28;
            this.label8.Text = "Right:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 107);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 17);
            this.label11.TabIndex = 30;
            this.label11.Text = "Bottom:";
            // 
            // srcRightNumeric
            // 
            this.srcRightNumeric.Enabled = false;
            this.srcRightNumeric.Location = new System.Drawing.Point(72, 77);
            this.srcRightNumeric.Maximum = new decimal(new int[] {
            8128,
            0,
            0,
            0});
            this.srcRightNumeric.Minimum = new decimal(new int[] {
            8128,
            0,
            0,
            -2147483648});
            this.srcRightNumeric.Name = "srcRightNumeric";
            this.srcRightNumeric.Size = new System.Drawing.Size(91, 22);
            this.srcRightNumeric.TabIndex = 26;
            this.srcRightNumeric.Value = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            // 
            // srcBottomNumeric
            // 
            this.srcBottomNumeric.Enabled = false;
            this.srcBottomNumeric.Location = new System.Drawing.Point(72, 105);
            this.srcBottomNumeric.Maximum = new decimal(new int[] {
            8128,
            0,
            0,
            0});
            this.srcBottomNumeric.Minimum = new decimal(new int[] {
            8128,
            0,
            0,
            -2147483648});
            this.srcBottomNumeric.Name = "srcBottomNumeric";
            this.srcBottomNumeric.Size = new System.Drawing.Size(91, 22);
            this.srcBottomNumeric.TabIndex = 29;
            this.srcBottomNumeric.Value = new decimal(new int[] {
            1080,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "Display:";
            // 
            // screensUpdateButton
            // 
            this.screensUpdateButton.Location = new System.Drawing.Point(395, 14);
            this.screensUpdateButton.Name = "screensUpdateButton";
            this.screensUpdateButton.Size = new System.Drawing.Size(75, 23);
            this.screensUpdateButton.TabIndex = 1;
            this.screensUpdateButton.Text = "_Update";
            this.screensUpdateButton.UseVisualStyleBackColor = true;
            this.screensUpdateButton.Click += new System.EventHandler(this.screensUpdateButton_Click);
            // 
            // screensComboBox
            // 
            this.screensComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.screensComboBox.FormattingEnabled = true;
            this.screensComboBox.Location = new System.Drawing.Point(80, 14);
            this.screensComboBox.Name = "screensComboBox";
            this.screensComboBox.Size = new System.Drawing.Size(309, 24);
            this.screensComboBox.TabIndex = 2;
            this.screensComboBox.SelectedValueChanged += new System.EventHandler(this.screensComboBox_SelectedValueChanged_1);
            // 
            // fpsNumeric
            // 
            this.fpsNumeric.Location = new System.Drawing.Point(85, 405);
            this.fpsNumeric.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.fpsNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.fpsNumeric.Name = "fpsNumeric";
            this.fpsNumeric.Size = new System.Drawing.Size(107, 22);
            this.fpsNumeric.TabIndex = 5;
            this.fpsNumeric.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "Port:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 405);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "FPS:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Address:";
            // 
            // showMouseCheckBox
            // 
            this.showMouseCheckBox.AutoSize = true;
            this.showMouseCheckBox.Checked = true;
            this.showMouseCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showMouseCheckBox.Location = new System.Drawing.Point(17, 433);
            this.showMouseCheckBox.Name = "showMouseCheckBox";
            this.showMouseCheckBox.Size = new System.Drawing.Size(118, 21);
            this.showMouseCheckBox.TabIndex = 7;
            this.showMouseCheckBox.Text = "_Show Mouse";
            this.showMouseCheckBox.UseVisualStyleBackColor = true;
            // 
            // portNumeric
            // 
            this.portNumeric.Location = new System.Drawing.Point(96, 94);
            this.portNumeric.Maximum = new decimal(new int[] {
            100500,
            0,
            0,
            0});
            this.portNumeric.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.portNumeric.Name = "portNumeric";
            this.portNumeric.Size = new System.Drawing.Size(172, 22);
            this.portNumeric.TabIndex = 11;
            this.portNumeric.Value = new decimal(new int[] {
            1234,
            0,
            0,
            0});
            // 
            // addressTextBox
            // 
            this.addressTextBox.Location = new System.Drawing.Point(96, 66);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(276, 22);
            this.addressTextBox.TabIndex = 10;
            this.addressTextBox.Text = "0.0.0.0";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(3, 495);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(177, 35);
            this.startButton.TabIndex = 17;
            this.startButton.Text = "_Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(198, 495);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(107, 35);
            this.stopButton.TabIndex = 18;
            this.stopButton.Text = "_Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // previewButton
            // 
            this.previewButton.Location = new System.Drawing.Point(355, 495);
            this.previewButton.Name = "previewButton";
            this.previewButton.Size = new System.Drawing.Size(97, 35);
            this.previewButton.TabIndex = 19;
            this.previewButton.Text = "_Preview";
            this.previewButton.UseVisualStyleBackColor = true;
            this.previewButton.Click += new System.EventHandler(this.previewButton_Click);
            // 
            // snippingToolButton
            // 
            this.snippingToolButton.Location = new System.Drawing.Point(19, 327);
            this.snippingToolButton.Name = "snippingToolButton";
            this.snippingToolButton.Size = new System.Drawing.Size(142, 23);
            this.snippingToolButton.TabIndex = 48;
            this.snippingToolButton.Text = "_SetScreenRect";
            this.snippingToolButton.UseVisualStyleBackColor = true;
            this.snippingToolButton.Click += new System.EventHandler(this.snippingToolButton_Click);
            // 
            // ScreenStreamerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.settingPanel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.previewButton);
            this.Name = "ScreenStreamerControl";
            this.Size = new System.Drawing.Size(479, 546);
            this.settingPanel.ResumeLayout(false);
            this.settingPanel.PerformLayout();
            this.destSizeGroupBox.ResumeLayout(false);
            this.destSizeGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.destWidthNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.destHeightNumeric)).EndInit();
            this.srcRectGroupBox.ResumeLayout(false);
            this.srcRectGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srcTopNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcLeftNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcRightNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcBottomNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpsNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.portNumeric)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel settingPanel;
        private System.Windows.Forms.CheckBox aspectRatioCheckBox;
        private System.Windows.Forms.GroupBox destSizeGroupBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown destWidthNumeric;
        private System.Windows.Forms.NumericUpDown destHeightNumeric;
        private System.Windows.Forms.GroupBox srcRectGroupBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown srcTopNumeric;
        private System.Windows.Forms.NumericUpDown srcLeftNumeric;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown srcRightNumeric;
        private System.Windows.Forms.NumericUpDown srcBottomNumeric;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button screensUpdateButton;
        private System.Windows.Forms.ComboBox screensComboBox;
        private System.Windows.Forms.NumericUpDown fpsNumeric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox showMouseCheckBox;
        private System.Windows.Forms.NumericUpDown portNumeric;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button previewButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox transportComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox encoderComboBox;
        private System.Windows.Forms.Button snippingToolButton;
    }
}
