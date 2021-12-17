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

            int lengthIndex = 0;

            for (int y = 0; y < 11; y++)
            {
                Color color = bitmap.GetPixel(0, y);

                int[] rgb = { color.R, color.G, color.B };

                for (int k = 0; k < 3 && lengthIndex != 32; k++)
                {
                    BitArray bitArray = rgb[k].ToBinary();

                    bitArray.SetBit(bitLengthMessage[lengthIndex++]);

                    rgb[k] = bitArray.ToNumeric();
                }

                bitmap.SetPixel(0, y, Color.FromArgb(rgb[0], rgb[1], rgb[2]));
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
                            BitArray bitArray = rgb[k].ToBinary();

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

            for (int y = 0; y < 11; y++)
            {
                Color color = bitmap.GetPixel(0, y);

                for (int k = 0; k < 3 && bitLengthMessage.Length != 32; k++)
                {
                    byte byteColor = k == 0 ? color.R : k == 1 ? color.G : color.B;

                    BitArray bitArray = byteColor.ToBinary();

                    bitLengthMessage = (bitArray[0] ? "1" : "0") + bitLengthMessage;
                }
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
