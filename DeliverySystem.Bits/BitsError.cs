using System.Runtime.InteropServices;
using DeliverySystem.Bits.Enums;

namespace DeliverySystem.Bits.Base;

//TODO: rework
public class BitsError
{
    private IBackgroundCopyError _error;
    private BitsJob _job;

    internal BitsError(BitsJob job, IBackgroundCopyError error)
    {
        if (null == error)
            throw new ArgumentNullException("error", "Parameter IBackgroundCopyError cannot be a null reference");
        _error = error;
        _job = job;
    }

    public string Description
    {
        get  
        {
            string description = string.Empty;
            try
            {
                _error.GetErrorDescription(Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
            }
            catch (COMException exception)
            {                    
                _job.PublishException(exception);
            }
            return description;
        }
    }

    public string ContextDescription
    {
        get
        {
            string description = string.Empty;
            try
            {
                _error.GetErrorContextDescription(Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
            }
            catch (COMException exception)
            {
                _job.PublishException(exception);
            }
            return description;
        }
    }

    public string Protocol
    {
        get
        {
            string protocol = string.Empty;
            try
            {
                _error.GetProtocol(out protocol);
            }
            catch (COMException exception)
            {
                _job.PublishException(exception);
            }
            return protocol;
        }
    }

    public BitsFile? File
    {
        get
        {
            IBackgroundCopyFile errorFile;
            try
            {
                _error.GetFile(out errorFile);
                return new BitsFile(_job, errorFile);
            }
            catch (COMException exception)
            {
                _job.PublishException(exception);
            }
            return null;    //couldn't create new job
        }
    }

    public ErrorContext ErrorContext
    {
        get 
        { 
            BG_ERROR_CONTEXT context;
            int errorCode;
            try
            {
                _error.GetError(out context, out errorCode);
                return (ErrorContext)context;
            }
            catch (COMException exception)
            {
                _job.PublishException(exception);
            }
            return ErrorContext.UnknownError;
        }
    }

    public int ErrorCode
    {
        get 
        { 
            BG_ERROR_CONTEXT context;
            int errorCode = 0;
            try
            {
                _error.GetError(out context, out errorCode);
                return errorCode;
            }
            catch (COMException exception)
            {
                _job.PublishException(exception);
            }
            return errorCode;
        }
    }

}
