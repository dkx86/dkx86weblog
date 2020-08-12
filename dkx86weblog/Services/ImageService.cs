using Microsoft.VisualBasic.CompilerServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;
using SQLitePCL;
using System;
using System.IO;

namespace dkx86weblog.Services
{
    public class ImageMetadata
    {
        public string Camera { get; internal set; }
        public string ExposureTime { get; internal set; }
        public double ExposureFNumber { get; internal set; }
        public double FocalLength { get; internal set; }
        public int ISO { get; internal set; }
        public int Height { get; internal set; }
        public int Width { get; internal set; }
    }

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

        internal ImageMetadata GetImageMetadata(string inputFile)
        {

            using (Image image = Image.Load(inputFile))
            {
                ImageMetadata imageMetadata = new ImageMetadata();
                var metadata = image.Metadata;
                if (metadata == null)
                    return null;
                var exif = metadata.ExifProfile;
                if (exif == null)
                    return null;

                // Camera model
                var cameraMaker = exif.GetValue(ExifTag.Make);
                var cameraModel = exif.GetValue(ExifTag.Model);
                if (cameraMaker != null && cameraModel != null)
                {
                    imageMetadata.Camera = cameraMaker.ToString() + ' ' + cameraModel.ToString();
                }

                // ISO
                var iso = exif.GetValue(ExifTag.ISOSpeedRatings).ToString();
                imageMetadata.ISO = string.IsNullOrEmpty(iso) ? -1 : int.Parse(iso);

                // Exposure time
                var exposureTime = exif.GetValue(ExifTag.ExposureTime);
                if (exposureTime != null)
                {
                    imageMetadata.ExposureTime = exposureTime.ToString();
                }

                // Aperture value (f-stop)
                var fNumber = exif.GetValue(ExifTag.FNumber);
                if (fNumber != null && fNumber.ToString().Length > 0)
                {
                    imageMetadata.ExposureFNumber = CalcFractionalValue(fNumber.ToString());
                }

                //Focal length
                var focalLength = exif.GetValue(ExifTag.FocalLength);
                if (focalLength != null && focalLength.ToString().Length > 0)
                {
                    imageMetadata.FocalLength = CalcFractionalValue(focalLength.ToString());
                }


                // height and width
                imageMetadata.Height = image.Height;
                imageMetadata.Width = image.Width;

                return imageMetadata;
            }

        }

        private double CalcFractionalValue(string val)
        {
            if (!val.Contains("/"))
                return double.Parse(val);

            string[] parts = val.Split("/");
            double left = double.Parse(parts[0]);
            double right = double.Parse(parts[1]);

            return left / right;
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
