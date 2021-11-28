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

            for (int i = 0; i < 32; i++)
            {
                Color color = bitmap.GetPixel(i, 0);
                BitArray bitArray = color.ToArgb().ToBinary();
                bitArray.SetBit(bitLengthMessage[i]);
                bitmap.SetPixel(i, 0, Color.FromArgb(bitArray.ToNumeric()));
            }

            int messIndex = 0;

            for (int i = 0; i < bitmap.Height && messIndex != bitMessage.Length; i++)
            {
                for (int j = 0; j < bitmap.Width && messIndex != bitMessage.Length; j++)
                {
                    if (!(j == 0 && i < 32))
                    {
                        Color color = bitmap.GetPixel(i, j);

                        BitArray bitArray = color.ToArgb().ToBinary();
                        bitArray.SetBit(bitMessage[messIndex++]);

                        bitmap.SetPixel(i, j, Color.FromArgb(bitArray.ToNumeric()));
                    }
                }
            }

            return bitmap;
        }

        public string LSBDecode(Bitmap bitmap)
        {
            string bitLengthMessage = "";

            for (int i = 0; i < 32; i++)
            {
                Color color = bitmap.GetPixel(i, 0);
                BitArray bitArray = color.ToArgb().ToBinary();
                bitLengthMessage = (bitArray[0] ? "1" : "0") + bitLengthMessage;
            }

            int lengthMessage = Convert.ToInt32(bitLengthMessage, 2);

            string bitMessage = "";

            for (int i = 0; i < bitmap.Height && lengthMessage != bitMessage.Length; i++)
            {
                for (int j = 0; j < bitmap.Width && lengthMessage != bitMessage.Length; j++)
                {
                    if (!(j == 0 && i < 32))
                    {
                        Color color = bitmap.GetPixel(i, j);

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
