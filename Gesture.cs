using System;
using System.Linq;
using UnityEngine;

namespace PDollarGestureRecognizer
{
    public class Gesture : ScriptableObject
    {
        [HideInInspector]
        public Point[] Points;
        public Vector2 Centroid
        {
            get
            {
                float cx = 0, cy = 0;
                for (int i = 0; i < Points.Length; i++)
                {
                    cx += Points[i].Position.x;
                    cy += Points[i].Position.y;
                }
                return new Vector2(cx / Points.Length, cy / Points.Length);
            }
        }

        public float Size
        {
            get
            {
                float minx = float.MaxValue, miny = float.MaxValue, maxx = float.MinValue, maxy = float.MinValue;
                for (int i = 0; i < Points.Length; i++)
                {
                    if (minx > Points[i].Position.x) minx = Points[i].Position.x;
                    if (miny > Points[i].Position.y) miny = Points[i].Position.y;
                    if (maxx < Points[i].Position.x) maxx = Points[i].Position.x;
                    if (maxy < Points[i].Position.y) maxy = Points[i].Position.y;
                }
                return Mathf.Abs(Mathf.Max(maxx - minx, maxy - miny));
            }
        }

        public static Gesture Build(string name, int sampleSize, Point[] points)
        {
            var gesture = CreateInstance<Gesture>();
            gesture.name = name;
            gesture.Points = points;
            gesture.Normalize(sampleSize);
            return gesture;
        }

        public void Normalize(int sampleSize)
        {
            Resample(sampleSize);
            
            var transform = Matrix4x4.Scale(GetScaleFactor()) * Matrix4x4.Translate(-GetCenteroid());
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i].Position3D = transform.MultiplyPoint3x4(Points[i].Position3D);
            }
        }

        private Vector3 GetCenteroid()
        {
            float cx = 0, cy = 0;
            for (int i = 0; i < Points.Length; i++)
            {
                cx += Points[i].Position.x;
                cy += Points[i].Position.y;
            }
            return new Vector2(cx / Points.Length, cy / Points.Length);
        }

        private Vector3 GetScaleFactor()
        {
            float minx = float.MaxValue, miny = float.MaxValue, maxx = float.MinValue, maxy = float.MinValue;
            for (int i = 0; i < Points.Length; i++)
            {
                if (minx > Points[i].Position.x) minx = Points[i].Position.x;
                if (miny > Points[i].Position.y) miny = Points[i].Position.y;
                if (maxx < Points[i].Position.x) maxx = Points[i].Position.x;
                if (maxy < Points[i].Position.y) maxy = Points[i].Position.y;
            }
            var maxDelta = Mathf.Abs(Mathf.Max(maxx - minx, maxy - miny));
            return Vector3.one / maxDelta;
        }

        private void Resample(int n)
        {
            if (Points.Length <= n) return;
            
            Point[] newPoints = new Point[n];
            newPoints[0] = new Point(Points[0].Position, Points[0].Id);
            int numPoints = 1;

            float interval = PathLength(Points) / (n - 1);
            float totalDistance = 0;
            for (int i = 1; i < Points.Length; i++)
            {
                if (Points[i].Id == Points[i - 1].Id)
                {
                    float distance = Vector2.Distance(Points[i - 1].Position, Points[i].Position);
                    if (totalDistance + distance >= interval)
                    {
                        Point firstPoint = Points[i - 1];
                        while (totalDistance + distance >= interval)
                        {
                            float t = Math.Min(Math.Max((interval - totalDistance) / distance, 0.0f), 1.0f);
                            if (float.IsNaN(t)) t = 0.5f;
                            newPoints[numPoints++] = new Point(
                                (1.0f - t) * firstPoint.Position.x + t * Points[i].Position.x,
                                (1.0f - t) * firstPoint.Position.y + t * Points[i].Position.y,
                                Points[i].Id
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
                newPoints[numPoints] = new Point(Points[Points.Length - 1].Position, Points[Points.Length - 1].Id);
            Points = newPoints;
        }

        private float PathLength(Point[] points)
        {
            float length = 0;
            for (int i = 1; i < points.Length; i++)
                if (points[i].Id == points[i - 1].Id)
                    length += Vector2.Distance(points[i - 1].Position, points[i].Position);
            return length;
        }
    }
}