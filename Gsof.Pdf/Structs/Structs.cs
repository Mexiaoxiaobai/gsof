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
        public float M11;
        public float M12;
        public float M21;
        public float M22;
        public float OffsetX;
        public float OffsetY;
    }
}