using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gsof.Xaml.Extensions;
using Gsof.Xaml.PdfViewer.Helper;
using Gsof.Xaml.PdfViewer.MuPdf;

namespace Gsof.Xaml.PdfViewer
{
    internal partial class SinglePageMoonPdfPanel : UserControl, IMoonPdfPanel
    {
        private PdfPanel parent;
        private ScrollViewer scrollViewer;
        private PdfImageProvider imageProvider;
        private int _currentPageIndex = 0;

        private int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                if (_currentPageIndex == value)
                {
                    return;
                }

                _currentPageIndex = value;
                if (CurrentPageChangedHandler != null)
                {
                    CurrentPageChangedHandler(this, EventArgs.Empty);
                }
            }
        } // starting at 0

        public SinglePageMoonPdfPanel(Gsof.Xaml.PdfViewer.MoonPdfPanel parent)
        {
            InitializeComponent();
            //this.parent = parent;
            this.SizeChanged += SinglePageMoonPdfPanel_SizeChanged;
        }

        public SinglePageMoonPdfPanel(PdfPanel parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.SizeChanged += SinglePageMoonPdfPanel_SizeChanged;
        }

        void SinglePageMoonPdfPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.scrollViewer = this.ChildOfType<ScrollViewer>();
        }

        public void Load(IPdfSource source, string password = null)
        {
            this.scrollViewer = this.ChildOfType<ScrollViewer>();
            this.imageProvider = new PdfImageProvider(source, this.parent.TotalPages,
                new PageDisplaySettings(this.parent.GetPagesPerRow(), this.parent.ViewType, this.parent.HorizontalMargin, this.parent.Rotation), false, password);

            if (scrollViewer != null)
            {
                scrollViewer.RemoveHandler(UIElement.MouseWheelEvent, new MouseWheelEventHandler(this.OnScrollWheel));
                scrollViewer.AddHandler(UIElement.MouseWheelEvent, new MouseWheelEventHandler(this.OnScrollWheel), true);
            }

            this.CurrentPageIndex = 0;

            if (this.scrollViewer != null)
                this.scrollViewer.Visibility = System.Windows.Visibility.Visible;

            if (this.parent.ZoomType == ZoomType.Fixed)
                this.SetItemsSource();
            else if (this.parent.ZoomType == ZoomType.FitToHeight)
                this.ZoomToHeight();
            else if (this.parent.ZoomType == ZoomType.FitToWidth)
                this.ZoomToWidth();
        }

        private void OnScrollWheel(object sender, MouseWheelEventArgs e)
        {
            if (scrollViewer == null)
            {
                return;
            }

            if (Math.Abs(this.scrollViewer.VerticalOffset - this.scrollViewer.ScrollableHeight) < double.Epsilon
                && e.Delta <= 0)
            {
                ((IMoonPdfPanel)this).GotoNextPage();
            }
            else if (Math.Abs(this.scrollViewer.VerticalOffset) < double.Epsilon
                && e.Delta >= 0)
            {
                ((IMoonPdfPanel)this).GotoPreviousPage(false);
            }
        }

        public void Unload()
        {
            this.scrollViewer.Visibility = System.Windows.Visibility.Collapsed;
            this.scrollViewer.ScrollToHorizontalOffset(0);
            this.scrollViewer.ScrollToVerticalOffset(0);
            this.CurrentPageIndex = 0;

            this.imageProvider = null;
        }


        ScrollViewer IMoonPdfPanel.ScrollViewer
        {
            get { return this.scrollViewer; }
        }

        UserControl IMoonPdfPanel.Instance
        {
            get { return this; }
        }

        void IMoonPdfPanel.GotoPage(int pageNumber)
        {
            this.CurrentPageIndex = pageNumber;
            this.SetItemsSource();

            if (this.scrollViewer != null)
                this.scrollViewer.ScrollToTop();
        }

        void IMoonPdfPanel.GotoPreviousPage(bool p_isTop = true)
        {
            var prevPageIndex = PageHelper.GetPreviousPageIndex(this.CurrentPageIndex, this.parent.ViewType);

            if (prevPageIndex == -1)
                return;

            this.CurrentPageIndex = prevPageIndex;

            this.SetItemsSource();

            if (this.scrollViewer == null)
            {
                return;
            }

            if (p_isTop)
            {
                this.scrollViewer.ScrollToTop();
            }
            else
            {
                this.scrollViewer.ScrollToEnd();
            }
        }

        void IMoonPdfPanel.GotoNextPage()
        {
            var nextPageIndex = PageHelper.GetNextPageIndex(this.CurrentPageIndex, this.parent.TotalPages, this.parent.ViewType);

            if (nextPageIndex == -1)
                return;

            this.CurrentPageIndex = nextPageIndex;

            this.SetItemsSource();

            if (this.scrollViewer != null)
                this.scrollViewer.ScrollToTop();
        }


        public event EventHandler CurrentPageChangedHandler;

        private void SetItemsSource()
        {
            var startIndex = PageHelper.GetVisibleIndexFromPageIndex(this.CurrentPageIndex, this.parent.ViewType);

            this.itemsControl.ItemsSource = this.imageProvider.FetchRange(startIndex, this.parent.GetPagesPerRow()).FirstOrDefault();
        }

        public int GetCurrentPageIndex(ViewType viewType)
        {
            return this.CurrentPageIndex;
        }

        #region Zoom specific code
        public float CurrentZoom
        {
            get
            {
                if (this.imageProvider != null)
                    return this.imageProvider.Settings.ZoomFactor;

                return 1.0f;
            }
        }

        public void ZoomToWidth()
        {
            if (this.scrollViewer == null)
            {
                return;
            }

            var scrollBarWidth = this.scrollViewer.ComputedVerticalScrollBarVisibility == System.Windows.Visibility.Visible ? SystemParameters.VerticalScrollBarWidth : 0;
            var zoomFactor = (this.parent.ActualWidth - scrollBarWidth) / this.parent.PageRowBounds[this.CurrentPageIndex].SizeIncludingOffset.Width;
            var pageBound = this.parent.PageRowBounds[this.CurrentPageIndex];

            if (scrollBarWidth == 0 && ((pageBound.Size.Height * zoomFactor) + pageBound.VerticalOffset) >= this.parent.ActualHeight)
                scrollBarWidth += SystemParameters.VerticalScrollBarWidth;

            scrollBarWidth += 2; // Magic number, sorry :)
            zoomFactor = (this.parent.ActualWidth - scrollBarWidth) / this.parent.PageRowBounds[this.CurrentPageIndex].SizeIncludingOffset.Width;

            ZoomInternal(zoomFactor);
        }

        public void ZoomToHeight()
        {
            var scrollBarHeight = this.scrollViewer.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Visible ? SystemParameters.HorizontalScrollBarHeight : 0;
            var zoomFactor = (this.parent.ActualHeight - scrollBarHeight) / this.parent.PageRowBounds[this.CurrentPageIndex].SizeIncludingOffset.Height;
            var pageBound = this.parent.PageRowBounds[this.CurrentPageIndex];

            if (scrollBarHeight == 0 && ((pageBound.Size.Width * zoomFactor) + pageBound.HorizontalOffset) >= this.parent.ActualWidth)
                scrollBarHeight += SystemParameters.HorizontalScrollBarHeight;

            zoomFactor = (this.parent.ActualHeight - scrollBarHeight) / this.parent.PageRowBounds[this.CurrentPageIndex].SizeIncludingOffset.Height;

            ZoomInternal(zoomFactor);
        }

        public void ZoomIn()
        {
            ZoomInternal(this.CurrentZoom + this.parent.ZoomStep);
        }

        public void ZoomOut()
        {
            ZoomInternal(this.CurrentZoom - this.parent.ZoomStep);
        }

        public void Zoom(double zoomFactor)
        {
            this.ZoomInternal(zoomFactor);
        }

        private void ZoomInternal(double zoomFactor)
        {
            if (zoomFactor > this.parent.MaxZoomFactor)
                zoomFactor = this.parent.MaxZoomFactor;
            else if (zoomFactor < this.parent.MinZoomFactor)
                zoomFactor = this.parent.MinZoomFactor;

            this.imageProvider.Settings.ZoomFactor = (float)zoomFactor;

            this.SetItemsSource();
        }
        #endregion

    }
}
