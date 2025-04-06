using System;
using AS.Utils.MathUtils;
using UnityEngine;

namespace AS.Utils
{
 public static class DebugUtils
    {
        private static int _circleSteps = 16;

        private static void CalculateCirclePositions()
        {   
            var step = Constants.TAU / _circleSteps;
            
            for (var i = 0; i < _circleSteps; i++)
            {
                _circlePositions[i].x = Mathf.Cos(i * step);
                _circlePositions[i].y = 0;
                _circlePositions[i].z = Mathf.Sin(i * step);
            }
        }

        private static Vector3[] _circlePositions = {};
        private static Vector3[] _drawCirclePositions = {};

        public static int CircleSteps
        {
            get => _circleSteps;
            set
            {
                _circlePositions = new Vector3[value];
                _drawCirclePositions = new Vector3[value];
                CalculateCirclePositions();
                _circleSteps = value;
            }
        }
        
        public static void GizmosDrawCircle(Vector3 center, Vector3 normal, float radius)
        {
            var up = (normal == Vector3.zero ? Vector3.up : normal.normalized) * radius;
            var forward = Vector3.Slerp(up, -up, 0.5f);
            var right = Vector3.Cross(up, forward).normalized * radius;

            var matrix = new Matrix4x4
            {
                [0] = right.x,
                [1] = right.y,
                [2] = right.z,
                [4] = up.x,
                [5] = up.y,
                [6] = up.z,
                [8] = forward.x,
                [9] = forward.y,
                [10] = forward.z,
                [12] = center.x,
                [13] = center.y,
                [14] = center.z,
            };

            if (_circleSteps != _circlePositions.Length)
            {
                CircleSteps = _circleSteps;
            }
            
            matrix.TransformPoints3x4(_circlePositions, _drawCirclePositions);
            
            Gizmos.DrawLineStrip(_drawCirclePositions, true);
        }

        public static void GizmosDrawCircularArc(Vector3 center, Vector3 normal, Vector3 from, float angle,
            float radius)
        {
            var circularArcFrom = from.normalized * radius;
            GizmosDrawArc(center, normal, circularArcFrom, angle, radius);
        }
        
        public static void GizmosDrawArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            var up = (normal == Vector3.zero ? Vector3.up : normal.normalized) * radius;
            var right = Vector3.Cross(up, from).normalized * radius;
            
            var matrix = new Matrix4x4
            {
                [0] = right.x,
                [1] = right.y,
                [2] = right.z,
                [4] = up.x,
                [5] = up.y,
                [6] = up.z,
                [8] = from.x,
                [9] = from.y,
                [10] = from.z,
                [12] = center.x,
                [13] = center.y,
                [14] = center.z,
            };
            
            if (_circleSteps != _circlePositions.Length)
            {
                CircleSteps = _circleSteps;
            }

            var stepValue = angle / (_circleSteps - 1) * Mathf.Deg2Rad;

            for (var i = 0; i < _circleSteps; i++)
            {
                var theta = Constants.PI_BY_2 + i * stepValue;
                
                _drawCirclePositions[i].x = Mathf.Cos(theta);
                _drawCirclePositions[i].y = 0;
                _drawCirclePositions[i].z = Mathf.Sin(theta);
            }
            
            matrix.TransformPoints3x4(_drawCirclePositions);
            
            Gizmos.DrawLineStrip(_drawCirclePositions, false);
        }

        public static void GizmosDrawSphericalSector(Vector3 center, Vector3 forward, Vector3 right, float length, float angle)
        {
            var up = Vector3.Cross(forward, right);
            
            var sinThetaRange = Mathf.Sin(angle * Mathf.Deg2Rad) * length;
            var cosThetaRange = Mathf.Cos(angle * Mathf.Deg2Rad) * length;
            
            Vector3[] points =
            {
                new(sinThetaRange, 0, cosThetaRange), 
                new(-sinThetaRange, 0, cosThetaRange),
                new(0, sinThetaRange, cosThetaRange), 
                new(0, -sinThetaRange, cosThetaRange)
            };
            
            GizmosDrawCircle(center + forward * cosThetaRange, forward, sinThetaRange);
            
            // Draw Lines
            Matrix4x4 matrix = new()
            {
                [0] = right.x,
                [1] = right.y,
                [2] = right.z,
                [4] = up.x,
                [5] = up.y,
                [6] = up.z,
                [8] = forward.x,
                [9] = forward.y,
                [10] = forward.z,
                [12] = center.x,
                [13] = center.y,
                [14] = center.z
            };

            GizmosDrawCircularArc(center, -right, matrix.MultiplyVector(points[2]), angle * 2, length);
            GizmosDrawCircularArc(center, up, matrix.MultiplyVector(points[0]), angle * 2, length);
            
            matrix.TransformPoints3x4(points);
            
            foreach (var point in points)
            {
                Gizmos.DrawLine(center, point);
            }

            for (var i = 0; i < points.Length - 1; i += 2)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }

        public static Color DebugDrawColor = Color.white;

        public static void DebugDrawLineStrip(ReadOnlySpan<Vector3> points, bool looped, Color? color = null, 
            float duration = 0, bool depthTest = true)
        {
            for (var i = 0; i < points.Length - 1; i++)
            {
                Debug.DrawLine(points[i], points[i + 1], color ?? DebugDrawColor, duration, depthTest);
            }

            if (looped)
            {
                Debug.DrawLine(points[^1], points[0], color ?? DebugDrawColor, duration, depthTest);
            }
        }
        
        public static void DebugDrawCircle(Vector3 center, Vector3 normal, float radius, Color? color = null, 
            float duration = 0, bool depthTest = true)
        {
            var up = (normal == Vector3.zero ? Vector3.up : normal.normalized) * radius;
            var forward = Vector3.Slerp(up, -up, 0.5f);
            var right = Vector3.Cross(up, forward).normalized * radius;

            var matrix = new Matrix4x4
            {
                [0] = right.x,
                [1] = right.y,
                [2] = right.z,
                [4] = up.x,
                [5] = up.y,
                [6] = up.z,
                [8] = forward.x,
                [9] = forward.y,
                [10] = forward.z,
                [12] = center.x,
                [13] = center.y,
                [14] = center.z,
            };

            if (_circleSteps != _circlePositions.Length)
            {
                CircleSteps = _circleSteps;
            }
            
            matrix.TransformPoints3x4(_circlePositions, _drawCirclePositions);
            
            DebugDrawLineStrip(_drawCirclePositions, true, color, duration, depthTest);
        }

        public static void DebugDrawCircularArc(Vector3 center, Vector3 normal, Vector3 from, float angle,
            float radius, Color? color = null, float duration = 0, bool depthTest = true)
        {
            var circularArcFrom = from.normalized * radius;
            DebugDrawArc(center, normal, circularArcFrom, angle, radius, color, duration, depthTest);
        }
        
        public static void DebugDrawArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, 
            Color? color = null, float duration = 0, bool depthTest = true)
        {
            var up = (normal == Vector3.zero ? Vector3.up : normal.normalized) * radius;
            var right = Vector3.Cross(up, from).normalized * radius;
            
            var matrix = new Matrix4x4
            {
                [0] = right.x,
                [1] = right.y,
                [2] = right.z,
                [4] = up.x,
                [5] = up.y,
                [6] = up.z,
                [8] = from.x,
                [9] = from.y,
                [10] = from.z,
                [12] = center.x,
                [13] = center.y,
                [14] = center.z,
            };
            
            if (_circleSteps != _circlePositions.Length)
            {
                CircleSteps = _circleSteps;
            }

            var stepValue = angle / (_circleSteps - 1) * Mathf.Deg2Rad;

            for (var i = 0; i < _circleSteps; i++)
            {
                var theta = Constants.PI_BY_2 + i * stepValue;
                
                _drawCirclePositions[i].x = Mathf.Cos(theta);
                _drawCirclePositions[i].y = 0;
                _drawCirclePositions[i].z = Mathf.Sin(theta);
            }
            
            matrix.TransformPoints3x4(_drawCirclePositions.AsSpan());
            
            DebugDrawLineStrip(_drawCirclePositions, false, color, duration, depthTest);
        }

        public static void DebugDrawSphericalSector(Vector3 center, Vector3 forward, Vector3 right, float length, 
            float angle, Color? color = null, float duration = 0, bool depthTest = true)
        {
            var up = Vector3.Cross(forward, right);
            
            var sinThetaRange = Mathf.Sin(angle * Mathf.Deg2Rad) * length;
            var cosThetaRange = Mathf.Cos(angle * Mathf.Deg2Rad) * length;
            
            Vector3[] points =
            {
                new(sinThetaRange, 0, cosThetaRange), 
                new(-sinThetaRange, 0, cosThetaRange),
                new(0, sinThetaRange, cosThetaRange), 
                new(0, -sinThetaRange, cosThetaRange)
            };
            
            DebugDrawCircle(center + forward * cosThetaRange, forward, sinThetaRange, color, duration, depthTest);
            
            // Draw Lines
            Matrix4x4 matrix = new()
            {
                [0] = right.x,
                [1] = right.y,
                [2] = right.z,
                [4] = up.x,
                [5] = up.y,
                [6] = up.z,
                [8] = forward.x,
                [9] = forward.y,
                [10] = forward.z,
                [12] = center.x,
                [13] = center.y,
                [14] = center.z
            };

            DebugDrawCircularArc(center, -right, matrix.MultiplyVector(points[2]), angle * 2, length, color, duration, depthTest);
            DebugDrawCircularArc(center, up, matrix.MultiplyVector(points[0]), angle * 2, length, color, duration, depthTest);
            
            matrix.TransformPoints3x4(points);
            
            foreach (var point in points)
            {
                Debug.DrawLine(center, point, color ?? DebugDrawColor, duration, depthTest);
            }

            for (var i = 0; i < points.Length - 1; i += 2)
            {
                Debug.DrawLine(points[i], points[i + 1], color ?? DebugDrawColor, duration, depthTest);
            }
        }
    }
}