using System;
using System.Collections.Generic;
using UnityEngine;

namespace PDollarGestureRecognizer {
    public class PointCloudRecognizer {
        public static string Classify(Point[] points, Gesture[] trainingSet) {
            var minDistance = float.MaxValue;
            var gestureClass = "";
            foreach (Gesture template in trainingSet)
            {
                float dist = GreedyCloudMatch(points, template.Points);
                if (dist < minDistance) {
                    minDistance = dist;
                    gestureClass = template.name;
                }
            }
            return gestureClass;
        }
        
        private static float GreedyCloudMatch(Point[] points1, Point[] points2) {
            var n = points1.Length;
            var eps = 0.5f;
            var step = (int)Math.Floor(Math.Pow(n, 1.0f - eps));
            var minDistance = float.MaxValue;
            for (var i = 0; i < n; i += step) {
                var dist1 = CloudDistance(points1, points2, i);
                var dist2 = CloudDistance(points2, points1, i);
                minDistance = Math.Min(minDistance, Math.Min(dist1, dist2));
            }
            return minDistance;
        }

        private static float CloudDistance(Point[] points1, Point[] points2, int startIndex) {
            var n = points1.Length;
            var matched = new bool[n];
            Array.Clear(matched, 0, n);

            var sum = 0f;
            var i = startIndex;
            do {
                var index = -1;
                var minDistance = float.MaxValue;
                for (var j = 0; j < n; j++) {
                    if (!matched[j]) {
                        var dist = (points1[i].Position - points2[j].Position).sqrMagnitude;
                        if (dist < minDistance) {
                            minDistance = dist;
                            index = j;
                        }
                    }
                }
                matched[index] = true;
                var weight = 1.0f - ((i - startIndex + n) % n) / (1.0f * n);
                sum += weight * minDistance;
                i = (i + 1) % n;
            } while (i != startIndex);
            return sum;
        }
    }
}