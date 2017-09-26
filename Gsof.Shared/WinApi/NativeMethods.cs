using System.Runtime.InteropServices;

namespace Gsof.WinApi
{
    internal class NativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern uint SetThreadExecutionState(uint esFlags);
    }
}
