using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace PictureOptimizer
{
    public class ImageResizer
    {
        private const int DefaultThumbnailHeight = 100;

        public static Stream CreateThumbnail(Image image)
        {
            int width = DefaultThumbnailHeight * image.Width / image.Height;
            var destRect = new Rectangle(0, 0, width, DefaultThumbnailHeight);
            var destImage = new Bitmap(width, DefaultThumbnailHeight);

            try
            {
                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return BitmapToStream(destImage);
            }
            catch (Exception)
            {
                destImage.Dispose();
                throw;
            }
        }

        private static Stream BitmapToStream(Image image)
        {
            var result = new MemoryStream();
            try
            {
                image.Save(result, ImageFormat.Jpeg);
                result.Position = 0;

                return result;
            }
            catch (Exception)
            {
                result.Dispose();
                throw;
            }
        }
    }
}