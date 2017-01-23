using Gsof.Pdf.Declarations;
using Gsof.Pdf.Structs;

namespace Gsof.Pdf.Implementation
{
    public class DefaultDpi : IDpi
    {
        public Dpi GetDpi()
        {
            return new Dpi(96, 96);
        }
    }
}