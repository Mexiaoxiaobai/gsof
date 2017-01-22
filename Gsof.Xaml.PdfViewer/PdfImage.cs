using System.Windows;
using System.Windows.Media;

namespace Gsof.Xaml.PdfViewer
{
	internal class PdfImage
	{
		public ImageSource ImageSource { get; set; }
        // we use only the "Right"-property of "Thickness", but we choose the "Thickness" structure instead of a simple double, because it makes data binding easier.
        public Thickness Margin { get; set; }
	}
}
