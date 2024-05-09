using System.Windows;

namespace DeliverySystem.Host.Wpf.Views
{
    /// <summary>
    /// Interaction logic for AddUploadFileView.xaml
    /// </summary>
    public partial class AddUploadFileView : Window
    {
        public AddUploadFileView()
        {
            InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
