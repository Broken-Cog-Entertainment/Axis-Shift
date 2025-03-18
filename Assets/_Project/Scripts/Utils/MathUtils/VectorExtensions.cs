using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public static Vector2 ClampIndividualElements(this Vector2 input, float min, float max)
        {
            return new Vector2(Mathf.Clamp(input.x, min, max), Mathf.Clamp(input.y, min, max));
        }
        
        public static Vector3 ClampIndividualElements(this Vector3 input, float min, float max)
        {
            return new Vector3(Mathf.Clamp(input.x, min, max), Mathf.Clamp(input.y, min, max), Mathf.Clamp(input.z, min, max));
        }
        
        public enum Axis3D
        {
            X,
            Y,
            Z,
        }

        public static Vector3 RandomInsideCircle(float radius = 1f, Axis3D axis = Axis3D.Z)
        {
            var randomPoint = Random.insideUnitCircle * radius;

            return randomPoint.Bulk(axis);
        }

        /// <summary>
        /// Convert a Vector3 to a Vector2 by discarding it's 'y' component.
        /// </summary>
        /// <param name="input">3D Vector to be flattened.</param>
        /// <param name="axis">Axis to 'Flatten' or ignore.</param>
        /// <returns>Resulting 2D Vector.</returns>
        public static Vector2 Flatten(this Vector3 input, Axis3D axis = Axis3D.Y)
        {
            return axis switch
            {
                Axis3D.X => new Vector2(input.y, input.z),
                Axis3D.Y => new Vector2(input.x, input.z),
                Axis3D.Z => new Vector2(input.x, input.y),
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid Axis")
            };
        }
        
        public static Vector3 Bulk(this Vector2 input, Axis3D zeroedAxis = Axis3D.Y, float value = 0)
        {
            return zeroedAxis switch
            {
                Axis3D.X => new Vector3(value, input.x, input.y),
                Axis3D.Y => new Vector3(input.x, value, input.y),
                Axis3D.Z => new Vector3(input.x, input.y, value),
                _ => throw new ArgumentOutOfRangeException(nameof(zeroedAxis), zeroedAxis, "Invalid Axis")
            };
        }
    }
}