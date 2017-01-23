using System.IO;
using Gsof.Pdf.Declarations;

namespace Gsof.Pdf.Implementation
{
    public class FileSource : IPdfSource
    {
        public string FileName { get; }

        public FileSource(string p_filename)
        {
            FileName = p_filename;
        }

        public Stream Open()
        {
            return File.Open(FileName, FileMode.Open, FileAccess.Read);
        }
    }
}