namespace PDollarGestureRecognizer
{
    public class Geometry
    {
        public static float SqrEuclideanDistance(Point a, Point b)
        {
            return (a.Position.x - b.Position.x) * (a.Position.x - b.Position.x) + 
                   (a.Position.y - b.Position.y) * (a.Position.y - b.Position.y);
        }
    }
}
