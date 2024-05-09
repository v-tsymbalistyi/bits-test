namespace DeliverySystem.Bits.Base;

public class BitsJobs: Dictionary<Guid, BitsJob>, IDisposable
{
    private IEnumBackgroundCopyJobs? _jobList;
    private BitsManager _manager;
    private bool disposed;

    //only required for initialization
    internal BitsJobs(BitsManager manager)
    {
        _manager = manager;
    }

    internal BitsJobs(BitsManager manager, IEnumBackgroundCopyJobs jobList)
    {
        _manager = manager;
        UpdateInternal(jobList);
    }

    internal void Update(IEnumBackgroundCopyJobs jobList)
    {
        lock (this) //avoid threading issues on list updates
        {
            UpdateInternal(jobList);
        }
    }

    private void UpdateInternal(IEnumBackgroundCopyJobs jobList)
    {
        _jobList = jobList;
        Dictionary<Guid, BitsJob> currentList = [];
        
        foreach (KeyValuePair<Guid, BitsJob> entry in this)
        {
            currentList.Add(entry.Key, entry.Value);
        }

        _jobList.Reset();
        Clear();

        _jobList.GetCount(out uint count);
        for (int i = 0; i < count; i++)
        {
            _jobList.Next(1, out IBackgroundCopyJob currentJob, out uint fetchedCount);
            if (fetchedCount == 1)
            {
                currentJob.GetId(out Guid guid);

                BitsJob job;
                if (currentList.TryGetValue(guid, out BitsJob? value))
                {
                    job = value;
                    currentList.Remove(guid);
                }
                else
                {
                    job = new BitsJob(_manager, currentJob);
                }
                Add(guid, job);
            }
        }

        foreach (BitsJob disposeJob in currentList.Values)
        {
            _manager.NotifyOnJobRemoval(disposeJob);
            disposeJob.Dispose();
        }
    }

    internal IEnumBackgroundCopyJobs? Jobs => _jobList;

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
                _jobList = null;
            }
        }
        disposed = true;
    }

    #endregion
}
