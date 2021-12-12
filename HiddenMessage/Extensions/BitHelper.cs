using System.Text;
using System.Collections;

namespace HiddenMessage.Extensions
{
    public static class BitHelper
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

        public static BitArray ToBinary(this byte number)
        {
            BitArray bitArray = new(new byte[] { number });
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
    }
}
