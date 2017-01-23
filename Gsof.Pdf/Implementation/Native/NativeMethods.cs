using System;
using System.Runtime.InteropServices;
using Gsof.Pdf.Structs;

namespace Gsof.Pdf.Implementation.Native
{
    internal static class NativeMethods
    {
        const string DLL = "libmupdf.dll";

        [DllImport(DLL, EntryPoint = "fz_new_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NewContext(IntPtr alloc, IntPtr locks, uint max_store);

        [DllImport(DLL, EntryPoint = "fz_free_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FreeContext(IntPtr ctx);

        [DllImport(DLL, EntryPoint = "fz_open_file_w", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenFile(IntPtr ctx, string fileName);

        [DllImport(DLL, EntryPoint = "fz_open_document_with_stream", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenDocumentStream(IntPtr ctx, string magic, IntPtr stm);

        [DllImport(DLL, EntryPoint = "fz_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CloseStream(IntPtr stm);

        [DllImport(DLL, EntryPoint = "fz_close_document", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CloseDocument(IntPtr doc);

        [DllImport(DLL, EntryPoint = "fz_count_pages", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CountPages(IntPtr doc);

        [DllImport(DLL, EntryPoint = "fz_bound_page", CallingConvention = CallingConvention.Cdecl)]
        public static extern Rectangle BoundPage(IntPtr doc, IntPtr page);

        [DllImport(DLL, EntryPoint = "fz_clear_pixmap_with_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearPixmap(IntPtr ctx, IntPtr pix, int byteValue);

        [DllImport(DLL, EntryPoint = "fz_find_device_colorspace", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FindDeviceColorSpace(IntPtr ctx, string colorspace);

        [DllImport(DLL, EntryPoint = "fz_free_device", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeDevice(IntPtr dev);

        [DllImport(DLL, EntryPoint = "fz_free_page", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreePage(IntPtr doc, IntPtr page);

        [DllImport(DLL, EntryPoint = "fz_load_page", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LoadPage(IntPtr doc, int pageNumber);

        [DllImport(DLL, EntryPoint = "fz_new_draw_device", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NewDrawDevice(IntPtr ctx, IntPtr pix);

        [DllImport(DLL, EntryPoint = "fz_new_pixmap", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NewPixmap(IntPtr ctx, IntPtr colorspace, int width, int height);

        [DllImport(DLL, EntryPoint = "fz_run_page", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunPage(IntPtr doc, IntPtr page, IntPtr dev, Matrix transform, IntPtr cookie);

        [DllImport(DLL, EntryPoint = "fz_drop_pixmap", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DropPixmap(IntPtr ctx, IntPtr pix);

        [DllImport(DLL, EntryPoint = "fz_pixmap_samples", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetSamples(IntPtr ctx, IntPtr pix);

        [DllImport(DLL, EntryPoint = "fz_needs_password", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NeedsPassword(IntPtr doc);

        [DllImport(DLL, EntryPoint = "fz_authenticate_password", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AuthenticatePassword(IntPtr doc, string password);

        [DllImport(DLL, EntryPoint = "fz_open_memory", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenStream(IntPtr ctx, IntPtr data, int len);
    }
}