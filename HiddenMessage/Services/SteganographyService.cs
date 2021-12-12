using System;
using System.Drawing;
using System.Collections;
using HiddenMessage.Extensions;

namespace HiddenMessage.Services
{
    public class SteganographyService
    {

        public Bitmap LSBEncode(Bitmap bitmap, string message)
        {
            BitArray bitMessage = message.ToBinary();

            BitArray bitLengthMessage = bitMessage.Length.ToBinary();

            for (int y = 0; y < 32; y++)
            {
                Color color = bitmap.GetPixel(0, y);
                BitArray bitArray = color.B.ToBinary();
                bitArray.SetBit(bitLengthMessage[y]);
                bitmap.SetPixel(0, y, Color.FromArgb(color.R, color.G, bitArray.ToNumeric()));
            }

            int messIndex = 0;

            for (int y = 0; y < bitmap.Height && messIndex != bitMessage.Length; y++)
            {
                for (int x = 0; x < bitmap.Width && messIndex != bitMessage.Length; x++)
                {
                    if (!(x == 0 && y < 32))
                    {
                        Color color = bitmap.GetPixel(x, y);

                        int[] rgb = { color.R, color.G, color.B };

                        for (int k = 0; k < 3 && messIndex != bitMessage.Length; k++)
                        {
                            byte byteColor = k == 0 ? color.R : k == 1 ? color.G : color.B;

                            BitArray bitArray = byteColor.ToBinary();
                            bitArray.SetBit(bitMessage[messIndex++]);

                            rgb[k] = bitArray.ToNumeric();
                        }

                        bitmap.SetPixel(x, y, Color.FromArgb(rgb[0], rgb[1], rgb[2]));
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
                BitArray bitArray = color.B.ToBinary();
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

                        for (int k = 0; k < 3 && lengthMessage != bitMessage.Length; k++)
                        {
                            byte byteColor = k == 0 ? color.R : k == 1 ? color.G : color.B;

                            BitArray bitArray = byteColor.ToBinary();
                            bitMessage = (bitArray[0] ? "1" : "0") + bitMessage;
                        }
                    }
                }
            }

            return bitMessage.BitsToChar().Reverse();
        }
    }
}
