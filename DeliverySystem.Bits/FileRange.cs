namespace DeliverySystem.Bits.Base;

public class FileRange
{
    private readonly BG_FILE_RANGE _fileRange;

    internal FileRange(BG_FILE_RANGE fileRange)
    {
        _fileRange = fileRange;
    }

    public FileRange(ulong initialOffset, ulong length)
    {
        _fileRange = new BG_FILE_RANGE();
        _fileRange.InitialOffset = initialOffset;
        _fileRange.Length = length;
    }

    public ulong InitialOffset => _fileRange.InitialOffset;

    public ulong Length => _fileRange.Length;

    internal BG_FILE_RANGE _BG_FILE_RANGE => _fileRange;
}
