using System;
using System.IO;
using System.Text;

namespace SimpleBotter
{
    public static class PacketBuilder
    {
        public static byte[] BuildHandshake(string host, int port, int protocolVersion, int nextState)
        {
            using var ms = new MemoryStream();
            WriteVarInt(ms, protocolVersion);
            WriteString(ms, host);
            WriteUShort(ms, (ushort)port);
            WriteVarInt(ms, nextState);
            return PrependVarInt(ms.ToArray());
        }

        private static void WriteVarInt(MemoryStream stream, int value)
        {
            do
            {
                byte temp = (byte)(value & 0x7F);
                value >>= 7;
                if (value != 0) temp |= 0x80;
                stream.WriteByte(temp);
            } while (value != 0);
        }

        private static void WriteString(MemoryStream stream, string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            WriteVarInt(stream, bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static void WriteUShort(MemoryStream stream, ushort value)
        {
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value & 0xFF));
        }

        private static byte[] PrependVarInt(byte[] data)
        {
            using var ms = new MemoryStream();
            WriteVarInt(ms, data.Length);
            ms.Write(data, 0, data.Length);
            return ms.ToArray();
        }
    }
}