using NeuralHandwritin.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace NeuralHandwritin
{
    public partial class DrawingForm : Form
    {
        private Bitmap bmp = new Bitmap(28, 28);
        private Graphics g;
        private Point lastPoint = Point.Empty;
        private NeuralNetwork nn;

        public DrawingForm(NeuralNetwork network)
        {
            InitializeComponent();
            nn = network;
            FormSetup();
        }

        private void FormSetup()
        {
            bmp = new Bitmap(28, 28);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.Black);

            panel1.Paint += PanelDraw_Paint;
            panel1.MouseDown += PanelDraw_MouseDown;
            panel1.MouseMove += PanelDraw_MouseMove;
            btnDigitize.Click += BtnDigitize_Click;
            btnClear.Click += BtnClear_Click;
        }

        //mouse events for drawing
        private void PanelDraw_MouseDown(object sender, MouseEventArgs e)
        {
            float scaleX = 28f / panel1.Width;
            float scaleY = 28f / panel1.Height;
            lastPoint = new Point((int)(e.X * scaleX), (int)(e.Y * scaleY));
        }

        private void PanelDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float scaleX = 28f / panel1.Width;
                float scaleY = 28f / panel1.Height;
                Point currentPoint = new Point((int)(e.X * scaleX), (int)(e.Y * scaleY));

                using (Pen pen = new Pen(Color.White, 3))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // smooth lines
                    g.DrawLine(pen, lastPoint, currentPoint);
                }

                lastPoint = currentPoint;
                panel1.Invalidate();
            }
        }

        private void PanelDraw_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.DrawImage(bmp, 0, 0, panel1.Width, panel1.Height);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.Black);
            panel1.Invalidate();
            txtResult.Text = "";
        }

        //the digitize button magic
        private void BtnDigitize_Click(object sender, EventArgs e)
        {
            //convert bmp to 784 double array
            double[] input = new double[28 * 28];
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    Color pixelColor = bmp.GetPixel(x, y);
                    double gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3.0 / 255.0;
                    input[y * 28 + x] = 1.0 - gray;
                }
            }

            double[] output = nn.Forward(input);
            int predictedDigit = nn.PredictDigit(output);

            txtResult.Text = $"{predictedDigit}";
        }


        private void DrawingForm_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }

        private void btnDigitize_Click(object sender, EventArgs e)
        {

        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            DrawingForm drawForm = new DrawingForm(nn);
            drawForm.ShowDialog();
        }
    }
}
