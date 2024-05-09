using System.Text;
using System.Diagnostics;
using DeliverySystem.Bits.Base;
using DeliverySystem.Bits.Enums;

namespace DeliverySystem.Bits;

internal static class Utils
{
    private static BitsVersion _localBitsVersion = GetBitsVersion();

    //TODO revise for sensitive info
    internal static string GetName(string SID)
    {
        const int size = 255;
        long cbUserName = size;
        long cbDomainName = size;
        nint ptrSID = new nint(0);
        int psUse = 0;
        StringBuilder userName = new StringBuilder(size);
        StringBuilder domainName = new StringBuilder(size);
        if (NativeMethods.ConvertStringSidToSidW(SID, ref ptrSID))
        {
            if (NativeMethods.LookupAccountSidW(string.Empty, ptrSID, userName, ref cbUserName, domainName, ref cbDomainName, ref psUse))
            {
                return string.Concat(domainName.ToString(), "\\", userName.ToString());
            }
        }
        return string.Empty;
    }

    internal static FILETIME DateTime2FileTime(DateTime dateTime)
    {
        long fileTime = 0;
        if (dateTime != DateTime.MinValue)      //Checking for MinValue
            fileTime = dateTime.ToFileTime();
        FILETIME resultingFileTime = new FILETIME();
        resultingFileTime.dwLowDateTime = (uint)(fileTime & 0xFFFFFFFF);
        resultingFileTime.dwHighDateTime = (uint)(fileTime >> 32);
        return resultingFileTime;
    }

    internal static DateTime FileTime2DateTime(FILETIME fileTime)
    {
        if (fileTime.dwHighDateTime == 0 && fileTime.dwLowDateTime == 0)    //Checking for MinValue
            return DateTime.MinValue;

        long dateTime = ((long)fileTime.dwHighDateTime << 32) + fileTime.dwLowDateTime;
        return DateTime.FromFileTime(dateTime);
    }

    /// <summary>
    /// maps version information from file version
    /// version number returned by qmgr.dll
    /// 6.0.xxxx = BITS 1.0
    /// 6.2.xxxx = BITS 1.2
    /// 6.5.xxxx = BITS 1.5
    /// 6.6.xxxx = BITS 2.0
    /// 6.7.xxxx = BITS 2.5
    /// 7.0.xxxx = BITS 3.0
    /// 7.5.xxxx = BITS 4.0
    /// </summary>
    /// <returns></returns>
    private static BitsVersion GetBitsVersion()
    {
        int major = 0;
        int minor = 0;
        try
        {
            string fileName = Path.Combine(Environment.SystemDirectory, "qmgr.dll");
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(fileName);

            major = fileVersion.FileMajorPart;
            minor = fileVersion.FileMinorPart;
        }
        catch (FileNotFoundException)
        {
            return BitsVersion.Bits0_0;
        }

        return major switch
        {
            6 => minor switch
            {
                0 => BitsVersion.Bits1_0,
                2 => BitsVersion.Bits1_2,
                5 => BitsVersion.Bits1_5,
                6 => BitsVersion.Bits2_0,
                7 => BitsVersion.Bits2_5,
                _ => BitsVersion.Bits0_0,
            },
            7 => minor switch
            {
                0 => BitsVersion.Bits3_0,
                5 => BitsVersion.Bits4_0,
                _ => BitsVersion.Bits0_0,
            },
            _ => BitsVersion.Bits0_0,
        };
    }

    internal static BitsVersion BITSVersion => _localBitsVersion;
}
