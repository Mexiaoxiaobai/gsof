using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Gsof.Pdf.Declarations;
using Gsof.Pdf.Implementation;
using Gsof.Xaml.PdfViewer.Commands;
using Microsoft.Win32;
using Gsof.Shared.Extensions;

namespace Gsof.Xaml.PdfViewer
{
    public class PdfPanel : Control
    {
        static PdfPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfPanel), new FrameworkPropertyMetadata(typeof(PdfPanel)));

            CommandManager.RegisterClassCommandBinding(typeof(PdfPanel), new CommandBinding(PdfCommands.Open, OnExecute, OnCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(PdfPanel), new CommandBinding(PdfCommands.Close, OnExecute, OnCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(PdfPanel), new CommandBinding(PdfCommands.Zoomin, OnExecute, OnCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(PdfPanel), new CommandBinding(PdfCommands.Zoomout, OnExecute, OnCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(PdfPanel), new CommandBinding(PdfCommands.Single, OnExecute, OnCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(PdfPanel), new CommandBinding(PdfCommands.Facing, OnExecute, OnCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(PdfPanel), new CommandBinding(PdfCommands.Book, OnExecute, OnCanExecute));
        }

        private static void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var pdf = sender as PdfPanel;
            if (pdf == null)
            {
                return;
            }

            if (e.Command != PdfCommands.Open)
            {
                e.CanExecute = pdf.CurrentSource != null;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private static void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            var pdf = sender as PdfPanel;
            if (pdf == null)
            {
                return;
            }

            if (e.Command == PdfCommands.Open)
            {
                pdf.Open();
            }
            else if (e.Command == PdfCommands.Close)
            {

            }
            else if (e.Command == PdfCommands.Zoomin)
            {
                pdf.ZoomIn();
            }
            else if (e.Command == PdfCommands.Zoomout)
            {
                pdf.ZoomOut();
            }
            else if (e.Command == PdfCommands.Single)
            {
                pdf.Single();
            }
            else if (e.Command == PdfCommands.Facing)
            {
                pdf.Facing();
            }
            else if (e.Command == PdfCommands.Book)
            {
                pdf.Book();
            }
        }

        #region Dependency properties

        internal static readonly DependencyProperty InnerPanelProperty =
            DependencyProperty.Register("InnerPanel", typeof(object), typeof(PdfPanel), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty PageMarginProperty = DependencyProperty.Register("PageMargin", typeof(Thickness),
                                                                            typeof(PdfPanel), new FrameworkPropertyMetadata(new Thickness(0, 2, 4, 2)));

        public static readonly DependencyProperty ZoomStepProperty = DependencyProperty.Register("ZoomStep", typeof(double),
                                                                            typeof(PdfPanel), new FrameworkPropertyMetadata(0.25));

        public static readonly DependencyProperty MinZoomFactorProperty = DependencyProperty.Register("MinZoomFactor", typeof(double),
                                                                            typeof(PdfPanel), new FrameworkPropertyMetadata(0.15));

        public static readonly DependencyProperty MaxZoomFactorProperty = DependencyProperty.Register("MaxZoomFactor", typeof(double),
                                                                            typeof(PdfPanel), new FrameworkPropertyMetadata(6.0));

        public static readonly DependencyProperty ViewTypeProperty = DependencyProperty.Register("ViewType", typeof(ViewType),
                                                                            typeof(PdfPanel), new FrameworkPropertyMetadata(ViewType.SinglePage));

        public static readonly DependencyProperty RotationProperty = DependencyProperty.Register("Rotation", typeof(ImageRotation),
                                                                            typeof(PdfPanel), new FrameworkPropertyMetadata(ImageRotation.None));

        public static readonly DependencyProperty PageRowDisplayProperty = DependencyProperty.Register("PageRowDisplay", typeof(PageRowDisplayType),
                                                                            typeof(PdfPanel), new FrameworkPropertyMetadata(PageRowDisplayType.SinglePageRow));

        private static readonly DependencyPropertyKey TotalPagesPropertyKey = DependencyProperty.RegisterReadOnly("TotalPages", typeof(int), typeof(PdfPanel), new PropertyMetadata(0));

        public static readonly DependencyProperty TotalPagesProperty = TotalPagesPropertyKey.DependencyProperty;


        internal IMoonPdfPanel InnerPanel
        {
            get { return GetValue(InnerPanelProperty) as IMoonPdfPanel; }
            set { SetValue(InnerPanelProperty, value); }
        }

        public Thickness PageMargin
        {
            get { return (Thickness)GetValue(PageMarginProperty); }
            set { SetValue(PageMarginProperty, value); }
        }

        public double ZoomStep
        {
            get { return (double)GetValue(ZoomStepProperty); }
            set { SetValue(ZoomStepProperty, value); }
        }

        public double MinZoomFactor
        {
            get { return (double)GetValue(MinZoomFactorProperty); }
            set { SetValue(MinZoomFactorProperty, value); }
        }

        public double MaxZoomFactor
        {
            get { return (double)GetValue(MaxZoomFactorProperty); }
            set { SetValue(MaxZoomFactorProperty, value); }
        }

        public ViewType ViewType
        {
            get { return (ViewType)GetValue(ViewTypeProperty); }
            set { SetValue(ViewTypeProperty, value); }
        }

        public ImageRotation Rotation
        {
            get { return (ImageRotation)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        public PageRowDisplayType PageRowDisplay
        {
            get { return (PageRowDisplayType)GetValue(PageRowDisplayProperty); }
            set { SetValue(PageRowDisplayProperty, value); }
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            private set { SetValue(TotalPagesPropertyKey, value); }
        }

        #endregion

        public ZoomType ZoomType
        {
            get { return (ZoomType)GetValue(ZoomTypeProperty); }
            set { SetValue(ZoomTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomTypeProperty =
            DependencyProperty.Register("ZoomType", typeof(ZoomType), typeof(PdfPanel), new FrameworkPropertyMetadata(ZoomType.Fixed, FrameworkPropertyMetadataOptions.Journal, OnZoomTypeChanged));

        private static void OnZoomTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pdf = d as PdfPanel;
            if (pdf == null)
            {
                return;
            }

            if (pdf.InnerPanel == null
                || pdf.InnerPanel.ScrollViewer == null)
            {
                return;
            }

            switch (pdf.ZoomType)
            {
                case ZoomType.Fixed:
                    pdf.SetFixedZoom();
                    break;
                case ZoomType.FitToHeight:
                    pdf.InnerPanel.ZoomToHeight();
                    break;
                case ZoomType.FitToWidth:
                    pdf.InnerPanel.ZoomToWidth();
                    break;
            }
        }



        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(PdfPanel), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Journal, OnCurrentPage));

        private static void OnCurrentPage(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pdf = d as PdfPanel;
            if (pdf == null)
            {
                return;
            }

            if (pdf._isinternalPageChanged)
            {
                return;
            }

            pdf.GotoPage(pdf.CurrentPage - 1);
        }


        public event EventHandler PdfLoaded;
        public event EventHandler ZoomTypeChanged;
        public event EventHandler ViewTypeChanged;
        public event EventHandler PageRowDisplayChanged;
        public event EventHandler<PasswordRequiredEventArgs> PasswordRequired;

        private MoonPdfPanelInputHandler inputHandler;
        private PageRowBound[] pageRowBounds;
        private DispatcherTimer resizeTimer;

        private bool _isinternalPageChanged;

        public double HorizontalMargin { get { return this.PageMargin.Right; } }
        public IPdfSource CurrentSource { get; private set; }
        public string CurrentPassword { get; private set; }

        internal PageRowBound[] PageRowBounds { get { return this.pageRowBounds; } }

        internal ScrollViewer ScrollViewer
        {
            get { return this.InnerPanel.ScrollViewer; }
        }

        public float CurrentZoom
        {
            get { return this.InnerPanel.CurrentZoom; }
        }

        public PdfPanel()
        {
            this.ChangeDisplayType(this.PageRowDisplay);
            this.inputHandler = new MoonPdfPanelInputHandler(this);

            this.SizeChanged += PdfViewerPanel_SizeChanged;

            resizeTimer = new DispatcherTimer();
            resizeTimer.Interval = TimeSpan.FromMilliseconds(150);
            resizeTimer.Tick += resizeTimer_Tick;
        }

        void PdfViewerPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.CurrentSource == null)
                return;

            resizeTimer.Stop();
            resizeTimer.Start();
        }

        void resizeTimer_Tick(object sender, EventArgs e)
        {
            resizeTimer.Stop();

            if (this.CurrentSource == null)
                return;

            if (this.ZoomType == ZoomType.FitToWidth)
                ZoomToWidth();
            else if (this.ZoomType == ZoomType.FitToHeight)
                ZoomToHeight();
        }

        public void OpenFile(string pdfFilename, string password = null)
        {
            if (!File.Exists(pdfFilename))
                throw new FileNotFoundException(string.Empty, pdfFilename);

            this.Open(new FileSource(pdfFilename), password);
        }

        public void Open(IPdfSource source, string password = null)
        {
            var pw = password;

            if (this.PasswordRequired != null && MuPdfWrapper.NeedsPassword(source) && pw == null)
            {
                var e = new PasswordRequiredEventArgs();
                this.PasswordRequired(this, e);

                if (e.Cancel)
                    return;

                pw = e.Password;
            }

            this.LoadPdf(source, pw);
            this.CurrentSource = source;
            this.CurrentPassword = pw;
            if (TotalPages > 0)
            {
                this.CurrentPage = 1;
            }

            if (this.PdfLoaded != null)
                this.PdfLoaded(this, EventArgs.Empty);
        }

        public void Open()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = " PDF文件|*.pdf";

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            var fileName = ofd.FileName;

            this.OpenFile(fileName);
        }

        public void Unload()
        {
            this.CurrentSource = null;
            this.CurrentPassword = null;
            this.TotalPages = 0;

            this.InnerPanel.Unload();

            if (this.PdfLoaded != null)
                this.PdfLoaded(this, EventArgs.Empty);
        }

        private void LoadPdf(IPdfSource source, string password)
        {
            var pageBounds = MuPdfWrapper.GetPageBounds(source, this.Rotation, password);
            this.pageRowBounds = CalculatePageRowBounds(pageBounds, this.ViewType);
            this.TotalPages = pageBounds.Length;
            this.InnerPanel.Load(source, password);
        }

        private PageRowBound[] CalculatePageRowBounds(Size[] singlePageBounds, ViewType viewType)
        {
            var pagesPerRow = Math.Min(GetPagesPerRow(), singlePageBounds.Length); // if multiple page-view, but pdf contains less pages than the pages per row
            var finalBounds = new List<PageRowBound>();
            var verticalBorderOffset = (this.PageMargin.Top + this.PageMargin.Bottom);

            if (viewType == ViewType.SinglePage)
            {
                finalBounds.AddRange(singlePageBounds.Select(p => new PageRowBound(p, verticalBorderOffset, 0)));
            }
            else
            {
                var horizontalBorderOffset = this.HorizontalMargin;

                for (int i = 0; i < singlePageBounds.Length; i++)
                {
                    if (i == 0 && viewType == ViewType.BookView)
                    {
                        finalBounds.Add(new PageRowBound(singlePageBounds[0], verticalBorderOffset, 0));
                        continue;
                    }

                    var subset = singlePageBounds.Take(i, pagesPerRow).ToArray();

                    // we get the max page-height from all pages in the subset and the sum of all page widths of the subset plus the offset between the pages
                    finalBounds.Add(new PageRowBound(new Size(subset.Sum(f => f.Width), subset.Max(f => f.Height)), verticalBorderOffset, horizontalBorderOffset * (subset.Length - 1)));
                    i += (pagesPerRow - 1);
                }
            }

            return finalBounds.ToArray();
        }

        internal int GetPagesPerRow()
        {
            return this.ViewType == ViewType.SinglePage ? 1 : 2;
        }

        public int GetCurrentPageNumber()
        {
            if (this.InnerPanel == null)
                return -1;

            return this.InnerPanel.GetCurrentPageIndex(this.ViewType) + 1;
        }

        public void ZoomToWidth()
        {
            this.InnerPanel.ZoomToWidth();
            this.ZoomType = ZoomType.FitToWidth;
        }

        public void ZoomToHeight()
        {
            this.InnerPanel.ZoomToHeight();
            this.ZoomType = ZoomType.FitToHeight;
        }

        public void ZoomIn()
        {
            this.InnerPanel.ZoomIn();
            this.ZoomType = ZoomType.Fixed;
        }

        public void ZoomOut()
        {
            this.InnerPanel.ZoomOut();
            this.ZoomType = ZoomType.Fixed;
        }

        public void Zoom(double zoomFactor)
        {
            this.InnerPanel.Zoom(zoomFactor);
            this.ZoomType = ZoomType.Fixed;
        }

        /// <summary>
        /// Sets the ZoomType back to Fixed
        /// </summary>
        public void SetFixedZoom()
        {
            this.ZoomType = ZoomType.Fixed;
        }

        public void GotoPreviousPage()
        {
            this.InnerPanel.GotoPreviousPage();
        }

        public void GotoNextPage()
        {
            this.InnerPanel.GotoNextPage();
        }

        public void GotoPage(int pageNumber)
        {
            this.InnerPanel.GotoPage(pageNumber);
        }

        public void GotoFirstPage()
        {
            this.GotoPage(1);
        }

        public void GotoLastPage()
        {
            this.GotoPage(this.TotalPages);
        }

        public void RotateRight()
        {
            if ((int)this.Rotation < Enum.GetValues(typeof(ImageRotation)).Length)
                this.Rotation = (ImageRotation)this.Rotation + 1;
            else
                this.Rotation = ImageRotation.None;
        }

        public void RotateLeft()
        {
            if ((int)this.Rotation > 0)
                this.Rotation = (ImageRotation)this.Rotation - 1;
            else
                this.Rotation = ImageRotation.Rotate270;
        }

        public void Rotate(ImageRotation rotation)
        {
            var currentPage = this.InnerPanel.GetCurrentPageIndex(this.ViewType) + 1;
            this.LoadPdf(this.CurrentSource, this.CurrentPassword);
            this.InnerPanel.GotoPage(currentPage);
        }

        public void TogglePageDisplay()
        {
            this.PageRowDisplay = (this.PageRowDisplay == PageRowDisplayType.SinglePageRow) ? PageRowDisplayType.ContinuousPageRows : PageRowDisplayType.SinglePageRow;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == PageRowDisplayProperty)
                ChangeDisplayType((PageRowDisplayType)e.NewValue);
            else if (e.Property == RotationProperty)
                this.Rotate((ImageRotation)e.NewValue);
            else if (e.Property == ViewTypeProperty)
                this.ApplyChangedViewType((ViewType)e.OldValue);
        }

        private void ApplyChangedViewType(ViewType oldViewType)
        {
            UpdateAndReload(() => { }, oldViewType);

            if (this.ViewTypeChanged != null)
                this.ViewTypeChanged(this, EventArgs.Empty);
        }

        private void ChangeDisplayType(PageRowDisplayType pageRowDisplayType)
        {
            UpdateAndReload(() =>
                {
                    // we need to remove the current InnerPanel
                    //this.pnlMain.Children.Clear();

                    if (InnerPanel != null)
                    {
                        InnerPanel.CurrentPageChangedHandler -= OnCurrentPageChanged;
                    }

                    if (pageRowDisplayType == PageRowDisplayType.SinglePageRow)
                        this.InnerPanel = new SinglePageMoonPdfPanel(this);
                    else
                        this.InnerPanel = new ContinuousMoonPdfPanel(this);


                    InnerPanel.CurrentPageChangedHandler += OnCurrentPageChanged;

                    //this.pnlMain.Children.Add(this.InnerPanel.Instance);
                }, this.ViewType);

            if (this.PageRowDisplayChanged != null)
                this.PageRowDisplayChanged(this, EventArgs.Empty);
        }

        private void OnCurrentPageChanged(object sender, EventArgs eventArgs)
        {
            _isinternalPageChanged = true;
            CurrentPage = this.InnerPanel.GetCurrentPageIndex(ViewType) + 1;
            _isinternalPageChanged = false;
        }

        private void UpdateAndReload(Action updateAction, ViewType viewType)
        {
            var currentPage = -1;
            var zoom = 1.0f;

            if (this.CurrentSource != null)
            {
                currentPage = this.InnerPanel.GetCurrentPageIndex(viewType) + 1;
                zoom = this.InnerPanel.CurrentZoom;
            }

            updateAction();

            if (currentPage > -1)
            {
                Action reloadAction = () =>
                    {
                        this.LoadPdf(this.CurrentSource, this.CurrentPassword);
                        this.InnerPanel.Zoom(zoom);
                        this.InnerPanel.GotoPage(currentPage);
                    };

                if (this.InnerPanel.Instance.IsLoaded)
                    reloadAction();
                else
                {
                    // we need to wait until the controls are loaded and then reload the pdf
                    this.InnerPanel.Instance.Loaded += (s, e) => reloadAction();
                }
            }
        }

        /// <summary>
        /// Will only be triggered if the AllowDrop-Property is set to true
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            var filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (filenames == null)
            {
                return;
            }

            var filename = filenames.FirstOrDefault();

            if (filename != null && File.Exists(filename))
            {
                string pw = null;

                if (MuPdfWrapper.NeedsPassword(new FileSource(filename)))
                {
                    if (this.PasswordRequired == null)
                        return;

                    var args = new PasswordRequiredEventArgs();
                    this.PasswordRequired(this, args);

                    if (args.Cancel)
                        return;

                    pw = args.Password;
                }

                try
                {
                    this.OpenFile(filename, pw);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occured: " + ex.Message));
                }
            }
        }

        public void Single()
        {
            this.PageRowDisplay = PageRowDisplayType.ContinuousPageRows;
            this.ViewType = ViewType.SinglePage;
        }

        public void Facing()
        {
            this.PageRowDisplay = PageRowDisplayType.SinglePageRow;
            this.ViewType = ViewType.Facing;
        }

        public void Book()
        {
            this.PageRowDisplay = PageRowDisplayType.ContinuousPageRows;
            this.ViewType = ViewType.Facing;
        }
    }

    public class PasswordRequiredEventArgs : EventArgs
    {
        public string Password { get; set; }
        public bool Cancel { get; set; }
    }
}
