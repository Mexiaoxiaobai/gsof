using System;
using System.Collections.Generic;
using System.Text;

namespace Gsof.Shared.WinApi
{
    public class Kernel
    {
        public static void SetThreadExecutionState(ThreadExecutionState state)
        {
            NativeMethods.SetThreadExecutionState((uint)state);
        }
    }
}
