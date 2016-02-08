using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    class groupingAndData
    {
        private grouping myGrouping;
        variable myVariable;
        analyticalDictionary myDD;

        public List<string> textRawData;
        public List<int> intRawData;
        public List<double> dblRawData;
        public List<int> catNums;
        public List<int> groupNums;
        
        public groupingAndData(grouping gGrouping)
        {
            myGrouping = gGrouping;
            myVariable = myGrouping.myVariable;
            myDD = myVariable.myAnalyticalDictionary;

        }

        public void assignTextGroupings()
        {
            if (!(catNums == null))
                {
                return;
                 }

            catNums = new List<int>();
            catAssigner myAssigner = new catAssigner(this.myVariable);
            foreach (string s in textRawData)
            {
                catNums.Add(myAssigner.assignTextCategory(s).catId);
            }
            groupAssigner myGroupAssigner = new groupAssigner(myGrouping);
            this.groupNums = myGroupAssigner.assignTextGroup(this.catNums);
        }


    }
}
