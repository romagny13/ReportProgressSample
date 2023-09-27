using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReportProgressSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void runButton_Click(object sender, RoutedEventArgs e)
        {

            IProgress<int> progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
                progressTextBlock.Text = $"{value}%";
            });

            await Task.Run(() => DoWork(100, progress));

            progressTextBlock.Text = "Completed";
        }

        private void DoWork(int count, IProgress<int> progress)
        {
            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(100);
                var percentage = (i * 100) / count;
                progress.Report(percentage);
            }
        }

        private async void runIndeterminatedButton_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            DirectoryInfo root = new DirectoryInfo(path);

            IProgress<string> progress = new Progress<string>(value =>
            {
                indeterminatedProgressTextBlock.Text = value;
            });

            indeterminatedProgressBar.Visibility = Visibility.Visible;

            await Task.Run(() => BrowseDirectory(root, progress));

            indeterminatedProgressBar.Visibility = Visibility.Hidden;
            indeterminatedProgressTextBlock.Text = "Completed";
        }

        private static void BrowseDirectory(DirectoryInfo parentDirectory, IProgress<string> progress)
        {
            foreach (var directory in parentDirectory.EnumerateDirectories())
            {
                // enumerate child directories and files
                BrowseDirectory(directory, progress);
            }

            foreach (var file in parentDirectory.EnumerateFiles())
            {
                // report
                string fullName = file.FullName;
                progress.Report(fullName);
            }
        }
    }
}
