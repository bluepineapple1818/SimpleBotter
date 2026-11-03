using System;
using System.IO;

namespace SimpleBotter.Utils
{
    public static class VarInt
    {
        public static int ReadVarInt(Stream stream)
        {
            int result = 0;
            int bytesRead = 0;
            byte b;
            do
            {
                b = (byte)stream.ReadByte();
                result |= (b & 0x7F) << (bytesRead++ * 7);
                if (bytesRead > 5) throw new Exception("VarInt too long");
            } while ((b & 0x80) == 0x80);
            return result;
        }
    }
}