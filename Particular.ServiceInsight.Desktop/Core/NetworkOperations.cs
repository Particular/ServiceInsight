using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace Particular.ServiceInsight.Desktop.Core
{
    public class NetworkOperations : INetworkOperations
    {
        public Task<IList<string>> GetMachines()
        {
            return Task.Run(() => NetworkBrowser.GetNetworkComputers());
        }

        public void Browse(string productUrl)
        {
            var process = new Process {StartInfo = {UseShellExecute = true, FileName = productUrl}};
            process.Start();
        }

        private class NetworkBrowser
        {
            [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            private static extern int NetServerEnum(string serverName, int dwLevel, ref IntPtr pBuf, int dwPrefMaxLen, out int dwEntriesRead, out int dwTotalEntries, int dwServerType, string domain, out int dwResumeHandle);

            [DllImport("Netapi32", SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            private static extern int NetApiBufferFree(IntPtr pBuf);

            [StructLayout(LayoutKind.Sequential)]
            private struct ServerInfo
            {
                internal int sv100_platform_id;
                
                [MarshalAs(UnmanagedType.LPWStr)] 
                internal string sv100_name;
            }

            public static IList<string> GetNetworkComputers()
            {
                var networkComputers = new List<string>();
                var MAX_PREFERRED_LENGTH = -1;
                var SV_TYPE_WORKSTATION = 1;
                var SV_TYPE_SERVER = 2;
                var buffer = IntPtr.Zero;
                var tmpBuffer = IntPtr.Zero;
                var sizeofINFO = Marshal.SizeOf(typeof(ServerInfo));
                int entriesRead;
                int totalEntries;
                int resHandle;

                try
                {
                    var result = NetServerEnum(null, 100, ref buffer, MAX_PREFERRED_LENGTH,
                                            out entriesRead,
                                            out totalEntries, 
                                            SV_TYPE_WORKSTATION | SV_TYPE_SERVER, 
                                            null, 
                                            out resHandle);
                    if (result == 0)
                    {
                        for (var i = 0; i < totalEntries; i++)
                        {
                            tmpBuffer = new IntPtr((long) buffer + (i*sizeofINFO));
                            var svrInfo = (ServerInfo)Marshal.PtrToStructure(tmpBuffer, typeof (ServerInfo));
                            networkComputers.Add(svrInfo.sv100_name);
                        }
                    }
                }
                catch (Exception ex)
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