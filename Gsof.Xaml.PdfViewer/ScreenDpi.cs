// ReSharper disable InconsistentNaming

using System.Windows;
using System.Windows.Media;
using Gsof.Pdf.Declarations;
using Gsof.Pdf.Structs;

namespace Gsof.Xaml.PdfViewer
{
    public class ScreenDpi : IDpi
    {
        public const float DEFAULT_DPI = 96;

        public Dpi GetDpi()
        {
            Visual visual = Application.Current.MainWindow;
            if (visual == null)
            {
                visual = Application.Current.Windows[0];
            }

            PresentationSource source = PresentationSource.FromVisual(visual);

            double dpiX = 1.0d;
            double dpiY = 1.0d;
            if (source != null && source.CompositionTarget != null)
            {
                dpiX = DEFAULT_DPI * source.CompositionTarget.TransformToDevice.M11;
                dpiY = DEFAULT_DPI * source.CompositionTarget.TransformToDevice.M22;
            }

            var dpi = new Dpi(dpiX, dpiY);
            return dpi;
        }
    }
}
