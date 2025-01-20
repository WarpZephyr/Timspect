using Timspect.Core.Formats;

namespace Timspect.Viewer.WinForms
{
    internal static class Extensions
    {
        public static Bitmap ToBitmap(this TIM tim)
        {
            var bm = new Bitmap(tim.Width, tim.Height);
            for (int x = 0; x < tim.Width; x++)
            {
                for (int y = 0; y < tim.Height; y++)
                {
                    bm.SetPixel(x, y, tim.GetPixelColor(x, y));
                }
            }

            return bm;
        }

        public static Bitmap ToBitmap(this TIM2 tim)
        {
            var pic = tim.Pictures[0];
            var bm = new Bitmap(pic.Width, pic.Height);
            for (int x = 0; x < pic.Width; x++)
            {
                for (int y = 0; y < pic.Height; y++)
                {
                    bm.SetPixel(x, y, pic.GetPixelColor(x, y));
                }
            }

            return bm;
        }

        public static Bitmap ToBitmap(this FSTIM2 tim)
        {
            var pic = tim.Pictures[0];
            var bm = new Bitmap(pic.Width, pic.Height);
            for (int x = 0; x < pic.Width; x++)
            {
                for (int y = 0; y < pic.Height; y++)
                {
                    bm.SetPixel(x, y, pic.GetPixelColor(x, y));
                }
            }

            return bm;
        }
    }
}
