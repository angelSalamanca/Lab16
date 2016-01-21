using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    public class groupAssigner
    {
        grouping myGrouping;

        public groupAssigner(grouping aGrouping)
        {
            myGrouping = aGrouping;
        }

        public group assignTextGroup(string sValue)
        {
            category cat = assignCat(sValue);

            foreach (KeyValuePair<Int32, group> kvp in myGrouping.myGroups)
            {
                group g = kvp.Value;
                if (g.myCategories.Keys.Contains(cat.catId))
                {
                    return g;
                }
            }
            
            return myGrouping.noGroup;
        }

        public List<int> assignTextGroup(List<int> catNums)
        {
            List<int> groupNums = new List<int>();
            int noGroupNum = myGrouping.noGroup.groupId;
            foreach (int catNum in catNums)
            {
                Boolean found = false;
                foreach (KeyValuePair<Int32, group> kvp in myGrouping.myGroups)
                {
                    group g = kvp.Value;
                    if (g.myCategories.Keys.Contains(catNum))
                    {
                        groupNums.Add(g.groupId);
                    }
                }
                if (!found)
                { groupNums.Add(noGroupNum); }

            }
            return (groupNums);
        } // assignTextGroup

        private category assignCat(string sValue)
        {
            variable var = myGrouping.myVariable;

                foreach (KeyValuePair<Int32, category> kvp in var.myCategories)
            {
                if (kvp.Value.stringValue == sValue) return kvp.Value;
            }

            // Unexpected
            return var.unexpectedCat;
        }


        public group assignIntGroup(int iValue)
        {
            foreach (KeyValuePair<int, group> kvp in myGrouping.myGroups)
            {
                group g = kvp.Value;
                if (!g.isNoGroup)
                {
                    if (g.contains(iValue))
                    {
                        return g;
                    }
                }
            }

            return myGrouping.noGroup;
        }

        public group assignDoubleGroup(double dValue)
        {
            foreach (KeyValuePair<int, group> kvp in myGrouping.myGroups)
            {
                group g = kvp.Value;
                if (!g.isNoGroup)
                {
                    if (g.contains(dValue))
                    {
                        return g;
                    }
                }
            }

            return myGrouping.noGroup;
        }

    }
}
