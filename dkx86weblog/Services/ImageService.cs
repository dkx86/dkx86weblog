using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace dkx86weblog.Services
{
    public class ImageService
    {
        private readonly int JPG_QUALITY = 95;

        public ImageResizeResult Resize(string inputFile, string outputFile, int longEdgeSize)
        {
            using (Image image = Image.Load(inputFile, out IImageFormat format))
            {
                ImageResizeResult resizeResult = new ImageResizeResult(image.Height, image.Width);
                
                if (!NeedResize(image, longEdgeSize))
                    return resizeResult;

                int width = 0;
                int height = 0;
                if (image.Width > image.Height)
                {
                    width = longEdgeSize;
                    height = AdjustedHeight(width, image.Height, image.Width);
                }
                else
                {
                    height = longEdgeSize;
                    width = AdjustedWidth(height, image.Height, image.Width);
                }

                image.Mutate(x => x.Resize(width, height));
                SaveMutatedImage(outputFile, image, format);
                resizeResult.Height = height;
                resizeResult.Width = width;
                return resizeResult;
            }
        }

        public ImageResizeResult ResizeByWidth(string inputFile, string outputFile, int width)
        {
            using (Image image = Image.Load(inputFile, out IImageFormat format))
            {
                ImageResizeResult resizeResult = new ImageResizeResult(image.Height, image.Width);
                int height = AdjustedHeight(width, image.Height, image.Width);
                image.Mutate(x => x.Resize(width, height));
                SaveMutatedImage(outputFile, image, format);
                resizeResult.Height = height;
                resizeResult.Width = width;
                return resizeResult;

            }
        }

        private void SaveMutatedImage(string outputFile, Image image, IImageFormat format)
        {
            using (var outputStream = new FileStream(outputFile, FileMode.Create))
            {
                if (format.Name == JpegFormat.Instance.Name)
                    image.SaveAsJpeg(outputStream, new JpegEncoder { Quality = JPG_QUALITY });
                else if (format.Name == PngFormat.Instance.Name)
                    image.SaveAsPng(outputStream, new PngEncoder { CompressionLevel = 9 });
                else
                    image.Save(outputStream, format);
            }
        }

        private int AdjustedHeight(int newWidth, int height, int width)
        {
            return newWidth * height / width;
        }

        private int AdjustedWidth(int newHeight, int height, int width)
        {
            return newHeight * width / height;
        }

        private float AspectRation(int height, int width)
        {
            return width / height;
        }

        private bool NeedResize(Image image, int longEdgeSize)
        {
            return image.Height > longEdgeSize || image.Width > longEdgeSize;
        }

        public class ImageResizeResult
        {
            public int OriginalHeight { get; }
            public int OriginalWidth { get; }
            public int Height { get; set; }
            public int Width { get; set; }

            public ImageResizeResult(int originalHeight, int originalWidth)
            {
                OriginalHeight = originalHeight;
                OriginalWidth = originalWidth;
            }
        }
    }
}
