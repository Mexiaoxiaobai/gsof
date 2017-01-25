using System;
using System.Windows.Controls;
using Gsof.Pdf.Declarations;

namespace Gsof.Xaml.PdfViewer
{
    /// <summary>
    /// Common interface for the two different display types, single pages (SinglePageMoonPdfPanel) and continuous pages (ContinuousMoonPdfPanel)
    /// </summary>
    internal interface IMoonPdfPanel
    {
        ScrollViewer ScrollViewer { get; }
        UserControl Instance { get; }
        float CurrentZoom { get; }
        void Load(IPdfSource source, string password = null);
        void Unload();
        void Zoom(double zoomFactor);
        void ZoomIn();
        void ZoomOut();
        void ZoomToWidth();
        void ZoomToHeight();
        void GotoPage(int pageNumber);
        void GotoPreviousPage(bool p_isTop = true);
        void GotoNextPage();
        int GetCurrentPageIndex(ViewType viewType);

        event EventHandler CurrentPageChangedHandler;
    }
}
