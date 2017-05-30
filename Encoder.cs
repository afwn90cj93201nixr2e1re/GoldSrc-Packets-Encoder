using System;
using System.Linq;

namespace GoldSrc.NET
{
    static class Encoder
    {
        private static byte[] MungifyTable = { 0x7A, 0x64, 0x05, 0xF1, 0x1B, 0x9B, 0xA0, 0xB5, 0xCA, 0xED, 0x61, 0x0D, 0x4A, 0xDF, 0x8E, 0xC7 };
        private static byte[] MungifyTable2 = { 0x05, 0x61, 0x7A, 0xED, 0x1B, 0xCA, 0x0D, 0x9B, 0x4A, 0xF1, 0x64, 0xC7, 0xB5, 0x8E, 0xDF, 0xA0 };
        private static byte[] MungifyTable3 = { 0x20, 0x07, 0x13, 0x61, 0x03, 0x45, 0x17, 0x72, 0x0A, 0x2D, 0x48, 0x0C, 0x4A, 0x12, 0xA9, 0xB5 };

        private static byte[] MungeInternal(byte[] data, int seq, byte[] table)
        {
            int size = (data.Length & ~3) >> 2;

            var result = new byte[data.Length];

            Array.Copy(data, result, data.Length);

            for (int i = 0; i < size; i++)
            {
                var b = BitConverter.GetBytes(BitConverter.ToInt32(data.Skip(i << 2).Take(4).ToArray(), 0) ^ ~seq).Reverse().ToArray();

                for (int j = 0; j < 4; j++)
                {
                    b[j] ^= (byte)(0xA5 | (j << j) | j | table[(i + j) & 0x0F]);
                }

                System.Buffer.BlockCopy(BitConverter.GetBytes(BitConverter.ToInt32(b, 0) ^ seq), 0, result, i << 2, 4);
            }

            return result;
        }

        private static byte[] UnMungeInternal(byte[] data, int seq, byte[] table)
        {
            int size = (data.Length & ~3) >> 2;

            var result = new byte[data.Length];

            Array.Copy(data, result, data.Length);

            for (int i = 0; i < size; i++)
            {
                var b = BitConverter.GetBytes(BitConverter.ToInt32(data.Skip(i << 2).Take(4).ToArray(), 0) ^ seq);

                for (int j = 0; j < 4; j++)
                {
                    b[j] ^= (byte)(0xA5 | (j << j) | j | table[(i + j) & 0x0F]);
                }

                System.Buffer.BlockCopy(BitConverter.GetBytes(BitConverter.ToInt32(b.Reverse().ToArray(), 0) ^ ~seq), 0, result, i << 2, 4);
            }

            return result;
        }

        public static byte[] Munge(byte[] data, int seq)
        {
            return MungeInternal(data, seq, MungifyTable);
        }

        public static byte[] UnMunge(byte[] data, int seq)
        {
            return UnMungeInternal(data, seq, MungifyTable);
        }

        public static byte[] Munge2(byte[] data, int seq)
        {
            return MungeInternal(data, seq, MungifyTable2);
        }

        public static byte[] UnMunge2(byte[] data, int seq)
        {
            return UnMungeInternal(data, seq, MungifyTable2);
        }

        public static byte[] Munge3(byte[] data, int seq)
        {
            return MungeInternal(data, seq, MungifyTable3);
        }

        public static byte[] UnMunge3(byte[] data, int seq)
        {
            return UnMungeInternal(data, seq, MungifyTable3);
        }
    }
}
