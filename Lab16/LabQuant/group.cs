using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    public class group
    {
        public string name;
        public Int32 groupId;
        public grouping myGrouping;
        public Boolean isNoGroup;
        public Int32 lowerInt;
        public Int32 upperInt;
        public Boolean closedlower;
        public Boolean closedUpper;
        public double lowerDouble;
        public double upperDouble;
      
        public Dictionary<Int32, category> myCategories; 

        public group(string gName, Int32 gId, Boolean gNoGroup, grouping gGrouping)
        {
            name = gName;
            groupId = gId;
            isNoGroup = gNoGroup;
            myGrouping = gGrouping;
            myCategories = new Dictionary<int, category>();
            if (isNoGroup)
            {
                name = General.noGroupName;
            }
        }

        public void groupCat(category cat)
        {
            myCategories.Add(cat.catId, cat);
        }

        public void unGroupCat(category cat)
        {
            if (myCategories.Values.Contains(cat)) myCategories.Remove(cat.catId);
        }

        public Boolean contains(int iValue)
        {
            Boolean contained = true;

            if (closedlower)
            { contained = contained & lowerInt <= iValue; }
            else
            { contained = contained & lowerInt < iValue; }

            if (closedUpper)
            { contained = contained & upperInt >= iValue; }
            else
            { contained = contained & upperInt > iValue; }
            
            return contained;
        }

        public Boolean contains(double dValue)
        {
            Boolean contained = true;

            if (closedlower)
            { contained = contained & lowerDouble <= dValue; }
            else
            { contained = contained & lowerDouble < dValue; }

            if (closedUpper)
            { contained = contained & upperDouble >= dValue; }
            else
            { contained = contained & upperDouble > dValue; }

            return contained;
        }

    } // class
}
