using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace PetzoldVectorDrawing
{
    public struct LineSegment2 : IGeometrySegment
    {
        readonly Point point1, point2;
        readonly double a, b;         // as in x = ay + b

        public LineSegment2(Point point1, Point point2) : this()
        {
            this.point1 = point1;
            this.point2 = point2;

            a = (point2.X - point1.X) / (point2.Y - point1.Y);
            b = point1.X - a * point1.Y;
        }

        public void GetAllX(double y, IList<double> xCollection)
        {
            if ((point2.Y > point1.Y && y >= point1.Y && y < point2.Y) ||
                (point2.Y < point1.Y && y <= point1.Y && y > point2.Y))
            {
                xCollection.Add(a * y + b);
            }
        }
    }
}
