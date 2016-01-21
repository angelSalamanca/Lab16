using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testDG
{
    class groupingAggs
    {
        List<catGroupAggs> groupAggs;

        public groupingAggs()
        {
            groupAggs = new List<catGroupAggs>();
        }

        public catGroupAggs  addGroup(string name)
        {
            catGroupAggs myGroup = new catGroupAggs();
            myGroup.name = name;
            myGroup.isCategory = false;
            groupAggs.Add(myGroup);
            return myGroup;
        }

        public catGroupAggs addCategory(catGroupAggs myGroup, string name, double cGoods, double cBads, double cAccept, double cTotal)
        {
            catGroupAggs myCat = new catGroupAggs();
            myCat.name = name;
            myCat.isCategory = true;
            myCat.numGoods = cGoods;
            myCat.numBads = cBads;
            myCat.numAccepts = cAccept;
            myCat.numTotal = cTotal;




            myGroup.childCats.Add(myCat);
            return myCat;

        }

        public List<catGroupAggs> flatList()
        {
            List<catGroupAggs> myList = new List<catGroupAggs>();

            foreach (catGroupAggs myGroup in groupAggs)
            {
                myList.Add(myGroup);
                foreach (catGroupAggs myCat in myGroup.childCats)
                {
                    myList.Add(myCat);
                }
            }


            return myList;

        }
    }
}
