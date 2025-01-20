using System.Runtime.CompilerServices;

namespace Timspect.Core
{
    internal static class MathHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinaryAlign(int num, int alignment)
            => (num + (--alignment)) & ~alignment;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long BinaryAlign(long num, long alignment)
            => (num + (--alignment)) & ~alignment;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIndex2D(int x, int y, int width)
            => (y * width) + x;
    }
}
