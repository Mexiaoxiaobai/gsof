using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Gsof.Extensions;
using Gsof.Pdf.Declarations;
using Gsof.Shared.Extensions;

namespace Gsof.Xaml.PdfViewer
{
    public class PdfBox : ItemsControl
    {
        private IPdf _pdf;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            int index = 0;
            var buffer = _pdf.Extrac(index);

            buffer.ChangeToBgra32();

            //WriteableBitmap wb = new WriteableBitmap();
        }
    }
}