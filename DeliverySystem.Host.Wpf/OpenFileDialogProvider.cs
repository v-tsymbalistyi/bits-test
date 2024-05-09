using Microsoft.Win32;
using System.IO;

namespace DeliverySystem.Host.Wpf
{
    public static class OpenFileDialogProvider
    {
        private static readonly OpenFileDialog _openFileDialog = new();

        public static OpenFileDialog GetNewFileDialog(string? destinationPath, string? fileName)
        {
            if (destinationPath != null)
            {
                _openFileDialog.FileName = destinationPath;
                var folderPath = Directory.GetParent(destinationPath)?.FullName;
                if (folderPath != null)
                    _openFileDialog.InitialDirectory = folderPath;
            }
            else
            {
                if (fileName != null)
                    _openFileDialog.FileName = fileName;
            }

            _openFileDialog.CheckFileExists = false;
            _openFileDialog.CheckPathExists = true;

            return _openFileDialog;
        }
    }
}
