using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LabQuant
{
    
   public class variable
    {
        public string name;
        public Int32 varId;
        public General.typeOfVariable variableType;
        public string Description;
        public General.blockType myBlock;

        public string missingReplacement;
        public string wrongReplacement;
        
        public Dictionary <Int32,category> myCategories;
        public Dictionary<Int32, grouping> myGroupings;

        public analyticalDictionary myAnalyticalDictionary;
        
        public grouping getGroupingByName(string gName)
        {
            foreach (KeyValuePair<Int32, grouping > kvp in myGroupings)
            {
                if (kvp.Value.name == gName) return kvp.Value;
            }
            return null;
        }

        public category unexpectedCat
        {
            get
            {
                foreach (KeyValuePair<Int32, category>  kvp in myCategories)
                { if (kvp.Value.isWrong) return kvp.Value; }

                return null;
            }
            set { }
        }


        public variable(string vName, General.typeOfVariable vType, Int32 vNum, analyticalDictionary vAD, General.blockType aBlock = General.blockType.Input )
        {
            name = vName;
            variableType = vType;
            varId = vNum;
            myAnalyticalDictionary = vAD;
            myBlock = aBlock;
            missingReplacement = "";
            wrongReplacement = "";

            // add no cat
            category missCat = new category("", myAnalyticalDictionary.getCatNum, this, true, false);
            category wrongCat = new category("", myAnalyticalDictionary.getCatNum, this, false, true);

            this.myCategories = new Dictionary<Int32, category>();
            this.myCategories.Add(missCat.catId, missCat);
            this.myCategories.Add(wrongCat.catId, wrongCat);
            myGroupings = new Dictionary<Int32, grouping>();

            addGrouping(General.mainGroupingName, true);
            
        }
    
    public category addCategory(string cName, Int32 CId, string cValue)
    {
            category myCat = new category(cName, CId, this, false, false);
            myCat.stringValue = cValue;
            this.myCategories.Add(CId, myCat);

            foreach (KeyValuePair<Int32, grouping> kvp in myGroupings)
            {
                kvp.Value.toNoGroup(myCat);
            }
            return myCat;
    }

        public grouping addGrouping(string groupingName, Boolean isNoGroup)
        {
            grouping newGrouping = new grouping(this, groupingName, myAnalyticalDictionary.getGroupingNum, isNoGroup);
            if (existsGrouping(groupingName))
            {
                return null;
            }
            myGroupings.Add(newGrouping.groupingId, newGrouping);
            return newGrouping;

        }

        public void renameGrouping(string oldName, string newName)
        {
            foreach (KeyValuePair<int, grouping> kvp in myGroupings)
            {
                if (kvp.Value.name == oldName)
                {
                    kvp.Value.name = newName;
                }
            }
        }

        public Boolean existsGrouping(string gName)
        {
            foreach (KeyValuePair<int, grouping> kvp in myGroupings)
            {
                if(kvp.Value.name == gName)
                { return true; }
            }
            return false;
        }

    } // class variable

    

}
