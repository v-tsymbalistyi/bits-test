using DeliverySystem.Bits.Base;
using DeliverySystem.Host.Wpf.Views;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DeliverySystem.Host.Wpf.ViewModels
{
    internal class AddDownloadJobViewModel: ReactiveObject
    {
        private NewDownloadFileViewModel? _selectedFile;
        
        public ICommand AddFileCommand { get; private set; }
        public ICommand RemoveFileCommand { get; private set; }

        public ObservableCollection<NewDownloadFileViewModel> Files { get; private set; } = [];
        public NewDownloadFileViewModel? SelectedFile
        {
            get => _selectedFile;
            set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
        }

        public AddDownloadJobViewModel()
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
            var addFileDialog = new AddDownloadFileView()
            {
                DataContext = new NewDownloadFileViewModel()
                {
                    SourceFile = new Uri(@"https://i.stack.imgur.com/lh8Jb.jpg"),
                    DestinationPath = @"G:\temp\lh8Jb.jpg"
                }
            };
            var result = addFileDialog.ShowDialog();
            if (result == true
                && addFileDialog.DataContext is NewDownloadFileViewModel newFile
                && newFile.SourceFile != null
                //&& newFile.SourceFile.IsFile
                && newFile.DestinationPath != null)
            {
                Files.Add(newFile);
            }
        }
    }
}
