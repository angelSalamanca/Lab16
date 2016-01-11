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
        public typeOfVariable variableType;
        public string Description;

        private string missingReplacement;
        private string wrongReplacement;
        
        public Dictionary <Int32,category> myCategories;
        public Dictionary<Int32, grouping> myGroupings;

        public analyticalDictionary myAnalyticalDictionary;

      


        public enum typeOfVariable : byte { String = 1, Integer, Double, Date };

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


        public variable(string vName, typeOfVariable vType, Int32 vNum, analyticalDictionary vAD)
        {
            name = vName;
            variableType = vType;
            varId = vNum;
            myAnalyticalDictionary = vAD;

            // add no cat
            category missCat = new category("", myAnalyticalDictionary.getCatNum, this, true, false);
            category wrongCat = new category("", myAnalyticalDictionary.getCatNum, this, false, true);

            this.myCategories = new Dictionary<Int32, category>();
            this.myCategories.Add(missCat.catId, missCat);
            this.myCategories.Add(wrongCat.catId, wrongCat);
            myGroupings = new Dictionary<Int32, grouping>();

            addGrouping(myAnalyticalDictionary.mainGroupingName, true);

           
            
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
            myGroupings.Add(newGrouping.groupingId, newGrouping);
            return newGrouping;

        }

    } // class variable

    

}
