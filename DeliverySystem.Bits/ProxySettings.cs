using DeliverySystem.Bits.Enums;

namespace DeliverySystem.Bits.Base;

public class ProxySettings
{
    private BG_JOB_PROXY_USAGE _proxyUsage;
    private string _proxyList;
    private string _proxyBypassList;
    private IBackgroundCopyJob _job;

    internal ProxySettings(IBackgroundCopyJob job)
    {
        _job = job;
        job.GetProxySettings(out _proxyUsage, out _proxyList, out _proxyBypassList);
    }

    public ProxyUsage ProxyUsage
    {
        get => (ProxyUsage)_proxyUsage;
        set => _proxyUsage = (BG_JOB_PROXY_USAGE)value;
    }

    public string ProxyList
    {
        get => _proxyList;
        set => _proxyList = value;
    }

    public string ProxyBypassList
    {
        get => _proxyBypassList;
        set => _proxyBypassList = value;
    }

    //TODO: move to service
    public void Update()
    {
        _job.SetProxySettings(_proxyUsage, _proxyList, _proxyBypassList);
    }
}
