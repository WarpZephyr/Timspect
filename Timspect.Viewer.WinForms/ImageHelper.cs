using Timspect.Core.Formats;

namespace Timspect.Viewer.WinForms
{
    internal static class ImageHelper
    {
        public static Image? ImportImage(string path)
        {
            Image? image;

            try
            {
                const StringComparison Comp = StringComparison.InvariantCultureIgnoreCase;
                if (path.EndsWith(".png", Comp)
                || path.EndsWith(".jpg", Comp)
                || path.EndsWith(".tiff", Comp)
                || path.EndsWith(".bmp", Comp)
                || path.EndsWith(".exif", Comp)
                || path.EndsWith(".ico", Comp)
                 || path.EndsWith(".webp", Comp))
                {
                    image = Image.FromFile(path);
                }
                else if (TIM.IsRead(path, out TIM? tim))
                {
                    image = tim.ToBitmap();
                }
                else if (TIM2.IsRead(path, out TIM2? tim2) && tim2.Pictures.Count > 0)
                {
                    image = tim2.ToBitmap();
                }
                else
                {
                    var fstim2 = FSTIM2.Read(path);
                    if (fstim2.Pictures.Count > 0)
                    {
                        image = fstim2.ToBitmap();
                    }
                    else
                    {
                        image = null;
                    }
                }
            }
            catch
            {
                image = null;
            }

            return image;
        }
    }
}
