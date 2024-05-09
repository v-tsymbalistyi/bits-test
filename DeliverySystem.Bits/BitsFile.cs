using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace DeliverySystem.Bits.Base;

public class BitsFile : IDisposable
{
    private IBackgroundCopyFile _file;
    private IBackgroundCopyFile2? _file2;
    private FileProgress? _progress;
    private bool _disposed;
    private BitsJob _job;

    internal BitsFile(BitsJob job, IBackgroundCopyFile file)
    {
        if (null == file)
            throw new ArgumentNullException("file", "Parameter IBackgroundCopyFile cannot be a null reference");
        _file = file;
        _file2 = file as IBackgroundCopyFile2;
        _job = job;
    }

    public string LocalName
    {
        get
        {
            string name = string.Empty;
            try
            {
                _file.GetLocalName(out name);
            }
            catch (COMException exception)
            {
                _job.PublishException(exception);
            }
            return name;
        }
    }

    public string RemoteName
    {
        get
        {
            string name = string.Empty;
            try
            {
                _file.GetRemoteName(out name);
            }
            catch (COMException exception)
            {
                _job.PublishException(exception);
            }
            return name;
        }
        set //supported in IBackgroundCopyFile2
        {
            try
            {
                if (_file2 != null)
                {
                    _file2.SetRemoteName(value);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyFile2");
                }
            }
            catch (COMException exception)
            {
                _job.PublishException(exception);
            }
        }
    }

    public FileProgress? Progress
    {
        get
        {
            if (null == _progress)
            {
                BG_FILE_PROGRESS fileProgress;
                try
                {
                    _file.GetProgress(out fileProgress);
                    _progress = new FileProgress(fileProgress);
                }
                catch (COMException exception)
                {
                    _job.PublishException(exception);
                }
            }
            return _progress;
        }
    }

    public Collection<FileRange> FileRanges
    {
        get
        {
            try
            {
                if (_file2 != null)
                {
                    uint count = 0;
                    Collection<FileRange> fileRanges = new Collection<FileRange>();
                    IntPtr rangePtr;
                    _file2.GetFileRanges(out count, out rangePtr);
                    for (int i = 0; i < count; i++)
                    {
                        BG_FILE_RANGE? range = (BG_FILE_RANGE?)Marshal.PtrToStructure(rangePtr, typeof(BG_FILE_RANGE));

                        //TODO: do something better
                        if (range == null)
                            throw new Exception("Something went wrong...");

                        fileRanges.Add(new FileRange(range.Value));
                        rangePtr = new IntPtr((int)rangePtr + Marshal.SizeOf(range));
                    }
                    return fileRanges;
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyFile2");
                }
            }
            catch (COMException exception)
            {
                _job.PublishException(exception);
                return new Collection<FileRange>();
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                //TODO: release COM resource
            }
        }
        _disposed = true;
    }
}
