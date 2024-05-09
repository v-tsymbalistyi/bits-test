namespace DeliverySystem.Bits.Base;

public class JobReplyProgress
{
    private BG_JOB_REPLY_PROGRESS _jobReplyProgress;

    internal JobReplyProgress(BG_JOB_REPLY_PROGRESS jobReplyProgress)
    {
        _jobReplyProgress = jobReplyProgress;
    }

    public ulong BytesTotal => _jobReplyProgress.BytesTotal == ulong.MaxValue ? 0 : _jobReplyProgress.BytesTotal;

    public ulong BytesTransferred => _jobReplyProgress.BytesTransferred;

}
