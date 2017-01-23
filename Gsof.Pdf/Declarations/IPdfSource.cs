using System.IO;

namespace Gsof.Pdf.Declarations
{
    public interface IPdfSource
    {
        Stream Open();
    }
}