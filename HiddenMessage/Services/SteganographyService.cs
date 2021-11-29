using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Threading.Tasks;
using HiddenMessage.Extensions;
using Microsoft.AspNetCore.Components.Forms;

namespace HiddenMessage.Services
{
    public class SteganographyService
    {
        private const int MaxInputFile = 5 * 1024 * 1024;

        public Bitmap LSBEncode(Bitmap bitmap, string message)
        {
            BitArray bitMessage = message.ToBinary();

            BitArray bitLengthMessage = bitMessage.Length.ToBinary();

            for (int y = 0; y < 32; y++)
            {
                Color color = bitmap.GetPixel(0, y);
                BitArray bitArray = color.ToArgb().ToBinary();
                bitArray.SetBit(bitLengthMessage[y]);
                bitmap.SetPixel(0, y, Color.FromArgb(bitArray.ToNumeric()));
            }

            int messIndex = 0;

            for (int y = 0; y < bitmap.Height && messIndex != bitMessage.Length; y++)
            {
                for (int x = 0; x < bitmap.Width && messIndex != bitMessage.Length; x++)
                {
                    if (!(x == 0 && y < 32))
                    {
                        Color color = bitmap.GetPixel(x, y);

                        BitArray bitArray = color.ToArgb().ToBinary();
                        bitArray.SetBit(bitMessage[messIndex++]);

                        bitmap.SetPixel(x, y, Color.FromArgb(bitArray.ToNumeric()));
                    }
                }
            }

            return bitmap;
        }

        public string LSBDecode(Bitmap bitmap)
        {
            string bitLengthMessage = "";

            for (int y = 0; y < 32; y++)
            {
                Color color = bitmap.GetPixel(0, y);
                BitArray bitArray = color.ToArgb().ToBinary();
                bitLengthMessage = (bitArray[0] ? "1" : "0") + bitLengthMessage;
            }

            int lengthMessage = Convert.ToInt32(bitLengthMessage, 2);

            string bitMessage = "";

            for (int y = 0; y < bitmap.Height && lengthMessage != bitMessage.Length; y++)
            {
                for (int x = 0; x < bitmap.Width && lengthMessage != bitMessage.Length; x++)
                {
                    if (!(x == 0 && y < 32))
                    {
                        Color color = bitmap.GetPixel(x, y);

                        BitArray bitArray = color.ToArgb().ToBinary();

                        bitMessage = (bitArray[0] ? "1" : "0") + bitMessage;
                    }
                }
            }

            return bitMessage.BitsToChar().Reverse();
        }

        public async Task<Bitmap> GetBitmapAsync(IBrowserFile file)
        {
            using Stream stream = file.OpenReadStream(MaxInputFile);

            MemoryStream memoryStream = new();

            await stream.CopyToAsync(memoryStream);

            return new Bitmap(memoryStream);
        }

        public string GetImageUri(Bitmap bitmap)
        {
            MemoryStream memoryStream = new();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return $"data:image/jpeg;base64,{Convert.ToBase64String(memoryStream.ToArray())}";
        }
    }
}
