using System;
using System.Text;

namespace Gsof.Extensions
{
    public static class ByteExtension
    {
        public static string ToHex(this byte[] p_buffer)
        {
            var buffer = p_buffer;
            if (buffer == null)
            {
                throw new ArgumentNullException("p_buffer");
            }

            var sb = new StringBuilder();
            foreach (var b in buffer)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
