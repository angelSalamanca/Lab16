using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    public class analyticalDictionary
    {
        public string name;
        public Dictionary<String, variable> varByName;
        public Dictionary<Int32, variable> varByNum;
        private Int32 varPointer, catPointer, groupingPointer, groupPointer;
        public readonly String mainGroupingName = "main";
        public readonly String missingCatName = "missing";
        public readonly String wrongCatName = "unexpected";
        public string[] separator = {"[##@##]"};

        // not thread safe
        private Int32 getVarNum
        {
            get {
                varPointer += 1;
                return varPointer;
            }
            set { }
        }

        public Int32 getCatNum
        {
            get
            {
                catPointer += 1;
                return catPointer;
            }
            set { }
        }

        public Int32 getGroupingNum
        {
            get
            {
                groupingPointer += 1;
                return groupingPointer;
            }
            set { }
        }

        public Int32 getGroupNum
        {
            get
            {
                groupPointer += 1;
                return groupPointer;
            }
            set { }
        }

        public analyticalDictionary(string dName)
        {
            name = dName;
            varPointer = 0; groupingPointer = 0; catPointer = 0; groupPointer = 0;
            varByName = new Dictionary<string, variable>();
            varByNum = new Dictionary<Int32, variable>();
        }

        public variable addVar(string vName, variable.typeOfVariable vType)
        {
            variable newVar = new variable(vName, vType, getVarNum, this);
            this.varByName.Add(vName, newVar);
            this.varByNum.Add(this.varPointer, newVar);


            return newVar;
        }

        public category addCat(variable myVar, string cName,  string cValue)
        {
            category myCat = myVar.addCategory(cName, getCatNum, cValue);
            return myCat;

        }

        public group addIntGroup(grouping myGrouping, string gName, Int32 iFrom, Int32 iTo,  Boolean isClosedLower, Boolean isClosedUpper)
        {

            group myGroup = myGrouping.addIntGroup(gName, this.getGroupNum, iFrom, iTo, isClosedLower, isClosedUpper);
            return myGroup;
        }

        public group addTextGroup(grouping myGrouping, string gName)
        {

            group myGroup = myGrouping.addTextGroup(gName, this.getGroupNum);
            return myGroup;
        }

        public string toText()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<String, variable> kvp in this.varByName)
            {
                variable var = kvp.Value;
                sb.Append(var.name);
                sb.Append(separator[0]);
                foreach (KeyValuePair<Int32, category> kvp2 in var.myCategories)
                {
                    category cat = kvp2.Value;
                    sb.Append("  cat: ");
                    sb.Append(kvp2.Key.ToString("###0 "));
                    sb.Append(cat.name);
                    sb.Append(separator[0]);
                }

                    foreach (KeyValuePair<Int32, grouping> kvpG in var.myGroupings)
                    {
                        grouping grp = kvpG.Value;
                        sb.Append("  grpng: ");
                        sb.Append(kvpG.Key.ToString("###0 "));
                        sb.Append(grp.name);
                        sb.Append(separator[0]);
                        foreach (KeyValuePair<Int32, group> kvpg in grp.myGroups)
                            {
                        group myGroup = kvpg.Value;
                        sb.Append("    group: ");
                        sb.Append(kvpg.Key.ToString("###0 "));
                        sb.Append(myGroup.name);
                        sb.Append(separator[0]);

                    }
                }
                }

            
            return sb.ToString();
        }

        public void groupCat(category cat, group g)
        {
            grouping myGrouping = g.myGrouping;
            myGrouping.unGroup(cat);
            g.groupCat(cat);

        }
    } //class
}
