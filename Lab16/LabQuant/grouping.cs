using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    public class grouping
    {
        public string name;
        public Int32 groupingId;
        public variable myVariable;
        public Dictionary<Int32, group> myGroups;
        public group myNoGroup;
        public Boolean isMain;

        public group noGroup
        {
            get
            {
                foreach (KeyValuePair<Int32, group> kvp in myGroups)
                { if (kvp.Value.isNoGroup) return kvp.Value; }

                return null;
            }
            set { }
        }

        public grouping(variable gVariable, string gname, Int32 gId, Boolean gIsMain)
        {
            name = gname;
            groupingId = gId;
            myVariable = gVariable;
            isMain = gIsMain;

            myNoGroup = new group("", myVariable.myAnalyticalDictionary.getGroupNum, true, this);
            myGroups = new Dictionary<Int32, group>();
            myGroups.Add(myNoGroup.groupId, myNoGroup);


        }

        public void toNoGroup(category cat)
        {
            unGroup(cat);
           myNoGroup.groupCat(cat);
        }

        public group addIntGroup(string gName, Int32 gId, Int32 iFrom, Int32 iTo,  Boolean isClosedLower, Boolean isClosedUpper)
        {
            group myGroup = new group(gName, gId, false, this);
            myGroup.closedlower = isClosedLower;
            myGroup.closedUpper = isClosedUpper;
            myGroup.lowerInt = iFrom;
            myGroup.upperInt = iTo;

            myGroups.Add(gId, myGroup);

            return myGroup;
        }

        public group addTextGroup(string gName, Int32 gId)
        {
            group myGroup = new group(gName, gId, false, this);
            

            myGroups.Add(gId, myGroup);

            return myGroup;
        }

        public void unGroup(category cat)
        {
            foreach (KeyValuePair<Int32, group> kvp in myGroups)
            {
                group g = kvp.Value;
                g.unGroupCat(cat); 
            }
        }

    }
}
