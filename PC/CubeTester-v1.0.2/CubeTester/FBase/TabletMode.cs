using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Drift.Classes
{
    public static class TabletMode
    {
        public static readonly Guid CLSID_ImmersiveShell = new Guid("C2F03A33-21F5-47FA-B4BB-156362A2F239");
        [ComImport()]
        [Guid("4FDA780A-ACD2-41F7-B4F2-EBE674C9BF2A")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITabletModeController
        {
            int GetMode(ref int mode);
            int SetMode(int mode, int modeTrigger);
        }
        [ComImport]
        [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IServiceProvider
        {
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object QueryService(ref Guid service, ref Guid riid);
        }

        public static void SwitchToTabletMode(bool enb)
        {
            try
            {
                var pSP = (IServiceProvider)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ImmersiveShell));
                var pTMC = (ITabletModeController)pSP.QueryService(typeof(ITabletModeController).GUID, typeof(ITabletModeController).GUID);
                if (pTMC != null)
                {
                    // 0 = Desktop, 1 = Tablet
                    int nMode = enb ? 1 : 0;
                    //int nRet = pTMC.GetMode(ref nMode);
                    int nRet = pTMC.SetMode(nMode, 4);
                }
            }
            catch
            {

            }

        }

        public static bool IsTabletMode()
        {
            var pSP = (IServiceProvider)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ImmersiveShell));
            var pTMC = (ITabletModeController)pSP.QueryService(typeof(ITabletModeController).GUID, typeof(ITabletModeController).GUID);
            if (pTMC != null)
            {
                int nMode = 0;
                int nRet = pTMC.GetMode(ref nMode);
                return nRet == 1;
            }

            return false;
        }
    }
}
