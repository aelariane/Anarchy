using UnityEngine;

namespace Optimization.Caching
{
    internal static class Vectors
    {
        internal static readonly Vector3 back = Vector3.back;
        internal static readonly Vector3 down = Vector3.down;
        internal static readonly Vector3 forward = Vector3.forward;
        internal static readonly Vector3 left = Vector3.left;
        internal static readonly Vector3 one = new Vector3(1f, 1f, 1f);
        internal static readonly Vector3 right = Vector3.right;
        internal static readonly Vector3 up = Vector3.up;
        internal static readonly Vector2 v2one = new Vector2(1f, 1f);
        internal static readonly Vector2 v2zero = Vector2.zero;
        internal static readonly Vector3 zero = Vector3.zero;

        #region VectorExtensions

        public static Vector3 Back(this Transform t) => t.rotation * back;

        public static Vector3 Down(this Transform t) => t.rotation * down;

        public static Vector3 Forward(this Transform t) => t.rotation * forward;

        public static Vector3 Left(this Transform t) => t.rotation * left;

        public static Vector3 Right(this Transform t) => t.rotation * right;

        public static Vector3 Up(this Transform t) => t.rotation * up;

        #endregion VectorExtensions
    }
}