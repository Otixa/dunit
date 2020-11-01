using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DUnit.DU
{
    public static class Utils
    {
        public static float[] ToLua(this Vector3 vec)
        {
            return new float[] { vec.X, vec.Y, vec.Z };
        }
        public static float[] ToLua(this Vector4 vec)
        {
            return new float[] { vec.X, vec.Y, vec.Z, vec.W };
        }

        public static Vector3 Multiply(this Quaternion quat, Vector3 vec)
        {
            var q1 = new Quaternion(vec, 0);
            var q2 = quat * q1 * Quaternion.Conjugate(quat);
            return new Vector3(q2.X, q2.Y, q2.Z);
        }

        public static Vector3 Max(this Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.X > b.X ? a.X : b.X,
                a.Y > b.Y ? a.Y : b.Y,
                a.Z > b.Z ? a.Z : b.Z
                );
        }
    }
}
