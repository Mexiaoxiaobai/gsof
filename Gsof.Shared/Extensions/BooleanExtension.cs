namespace Gsof.Extensions
{
    public static class BooleanExtension
    {
        public static int ToInt(this bool p_bool)
        {
            return p_bool ? 1 : 0;
        }

        public static uint ToUint(this bool p_bool)
        {
            return (uint)p_bool.ToInt();
        }
    }
}
