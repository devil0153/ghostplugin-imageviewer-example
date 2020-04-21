using Ghost;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    public class ImageHub : HubBase
    {
        public string[] GetDrives()
        {
            return DriveInfo.GetDrives().Select(x => x.Name).ToArray();
        }

        public string[] GetDirectories(string dir)
        {
            return Directory.GetDirectories(dir);
        }

        public string[] GetImages(string dir)
        {
            var files = Directory.GetFiles(dir);
            return files.Where(file => IsImage(file)).ToArray();
        }

        private bool IsImage(string file)
        {
            var ext = Path.GetExtension(file).ToLowerInvariant();
            return ext == ".jpg" || ext == ".jepg" || ext == ".png" || ext == ".bmp" || ext == ".tif" || ext == ".tiff" || ext == ".gif";
        }

        public string GetImageThumb(string image, long width, long height)
        {
            return MakeThumbnail(image, (int)width, (int)height);
        }

        public static string MakeThumbnail(string originalImagePath, int width, int height)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;


            if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
            {
                oh = originalImage.Height;
                ow = originalImage.Height * towidth / toheight;
                y = 0;
                x = (originalImage.Width - ow) / 2;
            }
            else
            {
                ow = originalImage.Width;
                oh = originalImage.Width * height / towidth;
                x = 0;
                y = (originalImage.Height - oh) / 2;
            }

            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                using (MemoryStream m = new MemoryStream())
                {
                    bitmap.Save(m, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] imageBytes = m.ToArray();

                    string base64String = Convert.ToBase64String(imageBytes);
                    return $"data:image/png;base64,{base64String}";
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
    }
}
