using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using DeliverySystem.Bits.Enums;

namespace DeliverySystem.Bits.Base;

public partial class BitsJob: IDisposable
{
    #region member fields
    private IBackgroundCopyJob _job;
    private readonly BitsManager _manager;
    private bool disposed;

    private BitsFiles? _cachedFiles;
    private JobTimes? _jobTimes;
    private ProxySettings? _cachedProxySettings;
    private BitsError? _cachedLastError;
    private JobProgress? _cachedLastProgress;
    private Guid? _jobId;
    //notification
    internal IBackgroundCopyCallback? _notificationTarget;
    #endregion

    #region .ctor
    internal BitsJob(BitsManager manager, IBackgroundCopyJob job)
    {
        _manager = manager;
        _job = job;
        _job2 = (IBackgroundCopyJob2)_job;
        _job3 = (IBackgroundCopyJob3)_job;
        _job4 = _job as IBackgroundCopyJob4;

        ///store existing notification handler and route message to this as well
        ///otherwise it may break system download jobs
        
        if (TryGetNotificationInterface(out var notificationInterface) && notificationInterface != null)
            _notificationTarget = notificationInterface;   //pointer to the existing one;

        TrySetNotificationInterface(manager.NotificationHandler);   //notification interface will be disabled when NotifyCmd is set
    }

    /// <summary>
    /// Display Name, max 256 chars
    /// </summary>
    public string GetDisplayName()
    {
        try
        {
            _job.GetDisplayName(out string displayName);
            return displayName;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return string.Empty;
        }
    }

    /// <summary>
    /// Display Name, max 256 chars
    /// </summary>
    public void SetDisplayName(string value)
    {
        try
        {
            _job.SetDisplayName(value);
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
        }
    }

    /// <summary>
    /// Description, max 1024 chars
    /// </summary>
    public bool TryGetDescription(out string description)
    {
        try
        {
            _job.GetDescription(out description);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            description = string.Empty;
            return false;
        }
    }

    /// <summary>
    /// Description, max 1024 chars
    /// </summary>
    public bool TrySetDescription(string value)
    {
        try
        {
            _job.SetDescription(value);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    /// <summary>
    /// SID of the job owner
    /// </summary>
    public bool TryGetOwnerSID(out string owner)
    {
        try
        {
            _job.GetOwner(out owner);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            owner = string.Empty;
            return false;
        }
    }

    /// <summary>
    /// resolved owner name from job owner SID
    /// </summary>
    public string? GetOwnerName()
    {
        try
        {
            return TryGetOwnerSID(out string ownerSID)
                ? Utils.GetName(ownerSID)
                : null;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return null;
        }
    }

    /// <summary>
    /// Job priority
    /// can not be set for jobs already in state Canceled or Acknowledged
    /// </summary>
    public bool TryGetPriority(out JobPriority? jobPriority)
    {
        try
        {
            _job.GetPriority(out var priority);
            jobPriority = (JobPriority?)priority;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            jobPriority = null;
            return false;
        }
    }

    /// <summary>
    /// Job priority
    /// can not be set for jobs already in state Canceled or Acknowledged
    /// </summary>
    public bool TrySetPriority(JobPriority value)
    {
        try
        {
            _job.SetPriority((BG_JOB_PRIORITY)value);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool TryGetProgress(out JobProgress? lastJobProgress)
    {
        try
        {
            _job.GetProgress(out BG_JOB_PROGRESS jobProgress);
            _cachedLastProgress = new JobProgress(jobProgress);
            lastJobProgress = _cachedLastProgress;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            lastJobProgress = _cachedLastProgress;
            return false;
        }
    }

    public bool TryRefreshFilesList(out BitsFiles? files)
    {
        try
        {
            _job.EnumFiles(out IEnumBackgroundCopyFiles? fileList);
            _cachedFiles = new BitsFiles(this, fileList);
            files = _cachedFiles;
            return true;
        }
        catch (COMException exception)
        {                
            _manager.PublishException(this, exception);
            files = _cachedFiles;
            return false;
        }
    }

    public BitsFiles? CachedFiles => _cachedFiles;

    public bool TryGetErrorCount(out ulong? errorCount)
    {
        try
        {
            _job.GetErrorCount(out var count);
            errorCount = count;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            errorCount = null;
            return false;
        }
    }

    public bool TryGetLastError(out BitsError? lastError)
    {
        try
        {
            if (TryGetState(out var state) && state == JobState.Error || state == JobState.TransientError)
            {
                if (_cachedLastError == null)
                {
                    _job.GetError(out IBackgroundCopyError error);
                    if (error != null)
                    {
                        _cachedLastError = new BitsError(this, error);
                    }
                }
            }
            lastError = _cachedLastError;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            lastError = _cachedLastError;
            return false;
        }
    }

    public bool TryGetMinimumRetryDelay(out uint? retryDelaySeconds)
    {
        try
        {
            _job.GetMinimumRetryDelay(out var seconds);
            retryDelaySeconds = seconds;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            retryDelaySeconds = null;
            return false;
        }
    }

    public bool TrySetMinimumRetryDelay(uint retryDelaySeconds)
    {
        try
        {
            _job.SetMinimumRetryDelay(retryDelaySeconds);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool GetNoProgressTimeout(out uint? timeoutSeconds)
    {
        try
        {
            _job.GetNoProgressTimeout(out var seconds);
            timeoutSeconds = seconds;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            timeoutSeconds = null;
            return false;
        }
    }

    public bool TrySetNoProgressTimeout(uint value)
    {
        try
        {
            _job.SetNoProgressTimeout(value);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool TryGetJobId(out Guid? jobId)
    {
        try
        {
            if (_jobId == null)
            {
                _job.GetId(out var queriedJobId);
                _jobId = queriedJobId;
            }
            
            jobId = _jobId;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            jobId = null;
            return false;
        }
    }

    public bool TryGetState(out JobState? jobState)
    {
        try
        {
            _job.GetState(out var state);
            jobState = (JobState?)state;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            jobState = null;
            return false;
        }
    }

    public bool TryGetJobTimes(out JobTimes? jobTimes)
    {
        try
        {
            _job.GetTimes(out BG_JOB_TIMES times);
            _jobTimes = new JobTimes(times);
            jobTimes = _jobTimes;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            jobTimes = null;
            return false;
        }
    }

    public bool TryGetJobType(out JobType jobType)
    {
        try
        {
            _job.GetType(out var bgJobType);
            jobType = (JobType)bgJobType;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            jobType = (JobType)BG_JOB_TYPE.BG_JOB_TYPE_UNKNOWN;
            return false;
        }
    }

    public ProxySettings GetProxySettings()
    {
        _cachedProxySettings ??= new ProxySettings(_job);
        return _cachedProxySettings;
    }

    public bool TrySuspend()
    {
        try
        {
            _job.Suspend();
            return true;
        }
        catch (COMException exception)
        {                
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool TryResume()
    {
        try
        {
            _job.Resume();
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool TryCancel()
    {
        try
        {
            _job.Cancel();
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool TryComplete()
    {
        try
        {
            _job.Complete();
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool TryTakeOwnership()
    {
        try
        {
            _job.TakeOwnership();
            return true;
        }
        catch (COMException exception) 
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool TryAddFile(string remoteName, string localName)
    {
        try
        {
            _job.AddFile(remoteName, localName);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool TryAddFile(BitsFileInfo fileInfo)
    {
        return
            fileInfo == null
            ? throw new ArgumentNullException(nameof(fileInfo))
            : TryAddFile(fileInfo.RemoteName, fileInfo.LocalName);
    }

    internal bool TryAddFiles(BG_FILE_INFO[] files)
    {
        try
        {
            uint count = Convert.ToUInt32(files.Length);
            _job.AddFileSet(count, files);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    public bool TryAddFiles(Collection<BitsFileInfo> files)
    {
        var fileArray = new BG_FILE_INFO[files.Count];
        for (int i = 0; i < files.Count; i++)
        {
            fileArray[i] = files[i]._BG_FILE_INFO;
        }

        return TryAddFiles(fileArray);
    }

    public bool TryGetNotificationFlags(out NotificationFlags? notificationFlags)
    {
        try
        {
            _job.GetNotifyFlags(out var flags);
            notificationFlags = (NotificationFlags?)flags;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            notificationFlags = null;
            return false;
        }
    }

    public bool TrySetNotificationFlags(NotificationFlags value)
    {
        try
        {
            _job.SetNotifyFlags((BG_JOB_NOTIFICATION_TYPE)value);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    internal bool TryGetNotificationInterface(out IBackgroundCopyCallback? backgroundCopyCallback)
    {
        try
        {
            _job.GetNotifyInterface(out var notificationInterface);
            backgroundCopyCallback = (IBackgroundCopyCallback?)notificationInterface;
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            backgroundCopyCallback = null;
            return false;
        }
        //TODO: investigate
        catch (InvalidCastException exception)
        {
            //_manager.PublishException(this, exception);
            backgroundCopyCallback = null;
            return false;
        }
    }

    internal bool TrySetNotificationInterface(IBackgroundCopyCallback? value)
    {
        try
        {
            _job.SetNotifyInterface(value);
            return true;
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return false;
        }
    }

    #endregion

    #region notification
    internal void JobTransferred(object? sender, NotificationEventArgs e)
    {
        OnJobTransferred?.Invoke(sender, new JobNotificationEventArgs());
    }

    internal void JobModified(object? sender, NotificationEventArgs e)
    {
        OnJobModified?.Invoke(sender, new JobNotificationEventArgs());
    }

    internal void JobError(object? sender, ErrorNotificationEventArgs e)
    {
        OnJobError?.Invoke(sender, new JobErrorNotificationEventArgs(e.Error));
    }
    #endregion

    #region public events
    public event EventHandler<JobNotificationEventArgs>? OnJobModified;

    public event EventHandler<JobNotificationEventArgs>? OnJobTransferred;

    public event EventHandler<JobErrorNotificationEventArgs>? OnJobError;
    #endregion

    #region internal
    internal IBackgroundCopyJob Job
    {
        get { return _job; }
    }

    internal void PublishException(COMException exception)
    {
        _manager.PublishException(this, exception);
    }
    #endregion

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
                if (_notificationTarget != null)
                    TrySetNotificationInterface(_notificationTarget);
                if (_cachedFiles != null)
                    _cachedFiles.Dispose();
            }
        }
        disposed = true;
    }
    #endregion
}
