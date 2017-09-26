using System;
using System.Collections.Generic;
using System.Text;

namespace Gsof.Shared.WinApi
{
    [Flags]
    public enum ThreadExecutionState : uint
    {
        System = 0x00000001,
        Display = 0x00000002,
        Continus = 0x80000000,
    }
}
