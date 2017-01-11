using System.Security.Cryptography;

namespace Gsof.Extensions
{
    public static class Md5Extension
    {
        public static byte[] ToMd5(this byte[] p_buffer)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(p_buffer);
            return hash;
        }
    }
}
