using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace PetzoldVectorDrawing
{
    public struct RoundCappedLine : IGeometrySegment
    {
        LineSegment2 lineSegment1;
        ArcSegment2 arcSegment1;
        LineSegment2 lineSegment2;
        ArcSegment2 arcSegment2;

        public RoundCappedLine(Point point1, Point point2, double radius) : this()
        {
            Vector2 vector = new Vector2(point2 - new Vector2(point1));
            Vector2 normVect = vector;
            normVect = normVect.Normalized;

            Point pt1a = point1 + radius * new Vector2(normVect.Y, -normVect.X);
            Point pt2a = pt1a + vector;
            Point pt1b = point1 + radius * new Vector2(-normVect.Y, normVect.X);
            Point pt2b = pt1b + vector;

            lineSegment1 = new LineSegment2(pt1a, pt2a);
            arcSegment1 = new ArcSegment2(point2, radius, pt2a, pt2b);
            lineSegment2 = new LineSegment2(pt2b, pt1b);
            arcSegment2 = new ArcSegment2(point1, radius, pt1b, pt1a);
        }

        public void GetAllX(double y, IList<double> xCollection)
        {
            arcSegment1.GetAllX(y, xCollection);
            lineSegment1.GetAllX(y, xCollection);
            arcSegment2.GetAllX(y, xCollection);
            lineSegment2.GetAllX(y, xCollection);
        }
    }
}
