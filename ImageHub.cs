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

            //新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
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
