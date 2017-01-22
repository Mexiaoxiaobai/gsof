namespace Gsof.Xaml.PdfViewer.Native
{
    internal struct Rectangle
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public float Width { get { return this.Right - this.Left; } }
        public float Height { get { return this.Bottom - this.Top; } }

    }

    internal struct BBox
    {
        public int Left, Top, Right, Bottom;
    }

    internal struct Matrix
    {
        public float A;
        public float B;
        public float C;
        public float D;
        public float E;
        public float F;
    }
}