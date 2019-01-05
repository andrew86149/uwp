using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetzoldVectorDrawing
{
    public interface IGeometrySegment
    {
        void GetAllX(double y, IList<double> xCollection);
    }
}
