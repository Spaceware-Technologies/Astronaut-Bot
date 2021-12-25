using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Astronaut.API.Utils
{
    internal class BufferRW
    {
        public static byte[] Vector3ToBytes(Vector3 vector3)
        {
            byte[] buffer = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(vector3.X), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vector3.Y), 0, buffer, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vector3.Z), 0, buffer, 8, 4);
            return buffer;
        }
        
        public static Vector3 ReadVector3(byte[] buffer, int index)
        {
            var x = BitConverter.ToSingle(buffer, index);
            var y = BitConverter.ToSingle(buffer, index + 4);
            var z = BitConverter.ToSingle(buffer, index + 8);
            return new Vector3(x, y, z);
        }
    }
}
