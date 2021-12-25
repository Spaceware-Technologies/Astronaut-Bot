using System;
using System.Collections.Generic;
using static USpeak.IL2CPPOpus.OpusCodec;


//Original USpeak classes by DayOfThePlay, fixed by Rin
namespace USpeak
{
    public class USpeakPoolUtils
    {
        public static float[] GetFloat(int length)
        {
            for (int i = 0; i < FloatPool.Count; i++)
            {
                if (FloatPool[i].Length == length)
                {
                    float[] result = FloatPool[i];
                    FloatPool.RemoveAt(i);
                    return result;
                }
            }
            return new float[length];
        }
        public static short[] GetShort(int length)
        {
            for (int i = 0; i < ShortPool.Count; i++)
            {
                if (ShortPool[i].Length == length)
                {
                    short[] result = ShortPool[i];
                    ShortPool.RemoveAt(i);
                    return result;
                }
            }
            return new short[length];
        }
        public static byte[] GetByte(int length)
        {
            for (int i = 0; i < BytePool.Count; i++)
            {
                if (BytePool[i].Length == length)
                {
                    byte[] result = BytePool[i];
                    BytePool.RemoveAt(i);
                    return result;
                }
            }
            return new byte[length];
        }

        public static void Return(float[] d)
        {
            FloatPool.Add(d);
        }
        public static void Return(byte[] d)
        {
            BytePool.Add(d);
        }
        public static void Return(short[] d)
        {
            ShortPool.Add(d);
        }

        private static List<byte[]> BytePool = new List<byte[]>();
        private static List<short[]> ShortPool = new List<short[]>();
        private static List<float[]> FloatPool = new List<float[]>();
    }
    
    public struct USpeakFrameContainer
    {
        public byte[] ToByteArray()
        {
            byte[] array = new byte[4 + this.encodedData.Length];
            int num = 0;
            byte[] bytes = BitConverter.GetBytes(this.FrameIndex);
            Array.Copy(bytes, 0, array, num, 2);
            num += 2;
            byte[] bytes2 = BitConverter.GetBytes((ushort)this.encodedData.Length);
            bytes2.CopyTo(array, num);
            num += 2;
            this.encodedData.CopyTo(array, num);
            return array;
        }
        public int GetByteLength()
        {
            return this.encodedData.Length + 4;
        }
        public ushort FrameIndex;

        public byte[] encodedData;
    }
}
