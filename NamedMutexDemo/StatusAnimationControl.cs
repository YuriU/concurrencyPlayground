using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NamedMutexDemo
{
    public partial class StatusAnimationControl : UserControl
    {
        public Image[] AngleImages = new Image[360];
        public StatusAnimationControl()
        {
            InitializeComponent();
            InitializeImages();
        }

        private void InitializeImages()
        {
            for (int i = 0; i < 360; i++)
            {
                AngleImages[i] = MakeImage(i);
            }
        }

        private Image MakeImage(int angle)
        {
            var bitmap = new Bitmap(200, 200);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                float pie1StartAngle = angle % 360;
                float pie2StartAngle = (angle + 90) % 360;
                float pie3StartAngle = (angle + 180) % 360;
                float pie4StartAngle = (angle + 270) % 360;
                float sweepAngle = 90;

                Rectangle rect = new Rectangle(0, 0, 200, 200);
                graphics.FillPie(new SolidBrush(Color.Aquamarine), rect, pie1StartAngle, sweepAngle);
                graphics.FillPie(new HatchBrush(HatchStyle.DiagonalBrick, Color.Aquamarine), rect, pie2StartAngle, sweepAngle);
                graphics.FillPie(new SolidBrush(Color.Aquamarine), rect, pie3StartAngle, sweepAngle);
                graphics.FillPie(new HatchBrush(HatchStyle.DiagonalBrick, Color.Aquamarine), rect, pie4StartAngle, sweepAngle);
            }

            return bitmap;
        }

        private int _iteration = 0;

        private void _animationTimer_Tick(object sender, EventArgs e)
        {
            var graphics = this.CreateGraphics();
            graphics.DrawImage(AngleImages[_iteration % 360], new Point(0, 0));
            _iteration+=5;
        }
    }
}
