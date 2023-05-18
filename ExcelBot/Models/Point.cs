namespace ExcelBot.Models
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y) { X = x; Y = y; }

        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
        public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Point a, Point b) => !(a == b);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public bool Equals(Point other) => this == other;
        public override bool Equals(object? other) => other is Point && this.Equals((Point)other);
        public int DistanceTo(Point other) => Math.Abs(this.X - other.X) + Math.Abs(this.Y - other.Y);
    }
}
