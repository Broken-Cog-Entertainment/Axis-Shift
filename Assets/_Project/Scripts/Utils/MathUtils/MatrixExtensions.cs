using System;
using UnityEngine;

namespace AS.Utils.MathUtils
{
    public static class UtilsMath
    {
        public static void TransformPoints3x4(this Matrix4x4 mat, Span<Vector3> points)
        {
            for (var i = 0; i < points.Length; i++)
            {
                points[i] = mat.MultiplyPoint3x4(points[i]);
            }
        }
        
        public static void TransformPoints3x4(this Matrix4x4 mat, ReadOnlySpan<Vector3> points, Span<Vector3> transformedPoints)
        {
            // Make sure we have enough space
            Debug.Assert(transformedPoints.Length >= points.Length);
            
            for (var i = 0; i < points.Length; i++)
            {
                transformedPoints[i] = mat.MultiplyPoint3x4(points[i]);
            }
        }
    }
}