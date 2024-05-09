namespace DeliverySystem.Bits.Base;

public class BitsFiles: List<BitsFile>, IDisposable
{
    private IEnumBackgroundCopyFiles fileList;
    private BitsJob job;
    private bool disposed;

    internal BitsFiles(BitsJob job, IEnumBackgroundCopyFiles fileList)
    {
        this.fileList = fileList;
        this.job = job;
        Refresh();
    }

    internal void Refresh()
    {
        uint count;
        IBackgroundCopyFile currentFile;
        uint fetchedCount = 0;
        fileList.Reset();
        Clear();
        fileList.GetCount(out count);
        for (int i = 0; i < count; i++)
        {
            fileList.Next(1, out currentFile, out fetchedCount);
            if (fetchedCount == 1)
            {
                Add(new BitsFile(job, currentFile));
            }
        }
    }

    #region IDisposable Members

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                //TODO: release COM resource
            }
        }
        disposed = true;
    }


    #endregion
}
