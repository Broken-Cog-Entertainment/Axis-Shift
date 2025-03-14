using UnityEngine;

namespace AS.Utils.MathUtils
{
    public static class VectorExtensions
    {
        public static Vector2 With(this Vector2 input, float? x = null, float? y = null)
        {
            return new Vector2(x ?? input.x, y ?? input.y);
        }
        
        public static Vector3 With(this Vector3 input, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? input.x, y ?? input.y, z ?? input.z);
        }

        public static Vector2 Clamp(this Vector2 input, float min, float max)
        {
            return new Vector2(Mathf.Clamp(input.x, min, max), Mathf.Clamp(input.y, min, max));
        }
        
        public static Vector3 Clamp(this Vector3 input, float min, float max)
        {
            return new Vector3(Mathf.Clamp(input.x, min, max), Mathf.Clamp(input.y, min, max), Mathf.Clamp(input.z, min, max));
        }
    }
}