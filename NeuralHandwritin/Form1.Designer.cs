namespace NeuralHandwritin
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
            btnSelectFolder = new Button();
            lblFolderPath = new Label();
            btnTrain = new Button();
            proggressBarTraining = new ProgressBar();
            lblStatus = new Label();
            btnDraw = new Button();
            SuspendLayout();
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.BackColor = SystemColors.AppWorkspace;
            btnSelectFolder.Location = new Point(12, 166);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(136, 29);
            btnSelectFolder.TabIndex = 0;
            btnSelectFolder.Text = "Select Folder";
            btnSelectFolder.UseVisualStyleBackColor = false;
            btnSelectFolder.Click += btnSelectFolder_Click;
            // 
            // lblFolderPath
            // 
            lblFolderPath.AutoSize = true;
            lblFolderPath.Location = new Point(12, 143);
            lblFolderPath.Name = "lblFolderPath";
            lblFolderPath.Size = new Size(136, 20);
            lblFolderPath.TabIndex = 1;
            lblFolderPath.Text = "No Folder Selected";
            lblFolderPath.Click += lblFolderPath_Click;
            // 
            // btnTrain
            // 
            btnTrain.Enabled = false;
            btnTrain.Location = new Point(12, 249);
            btnTrain.Name = "btnTrain";
            btnTrain.Size = new Size(94, 29);
            btnTrain.TabIndex = 2;
            btnTrain.Text = "TRAIN";
            btnTrain.UseVisualStyleBackColor = true;
            btnTrain.Click += btnTrain_Click;
            // 
            // proggressBarTraining
            // 
            proggressBarTraining.Location = new Point(12, 284);
            proggressBarTraining.Name = "proggressBarTraining";
            proggressBarTraining.Size = new Size(125, 29);
            proggressBarTraining.TabIndex = 3;
            proggressBarTraining.Visible = false;
            proggressBarTraining.Click += proggressBarTraining_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(556, 172);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(97, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Status: Ready";
            lblStatus.Click += lblStatus_Click;
            // 
            // btnDraw
            // 
            btnDraw.BackColor = SystemColors.AppWorkspace;
            btnDraw.Enabled = false;
            btnDraw.Location = new Point(556, 195);
            btnDraw.Name = "btnDraw";
            btnDraw.Size = new Size(94, 29);
            btnDraw.TabIndex = 5;
            btnDraw.Text = "DRAW";
            btnDraw.UseVisualStyleBackColor = false;
            btnDraw.Click += btnDraw_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnDraw);
            Controls.Add(lblStatus);
            Controls.Add(proggressBarTraining);
            Controls.Add(btnTrain);
            Controls.Add(lblFolderPath);
            Controls.Add(btnSelectFolder);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSelectFolder;
        private Label lblFolderPath;
        private Button btnTrain;
        private ProgressBar proggressBarTraining;
        private Label lblStatus;
        private Button btnDraw;
    }
}
