namespace Gsof.Pdf.Structs
{
    public struct Rectangle
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public float Width { get { return this.Right - this.Left; } }
        public float Height { get { return this.Bottom - this.Top; } }

    }

    public struct BBox
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public struct Matrix
    {
        public double M11;
        public double M12;
        public double M21;
        public double M22;
        public double OffsetX;
        public double OffsetY;
    }
}