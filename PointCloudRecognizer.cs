﻿using System;
using UnityEngine;

namespace PDollarGestureRecognizer
{
    public class PointCloudRecognizer
    {
        public static string Classify(Gesture candidate, Gesture[] trainingSet)
        {
            float minDistance = float.MaxValue;
            string gestureClass = "";
            foreach (Gesture template in trainingSet)
            {
                float dist = GreedyCloudMatch(candidate.Points, template.Points);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    gestureClass = template.name;
                }
            }
            return gestureClass;
        }
        
        private static float GreedyCloudMatch(Point[] points1, Point[] points2)
        {
            int n = points1.Length;
            float eps = 0.5f;
            int step = (int)Math.Floor(Math.Pow(n, 1.0f - eps));
            float minDistance = float.MaxValue;
            for (int i = 0; i < n; i += step)
            {
                float dist1 = CloudDistance(points1, points2, i);
                float dist2 = CloudDistance(points2, points1, i);
                minDistance = Math.Min(minDistance, Math.Min(dist1, dist2));
            }
            return minDistance;
        }

        private static float CloudDistance(Point[] points1, Point[] points2, int startIndex)
        {
            int n = points1.Length;
            bool[] matched = new bool[n];
            Array.Clear(matched, 0, n);

            float sum = 0;
            int i = startIndex;
            do
            {
                int index = -1;
                float minDistance = float.MaxValue;
                for(int j = 0; j < n; j++)
                    if (!matched[j])
                    {
                        float dist = Geometry.SqrEuclideanDistance(points1[i], points2[j]);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            index = j;
                        }
                    }
                matched[index] = true;
                float weight = 1.0f - ((i - startIndex + n) % n) / (1.0f * n);
                sum += weight * minDistance;
                i = (i + 1) % n;
            } while (i != startIndex);
            return sum;
        }
    }
}