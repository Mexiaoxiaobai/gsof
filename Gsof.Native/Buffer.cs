using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Gsof.Native
{
    public class Buffer : IDisposable
    {
        private IntPtr _point;

        public IntPtr Point
        {
            get { return _point; }
            protected set { _point = value; }
        }

        public int Length { get; protected set; }

        public Buffer(int size)
        {
            Point = InternalAlloc(size);
            Length = size;
        }

        public Buffer(byte[] p_buffer, int index, int length)
        {
            var buffer = p_buffer;
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(p_buffer));
            }

            if (index >= buffer.Length || length + index >= buffer.Length)
            {
                throw new RankException();
            }

            var ptr = Point = InternalAlloc(buffer.Length);
            Marshal.Copy(buffer, index, ptr, length);
            Length = length;
        }

        public Buffer(byte[] p_buffer, int index = 0) : this(p_buffer, index, p_buffer?.Length ?? 0 - 1 - index)
        {

        }



        #region Public

        public byte[] ReadBytes(int pos, int length)
        {
            CheckPtr();

            if (length < 0)
            {
                throw new ArgumentException("the buffer's length less than zero");
            }
            var buffer = new byte[length];

            Marshal.Copy(_point, buffer, pos, length);
            return buffer;
        }

        public byte[] ReadBytes(int pos = 0)
        {
            return ReadBytes(pos, this.Length - pos);
        }

        public int ReadInt32(int offset = 0)
        {
            CheckPtr();
            return Marshal.ReadInt32(Point, offset);
        }

        public long ReadInt64(int offset = 0)
        {
            CheckPtr();
            return Marshal.ReadInt64(Point, offset);
        }

        public string ReadString(Encoding encoding, int offset = 0)
        {
            CheckPtr();
            return ReadString(encoding, offset, this.Length - offset);
        }

        public string ReadString(Encoding encoding, int offset, int lenght)
        {
            CheckPtr();
            if (encoding == null)
            {
                throw new ArgumentException("Encoding can not be null");
            }
            var bytes = ReadBytes(offset, lenght - offset);

            return encoding.GetString(bytes);
        }

        public T GetStruct<T>()
        {
            CheckPtr();
            return (T)Marshal.PtrToStructure(Point, typeof(T));
        }


        #endregion



        #region Private

        private void CheckPtr()
        {
            if (_point == IntPtr.Zero)
            {
                throw new ArgumentException("the memory do not alloc or free.");
            }
        }

        private IntPtr InternalAlloc(int p_size)
        {
            IntPtr p = Marshal.AllocHGlobal(p_size);
            return p;
        }

        private void InternalFree(IntPtr p_intPtr)
        {
            if (p_intPtr == IntPtr.Zero)
            {
                return;
            }

            Marshal.FreeHGlobal(p_intPtr);
        }

        #endregion

        #region Dispose

        public void Free()
        {
            InternalFree(Point);
            Point = IntPtr.Zero;
        }

        public void Dispose()
        {
            Free();
        }

        #endregion

        #region Static

        public static Buffer Alloc(int p_size)
        {
            return new Buffer(p_size);
        }

        public static void Free(Buffer buffer)
        {
            buffer?.Dispose();
        }


        #endregion
    }
}
