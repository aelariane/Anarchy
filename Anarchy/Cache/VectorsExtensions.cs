using UnityEngine;

namespace Cache
{
    public static class VectorsExtensions
    {
        public static Vector3 Back(this Transform t)
        {
            return t.rotation * Vectors.Back;
        }

        public static Vector3 Down(this Transform t)
        {
            return t.rotation * Vectors.Down;
        }

        public static Vector3 Forward(this Transform t)
        {
            return t.rotation * Vectors.Forward;
        }

        public static Vector3 Left(this Transform t)
        {
            return t.rotation * Vectors.Left;
        }

        public static Vector3 Right(this Transform t)
        {
            return t.rotation * Vectors.Right;
        }

        public static Vector3 Up(this Transform t)
        {
            return t.rotation * Vectors.Up;
        }
    }
}