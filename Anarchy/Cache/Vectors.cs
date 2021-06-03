using UnityEngine;

namespace Cache
{
    /// <summary>
    /// Cached basic set of <see cref="Vector3"/>
    /// </summary>
    public static class Vectors
    {
        public static readonly Vector3 Back;
        public static readonly Vector3 Down;
        public static readonly Vector3 Forward;
        public static readonly Vector3 Left;
        public static readonly Vector3 One;
        public static readonly Vector3 Right;
        public static readonly Vector3 Up;
        public static readonly Vector2 V2One;
        public static readonly Vector2 V2Zero;
        public static readonly Vector3 Zero;

        static Vectors()
        {
            Back = Vector3.back;
            Down = Vector3.down;
            Forward = Vector3.forward;
            Left = Vector3.left;
            One = new Vector3(1f, 1f, 1f);
            Right = Vector3.right;
            Up = Vector3.up;
            V2One = Vector3.one;
            V2Zero = Vector3.zero;
            Zero = Vector3.zero;
        }
    }
}