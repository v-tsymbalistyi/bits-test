using DeliverySystem.Bits.Base;
using DeliverySystem.Bits.Enums;
using ReactiveUI;

namespace DeliverySystem.Host.Wpf.ViewModels
{
    public class BitsJobViewModel : ReactiveObject
    {
        private readonly BitsJob _bitsJob;
        private string? _ownerSID;
        private string? _ownerName;
        private string? _displayName;
        private Guid? _jobId;
        private JobType? _jobType;
        private JobState? _state;

        public BitsJobViewModel(BitsJob bitsJob)
        {
            _bitsJob = bitsJob;
        }

        public string? OwnerSID
        {
            get => _ownerSID;
            private set => this.RaiseAndSetIfChanged(ref _ownerSID, value);
        }
        public string? OwnerName
        {
            get => _ownerName;
            private set => this.RaiseAndSetIfChanged(ref _ownerName, value);
        }
        public string? DisplayName
        {
            get => _displayName;
            private set => this.RaiseAndSetIfChanged(ref _displayName, value);
        }
        public Guid? JobId
        {   
            get => _jobId;
            private set => this.RaiseAndSetIfChanged(ref _jobId, value);
        }
        public JobType? JobType
        {
            get => _jobType;
            private set => this.RaiseAndSetIfChanged(ref _jobType, value);
        }
        public JobState? State
        {
            get => _state;
            private set => this.RaiseAndSetIfChanged(ref _state, value);
        }

        public void UpdateInfo()
        {
            if (_bitsJob.TryGetJobId(out var jobId))
                JobId = jobId;

            if (_bitsJob.TryGetOwnerSID(out var ownerSID))
                OwnerSID = ownerSID;

            OwnerName = _bitsJob.GetOwnerName();

            DisplayName = _bitsJob.GetDisplayName();

            if (_bitsJob.TryGetJobType(out var jobType))
                JobType = jobType;

            if (_bitsJob.TryGetState(out var state))
                State = state;
        }

        public void CancelJob()
        {
            _bitsJob.TryCancel();
        }

        internal bool Start()
        {
            return _bitsJob.TryResume();
        }

        internal bool Complete()
        {
            return _bitsJob.TryComplete();
        }
    }
}
