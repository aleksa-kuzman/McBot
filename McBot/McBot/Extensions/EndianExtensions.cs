using System;
using System.Linq;

namespace McBot.Extensions
{
    public static class EndianExtensions
    {
        public static Int32 ConvertToBigEndian(this Int32 integer)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt32(BitConverter.GetBytes(integer).Reverse().ToArray());
            }
            return integer;
        }

        public static UInt32 ConvertToBigEndian(this UInt32 integer)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(BitConverter.GetBytes(integer).Reverse().ToArray());
            }
            return integer;
        }

        public static Int16 ConvertToBigEndian(this Int16 integer)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt16(BitConverter.GetBytes(integer).Reverse().ToArray());
            }
            return integer;
        }

        public static UInt16 ConvertToBigEndian(this UInt16 integer)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt16(BitConverter.GetBytes(integer).Reverse().ToArray());
            }
            return integer;
        }
    }
}