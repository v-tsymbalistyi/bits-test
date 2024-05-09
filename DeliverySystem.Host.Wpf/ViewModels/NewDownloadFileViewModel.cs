using ReactiveUI;
using System.Windows.Input;

namespace DeliverySystem.Host.Wpf.ViewModels
{
    internal class NewDownloadFileViewModel : ReactiveObject
    {
        public ICommand BrowseDestinationFileNameCommand { get; private set; }

        public Uri? SourceFile { get; set; }
        public string? DestinationPath { get; set; }

        public NewDownloadFileViewModel()
        {
            BrowseDestinationFileNameCommand = ReactiveCommand.Create(OnBrowseFile);
        }

        private void OnBrowseFile()
        {
            var last = SourceFile?.Segments.LastOrDefault();
            var dialog =
                last?.EndsWith('/') == false
                ? OpenFileDialogProvider.GetNewFileDialog(DestinationPath, last)
                : OpenFileDialogProvider.GetNewFileDialog(DestinationPath, null);

            var result = dialog.ShowDialog();
            if (result == true)
            {
                DestinationPath = dialog.FileName;
            }
        }
    }
}
