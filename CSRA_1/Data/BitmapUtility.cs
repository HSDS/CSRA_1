using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSRA_1.Data
{
    /// <summary>
    /// Utility class for converting bitmap formats 
    /// </summary>
    public class BitmapUtility
    {
        #region Static Functions

        static public Bitmap CopyToBitmap(double[,] data)
        {
            //  get max value
            double maxVal = 0.0001;
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    maxVal = Math.Max(maxVal, data[i, j]);
                }
            }

            //  get scaled byte array
            byte[] imageData = new byte[width * height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int k = (int)Math.Min(Math.Max(data[i, j] * 255.0 / maxVal, 0), 255);
                    imageData[i + j * width] = (byte)k;
                }
            }

            Bitmap bmp = null;
            CopyBytesTo8bppBitmap(width, height, imageData, ref bmp);

            return bmp;
        }

        static public Bitmap LoadImage()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image files (*.bmp,*.jpg)|*.bmp;*.jpg";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return (Bitmap)Bitmap.FromFile(dlg.FileName);
                }
            }

            return null;
        }

        static public bool SaveImage(Bitmap bmp)
        {
            return SaveImage(bmp, "");
        }

        static public bool SaveImage(Bitmap bmp, string fileName)
        {
            //  get filename if we need to
            if (fileName == null || fileName == "")
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Filter = "Image files (*.bmp,*.jpg)|*.bmp;*.jpg";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        fileName = dlg.FileName;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            string ext = Path.GetExtension(fileName).ToUpper();
            try
            {
                if (ext == ".BMP")
                {
                    bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                    return true;
                }
                else if (ext == ".JPG")
                {
                    bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return true;
                }
            }
            catch
            {
            }

            MessageBox.Show("Could not save image.");
            return false;
        }

        static public void SetGreenLut(Bitmap bmp)
        {
            //	create monochrome palette
            ColorPalette pal = bmp.Palette;
            for (int i = 0; i < pal.Entries.Length; i++)
            {
                pal.Entries[i] = Color.FromArgb(0, i, 0);
            }
            bmp.Palette = pal;

        }

        /// <summary>
        /// Sets the 8bpp bitmap to the bytes provided.
        /// </summary>
        /// <param name="width">Width of the bitmap in pixels.</param>
        /// <param name="height">Height of the bitmap in pixels.</param>
        /// <param name="imageBytes">Bytes to set the image to.</param>
        /// <param name="bitmap">Bitmap to copy bytes to</param>
        static unsafe public void CopyBytesTo8bppBitmap(int width, int height, byte[] imageBytes, ref Bitmap bitmap)
        {
            //	set size of image
            bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            //	create monochrome palette
            ColorPalette pal = bitmap.Palette;
            for (int i = 0; i < pal.Entries.Length; i++)
            {
                pal.Entries[i] = Color.FromArgb(i, i, i);
            }
            bitmap.Palette = pal;

            //	copy the bytes
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            IntPtr pixels = bitmapData.Scan0;
            byte* pBits = (byte*)pixels.ToPointer();
            uint stride = (uint)Math.Abs(bitmapData.Stride);
            for (uint row = 0; row < height; row++)
            {
                for (uint col = 0; col < width; col++)
                {

                    pBits[col + row * stride] = imageBytes[col + row * width];

                }
            }

            bitmap.UnlockBits(bitmapData);
        }

        static unsafe public void Copy24bppBitmapToColorBytes(Bitmap bitmap, out byte[] imageBytes)
        {
            imageBytes = null;

            if (bitmap == null)
            {
                return;
            }

            int bytesPerPel = 0;
            if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                bytesPerPel = 3;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                bytesPerPel = 4;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format32bppRgb)
            {
                bytesPerPel = 4;
            }
            else
            {
                return;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;
            int depth = 3;

            imageBytes = new byte[width * height * depth];

            //	copy the bytes
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            IntPtr pixels = bitmapData.Scan0;
            byte* pBits = (byte*)pixels.ToPointer();
            uint stride = (uint)Math.Abs(bitmapData.Stride);
            for (uint row = 0; row < height; row++)
            {
                for (uint col = 0; col < width; col++)
                {
                    int dstAddr = (int)(depth * (col + row * width));
                    int srcAddr = (int)(col * bytesPerPel + row * stride);

                    imageBytes[dstAddr++] = pBits[srcAddr + 2];
                    imageBytes[dstAddr++] = pBits[srcAddr + 1];
                    imageBytes[dstAddr] = pBits[srcAddr + 0];
                }
            }

            bitmap.UnlockBits(bitmapData);

        }

        static unsafe public void Copy24bppBitmapToColorBytes(Bitmap bitmap, out byte[] redImageBytes, out byte[] greenImageBytes, out byte[] blueImageBytes)
        {
            redImageBytes = null;
            greenImageBytes = null;
            blueImageBytes = null;

            if (bitmap == null)
            {
                return;
            }

            int bytesPerPel = 0;
            if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                bytesPerPel = 3;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                bytesPerPel = 4;
            }
            else
            {
                return;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            redImageBytes = new byte[width * height];
            greenImageBytes = new byte[width * height];
            blueImageBytes = new byte[width * height];

            //	copy the bytes
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            IntPtr pixels = bitmapData.Scan0;
            byte* pBits = (byte*)pixels.ToPointer();
            uint stride = (uint)Math.Abs(bitmapData.Stride);
            for (uint row = 0; row < height; row++)
            {
                for (uint col = 0; col < width; col++)
                {
                    int dstAddr = (int)(col + row * width);
                    int srcAddr = (int)(col * bytesPerPel + row * stride);

                    redImageBytes[dstAddr] = pBits[srcAddr + 2];
                    greenImageBytes[dstAddr] = pBits[srcAddr + 1];
                    blueImageBytes[dstAddr] = pBits[srcAddr + 0];
                }
            }

            bitmap.UnlockBits(bitmapData);

        }

        static unsafe public Bitmap CopyColorBytesTo24bppBitmap(int width, int height, int depth, byte[] data)
        {
            //	set size of image
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //	copy the bytes
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            IntPtr pixels = bitmapData.Scan0;
            byte* pBits = (byte*)pixels.ToPointer();
            uint stride = (uint)Math.Abs(bitmapData.Stride);
            for (uint row = 0; row < height; row++)
            {
                for (uint col = 0; col < width; col++)
                {
                    int srcAddr = (int)(depth * (col + row * width));
                    int dstAddr = (int)(col * 3 + row * stride);

                    pBits[dstAddr + 2] = data[srcAddr++];
                    pBits[dstAddr + 1] = data[srcAddr++];
                    pBits[dstAddr + 0] = data[srcAddr];
                }
            }

            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        static unsafe public Bitmap CopyColorBytesTo24bppBitmap(int width, int height, byte[] redImageBytes, byte[] greenImageBytes, byte[] blueImageBytes)
        {
            //	set size of image
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //	copy the bytes
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            IntPtr pixels = bitmapData.Scan0;
            byte* pBits = (byte*)pixels.ToPointer();
            uint stride = (uint)Math.Abs(bitmapData.Stride);
            for (uint row = 0; row < height; row++)
            {
                for (uint col = 0; col < width; col++)
                {
                    int srcAddr = (int)(col + row * width);
                    int dstAddr = (int)(col * 3 + row * stride);
                    pBits[dstAddr + 2] = redImageBytes[srcAddr];
                    pBits[dstAddr + 1] = greenImageBytes[srcAddr];
                    pBits[dstAddr + 0] = blueImageBytes[srcAddr];
                }
            }

            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }
        /// <summary>
        /// Sets the 8bpp bitmap to the bytes provided.
        /// </summary>
        /// <param name="bitmap">Bitmap to copy bytes from</param>
        /// <param name="imageBytes">Bytes to hold image.</param>
        static unsafe public void Copy24bppBitmapToMonochromeBytes(Bitmap bitmap, ref byte[] imageBytes)
        {
            // if not 24bpp, don't do anything
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
            {
                return;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            //	set size of bytes
            imageBytes = new byte[width * height];

            //	copy the bytes
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr pixels = bitmapData.Scan0;
            byte* pBits = (byte*)pixels.ToPointer();
            uint stride = (uint)Math.Abs(bitmapData.Stride);
            //	just get red plane
            for (uint row = 0; row < height; row++)
            {
                for (uint col = 0; col < width; col++)
                {
                    imageBytes[col + row * width] = pBits[col * 3 + row * stride];
                }
            }

            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// Sets the 8bpp bitmap to the bytes provided.
        /// </summary>
        /// <param name="bitmap">Bitmap to copy bytes from</param>
        /// <param name="imageBytes">Bytes to hold image.</param>
        static unsafe public void Copy32bppBitmapToMonochromeBytes(Bitmap bitmap, ref byte[] imageBytes)
        {
            // if not 32bpp, don't do anything
            if (bitmap.PixelFormat != PixelFormat.Format32bppRgb)
            {
                return;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            //	set size of bytes
            imageBytes = new byte[width * height];

            //	copy the bytes
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            IntPtr pixels = bitmapData.Scan0;
            byte* pBits = (byte*)pixels.ToPointer();
            uint stride = (uint)Math.Abs(bitmapData.Stride);
            //	just get red plane
            for (uint row = 0; row < height; row++)
            {
                for (uint col = 0; col < width; col++)
                {
                    imageBytes[col + row * width] = pBits[col * 4 + row * stride];
                }
            }

            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// Sets the 8bpp bitmap to the bytes provided.
        /// </summary>
        /// <param name="bitmap">Bitmap to copy bytes from</param>
        /// <param name="imageBytes">Bytes to hold image.</param>
        static unsafe public void Copy8bppIndexedBitmapToMonochromeBytes(Bitmap bitmap, ref byte[] imageBytes)
        {
            // if not 8bpp indexed, don't do anything
            if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                return;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            //	get palette and make monochrome look up table
            ColorPalette pal = bitmap.Palette;
            byte[] LUT = new byte[pal.Entries.Length];
            for (int i = 0; i < pal.Entries.Length; i++)
            {
                LUT[i] = (byte)(255.0 * pal.Entries[i].GetBrightness());
            }

            //	set size of bytes
            imageBytes = new byte[width * height];

            //	copy the bytes
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            IntPtr pixels = bitmapData.Scan0;
            byte* pBits = (byte*)pixels.ToPointer();
            uint stride = (uint)Math.Abs(bitmapData.Stride);
            //	just get red plane
            for (uint row = 0; row < height; row++)
            {
                for (uint col = 0; col < width; col++)
                {
                    imageBytes[col + row * width] = LUT[pBits[col + row * stride]];
                }
            }

            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// Converts bitmap from Monochrome to 8bpp Indexed
        /// </summary>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="imageBytes">Image bytes</param>
        /// <returns></returns>
        static unsafe public Bitmap CopyMonochromeBytesTo8bppIndexedBitmap(int width, int height, byte[] imageBytes)
        {
            Bitmap bitmap = null;
            if (width > 1 && height > 1)
            {
                bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                ColorPalette pal = bitmap.Palette;
                for (int i = 0; i < pal.Entries.Length; i++)
                {
                    pal.Entries[i] = Color.FromArgb(i, i, i);
                }
                bitmap.Palette = pal;

                //	copy the bytes
                Rectangle rect = new Rectangle(0, 0, width, height);
                BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                IntPtr pixels = bitmapData.Scan0;
                byte* pBits = (byte*)pixels.ToPointer();
                uint stride = (uint)Math.Abs(bitmapData.Stride);
                //	
                for (uint row = 0; row < height; row++)
                {
                    for (uint col = 0; col < width; col++)
                    {
                        pBits[col + row * stride] = imageBytes[col + row * width];
                    }
                }

                bitmap.UnlockBits(bitmapData);
            }
            return bitmap;
        }

        /// <summary>
        /// Converts bytes from an image to monochrome then copies them to a byte array
        /// </summary>
        /// <param name="bitmap">Source image</param>
        /// <param name="imageBytes">Byte array</param>
        static unsafe public void CopyBitmapToMonochromeBytes(Bitmap bitmap, ref byte[] imageBytes)
        {
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format32bppRgb:
                    Copy32bppBitmapToMonochromeBytes(bitmap, ref imageBytes);
                    break;
                case PixelFormat.Format24bppRgb:
                    Copy24bppBitmapToMonochromeBytes(bitmap, ref imageBytes);
                    break;
                case PixelFormat.Format8bppIndexed:
                    Copy8bppIndexedBitmapToMonochromeBytes(bitmap, ref imageBytes);
                    break;
            }
        }

        #endregion


        public static Bitmap ConvertTo8Bpp(Bitmap tmpBmp)
        {
            //  convert to bytes
            byte[] imageBytes = null;

            CopyBitmapToMonochromeBytes(tmpBmp, ref imageBytes);

            //  convert to bmp
            return CopyMonochromeBytesTo8bppIndexedBitmap(tmpBmp.Width, tmpBmp.Height, imageBytes);
        }

        internal static Bitmap BlendBitmaps(Bitmap bitmap1, Bitmap bitmap2, float bitmap2Frac)
        {
            if (bitmap1.Width != bitmap2.Width || bitmap1.Height != bitmap2.Height)
            {
                return bitmap1;
            }

            byte[] imageBytes1 = null;
            byte[] imageBytes2 = null;

            CopyBitmapToMonochromeBytes(bitmap1, ref imageBytes1);
            CopyBitmapToMonochromeBytes(bitmap2, ref imageBytes2);

            for (int n = 0; n < imageBytes1.Length; n++)
            {
                float a = imageBytes1[n];
                float b = imageBytes2[n];
                float c = a + bitmap2Frac * (b - a);
                imageBytes1[n] = (byte)c;
            }

            return CopyMonochromeBytesTo8bppIndexedBitmap(bitmap1.Width, bitmap1.Height, imageBytes1);
        }
    }
}
