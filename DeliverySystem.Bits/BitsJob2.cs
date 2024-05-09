using System.Runtime.InteropServices;
using System.Globalization;
using DeliverySystem.Bits.Enums;

namespace DeliverySystem.Bits.Base;

public partial class BitsJob : IDisposable
{
    private IBackgroundCopyJob2 _job2;
    private JobReplyProgress? _replyProgress;

    #region public properties

    #region IBackgroundCopyJob2
    public void AddCredentials(BitsCredentials credentials)
    {
        try
        {
            if (_job2 != null && credentials != null)   // only supported from IBackgroundCopyJob2 and above
            {
                BG_AUTH_CREDENTIALS bgCredentials = new BG_AUTH_CREDENTIALS();
                bgCredentials.Scheme = (BG_AUTH_SCHEME)credentials.AuthenticationScheme;
                bgCredentials.Target = (BG_AUTH_TARGET)credentials.AuthenticationTarget;
                bgCredentials.Credentials.Basic.Password = credentials.Password.ToString();
                bgCredentials.Credentials.Basic.UserName = credentials.UserName.ToString();
                _job2.SetCredentials(ref bgCredentials);
            }
            else
            {
                throw new NotSupportedException("IBackgroundCopyJob2");
            }
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
        }
    }

    public void RemoveCredentials(BitsCredentials credentials)
    {
        try
        {
            if (_job2 != null && credentials != null) // only supported from IBackgroundCopyJob2 and above
            {
                _job2.RemoveCredentials((BG_AUTH_TARGET)credentials.AuthenticationTarget, (BG_AUTH_SCHEME)credentials.AuthenticationScheme);
            }
            else
            {
                throw new NotSupportedException("IBackgroundCopyJob2");
            }
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
        }
    }

    public void RemoveCredentials(AuthenticationTarget target, AuthenticationScheme scheme)
    {
        try
        {
            if (_job2 != null)// only supported from IBackgroundCopyJob2 and above
            {
                _job2.RemoveCredentials((BG_AUTH_TARGET)target, (BG_AUTH_SCHEME)scheme);
            }
            else
            {
                throw new NotSupportedException("IBackgroundCopyJob2");
            }
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
        }
    }

    public string NotifyCommandLineProgram
    {
        set
        {
            try
            {
                if (_job2 != null)
                {
                    if (null == value)  //removing command line, thus re-enabling the notification interface
                    {
                        _job2.SetNotifyCmdLine(null, null);
                        TrySetNotificationInterface(_manager.NotificationHandler);
                    }
                    else
                    {
                        TrySetNotificationInterface(null);
                        _job2.GetNotifyCmdLine(out string program, out string parameters);
                        _job2.SetNotifyCmdLine(value, parameters);
                    }
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
            }
        }
        get
        {
            try
            {
                if (_job2 != null)
                {
                    string program, parameters;
                    _job2.GetNotifyCmdLine(out program, out parameters);
                    return program;

                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
                return string.Empty;
            }
        }
    }

    public string NotifyCommandLineParameters
    {
        set
        {
            try
            {
                if (_job2 != null)
                {
                    string program, parameters;
                    _job2.GetNotifyCmdLine(out program, out parameters);
                    if (value != null)  //the command line program should be passed as first parameter, enclosed by quotes
                    {
                        value = string.Format(CultureInfo.InvariantCulture, "\"{0}\" {1}", program, value);
                    }
                    _job2.SetNotifyCmdLine(program, value);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
            }
        }
        get
        {
            try
            {
                if (_job2 != null)
                {
                    string program, parameters;
                    _job2.GetNotifyCmdLine(out program, out parameters);
                    return parameters;

                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
                return string.Empty;
            }
        }
    }

    public string ReplyFileName
    {
        get
        {
            try
            {
                if (_job2 != null)
                {
                    string replyFileName;
                    _job2.GetReplyFileName(out replyFileName);
                    return replyFileName;
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
                return string.Empty;
            }
        }
        set
        {
            try
            {
                if (_job2 != null)   // only supported from IBackgroundCopyJob2 and above
                {
                    _job2.SetReplyFileName(value);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
            }
        }
    }

    public byte[]? GetReplyData()
    {
        try
        {
            if (_job2 != null)
            {
                ulong length;
                IntPtr bufferPtr = new IntPtr();
                _job2.GetReplyData(bufferPtr, out length);
                Byte[] buffer = new Byte[length];
                Marshal.Copy(bufferPtr, buffer, 0, (int)length);    //truncating length to int may be ok as the buffer could be 1MB maximum
                return buffer;
            }
            else
            {
                throw new NotSupportedException("IBackgroundCopyJob2");
            }
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return null;
        }
    }

    public JobReplyProgress? GetReplyProgress()
    {
        try
        {
            if (_job2 != null)
            {
                _job2.GetReplyProgress(out BG_JOB_REPLY_PROGRESS jobReplyProgress);
                _replyProgress = new JobReplyProgress(jobReplyProgress);
            }
            else
            {
                throw new NotSupportedException("IBackgroundCopyJob2");
            }
        }
        catch (COMException exception)
        {
            _manager.PublishException(this, exception);
            return null;
        }
        return _replyProgress;
    }

    #endregion

    #endregion
}
