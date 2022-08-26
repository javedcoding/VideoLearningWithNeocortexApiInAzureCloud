using Emgu.CV;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyExperiment.SEProject
{
    /// <summary>
    /// This changes images to be able to handled by SkiaSharp
    /// </summary>
    public class ImageMaker
    {
        /// <summary>
        /// This creates Bitmap image from a matrix
        /// </summary>
        /// <param name="matrix">Emgu.CV.Mat object for data</param>
        /// <returns></returns>
        public static SKBitmap MatToImage(Mat matrix)
        {

            byte[,,] pixelArray = (byte[,,])matrix.GetData();
            int width = pixelArray.GetLength(1);
            int height = pixelArray.GetLength(0);

            uint[] pixelValues = new uint[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte alpha = 255;
                    byte red = pixelArray[y, x, 0];
                    byte green = pixelArray[y, x, 1];
                    byte blue = pixelArray[y, x, 2];
                    uint pixelValue = (uint)(blue << 0) + (uint)(green << 8) + (uint)(red << 16) + (uint)(alpha << 24);
                    pixelValues[y * width + x] = pixelValue;
                }
            }

            SKBitmap bitmap = new SKBitmap();
            GCHandle gcHandle = GCHandle.Alloc(pixelValues, GCHandleType.Pinned);
            SKImageInfo info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);

            IntPtr ptr = gcHandle.AddrOfPinnedObject();
            int rowBytes = info.RowBytes;
            bitmap.InstallPixels(info, ptr, rowBytes, delegate { gcHandle.Free(); });

            return bitmap;
        }

        /// <summary>
        /// This creates Matrix from a bitmap image
        /// </summary>
        /// <param name="a">Bitmap Representation of an image</param>
        /// <returns>Emgu.CV.Mat object</returns>
        public static Mat ImageToMat(SKBitmap a)
        {
            Mat d = new Mat(a.Height, a.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 4);

            d.SetTo((a.Copy(SKColorType.Bgra8888)).Bytes);
            return d;
        }
    }
}
