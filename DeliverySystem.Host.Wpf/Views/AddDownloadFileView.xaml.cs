using System.Windows;

namespace DeliverySystem.Host.Wpf.Views
{
    /// <summary>
    /// Interaction logic for AddDownloadFileView.xaml
    /// </summary>
    public partial class AddDownloadFileView : Window
    {
        public AddDownloadFileView()
        {
            InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
