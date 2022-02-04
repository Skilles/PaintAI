using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Media;
using SimpleEngine;

namespace PaintAI
{
    public class Helper
    {
        private VideoFrame cropped_vf = null;

        private class DirectBuffer : Windows.Storage.Streams.IBuffer
        {
            public uint Capacity { get; set; }

            public uint Length { get; set; }

            public DirectBuffer(DirectBitmap bitmap)
            {
                this.Length = (uint)(bitmap.Width * bitmap.Height / 8);
                this.Capacity = (uint)(bitmap.Width * bitmap.Height / 8);
            }
        }

        public async Task<VideoFrame> GetHandWrittenImage(PictureBoxRenderer boxRenderer)
        {
            var bitmap = boxRenderer.GetBitmap().Bitmap;
            IBuffer buffer;
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                buffer = stream.ToArray().AsBuffer();
            }
            var softwareBitmap = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, bitmap.Width, bitmap.Height, BitmapAlphaMode.Ignore);

            buffer = null;
            bitmap.Dispose();

            VideoFrame vf = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);
            await CropAndDisplayInputImageAsync(vf);

            return cropped_vf;
        }

        private async Task CropAndDisplayInputImageAsync(VideoFrame inputVideoFrame)
        {
            bool useDX = inputVideoFrame.SoftwareBitmap == null;

            BitmapBounds cropBounds = new BitmapBounds();
            uint h = 28;
            uint w = 28;
            var frameHeight = useDX ? inputVideoFrame.Direct3DSurface.Description.Height : inputVideoFrame.SoftwareBitmap.PixelHeight;
            var frameWidth = useDX ? inputVideoFrame.Direct3DSurface.Description.Width : inputVideoFrame.SoftwareBitmap.PixelWidth;

            var requiredAR = ((float)28 / 28);
            w = Math.Min((uint)(requiredAR * frameHeight), (uint)frameWidth);
            h = Math.Min((uint)(frameWidth / requiredAR), (uint)frameHeight);
            cropBounds.X = (uint)((frameWidth - w) / 2);
            cropBounds.Y = 0;
            cropBounds.Width = w;
            cropBounds.Height = h;

            cropped_vf = new VideoFrame(BitmapPixelFormat.Bgra8, 28, 28, BitmapAlphaMode.Ignore);

            await inputVideoFrame.CopyToAsync(cropped_vf, cropBounds, null);
        }
    }
}