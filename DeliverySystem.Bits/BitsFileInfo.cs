namespace DeliverySystem.Bits.Base;

public class BitsFileInfo
{
    private BG_FILE_INFO _fileInfo;

    internal BitsFileInfo(BG_FILE_INFO fileInfo)
    {
        _fileInfo = fileInfo;
    }

    public BitsFileInfo(string remoteName, string localName)
    {
        _fileInfo = new BG_FILE_INFO
        {
            RemoteName = remoteName,
            LocalName = localName
        };
    }

    public string RemoteName => _fileInfo.RemoteName;

    public string LocalName => _fileInfo.LocalName;

    internal BG_FILE_INFO _BG_FILE_INFO => _fileInfo;
}
