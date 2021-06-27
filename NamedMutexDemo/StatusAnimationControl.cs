using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NamedMutexDemo
{
    public partial class StatusAnimationControl : UserControl
    {
        private readonly Image[] _angleImages = new Image[360];

        private Func<long> _valueResolver;

        public StatusAnimationControl()
        {
            InitializeComponent();
            InitializeImages();
        }

        public void SetValueResolver(Func<long> func)
        {
            _valueResolver = func;
        }

        private void InitializeImages()
        {
            for (int i = 0; i < 360; i++)
            {

                _angleImages[i] = MakeImage(i);
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

        private void _animationTimer_Tick(object sender, EventArgs e)
        {
            var valueResolver = _valueResolver;

            if (valueResolver != null)
            {
                var graphics = this.CreateGraphics();
                var value = _valueResolver();
                graphics.DrawImage(_angleImages[value % 360], new Point(0, 0));
                _lblIterationValue.Text = value.ToString();
            }
        }
    }
}
