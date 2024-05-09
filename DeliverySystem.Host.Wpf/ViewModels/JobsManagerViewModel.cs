using DeliverySystem.Bits.Base;
using DeliverySystem.Host.Wpf.Views;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DeliverySystem.Host.Wpf.ViewModels
{
    public class JobsManagerViewModel: ReactiveObject
    {
        private readonly BitsManager _jobsManager;
        //TODO: dispose
        private readonly IDisposable _updateJobsObservable;
        private BitsJobViewModel? _selectedJob;

        public ICommand AddDownloadJobCommand {  get; private set; }
        public ICommand AddUploadJobCommand { get; private set; }

        public ICommand CancelSelectedJobCommand { get; private set; }
        public ICommand StartSelectedJobCommand { get; private set; }
        public ICommand CompleteSelectedJobCommand { get; private set; }

        public ICommand CleanLogsCommand { get; private set; }

        public ObservableCollection<string> Logs { get; private set; } = [];
        public ObservableCollection<BitsJobViewModel> Jobs { get; private set; } = [];
        public BitsJobViewModel? SelectedJob
        {
            get => _selectedJob;
            set => this.RaiseAndSetIfChanged(ref _selectedJob, value);
        }

        public JobsManagerViewModel()
        {
            _jobsManager = new BitsManager();
            _jobsManager.OnInterfaceError += OnJobsManagerInterfaceError;
            _jobsManager.OnJobAdded += (s, e) => Log($"Job added: {GetJobInfo(e)}");
            _jobsManager.OnJobError += OnJobsManagerJobError;
            _jobsManager.OnJobRemoved += (s, e) => Log($"Job removed: {GetJobInfo(e)}"); ;
            _jobsManager.OnJobTransferred += (s, e) => Log($"Job transferred: {GetJobInfo(e)}"); ;
            _jobsManager.OnJobModified += (s, e) => Log($"Job modified: {GetJobInfo(e)}"); ;

            AddDownloadJobCommand = ReactiveCommand.Create(AddDownloadJob);
            AddUploadJobCommand = ReactiveCommand.Create(AddUploadJob);
            CancelSelectedJobCommand = ReactiveCommand.Create(CancelJob);
            StartSelectedJobCommand = ReactiveCommand.Create(StartJob);
            CompleteSelectedJobCommand = ReactiveCommand.Create(CompleteJob);
            CleanLogsCommand = ReactiveCommand.Create(ClearLogs);

            _updateJobsObservable = Observable
                .Interval(TimeSpan.FromSeconds(3))
                .Subscribe(_ => UpdateJobs());
        }

        private string GetJobInfo(NotificationEventArgs e)
        {
            var builder = new StringBuilder();
            if (e.Job.TryGetJobType(out var type))
                builder.AppendLine($"{e.Job.GetDisplayName()} {type}");
            else
                builder.AppendLine($"{e.Job.GetDisplayName()} of Unknown type");

            if (e.Job.TryRefreshFilesList(out var files) && files != null)
            {
                foreach (var file in files)
                    builder.AppendLine($"Local: {file.LocalName}; Remote: {file.RemoteName}");
            }
            else
                builder.AppendLine("Unknown files");
                
            return builder.ToString();
        }

        private void OnJobsManagerJobError(object? sender, ErrorNotificationEventArgs e)
        {
            Log($"Job error: {e.Error.Description}, {e.Error.ErrorContext}{Environment.NewLine}Job error context: {e.Error.ContextDescription}, {e.Error.Protocol}, {e.Error.ErrorCode}");
            GetJobInfo(e);
        }

        private void OnJobsManagerInterfaceError(object? sender, BitsInterfaceNotificationEventArgs e)
        {
            Log($"Interface error {e.Message}, {e.Description}");
            GetJobInfo(e);
        }

        private void Log(string message)
        {
            Application.Current.Dispatcher.Invoke(() => Logs.Add(message));
        }

        private void ClearLogs()
        {
            Application.Current.Dispatcher.Invoke(Logs.Clear);
        }

        private void AddDownloadJob()
        {
            var addDownloadJobDialog = new AddDownloadJobView() { DataContext = new AddDownloadJobViewModel() };
            var result = addDownloadJobDialog.ShowDialog();
            if(result == true
                && addDownloadJobDialog.DataContext is AddDownloadJobViewModel addJobViewModel
                && addJobViewModel.Files.Count > 0)
            {
                var job = _jobsManager.CreateJob($"test download job {Guid.NewGuid()}", Bits.Enums.JobType.Download);
                foreach (var file in addJobViewModel.Files)
                {
                    job.TryAddFile(file.SourceFile.ToString(), file.DestinationPath);
                }
            }
        }

        private void AddUploadJob()
        {
            var addDownloadJobDialog = new AddUploadJobView() { DataContext = new AddUploadJobViewModel() };
            var result = addDownloadJobDialog.ShowDialog();
            if (result == true
                && addDownloadJobDialog.DataContext is AddUploadJobViewModel addJobViewModel
                && addJobViewModel.Files.Count > 0)
            {
                var job = _jobsManager.CreateJob($"test upload job {Guid.NewGuid()}", Bits.Enums.JobType.Upload);
                foreach (var file in addJobViewModel.Files)
                {
                    job.TryAddFile(file.DestinationPath.ToString(), file.SourceFile);
                }
            }
        }

        private void UpdateJobs()
        {
            var newJobs = _jobsManager.EnumJobs();
            var existingJobLookup = Jobs.ToDictionary(_ => _.JobId, _ => _);
            
            foreach (var job in newJobs)
            {
                if (existingJobLookup.ContainsKey(job.Key))
                {
                    existingJobLookup[job.Key].UpdateInfo();
                }   
                else
                {
                    var jobViewModel = new BitsJobViewModel(job.Value);
                    jobViewModel.UpdateInfo();
                    Application.Current.Dispatcher.Invoke(() => { Jobs.Add(jobViewModel); });
                }
            }

            foreach (var existingJobKey in existingJobLookup.Keys)
            {
                if (!newJobs.ContainsKey(existingJobKey.Value))
                    Application.Current.Dispatcher.Invoke(() => { Jobs.Remove(existingJobLookup[existingJobKey]); });
            }
        }

        private void CompleteJob()
        {
            SelectedJob?.Complete();
        }

        private void StartJob()
        {
            SelectedJob?.Start();
        }

        private void CancelJob()
        {
            SelectedJob?.CancelJob();
            Application.Current.Dispatcher.Invoke(() =>
            {
                SelectedJob = null;
            });
        }
    }
}
