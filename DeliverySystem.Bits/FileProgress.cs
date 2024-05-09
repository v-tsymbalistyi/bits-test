namespace DeliverySystem.Bits.Base;

public class FileProgress
{
    private readonly BG_FILE_PROGRESS _fileProgress;

    internal FileProgress(BG_FILE_PROGRESS fileProgress)
    {
        _fileProgress = fileProgress;
    }

    public ulong BytesTotal => _fileProgress.BytesTotal == ulong.MaxValue ? 0 : _fileProgress.BytesTotal;

    public ulong BytesTransferred => _fileProgress.BytesTransferred;

    public bool Completed => Convert.ToBoolean(_fileProgress.Completed);
}
