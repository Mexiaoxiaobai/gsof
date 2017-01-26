using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gsof.Xaml.PdfViewer
{
    /// <summary>
    ///
    /// </summary>
    public class PdfBoxItem : ContentControl
    {
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(PdfBoxItem), new PropertyMetadata(null));

        static PdfBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfBoxItem), new FrameworkPropertyMetadata(typeof(PdfBoxItem)));
        }
    }
}
