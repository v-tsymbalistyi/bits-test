﻿using System.Windows;

namespace DeliverySystem.Host.Wpf.Views
{
    /// <summary>
    /// Interaction logic for AddJobView.xaml
    /// </summary>
    public partial class AddDownloadJobView : Window
    {
        public AddDownloadJobView()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
