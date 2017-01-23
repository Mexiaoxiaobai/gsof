using System;
using Gsof.Pdf.Declarations;
using Gsof.Pdf.Implementation.Native;
using Gsof.Pdf.Structs;
using Gsof.Shared.Extensions;

namespace Gsof.Pdf.Implementation
{
    public class MuPdf : IPdf
    {
        const uint FZ_STORE_DEFAULT = 256 << 20;

        private IntPtr _context;
        private IntPtr _document;
        private IntPtr _stream;
        private string _password;
        private IDpi _dpi;

        public bool IsEncrypt { get; private set; }

        public int Pages { get; private set; }

        public float Zoom { get; set; }

        public Rotation Rotation { get; set; }

        public MuPdf()
        {
            _dpi = new DefaultDpi();
        }

        public MuPdf(IDpi p_dpi)
        {
            _dpi = p_dpi;
        }

        public void Open(IPdfSource p_source, string p_password = null)
        {
            if (p_source == null)
            {
                throw new ArgumentNullException(nameof(p_source));
            }

            _password = p_password;

            using (var stream = p_source.Open())
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                var data = buffer.ToIntPtr();

                _context = NativeMethods.NewContext(IntPtr.Zero, IntPtr.Zero, FZ_STORE_DEFAULT);
                _stream = NativeMethods.OpenStream(_context, data, buffer.Length);
                _document = NativeMethods.OpenDocumentStream(_context, ".pdf", _stream);

                IsEncrypt = NativeMethods.NeedsPassword(_document) != 0;

                var isAuthenticate = true;

                if (IsEncrypt && !string.IsNullOrEmpty(_password))
                {
                    isAuthenticate = AuthenticatePassword(_password);
                }

                if (isAuthenticate)
                {
                    Pages = NativeMethods.CountPages(_document);
                }

                data.Free();
            }
        }

        private bool AuthenticatePassword(string password)
        {
            if (_document.IsZero())
            {
                throw new ArgumentNullException();
            }

            return NativeMethods.AuthenticatePassword(_document, password) == 0;
        }

        public Size[] GetPageBounds()
        {
            Func<double, double, Size> callback = (width, height) => new Size(width, height);

            if (Rotation == Rotation.Rotate90 || Rotation == Rotation.Rotate270)
                callback = (width, height) => new Size(height, width);

            var count = Pages;
            var bounds = new Size[count];

            for (int i = 0; i < count; i++)
            {
                IntPtr p = NativeMethods.LoadPage(_document, i);
                Rectangle pageBound = NativeMethods.BoundPage(_document, p);
                bounds[i] = callback(pageBound.Width, pageBound.Height);
                NativeMethods.FreePage(_document, p); // releases the resources consumed by the page
            }

            return bounds;
        }

        public byte[] Extrac(int p_index)
        {
            var index = p_index; //Math.Max(0, p_index - 1);
            IntPtr p = NativeMethods.LoadPage(_document, index); // loads the page
            var bmp = RenderPage(_context, _document, p, Zoom);
            NativeMethods.FreePage(_document, p); // releases the resources consumed by the page
            return bmp;
        }

        byte[] RenderPage(IntPtr context, IntPtr document, IntPtr page, float zoomFactor)
        {
            Rectangle pageBound = NativeMethods.BoundPage(document, page);
            Matrix ctm = new Matrix();
            IntPtr pix = IntPtr.Zero;
            IntPtr dev = IntPtr.Zero;

            var currentDpi = _dpi.GetDpi();
            var zoomX = zoomFactor * (currentDpi.X / 96f);
            var zoomY = zoomFactor * (currentDpi.Y / 96f);

            // gets the size of the page and multiplies it with zoom factors
            int width = (int)(pageBound.Width * zoomX);
            int height = (int)(pageBound.Height * zoomY);

            // sets the matrix as a scaling matrix (zoomX,0,0,zoomY,0,0)
            ctm.M11 = zoomX;
            ctm.M12 = zoomY;

            // creates a pixmap the same size as the width and height of the page
            pix = NativeMethods.NewPixmap(context, NativeMethods.FindDeviceColorSpace(context, "DeviceRGB"), width, height);
            // sets white color as the background color of the pixmap
            NativeMethods.ClearPixmap(context, pix, 0xFF);

            // creates a drawing device
            dev = NativeMethods.NewDrawDevice(context, pix);
            // draws the page on the device created from the pixmap
            NativeMethods.RunPage(document, page, dev, ctm, IntPtr.Zero);
            NativeMethods.FreeDevice(dev); // frees the resources consumed by the device

            var src = NativeMethods.GetSamples(context, pix);
            var result = src.GetBytes(width * height * 4);
            NativeMethods.DropPixmap(context, pix);
            //bmp.SetResolution(currentDpi.HorizontalDpi, currentDpi.VerticalDpi);

            return result;
        }

        public void Dispose()
        {
            if (!_document.IsZero())
            {
                NativeMethods.CloseDocument(_document);
                _document = IntPtr.Zero;
            }

            if (!_stream.IsZero())
            {
                NativeMethods.CloseStream(_stream);
                _stream = IntPtr.Zero;
            }

            if (_context.IsZero())
            {
                NativeMethods.FreeContext(_context);
                _context = IntPtr.Zero;
            }
        }
    }
}