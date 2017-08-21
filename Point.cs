using System;
using UnityEngine;

namespace PDollarGestureRecognizer {
    [Serializable]
    public struct Point {
        public Vector3 Position3D {
            set { Position = new Vector2(value.x, value.y); }
            get { return new Vector3(Position.x, Position.y); }
        }
        [SerializeField] public Vector2 Position;
        [SerializeField] public int Id;      

        public Point(float x, float y, int strokeId) {
            Position = new Vector2(x, y);
            Id = strokeId;
        }

        public Point(Vector2 position, int id) {
            Position = position;
            Id = id;
        }
    }

    public static class PointExtension {
        public static Point[] Resample(this Point[] points, int n) {
            if (points.Length == n) return points;
            if (points.Length < n) throw new ArgumentException("Point count lower than sample size.");
            
            var newPoints = new Point[n];
            newPoints[0] = new Point(points[0].Position, points[0].Id);
            var numPoints = 1;

            var interval = points.PathLength() / (n - 1);
            var totalDistance = 0f;
            for (var i = 1; i < points.Length; i++) {
                if (points[i].Id == points[i - 1].Id) {
                    var distance = Vector2.Distance(points[i - 1].Position, points[i].Position);
                    if (totalDistance + distance >= interval)
                    {
                        Point firstPoint = points[i - 1];
                        while (totalDistance + distance >= interval) {
                            var t = Math.Min(Math.Max((interval - totalDistance) / distance, 0.0f), 1.0f);
                            if (float.IsNaN(t)) t = 0.5f;
                            newPoints[numPoints++] = new Point(
                                (1.0f - t) * firstPoint.Position.x + t * points[i].Position.x,
                                (1.0f - t) * firstPoint.Position.y + t * points[i].Position.y,
                                points[i].Id
                            );

                            distance = totalDistance + distance - interval;
                            totalDistance = 0;
                            firstPoint = newPoints[numPoints - 1];
                        }
                        totalDistance = distance;
                    }
                    else totalDistance += distance;
                }
            }

            if (numPoints == n - 1)
                newPoints[numPoints] = new Point(points[points.Length - 1].Position, points[points.Length - 1].Id);
            return newPoints;
        }

        public static float PathLength(this Point[] points) {
            float length = 0;
            for (var i = 0; i < points.Length - 1; i++)
                if (points[i].Id == points[i + 1].Id)
                    length += Vector2.Distance(points[i + 1].Position, points[i].Position);
            return length;
        }

        public static float Size(this Point[] points) {
            float minx = float.MaxValue, miny = float.MaxValue, maxx = float.MinValue, maxy = float.MinValue;
            for (var i = 0; i < points.Length; i++) {
                if (minx > points[i].Position.x) minx = points[i].Position.x;
                if (miny > points[i].Position.y) miny = points[i].Position.y;
                if (maxx < points[i].Position.x) maxx = points[i].Position.x;
                if (maxy < points[i].Position.y) maxy = points[i].Position.y;
            }
            return Mathf.Abs(Mathf.Max(maxx - minx, maxy - miny));
        }

        public static Vector2 Centroid(this Point[] points) {
            float cx = 0, cy = 0;
            for (var i = 0; i < points.Length; i++) {
                cx += points[i].Position.x;
                cy += points[i].Position.y;
            }
            return new Vector2(cx / points.Length, cy / points.Length);
        }

        public static Point[] Normalize(this Point[] points, int sampleSize, PointOrigin origin) {
            points = points.Resample(sampleSize);
            var size = points.Size();
            var scale = origin == PointOrigin.TopLeft ? new Vector3(1, -1, 1) / size : new Vector3(1, 1, 1) / size;
            var translate = -points.Centroid();
            var transform = Matrix4x4.Scale(scale) * Matrix4x4.Translate(translate);
            for (var i = 0; i < points.Length; i++) {
                points[i].Position3D = transform.MultiplyPoint3x4(points[i].Position3D);
            }
            return points;
        }
    }

    public enum PointOrigin {
        TopLeft = 0,
        BottomLeft
    }
}
