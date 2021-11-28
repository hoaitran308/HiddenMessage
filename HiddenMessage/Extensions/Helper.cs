using System.Text;
using System.Collections;
using System.Drawing;
using System;

namespace HiddenMessage.Extensions
{
    public static class Helper
    {
        public static BitArray ToBinary(this string str)
        {
            byte[] asciiBytes = Encoding.UTF8.GetBytes(str);
            BitArray bitArray = new(asciiBytes);
            return bitArray;
        }

        public static BitArray ToBinary(this int number)
        {
            BitArray bitArray = new(new int[] { number });
            return bitArray;
        }

        public static int ToNumeric(this BitArray bitArray)
        {
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        public static void SetBit(this BitArray bitArray, bool value)
        {
            bitArray.Set(0, value);
        }

        public static int MessageLength(this Bitmap bitmap) => (bitmap.Height * bitmap.Width - 32) / 8;

        private static char ConvertToChar(string value)
        { 
            return (char)Convert.ToInt32(value, 2);
        }

        public static string BitsToChar(this string value)
        {
            string result = "";

            for (int i = 0; i < value.Length / 8; ++i)
            {
                result += (ConvertToChar(value.Substring(8 * i, 8)));
            }

            return result;
        }

        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
