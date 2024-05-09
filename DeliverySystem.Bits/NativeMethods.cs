using System.Runtime.InteropServices;
using System.Text;
using DeliverySystem.Bits.Enums;

namespace DeliverySystem.Bits;

internal static class NativeMethods
{
    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ConvertStringSidToSidW(string stringSID, ref nint SID);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool LookupAccountSidW(string systemName, nint SID,
        StringBuilder name, ref long cbName, StringBuilder domainName, ref long cbDomainName, ref int psUse);

    [DllImport("ole32.dll", CharSet = CharSet.Auto)]
    public static extern int CoInitializeSecurity(nint pVoid, int cAuthSvc, nint asAuthSvc, nint pReserved1, RpcAuthnLevel level,
        RpcImpLevel impers, nint pAuthList, EoAuthnCap dwCapabilities, nint pReserved3);
}
