using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    class groupReplacer
    {
        private grouping myGrouping;
        private groupAssigner myAssigner;

        public groupReplacer(grouping gGrouping)
        {
            myGrouping = gGrouping;
            myAssigner = new groupAssigner(myGrouping);
        }

        public List<int> textReplace(List<string> sValues)
        {
            List<int> groups = new List<int>();

            foreach(string sValue in sValues)
            { groups.Add(myAssigner.assignTextGroup(sValue).groupId); }

            return groups;
        }

        public List<int> intReplace(List<int> iValues)
        {
            List<int> groups = new List<int>();

            foreach (int iValue in iValues)
            { groups.Add(myAssigner.assignIntGroup(iValue).groupId); }

            return groups;
        }

        public List<int> doubleReplace(List<double> dValues)
        {
            List<int> groups = new List<int>();

            foreach (double dValue in dValues)
            { groups.Add(myAssigner.assignDoubleGroup(dValue).groupId); }

            return groups;
        }
    }
}
