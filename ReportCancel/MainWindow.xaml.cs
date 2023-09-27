using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReportCancel
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        CancellationTokenSource _tokenSource;

        private async void runButton_Click(object sender, RoutedEventArgs e)
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            IProgress<int> progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
                progressTextBlock.Text = $"{value}%";
            });

            try
            {
                await Task.Run(() => DoWork(100, progress, token));
                progressTextBlock.Text = "Completed";
            }
            catch (OperationCanceledException ex)
            {
                progressTextBlock.Text = "Cancelled";
            }    
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _tokenSource.Cancel();
        }

        private void DoWork(int count, IProgress<int> progress, CancellationToken token)
        {
            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(100);
                var percentage = (i * 100) / count;
                progress.Report(percentage);

                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
            }
        }

    }
}
