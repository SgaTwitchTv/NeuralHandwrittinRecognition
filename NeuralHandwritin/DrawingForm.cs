using NeuralHandwritin.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NeuralHandwritin
{
    public partial class DrawingForm : Form
    {
        private Panel panelDraw;
        private Button btnDigitize;
        private TextBox txtResult;
        private Button btnClear;

        private Bitmap bmp;
        private Graphics g;
        private Point lastPoint = Point.Empty;
        private NeuralNetwork nn;

        public DrawingForm(NeuralNetwork network)
        {
            nn = network;
            SetupForm();
            InitializeDrawing();
        }

        private void SetupForm()
        {
            // Form settings
            this.Text = "Draw a Digit";
            this.Size = new Size(400, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 30); // dark background

            // Drawing panel (big black square with border)
            panelDraw = new Panel
            {
                Location = new Point(50, 30),
                Size = new Size(300, 300),
                BackColor = Color.Black,
                BorderStyle = BorderStyle.Fixed3D
            };
            this.Controls.Add(panelDraw);

            // DIGITIZE button
            btnDigitize = new Button
            {
                Location = new Point(150, 350),
                Size = new Size(100, 40),
                Text = "DIGITIZE",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.Controls.Add(btnDigitize);

            // Result textbox
            txtResult = new TextBox
            {
                Location = new Point(50, 410),
                Size = new Size(300, 100),
                Multiline = true,
                Font = new Font("Segoe UI", 14),
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = true,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.LightGreen,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtResult);

            btnClear = new Button
            {
                Location = new Point(270, 350),
                Size = new Size(80, 40),
                Text = "CLEAR",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.Controls.Add(btnClear);
        }

        private void InitializeDrawing()
        {
            bmp = new Bitmap(28, 28);
            g = Graphics.FromImage(bmp);
            ClearCanvas();

            // Events
            panelDraw.Paint += PanelDraw_Paint;
            panelDraw.MouseDown += PanelDraw_MouseDown;
            panelDraw.MouseMove += PanelDraw_MouseMove;
            btnDigitize.Click += BtnDigitize_Click;
            btnClear.Click += (s, e) => ClearCanvas();
        }

        private void ClearCanvas()
        {
            g.Clear(Color.Black);
            panelDraw.Invalidate();
            txtResult.Text = "";
        }

        private void PanelDraw_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(bmp, 0, 0, panelDraw.Width, panelDraw.Height);
        }

        private void PanelDraw_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = ScalePoint(e.Location);
        }

        private void PanelDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point current = ScalePoint(e.Location);
                using (Pen pen = new Pen(Color.White, 2))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.DrawLine(pen, lastPoint, current);
                }
                lastPoint = current;
                panelDraw.Invalidate();
            }
        }

        private Point ScalePoint(Point p)
        {
            return new Point(
                (int)(p.X * 28f / panelDraw.Width),
                (int)(p.Y * 28f / panelDraw.Height)
            );
        }

        private void BtnDigitize_Click(object sender, EventArgs e)
        {
            double[] input = new double[784];

            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    double gray = (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0;
                    input[y * 28 + x] = gray; // white = high value (1.0), black = 0.0
                }
            }

            double[] output = nn.Forward(input);
            int predicted = 0;
            double maxConf = output[0];
            for (int i = 1; i < 10; i++)
            {
                if (output[i] > maxConf)
                {
                    maxConf = output[i];
                    predicted = i;
                }
            }

            if (maxConf > 0.8)
            {
                txtResult.ForeColor = Color.LightGreen;

            }
            else if (maxConf > 0.5)
            {
                txtResult.ForeColor = Color.Yellow;
            }
            else
            {
                txtResult.ForeColor = Color.Red;
            }

            if(maxConf < 0.5)
            {
                txtResult.Text = "Uncertain";
                txtResult.Font = new Font(txtResult.Font.FontFamily, 24, FontStyle.Italic);
                return;
            }
            txtResult.Text = predicted.ToString();
            txtResult.Font = new Font(txtResult.Font.FontFamily, 36, FontStyle.Bold);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                g?.Dispose();
                bmp?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}