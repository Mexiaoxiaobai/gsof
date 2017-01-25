using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gsof.Xaml.Extensions;

namespace Gsof.Xaml.PdfViewer
{
    /// <summary>
    /// 
    /// </summary>
    public class PdfViewer : Control
    {
        public ItemsPanelTemplate PagePanelTemplate
        {
            get { return (ItemsPanelTemplate)GetValue(PagePanelTemplateProperty); }
            set { SetValue(PagePanelTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PagePanelTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PagePanelTemplateProperty =
            DependencyProperty.Register("PagePanelTemplate", typeof(ItemsPanelTemplate), typeof(PdfViewer), new PropertyMetadata(ItemsPanelTemplateExtensions.CreateItemsPanelTemplate<VirtualizingStackPanel>()));
        

        static PdfViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfViewer), new FrameworkPropertyMetadata(typeof(PdfViewer)));
        }


    }
}
