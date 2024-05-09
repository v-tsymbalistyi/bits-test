using System.Runtime.InteropServices;

namespace DeliverySystem.Bits.Base;

#region Notification Event Arguments
public class JobNotificationEventArgs : EventArgs
{
}

public class JobErrorNotificationEventArgs : JobNotificationEventArgs
{

    private BitsError error;

    internal JobErrorNotificationEventArgs(BitsError error)
        : base()
    {
        this.error = error;
    }

    public BitsError Error
    {
        get { return error; }
    }
}

public class NotificationEventArgs : JobNotificationEventArgs
{
    private BitsJob job;

    internal NotificationEventArgs(BitsJob job)
    {
        this.job = job;
    }

    public BitsJob Job
    {
        get { return job; }
    }
}

public class ErrorNotificationEventArgs : NotificationEventArgs
{

    private BitsError error;

    internal ErrorNotificationEventArgs(BitsJob job, BitsError error)
        : base(job)
    {
        this.error = error;
    }

    public BitsError Error
    {
        get { return error; }
    }
}

public class BitsInterfaceNotificationEventArgs : NotificationEventArgs
{
    private COMException exception;
    private string description;

    internal BitsInterfaceNotificationEventArgs(BitsJob job, COMException exception, string description)
        : base(job)
    {
        this.description = description;
        this.exception = exception;
    }

    public string Message
    {
        get { return exception.Message; }
    }

    public string Description
    {
        get { return description; }
    }

    public int HResult
    {
        get { return exception.ErrorCode; }
    }
}

#endregion

internal class BitsNotification : IBackgroundCopyCallback
{
    #region IBackgroundCopyCallback Members
    private BitsManager _manager;

    internal BitsNotification(BitsManager manager)
    {
        _manager = manager;
    }

    public void JobTransferred(IBackgroundCopyJob pJob)
    {
        BitsJob job;
        if (OnJobTransferredEvent != null)
        {
            pJob.GetId(out Guid guid);
            if (_manager.Jobs.TryGetValue(guid, out BitsJob? value))
            {
                job = value;
            }
            else
            {
                // Update Joblist to check whether the job still exists. If not, just return
                var jobs = EnumManagerJobs();
                if (jobs.ContainsKey(guid))
                {
                    job = _manager.Jobs[guid];
                }
                else
                    return;
            }

            OnJobTransferredEvent.Invoke(this, new NotificationEventArgs(job));
            //forward event
            if (job._notificationTarget != null)
                job._notificationTarget.JobTransferred(pJob);
        }
    }

    public void JobError(IBackgroundCopyJob pJob, IBackgroundCopyError pError)
    {
        BitsJob job;
        if (OnJobErrored != null)
        {
            Guid guid;
            pJob.GetId(out guid);
            if (_manager.Jobs.ContainsKey(guid))
            {
                job = _manager.Jobs[guid];
            }
            else
            {
                var jobs = EnumManagerJobs();

                if (jobs.TryGetValue(guid, out BitsJob? value))
                {
                    job = value;
                }
                else
                    return;
            }
            OnJobErrored.Invoke(this, new ErrorNotificationEventArgs(job, new BitsError(job, pError)));
            //forward event
            if (job._notificationTarget != null)
                job._notificationTarget.JobError(pJob, pError);
        }
    }

    private BitsJobs EnumManagerJobs()
    {
        // Update Joblist to check whether the job still exists. If not, just return
        return _manager._currentOwner == null
            ? _manager.EnumJobs()
            : _manager.EnumJobs(_manager._currentOwner.Value);
    }

    public void JobModification(IBackgroundCopyJob pJob, uint dwReserved)
    {
        BitsJob job;
        if (OnJobModified != null)
        {
            Guid guid;
            pJob.GetId(out guid);
            if (_manager.Jobs.ContainsKey(guid))
            {
                job = _manager.Jobs[guid];
            }
            else
            {
                // Update Joblist to check whether the job still exists. If not, just return

                var jobs = EnumManagerJobs();
                if (jobs.ContainsKey(guid))
                {
                    job = _manager.Jobs[guid];
                }
                else
                    return;
            }
            OnJobModified.Invoke(this, new NotificationEventArgs(job));
            //forward event
            if (job._notificationTarget != null)
                job._notificationTarget.JobModification(pJob, dwReserved);
        }
    }

    public event EventHandler<NotificationEventArgs>? OnJobModified;

    public event EventHandler<NotificationEventArgs>? OnJobTransferredEvent;

    public event EventHandler<ErrorNotificationEventArgs>? OnJobErrored;
    #endregion
}