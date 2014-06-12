using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRA_1.Data
{
    public class ImageData
    {
        #region Class Variables

        private int width;
        private int height;
        private byte[] data;
        private int depth = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Width property
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Gets or sets the Height property
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        public void SetValue(byte v)
        {
            for (int n = 0; n < data.Length; n++)
            {
                data[n] = v;
            }
        }

        public byte this[int x, int y, int z]
        {
            get { return data[z + depth * (x + y * width)]; }
            set { data[z + depth * (x + y * width)] = value; }
        }

        /// <summary>
        /// Gets or sets a Value in the underlying data array
        /// </summary>
        public byte this[int x, int y]
        {
            get
            {
                if (ValidAddress(x, y))
                {
                    return data[depth * (x + y * width)];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (ValidAddress(x, y))
                {
                    data[depth * (x + y * width)] = value;
                }
            }
        }

        bool ValidAddress(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// Initializes class variables
        /// </summary>
        public ImageData()
        {
            width = 1;
            height = 1;
            depth = 1;
            data = new byte[width * height * Depth];
        }

        public ImageData(int w, int h)
        {
            width = w;
            height = h;
            depth = 1;
            data = new byte[width * height * Depth];
        }

        public ImageData(ImageData a)
        {
            width = a.Width;
            height = a.Height;
            depth = a.Depth;
            data = new byte[width * height * Depth];

            byte[] aData = a.GetData();
            for (int n = 0; n < data.Length; n++)
            {
                data[n] = aData[n];
            }

        }

        #endregion

        #region Functions

        public void StretchMaxTo(int maxVal)
        {
            // find max

            int max = GetMax();

            if (max > 0)
            {
                //  compute scale
                double scale = (double)maxVal / (double)max;

                //  apply scale via LUT
                byte[] LUT = new byte[256];
                for (int n = 0; n < 256; n++)
                {
                    int k = Math.Min(Math.Max((int)(n * scale), 0), 255);
                    LUT[n] = (byte)k;
                }

                //  apply LUT
                for (int n = 0; n < data.Length; n++)
                {
                    data[n] = LUT[data[n]];
                }
            }
        }

        public int GetMax()
        {
            int max = -1;
            for (int n = 0; n < data.Length; n++)
            {
                max = Math.Max(max, data[n]);
            }

            return max;
        }

        /// <summary>
        /// Returns the byte array data in the form of a Bitmap
        /// </summary>
        /// <returns>Bitmap</returns>
        public Bitmap GetBitmap()
        {
            if (data.Length < 10)
            {
                return null;
            }

            Bitmap bmp = null;
            if (Depth == 1)
            {
                BitmapUtility.CopyBytesTo8bppBitmap(Width, Height, data, ref bmp);
            }
            else
            {
                bmp = BitmapUtility.CopyColorBytesTo24bppBitmap(Width, Height, Depth, data);
            }
            return bmp;
        }

        /// <summary>
        /// Gets the underlying data array
        /// </summary>
        /// <returns>Byte array</returns>
        public byte[] GetData()
        {
            return data;
        }

        public void MakeMonochrome()
        {
            if (Depth == 3)
            {
                byte[] newData = new byte[Width * Height];
                for (int j = 0; j < Height; j++)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        newData[i + j * Width] = (byte)((this[i, j, 0] + this[i, j, 1] + this[i, j, 2]) / 3);
                    }
                }
                SetData(Width, Height, newData);
            }
        }

        /// <summary>
        /// Initializes underlying data array
        /// </summary>
        /// <param name="w">Array width</param>
        /// <param name="h">Array height</param>
        /// <param name="dataBytes">Data array</param>
        public void SetData(int w, int h, byte[] dataBytes)
        {
            SetData(w, h, 1, dataBytes);
        }

        public void SetData(int w, int h, int d, byte[] dataBytes)
        {
            Width = w;
            Height = h;
            Depth = d;
            data = dataBytes;
        }

        /// <summary>
        /// Initializes the underlying array to the width and height provided
        /// </summary>
        /// <param name="newWidth">New width</param>
        /// <param name="newHeight">New height</param>
        public void Initialize(int newWidth, int newHeight)
        {
            Initialize(newWidth, newHeight, 1);
        }

        /// <summary>
        /// Initializes the underlying array to the width and height provided
        /// </summary>
        /// <param name="newWidth">New width</param>
        /// <param name="newHeight">New height</param>
        public void Initialize(int newWidth, int newHeight, int newDepth)
        {
            Width = newWidth;
            Height = newHeight;
            Depth = newDepth;
            data = new byte[Depth * Width * Height];
        }

        /// <summary>
        /// Saves the image to the file system
        /// </summary>
        /// <param name="filename">File path on the file system to save to</param>
        /// <returns>true if file saved successfully, otherwise false</returns>
        public bool Save(string filename)
        {
            bool status = true;

            if (status == true && filename == null)
            {
                status = false;
            }

            if (status == true && filename.Length < 5)
            {
                status = false;
            }

            try
            {
                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(filename)) == false)
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
                }
            }
            catch
            {
                status = false;
            }

            if (status == true)
            {
                Bitmap bmp = GetBitmap();

                if (bmp != null)
                {
                    string s = Path.GetExtension(filename);
                    s = s.ToUpper();
                    if (s == ".JPG")
                    {
                        bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    else if (s == ".BMP")
                    {
                        bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                    }

                    bmp.Dispose();
                }
            }

            return status;
        }

        /// <summary>
        /// Loads byte array from an image on the file system
        /// </summary>
        /// <param name="filename">File path to image on the file system</param>
        /// <returns>true if image loaded successfully, otherwise false</returns>
        public bool Load(string filename)
        {
            bool status = true;

            if (status == true && filename == null)
            {
                status = false;
            }

            if (status == true && System.IO.File.Exists(filename) == false)
            {
                status = false;
            }

            Bitmap bmp = null;
            if (status == true)
            {
                try
                {
                    bmp = (Bitmap)Bitmap.FromFile(filename);
                    Load(bmp);
                    bmp.Dispose();
                }
                catch
                {
                    status = false;
                }
            }
            return status;
        }

        /// <summary>
        /// Loads byte array from a bitmap
        /// </summary>
        /// <param name="bmp">Bitmap with byte data</param>
        /// <returns>true if image loaded successfully, otherwise false</returns>
        public bool Load(Bitmap bmp)
        {
            bool status = true;

            try
            {
                Width = bmp.Width;
                Height = bmp.Height;
                Depth = (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed) ? 1 : 3;
                if (Depth == 1)
                {
                    BitmapUtility.CopyBitmapToMonochromeBytes(bmp, ref data);
                }
                else
                {
                    BitmapUtility.Copy24bppBitmapToColorBytes(bmp, out data);
                }
            }
            catch
            {
                status = false;
            }

            return status;
        }

        public byte[,] Get2DArray()
        {
            byte[,] valueArray = new byte[Width, Height];

            if (Depth == 1)
            {
                for (int j = 0; j < Height; j++)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        valueArray[i, j] = this[i, j];
                    }
                }
            }
            else
            {
                for (int j = 0; j < Height; j++)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        valueArray[i, j] = (byte)((this[i, j, 0] + this[i, j, 1] + this[i, j, 2]) / 3);
                    }
                }
            }

            return valueArray;
        }

        static public byte Interpolate(int width, int height, byte[] tempData, double x, double y)
        {
            double v = 0;

            int x0 = (int)x;
            int y0 = (int)y;
            double xFrac = x - x0;
            double yFrac = y - y0;

            if (x0 >= 0 && x0 < width - 1 && y0 >= 0 && y0 < height - 1)
            {
                double p00 = tempData[x0 + y0 * width];
                double p01 = tempData[x0 + (y0 + 1) * width];
                double p10 = tempData[x0 + 1 + y0 * width];
                double p11 = tempData[x0 + 1 + (y0 + 1) * width];

                double a = p00 + xFrac * (p10 - p00);
                double b = p01 + xFrac * (p11 - p01);
                v = a + yFrac * (b - a);
                v = Math.Min(Math.Max(v, 0), 255);
            }

            return (byte)v;
        }

        public void RotateScaleArray(double sx, double sy, double thetaDeg)
        {
            byte[] tempData = (byte[])data.Clone();

            double thetaRad = -thetaDeg * Math.PI / 180.0;
            double c = Math.Cos(thetaRad);
            double s = Math.Sin(thetaRad);

            int xc = Width / 2;
            int yc = Height / 2;

            for (int jDst = 0; jDst < Height; jDst++)
            {
                for (int iDst = 0; iDst < Width; iDst++)
                {
                    double dx = (iDst - xc) * sx;
                    double dy = (jDst - yc) * sy;
                    double iSrc = c * dx - s * dy + xc;
                    double jSrc = s * dx + c * dy + yc;

                    this[iDst, jDst] = Interpolate(Width, Height, tempData, iSrc, jSrc);
                }
            }
        }

        public void NewCenterPoint(int newXc, int newYc)
        {
            int xc = Width / 2;
            int yc = Height / 2;

            int tx = xc - newXc;
            int ty = yc - newYc;

            Translate(tx, ty);
        }

        public void Translate(double tx, double ty)
        {
            byte[] tempData = (byte[])data.Clone();

            data.Initialize();

            for (int jDst = 0; jDst < Height; jDst++)
            {
                for (int iDst = 0; iDst < Width; iDst++)
                {
                    double iSrc = iDst - tx;
                    double jSrc = jDst - ty;

                    if (iSrc >= 0 && iSrc < Width && jSrc >= 0 && jSrc < Height)
                    {
                        this[iDst, jDst] = Interpolate(Width, Height, tempData, iSrc, jSrc);
                    }
                }
            }
        }

        public void Translate(int tx, int ty)
        {
            byte[] tempData = (byte[])data.Clone();

            data.Initialize();

            for (int jDst = 0; jDst < Height; jDst++)
            {
                for (int iDst = 0; iDst < Width; iDst++)
                {
                    int iSrc = iDst - tx;
                    int jSrc = jDst - ty;

                    if (iSrc >= 0 && iSrc < Width && jSrc >= 0 && jSrc < Height)
                    {
                        this[iDst, jDst] = tempData[iSrc + jSrc * Width];
                    }
                }
            }
        }

        public PointF GetCentroid(int threshold)
        {
            PointF centroid = new PointF();

            double sumX = 0;
            double sumY = 0;
            double sum = 0;

            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    int k = this[i, j];
                    if (k > threshold)
                    {
                        sumX += k * i;
                        sumY += k * j;
                        sum += k;
                    }
                }
            }

            if (sum == 0)
            {
                centroid.X = Width / 2;
                centroid.Y = Height / 2;
            }
            else
            {
                centroid.X = (float)(sumX / sum);
                centroid.Y = (float)(sumY / sum);
            }

            return centroid;
        }

        #endregion

        public int[] GetHistogram(int x0, int y0, int x1, int y1)
        {
            int[] hist = new int[256];

            for (int j = y0; j < y1; j++)
            {
                for (int i = x0; i < x1; i++)
                {
                    int k = this[i, j];
                    hist[k] += 1;
                }
            }

            return hist;
        }

        public void ApplyLUT(int[] LUT)
        {
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    this[i, j] = (byte)LUT[this[i, j]];
                }
            }
        }

        public void AddOffset(int offset)
        {
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    int k = this[i, j] + offset;
                    this[i, j] = (byte)Math.Min(Math.Max(k, 0), 255);
                }
            }

        }

        public int[] GetHistogram()
        {
            return GetHistogram(0, 0, Width, Height);
        }

        internal byte Interpolate(double x, double y)
        {
            byte v = 0;

            int x0 = (int)x;
            int y0 = (int)y;
            double xFrac = x - x0;
            double yFrac = y - y0;

            if (x0 >= 0 && x0 < width - 1 && y0 >= 0 && y0 < height - 1)
            {
                double p00 = this[x0, y0];
                double p01 = this[x0, y0 + 1];
                double p10 = this[x0 + 1, y0];
                double p11 = this[x0 + 1, y0 + 1];

                double a = p00 + xFrac * (p10 - p00);
                double b = p01 + xFrac * (p11 - p01);
                double c = a + yFrac * (b - a);

                v = (byte)Math.Min(Math.Max(c, 0.0), 255.0);
            }

            return v;

        }

        internal void ScaleRotateTranslate(double sx, double sy, double thetaDeg, double tx, double ty)
        {
            byte[] tempData = (byte[])data.Clone();

            double thetaRad = thetaDeg * Math.PI / 180.0;
            double c = Math.Cos(thetaRad);
            double s = Math.Sin(thetaRad);

            for (int jDst = 0; jDst < Height; jDst++)
            {
                for (int iDst = 0; iDst < Width; iDst++)
                {
                    double dx = (iDst - tx) / sx;
                    double dy = (jDst - ty) / sy;
                    double iSrc = c * dx - s * dy;
                    double jSrc = s * dx + c * dy;

                    this[iDst, jDst] = Interpolate(Width, Height, tempData, iSrc, jSrc);
                }
            }
        }

        public void FlipTopToBottom()
        {
            byte[] tempData = (byte[])data.Clone();

            for (int jDst = 0; jDst < Height; jDst++)
            {
                for (int iDst = 0; iDst < Width; iDst++)
                {
                    double iSrc = iDst;
                    double jSrc = Height - 1 - jDst;

                    this[iDst, jDst] = Interpolate(Width, Height, tempData, iSrc, jSrc);
                }
            }
        }

        public void MaskOutsideCircle(double xcSpots, double ycSpots, double radius)
        {
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    double dx = i - xcSpots;
                    double dy = j - ycSpots;
                    double r = Math.Sqrt(dx * dx + dy * dy);
                    if (r > radius)
                    {
                        this[i, j] = 0;
                    }
                }
            }
        }

        public void StretchInsideCircle(double xcSpots, double ycSpots, double radius, double desiredMean = 100)
        {
            double sum = 0;
            double num = 0;

            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    double dx = i - xcSpots;
                    double dy = j - ycSpots;
                    double r = Math.Sqrt(dx * dx + dy * dy);
                    if (r <= radius)
                    {
                        sum += this[i, j];
                        num += 1;
                    }
                }
            }

            if (num > 0 && sum > 0)
            {
                double scale = desiredMean * num / sum;

                for (int j = 0; j < Height; j++)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        double dx = i - xcSpots;
                        double dy = j - ycSpots;
                        double r = Math.Sqrt(dx * dx + dy * dy);
                        if (r <= radius)
                        {
                            double t = this[i, j] * scale;
                            t = Math.Min(Math.Max(t, 0), 255);
                            this[i, j] = (byte)t;
                        }
                    }
                }
            }

        }
    }

}
