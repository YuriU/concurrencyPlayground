using System;
using System.Threading;
using System.Windows.Forms;

namespace NamedMutexDemo
{
    public partial class MainForm : Form
    {
        private Thread WorkerThread = new Thread(WorkerThreadWrapper);

        private Mutex _interProcessMutex;

        

        private long _iteration = 0;

        private volatile int _passControlToNext = 0;

        public MainForm()
        {
            _interProcessMutex = CreateOrOpenNamedMutex("MyMytexName");

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
                try
                {
                    _interProcessMutex.WaitOne();
                }
                catch (AbandonedMutexException) {}
                
                _passControlToNext = 0;

                while (true)
                {
                    if (_passControlToNext == 1)
                    {
                        break;
                    }

                    Interlocked.Increment(ref _iteration);
                    Thread.Sleep(10);
                }

                _interProcessMutex.ReleaseMutex();
            }
        }

        private Mutex CreateOrOpenNamedMutex(string name)
        {
            Mutex m = new Mutex(false, name, out var createdNew);
            return m;
        }

        private void _btnPass_Click(object sender, EventArgs e)
        {
            Interlocked.CompareExchange(ref _passControlToNext, 1, 0);
        }
    }
}
