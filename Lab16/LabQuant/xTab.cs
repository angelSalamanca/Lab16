using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    public class pairOfInt
    {
       public int i1, i2;
        public pairOfInt(int j, int k)
        { i1 = j;  i2 = k; }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            pairOfInt p = obj as pairOfInt;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (i1 == p.i1) && (i2 == p.i2);
        }

        public bool Equals(pairOfInt p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (i1 == p.i1) && (i2 == p.i2);
        }

        public override int GetHashCode()
        {
            return i1 + 100000 * i2;
        }

    } // pairOfInt
    public class xTab
    {
        private List<int> x, y;
        private List<string> xs, ys;
        private List<double> xd, yd;
        public Dictionary<pairOfInt, int> xCounts;
         
        public xTab(List<int> xx, List<int> xy)
        {
            x = xx; y = xy;
            xCounts = new Dictionary<pairOfInt, int>();
        }

        public xTab(int[,] xy)
        {
            x = new List<int>();
            y = new List<int>();
            for (int j = 0; j < xy.GetLength(1); j++)
            {
                x.Add(xy[0, j]); y.Add(xy[1, j]);
            }

            xCounts = new Dictionary<pairOfInt, int>();
        }

        public void count()
        {
            int numObs = Math.Min(x.Count, y.Count);
                for (int i=0; i< numObs; i++)
            {
                addTo(x[i], y[i]);
            }
        }
    
        private void addTo(int ix, int iy)
        {
            pairOfInt pi = new pairOfInt(ix, iy);
            if (xCounts.Keys.Contains(pi))
            { xCounts[pi] += 1; }
            else
            { xCounts.Add(pi, 1); }
        }
    }
}
