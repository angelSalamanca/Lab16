using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    public class bivariate
    {
        List<int> bx, by;
        int[,] xy;
        grouping xGrouping, yGrouping;
        public xTab myXTab;
            
        public bivariate(grouping bxG, List<int> ix, grouping byG, List<string> sy)
        {
            xGrouping = bxG; yGrouping = byG;

            // replace with group nums
            groupReplacer xRep = new groupReplacer(xGrouping);
            bx = xRep.intReplace(ix);
            groupReplacer yRep = new groupReplacer(yGrouping);
            by = yRep.textReplace(sy);

            countPerCell();
           

        }

        private void countPerCell()
        {
            int numObs = Math.Min(bx.Count, by.Count);
            xy = (int[,])Array.CreateInstance(typeof(Int32), 2, numObs);
            for (int j=0; j<numObs; j++)
            {
                xy[0, j] = bx[j]; xy[1, j] = by[j];
            }
            
            myXTab = new xTab(xy);
            myXTab.count();
        }
    }
}
