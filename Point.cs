using System;
using UnityEngine;

namespace PDollarGestureRecognizer
{
    [Serializable]
    public struct Point
    {
        public Vector3 Position3D
        {
            set { Position = new Vector2(value.x, value.y); }
            get { return new Vector3(Position.x, Position.y); }
        }
        [SerializeField] public Vector2 Position;
        [SerializeField] public int Id;      

        public Point(float x, float y, int strokeId)
        {
            Position = new Vector2(x, y);
            Id = strokeId;
        }

        public Point(Vector2 position, int id)
        {
            Position = position;
            Id = id;
        }
    }
}
