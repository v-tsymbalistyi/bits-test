namespace DeliverySystem.Bits.Base;

public class JobProgress
{
    private readonly BG_JOB_PROGRESS _jobProgress;

    internal JobProgress(BG_JOB_PROGRESS jobProgress)
    {
        _jobProgress = jobProgress;
    }

    public ulong BytesTotal => _jobProgress.BytesTotal == ulong.MaxValue ? 0 : _jobProgress.BytesTotal;

    public ulong BytesTransferred => _jobProgress.BytesTransferred;

    public uint FilesTotal => _jobProgress.FilesTotal;

    public uint FilesTransferred => _jobProgress.FilesTransferred;
}
