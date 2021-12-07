using System;

namespace HiddenMessage.Extensions
{
    public static class TextHelper
    {
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
