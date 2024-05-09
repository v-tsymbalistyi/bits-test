using DeliverySystem.Host.Wpf.Views;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DeliverySystem.Host.Wpf.ViewModels
{
    internal class AddUploadJobViewModel : ReactiveObject
    {
        private NewUploadFileViewModel? _selectedFile;

        public ICommand AddFileCommand { get; private set; }
        public ICommand RemoveFileCommand { get; private set; }

        public ObservableCollection<NewUploadFileViewModel> Files { get; private set; } = [];
        public NewUploadFileViewModel? SelectedFile
        {
            get => _selectedFile;
            set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
        }

        public AddUploadJobViewModel()
        {
            AddFileCommand = ReactiveCommand.Create(OnAddFile);
            RemoveFileCommand = ReactiveCommand.Create(OnRemoveFile);
        }

        private void OnRemoveFile()
        {
            if (SelectedFile != null)
                Files.Remove(SelectedFile);
        }

        private void OnAddFile()
        {
            var addFileDialog = new AddUploadFileView()
            {
                DataContext = new NewUploadFileViewModel()
                {
                    SourceFile = @"G:\temp\lh8Jb.jpg",
                    DestinationPath = new Uri(@"http://localhost:80/lh8Jb.jpg")
                }
            };
            var result = addFileDialog.ShowDialog();
            if (result == true
                && addFileDialog.DataContext is NewUploadFileViewModel newFile
                && newFile.SourceFile != null
                //&& newFile.SourceFile.IsFile
                && newFile.DestinationPath != null)
            {
                Files.Add(newFile);
            }
        }
    }
}
