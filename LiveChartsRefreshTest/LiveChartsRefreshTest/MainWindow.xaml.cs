using LiveCharts;
using LiveCharts.Defaults;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System;

namespace LiveChartsRefreshTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ChartValues<double> PointCloud { get; set; }

        private Random rnd;

        Thread workerThread;

        private bool running;

        private int _counter;
        public int Counter
        {
            get { return _counter; }
            set
            {
                if (value != _counter)
                {
                    _counter = value;
                    OnPropertyChanged("Counter");
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            PointCloud = new ChartValues<double>();
            rnd = new Random();
            ((INotifyCollectionChanged)PointCloud).CollectionChang‌​ed += WTFWPF;
            Counter = 0;
            running = true;

            DataContext = this;
        }

        private void WTFWPF(object sender, NotifyCollectionChangedEventArgs e)
        {
            Counter++;
        }

        private void ProcessingThread()
        {
            while (running)
            {
                double[] tmpList = new double[100];
                for (int i = 0; i < 100; i++)
                {
                    tmpList[i] = rnd.NextDouble();
                }

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    PointCloud.Clear();
                    PointCloud.AddRange(tmpList);
                }), System.Windows.Threading.DispatcherPriority.Background);

                Thread.Sleep(50);
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            workerThread = new Thread(new ThreadStart(ProcessingThread));
            workerThread.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            running = false;
            workerThread.Join();
        }
    }
}
