using UnityEngine;

namespace Docien.Math
{
    public static class MathUtils
    {
        /// <summary>
        /// Returns a normalized vector that points in the same direction on the XZ plane.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 FlattenToWorld(this Vector3 v)
        {
            v.y = 0f;
            return v.normalized;
        }
    }
}