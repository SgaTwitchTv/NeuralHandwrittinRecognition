namespace NeuralHandwritin
{
    partial class DrawingForm
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
            panel1 = new Panel();
            btnDigitize = new Button();
            txtResult = new TextBox();
            btnClear = new Button();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Location = new Point(94, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(280, 280);
            panel1.TabIndex = 0;
            panel1.Paint += panel1_Paint;
            // 
            // btnDigitize
            // 
            btnDigitize.Location = new Point(194, 298);
            btnDigitize.Name = "btnDigitize";
            btnDigitize.Size = new Size(94, 29);
            btnDigitize.TabIndex = 1;
            btnDigitize.Text = "DIGITIZE";
            btnDigitize.UseVisualStyleBackColor = true;
            btnDigitize.Click += btnDigitize_Click;
            // 
            // txtResult
            // 
            txtResult.Location = new Point(194, 333);
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            txtResult.Size = new Size(125, 27);
            txtResult.TabIndex = 2;
            txtResult.TextChanged += txtResult_TextChanged;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(380, 12);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(94, 82);
            btnClear.TabIndex = 3;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // DrawingForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(476, 450);
            Controls.Add(btnClear);
            Controls.Add(txtResult);
            Controls.Add(btnDigitize);
            Controls.Add(panel1);
            Name = "DrawingForm";
            Text = "DrawingForm";
            Load += DrawingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Button btnDigitize;
        private TextBox txtResult;
        private Button btnClear;
    }
}