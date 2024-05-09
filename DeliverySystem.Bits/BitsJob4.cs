using System.Runtime.InteropServices;
using DeliverySystem.Bits.Enums;

namespace DeliverySystem.Bits.Base;

public partial class BitsJob : IDisposable
{
    private IBackgroundCopyJob4? _job4;

    #region public properties

    #region IBackgroundCopyJob4
    public ulong MaximumDownloadTime
    {
        get
        {
            ulong maxTime = 0;
            try
            {
                if (_job4 != null)// only supported from IBackgroundCopyJob4 and above
                {
                    _job4.GetMaximumDownloadTime(out maxTime);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob4");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
            }
            return maxTime;
        }
        set
        {
            try
            {
                if (_job4 != null)// only supported from IBackgroundCopyJob4 and above
                {
                    _job4.SetMaximumDownloadTime(value);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob4");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
            }
        }
    }

    public PeerCachingFlags PeerCachingFlags
    {
        get
        {
            PEER_CACHING_FLAGS peerCaching = 0;
            try
            {
                if (_job4 != null)// only supported from IBackgroundCopyJob4 and above
                {
                    _job4.GetPeerCachingFlags(out peerCaching);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob4");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
            }
            return (PeerCachingFlags)peerCaching;
        }
        set
        {
            try
            {
                if (_job4 != null)// only supported from IBackgroundCopyJob4 and above
                {
                    _job4.SetPeerCachingFlags((PEER_CACHING_FLAGS)value);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob4");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
            }
        }
    }

    public ulong OwnerIntegrityLevel
    {
        get
        {
            ulong integrityLevel = 0;
            try
            {
                if (_job4 != null)// only supported from IBackgroundCopyJob4 and above
                {
                    _job4.GetOwnerIntegrityLevel(out integrityLevel);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob4");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
            }
            return integrityLevel;
        }
    }

    public bool OwnerElevationState
    {
        get
        {
            bool elevated = false;
            try
            {
                if (_job4 != null)// only supported from IBackgroundCopyJob4 and above
                {
                    _job4.GetOwnerElevationState(out elevated);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob4");
                }
            }
            catch (COMException exception)
            {
                _manager.PublishException(this, exception);
            }
            return elevated;
        }
    }
    #endregion

    #endregion
}
