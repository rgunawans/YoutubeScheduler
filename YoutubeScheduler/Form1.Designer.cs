namespace YoutubeScheduler
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnTambahJadwal = new Button();
            dataGridViewSchedules = new DataGridView();
            btnCreateYoutube = new Button();
            btnSetPublicVisibility = new Button();
            btnSetHiddenVisibility = new Button();
            dateTimePickerStart = new DateTimePicker();
            dateTimePickerEnd = new DateTimePicker();
            label1 = new Label();
            label2 = new Label();
            checkBoxDisableChat = new CheckBox();
            checkBoxDisableMonetize = new CheckBox();
            errorTextBox = new TextBox();
            btnGetBroadcastInfo = new Button();
            btnGenerateExcel = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewSchedules).BeginInit();
            SuspendLayout();
            // 
            // btnTambahJadwal
            // 
            btnTambahJadwal.Location = new Point(25, 397);
            btnTambahJadwal.Margin = new Padding(3, 4, 3, 4);
            btnTambahJadwal.Name = "btnTambahJadwal";
            btnTambahJadwal.Size = new Size(174, 56);
            btnTambahJadwal.TabIndex = 0;
            btnTambahJadwal.Text = "Tambah Jadwal";
            btnTambahJadwal.UseVisualStyleBackColor = true;
            btnTambahJadwal.Click += btnTambahJadwal_Click;
            // 
            // dataGridViewSchedules
            // 
            dataGridViewSchedules.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewSchedules.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewSchedules.Location = new Point(25, 27);
            dataGridViewSchedules.Margin = new Padding(3, 4, 3, 4);
            dataGridViewSchedules.Name = "dataGridViewSchedules";
            dataGridViewSchedules.RowHeadersWidth = 51;
            dataGridViewSchedules.Size = new Size(1145, 296);
            dataGridViewSchedules.TabIndex = 1;
            // 
            // btnCreateYoutube
            // 
            btnCreateYoutube.BackColor = Color.Red;
            btnCreateYoutube.ForeColor = Color.White;
            btnCreateYoutube.Location = new Point(206, 397);
            btnCreateYoutube.Margin = new Padding(3, 4, 3, 4);
            btnCreateYoutube.Name = "btnCreateYoutube";
            btnCreateYoutube.Size = new Size(174, 56);
            btnCreateYoutube.TabIndex = 2;
            btnCreateYoutube.Text = "Buat di YouTube";
            btnCreateYoutube.UseVisualStyleBackColor = false;
            btnCreateYoutube.Click += btnCreateYoutube_Click;
            // 
            // btnSetPublicVisibility
            // 
            btnSetPublicVisibility.BackColor = Color.Green;
            btnSetPublicVisibility.ForeColor = Color.White;
            btnSetPublicVisibility.Location = new Point(387, 397);
            btnSetPublicVisibility.Margin = new Padding(3, 4, 3, 4);
            btnSetPublicVisibility.Name = "btnSetPublicVisibility";
            btnSetPublicVisibility.Size = new Size(174, 56);
            btnSetPublicVisibility.TabIndex = 9;
            btnSetPublicVisibility.Text = "Set Public Visibility";
            btnSetPublicVisibility.UseVisualStyleBackColor = false;
            btnSetPublicVisibility.Click += btnSetPublicVisibility_Click;
            // 
            // btnSetHiddenVisibility
            // 
            btnSetHiddenVisibility.BackColor = Color.Blue;
            btnSetHiddenVisibility.ForeColor = Color.White;
            btnSetHiddenVisibility.Location = new Point(568, 397);
            btnSetHiddenVisibility.Margin = new Padding(3, 4, 3, 4);
            btnSetHiddenVisibility.Name = "btnSetHiddenVisibility";
            btnSetHiddenVisibility.Size = new Size(174, 56);
            btnSetHiddenVisibility.TabIndex = 10;
            btnSetHiddenVisibility.Text = "Set Hidden Visibility";
            btnSetHiddenVisibility.UseVisualStyleBackColor = false;
            btnSetHiddenVisibility.Click += btnSetHiddenVisibility_Click;
            // 
            // dateTimePickerStart
            // 
            dateTimePickerStart.Format = DateTimePickerFormat.Short;
            dateTimePickerStart.Location = new Point(25, 360);
            dateTimePickerStart.Margin = new Padding(3, 4, 3, 4);
            dateTimePickerStart.Name = "dateTimePickerStart";
            dateTimePickerStart.Size = new Size(173, 27);
            dateTimePickerStart.TabIndex = 3;
            // 
            // dateTimePickerEnd
            // 
            dateTimePickerEnd.Format = DateTimePickerFormat.Short;
            dateTimePickerEnd.Location = new Point(206, 360);
            dateTimePickerEnd.Margin = new Padding(3, 4, 3, 4);
            dateTimePickerEnd.Name = "dateTimePickerEnd";
            dateTimePickerEnd.Size = new Size(173, 27);
            dateTimePickerEnd.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 336);
            label1.Name = "label1";
            label1.Size = new Size(105, 20);
            label1.TabIndex = 5;
            label1.Text = "Tanggal Mulai:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(206, 336);
            label2.Name = "label2";
            label2.Size = new Size(102, 20);
            label2.TabIndex = 6;
            label2.Text = "Tanggal Akhir:";
            // 
            // checkBoxDisableChat
            // 
            checkBoxDisableChat.AutoSize = true;
            checkBoxDisableChat.Checked = true;
            checkBoxDisableChat.CheckState = CheckState.Checked;
            checkBoxDisableChat.Location = new Point(385, 336);
            checkBoxDisableChat.Name = "checkBoxDisableChat";
            checkBoxDisableChat.Size = new Size(145, 24);
            checkBoxDisableChat.TabIndex = 7;
            checkBoxDisableChat.Text = "Nonaktifkan Chat";
            checkBoxDisableChat.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisableMonetize
            // 
            checkBoxDisableMonetize.AutoSize = true;
            checkBoxDisableMonetize.Checked = true;
            checkBoxDisableMonetize.CheckState = CheckState.Checked;
            checkBoxDisableMonetize.Location = new Point(385, 365);
            checkBoxDisableMonetize.Name = "checkBoxDisableMonetize";
            checkBoxDisableMonetize.Size = new Size(186, 24);
            checkBoxDisableMonetize.TabIndex = 8;
            checkBoxDisableMonetize.Text = "Nonaktifkan Monetisasi";
            checkBoxDisableMonetize.UseVisualStyleBackColor = true;
            // 
            // errorTextBox
            // 
            errorTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            errorTextBox.Location = new Point(25, 460);
            errorTextBox.Margin = new Padding(3, 4, 3, 4);
            errorTextBox.Multiline = true;
            errorTextBox.Name = "errorTextBox";
            errorTextBox.ScrollBars = ScrollBars.Vertical;
            errorTextBox.Size = new Size(1145, 100);
            errorTextBox.TabIndex = 11;
            // 
            // btnGetBroadcastInfo
            // 
            btnGetBroadcastInfo.BackColor = Color.Orange;
            btnGetBroadcastInfo.ForeColor = Color.White;
            btnGetBroadcastInfo.Location = new Point(749, 397);
            btnGetBroadcastInfo.Margin = new Padding(3, 4, 3, 4);
            btnGetBroadcastInfo.Name = "btnGetBroadcastInfo";
            btnGetBroadcastInfo.Size = new Size(182, 56);
            btnGetBroadcastInfo.TabIndex = 12;
            btnGetBroadcastInfo.Text = "Get Broadcast Info";
            btnGetBroadcastInfo.UseVisualStyleBackColor = false;
            btnGetBroadcastInfo.Click += btnGetBroadcastInfo_Click;
            // 
            // btnGenerateExcel
            // 
            btnGenerateExcel.BackColor = Color.DarkGreen;
            btnGenerateExcel.ForeColor = Color.White;
            btnGenerateExcel.Location = new Point(938, 397);
            btnGenerateExcel.Margin = new Padding(3, 4, 3, 4);
            btnGenerateExcel.Name = "btnGenerateExcel";
            btnGenerateExcel.Size = new Size(182, 56);
            btnGenerateExcel.TabIndex = 13;
            btnGenerateExcel.Text = "Generate Excel";
            btnGenerateExcel.UseVisualStyleBackColor = false;
            btnGenerateExcel.Click += btnGenerateExcel_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1195, 620);
            Controls.Add(btnGenerateExcel);
            Controls.Add(btnGetBroadcastInfo);
            Controls.Add(errorTextBox);
            Controls.Add(checkBoxDisableMonetize);
            Controls.Add(checkBoxDisableChat);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dateTimePickerEnd);
            Controls.Add(dateTimePickerStart);
            Controls.Add(btnCreateYoutube);
            Controls.Add(btnSetPublicVisibility);
            Controls.Add(btnSetHiddenVisibility);
            Controls.Add(dataGridViewSchedules);
            Controls.Add(btnTambahJadwal);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KGR Youtube Schedule Manager";
        
            ((System.ComponentModel.ISupportInitialize)dataGridViewSchedules).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnTambahJadwal;
        private DataGridView dataGridViewSchedules;
        private Button btnCreateYoutube;
        private Button btnSetPublicVisibility;
        private Button btnSetHiddenVisibility;
        private DateTimePicker dateTimePickerStart;
        private DateTimePicker dateTimePickerEnd;
        private Label label1;
        private Label label2;
        private CheckBox checkBoxDisableChat;
        private CheckBox checkBoxDisableMonetize;
        private TextBox errorTextBox;
        private Button btnGetBroadcastInfo;
        private Button btnGenerateExcel;
    }
}

