using ReactiveUI;
using System.Windows.Input;

namespace DeliverySystem.Host.Wpf.ViewModels
{
    internal class NewUploadFileViewModel : ReactiveObject
    {
        public ICommand BrowseSourceFileNameCommand { get; private set; }

        public string? SourceFile { get; set; }
        public Uri? DestinationPath { get; set; }

        public NewUploadFileViewModel()
        {
            BrowseSourceFileNameCommand = ReactiveCommand.Create(OnBrowseFile);
        }

        private void OnBrowseFile()
        {
            var last = DestinationPath?.Segments.LastOrDefault();
            var dialog =
                last?.EndsWith("/") == false
                ? OpenFileDialogProvider.GetNewFileDialog(SourceFile, last)
                : OpenFileDialogProvider.GetNewFileDialog(SourceFile, null);

            var result = dialog.ShowDialog();
            if (result == true)
            {
                SourceFile = dialog.FileName;
            }
        }
    }
}
