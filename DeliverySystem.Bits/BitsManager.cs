using System.Runtime.InteropServices;
using DeliverySystem.Bits.Enums;

namespace DeliverySystem.Bits.Base;

/// <summary>
/// Use the IBackgroundCopyManager interface to create transfer jobs, 
/// retrieve an enumerator object that contains the jobs in the queue, 
/// and to retrieve individual jobs from the queue.
/// </summary>
public class BitsManager : IDisposable
{
    private readonly IBackgroundCopyManager _manager;
    private readonly BitsNotification _notificationHandler;
    private BitsJobs _jobs;
    internal JobOwner? _currentOwner;
    private bool _disposed;

    public BitsManager()
    {
        // Set threading apartment
        Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
        int hResult = NativeMethods.CoInitializeSecurity(IntPtr.Zero, -1, IntPtr.Zero, IntPtr.Zero, RpcAuthnLevel.Connect, RpcImpLevel.Impersonate, IntPtr.Zero, EoAuthnCap.None, IntPtr.Zero);

        var managerInstance = new BackgroundCopyManager();
        if (managerInstance != null && managerInstance is IBackgroundCopyManager)
            _manager = (IBackgroundCopyManager)managerInstance;
        else
            throw new NotImplementedException("The bits infrastructure is not what was expected...");

        _jobs = new BitsJobs(this); // will be set correctly later after initialization
        _notificationHandler = new BitsNotification(this);
        _notificationHandler.OnJobErrored += OnJobErrorEvent;
        _notificationHandler.OnJobModified += OnJobModifiedEvent;
        _notificationHandler.OnJobTransferredEvent += OnJobTransferredEvent;
    }

    #region event handler for notication interface
    private void OnJobTransferredEvent(object? sender, NotificationEventArgs e)
    {
        if (!e.Job.TryGetJobId(out Guid? jobId) || jobId == null)
            return;

        // route the event to the job
        if (_jobs.TryGetValue(jobId.Value, out BitsJob? job))
            job.JobTransferred(sender, e);
        
        //publish the event to other subscribers
        OnJobTransferred?.Invoke(sender, e);
    }

    private void OnJobModifiedEvent(object? sender, NotificationEventArgs e)
    {
        if (!e.Job.TryGetJobId(out Guid? jobId) || jobId == null)
            return;

        // route the event to the job
        if (_jobs.TryGetValue(jobId.Value, out BitsJob? job))
            job.JobModified(sender, e);

        //publish the event to other subscribers
        OnJobModified?.Invoke(sender, e);
    }

    private void OnJobErrorEvent(object? sender, ErrorNotificationEventArgs e)
    {
        if (!e.Job.TryGetJobId(out Guid? jobId) || jobId == null)
            return;

        // route the event to the job
        if (_jobs.TryGetValue(jobId.Value, out BitsJob? job))
            job.JobError(sender, e);

        //publish the event to other subscribers
        OnJobError?.Invoke(sender, e);
    }
    #endregion

    public BitsJobs EnumJobs()
    {
        return EnumJobs(JobOwner.CurrentUser);
    }

    public BitsJobs EnumJobs(JobOwner jobOwner)
    {
        _currentOwner = jobOwner;

        _manager.EnumJobs((UInt32)jobOwner, out IEnumBackgroundCopyJobs? jobList);
        if (_jobs == null)
        {
            _jobs = new BitsJobs(this, jobList);
        }
        else
        {
            _jobs.Update(jobList);
        }

        return _jobs;
    }

    /// <summary>
    /// Creates a new transfer job.
    /// </summary>
    /// <param name="displayName">Null-terminated string that contains a display name for the job. 
    /// Typically, the display name is used to identify the job in a user interface. 
    /// Note that more than one job may have the same display name. Must not be NULL. 
    /// The name is limited to 256 characters, not including the null terminator.</param>
    /// <param name="jobType"> Type of transfer job, such as JobType.Download. For a list of transfer types, see the JobType enumeration</param>
    /// <returns></returns>
    public BitsJob CreateJob(string displayName, JobType jobType)
    {
        _manager.CreateJob(displayName, (BG_JOB_TYPE)jobType, out Guid guid, out IBackgroundCopyJob pJob);
        BitsJob job;
        lock (_jobs)
        {
            job = new BitsJob(this, pJob);
            _jobs.Add(guid, job);
        }

        OnJobAdded?.Invoke(this, new NotificationEventArgs(job));

        return job; 
    }

    public BitsJobs Jobs => _jobs;

    #region convert HResult into meaningful error message
    public string GetErrorDescription(int hResult)
    {
        string description;
        _manager.GetErrorDescription(hResult, Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
        return description;
    }
    #endregion

    #region notification interface

    #region internal notification handling
    internal BitsNotification NotificationHandler
    {
        get { return _notificationHandler; }
    }

    internal void NotifyOnJobRemoval(BitsJob job)
    {
        OnJobRemoved?.Invoke(this, new NotificationEventArgs(job));
    }

    internal void PublishException(BitsJob job, COMException exception)
    {
        if (OnInterfaceError != null)
        {
            var description = GetErrorDescription(exception.ErrorCode);
            OnInterfaceError(this, new BitsInterfaceNotificationEventArgs(job, exception, description));
        }

    }
    #endregion

    #region public events
    public event EventHandler<NotificationEventArgs>? OnJobModified;

    public event EventHandler<NotificationEventArgs>? OnJobTransferred;

    public event EventHandler<ErrorNotificationEventArgs>? OnJobError;

    public event EventHandler<NotificationEventArgs>? OnJobAdded;

    public event EventHandler<NotificationEventArgs>? OnJobRemoved;

    public event EventHandler<BitsInterfaceNotificationEventArgs>? OnInterfaceError;
    #endregion

    #endregion

    internal IBackgroundCopyManager BackgroundCopyManager => _manager;

    #region util methods
    public static BitsVersion BitsVersion => Utils.BITSVersion;

    #endregion

    #region IDisposable Members

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
                foreach (BitsJob job in Jobs.Values)
                {
                    job.Dispose();
                }

                _jobs.Clear();
                _jobs.Dispose();
                if (_manager != null)
#pragma warning disable CA1416 // Validate platform compatibility
                    Marshal.ReleaseComObject(_manager);
#pragma warning restore CA1416 // Validate platform compatibility
            }
        }
        _disposed = true;
    }
    #endregion
}
