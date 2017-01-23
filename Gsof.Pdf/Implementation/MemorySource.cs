using System.IO;
using Gsof.Pdf.Declarations;

namespace Gsof.Pdf.Implementation
{
    public class MemorySource : IPdfSource
    {
        public byte[] Buffer { get; }

        public MemorySource(byte[] p_buffer)
        {
            Buffer = p_buffer;
        }

        public Stream Open()
        {
            return new MemoryStream(Buffer);
        }
    }
}