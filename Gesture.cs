using System;
using UnityEngine;

namespace PDollarGestureRecognizer {
    public class Gesture : ScriptableObject {
        [HideInInspector]
        public Point[] Points;

        public static Gesture Build(string name, Point[] points) {
            var gesture = CreateInstance<Gesture>();
            gesture.name = name;
            gesture.Points = points;
            return gesture;
        }
    }
}