namespace ServiceInsight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Threading.Tasks;

    public class NetworkOperations
    {
        public Task<IList<string>> GetMachines() => Task.Run(() => NetworkBrowser.GetNetworkComputers());

        public void Browse(string productUrl)
        {
            var process = new Process { StartInfo = { UseShellExecute = true, FileName = productUrl } };
            process.Start();
        }

        public void OpenContactUs()
        {
            Browse("https://particular.net/contactus");
        }

        public void OpenExtendLicense(bool firstTime = false)
        {
            Browse($"https://particular.net/license/nservicebus?t={(firstTime ? 0 : 1)}&p=serviceinsight");
        }

        class NetworkBrowser
        {
            [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            static extern int NetServerEnum(string serverName, int dwLevel, ref IntPtr pBuf, int dwPrefMaxLen, out int dwEntriesRead, out int dwTotalEntries, int dwServerType, string domain, out int dwResumeHandle);

            [DllImport("Netapi32", SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            static extern int NetApiBufferFree(IntPtr pBuf);

            [StructLayout(LayoutKind.Sequential)]
            struct ServerInfo
            {
                internal int sv100_platform_id;

                [MarshalAs(UnmanagedType.LPWStr)]
                internal string sv100_name;
            }

            public static IList<string> GetNetworkComputers()
            {
                var networkComputers = new List<string>();
                var mAX_PREFERRED_LENGTH = -1;
                var sV_TYPE_WORKSTATION = 1;
                var sV_TYPE_SERVER = 2;
                var buffer = IntPtr.Zero;
                var sizeofINFO = Marshal.SizeOf(typeof(ServerInfo));

                try
                {
                    var result = NetServerEnum(
                        null,
                        100,
                        ref buffer,
                        mAX_PREFERRED_LENGTH,
                        out _,
                        out int totalEntries,
                        sV_TYPE_WORKSTATION | sV_TYPE_SERVER,
                        null,
                        out _);
                    if (result == 0)
                    {
                        for (var i = 0; i < totalEntries; i++)
                        {
                            var tmpBuffer = new IntPtr((long)buffer + (i * sizeofINFO));
                            var svrInfo = (ServerInfo)Marshal.PtrToStructure(tmpBuffer, typeof(ServerInfo));
                            networkComputers.Add(svrInfo.sv100_name);
                        }
                    }
                }
                catch (Exception)
                {
                    return networkComputers;
                }
                finally
                {
                    NetApiBufferFree(buffer);
                }
                return networkComputers;
            }
        }
    }
}