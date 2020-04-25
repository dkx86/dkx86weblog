using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace dkx86weblog.Services
{
    public class ImageService
    {
        public void Resize(string inputFile, string outputFile, int longEdgeSize)
        {
            using (Image image = Image.Load(inputFile, out IImageFormat format))
            {
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
            }
        }

        public void ResizeByWidth(string inputFile, string outputFile, int width)
        {
            using (Image image = Image.Load(inputFile, out IImageFormat format))
            {
                int height = AdjustedHeight(width, image.Height, image.Width);
                image.Mutate(x => x.Resize(width, height));
                SaveMutatedImage(outputFile, image, format);
            }
        }

        private void SaveMutatedImage(string outputFile, Image image, IImageFormat format)
        {
            using (var outputStream = new FileStream(outputFile, FileMode.Create))
            {
                if (format.Name == JpegFormat.Instance.Name)
                    image.SaveAsJpeg(outputStream, new JpegEncoder { Quality = 90 });
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
    }
}
