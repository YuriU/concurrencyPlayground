using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace NamedMutexDemo
{
    public partial class MainForm : Form
    {
        private Thread WorkerThread = new Thread(WorkerThreadWrapper);

        private ManualResetEvent _disposedEvent = new ManualResetEvent(false);

        private bool isDisposed = false;

        private static long _iteration = 0;

        public MainForm()
        {
            InitializeComponent();
            _statusAnimationControl.SetValueResolver(() =>
            {
                return Interlocked.Read(ref _iteration);
            });
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            WorkerThread.IsBackground = true;
            WorkerThread.Start(this);
        }

        private static void WorkerThreadWrapper(object param)
        {
            var form = (MainForm)param;
            form.WorkerThreadProc();
        }

        private void WorkerThreadProc()
        {
            while (true)
            {
                Interlocked.Increment(ref _iteration);
                Thread.Sleep(100);
            }
        }
    }
}
