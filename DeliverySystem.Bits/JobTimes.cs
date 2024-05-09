namespace DeliverySystem.Bits.Base;

public class JobTimes
{
    private BG_JOB_TIMES _jobTimes;

    internal JobTimes(BG_JOB_TIMES jobTimes)
    {
        _jobTimes = jobTimes;
    }

    public DateTime CreationTime => Utils.FileTime2DateTime(_jobTimes.CreationTime);

    public DateTime ModificationTime => Utils.FileTime2DateTime(_jobTimes.ModificationTime);

    public DateTime TransferCompletionTime => Utils.FileTime2DateTime(_jobTimes.TransferCompletionTime);
}
