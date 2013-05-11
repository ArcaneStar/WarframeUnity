using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarframeUnity.Feed;

namespace WarframeUnity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private Thread updateThread;
        private bool running = false;

        private Alerts alerts;
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            alerts = new Alerts();
            alerts.OnAlert += OnAlert;

            Start();
        }

        private void OnAlert()
        {
            console.Dispatcher.Invoke(new MethodInvoker(() =>
                {
                    console.AppendText("New Alerts!\r\n");
                    foreach (Alert alert in alerts.List)
                    {
                        console.AppendText(alert.Text + "\r\n");
                    }
                    alerts.List.Clear();
                    console.AppendText("\r\n\r\n");
                }));
        }
        #endregion

        #region Methods
        public void Start()
        {

            running = true;
            updateThread = new Thread(new ThreadStart(Update));
            updateThread.IsBackground = true;
            updateThread.Start();
        }

        public void Stop()
        {
            running = false;
            updateThread.Join(2000);
        }

        public void Update()
        {
            while (running)
            {
                alerts.Update();
            }
        }
        #endregion
    }
}
