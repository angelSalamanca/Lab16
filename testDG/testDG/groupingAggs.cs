using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace testDG
{
    public class groupingAggs
    {
        List<catGroupAggs> groupAggs;
        DataSet groupingDataSet;
        public double totalGoods
        {
            get
            {
                double agg = 0.0;
                foreach (catGroupAggs cga in groupAggs)
                {
                    if (!cga.isCategory)
                    {
                        agg += cga.numGoods;
                    }
                }
                return agg;
            }

        }
        public double totalBads
        {
            get
            {
                double agg = 0.0;
                foreach (catGroupAggs cga in groupAggs)
                {
                    if (!cga.isCategory)
                    {
                        agg += cga.numBads;
                    }
                }
                return agg;
            }
        }

        public double totalAccept
        {
            get
            {
                double agg = 0.0;
                foreach (catGroupAggs cga in groupAggs)
                {
                    if (!cga.isCategory)
                    {
                        agg += cga.numAccepts;
                    }
                }
                return agg;
            }
        }

        public double totalTotal
        {
            get
            {
                double agg = 0.0;
                foreach (catGroupAggs cga in groupAggs)
                {
                    if (!cga.isCategory)
                    {
                        agg += cga.numTotal;
                    }
                }
                return agg;
            }
        }

        public double Odds
        {
            get
            {
                if (totalBads > 0)
                { return totalGoods  / totalBads ; }
                else
                { return 0.0; }
            }
        }

        public DataTable dataSource
        {
            get
                {
                return groupingDataSet.Tables["grouping"];
                }
        }

        public groupingAggs()
        {
            groupAggs = new List<catGroupAggs>();
        }

        public catGroupAggs addGroup(string name, int cgId)
        {
            catGroupAggs myGroup = new catGroupAggs();
            myGroup.name = name;
            myGroup.isCategory = false;
            myGroup.Id = cgId;
            groupAggs.Add(myGroup);
            return myGroup;
        }

        public catGroupAggs addCategory(catGroupAggs myGroup, string name, double cGoods, double cBads, double cAccept, double cTotal, int Id)
        {
            catGroupAggs myCat = new catGroupAggs();
            myCat.name = name;
            myCat.isCategory = true;
            myCat.numGoods = cGoods;
            myCat.numBads = cBads;
            myCat.numAccepts = cAccept;
            myCat.numTotal = cTotal;
            myCat.Id = Id;

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

        public void propagateOdds()
        {
            double totalOdds = this.Odds;
            foreach (catGroupAggs ga in this.groupAggs)
            {
                ga.totalOdds = totalOdds;
                foreach (catGroupAggs ca in ga.childCats )
                {
                    ca.totalOdds = totalOdds ;
                }
            }
        }

        public void buildGroupingDataSet()
        {
            groupingDataSet = new DataSet();
            DataTable dt = new DataTable("grouping");
            groupingDataSet.Tables.Add(dt);
            this.addColumns();
            this.addRows();
            
        }

        private void addColumns()
        {
            DataTable dt = groupingDataSet.Tables["grouping"];

            dt.Columns.Add(new DataColumn("isCategory", System.Type.GetType("System.Boolean")));
            dt.Columns.Add(new DataColumn("name", System.Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("numGoods", System.Type.GetType("System.Double")));
            dt.Columns.Add(new DataColumn("numBads", System.Type.GetType("System.Double")));
            dt.Columns.Add(new DataColumn("numAccepts", System.Type.GetType("System.Double")));
            dt.Columns.Add(new DataColumn("numTotal", System.Type.GetType("System.Double")));
            dt.Columns.Add(new DataColumn("badRate", System.Type.GetType("System.Double")));
            dt.Columns.Add(new DataColumn("odds", System.Type.GetType("System.Double")));
            dt.Columns.Add(new DataColumn("oddsIndex", System.Type.GetType("System.Double")));
            dt.Columns.Add(new DataColumn("WOE", System.Type.GetType("System.Double")));
            dt.Columns.Add(new DataColumn("Id", System.Type.GetType("System.Int32")));


        }

        public void addRows()
        {
            var dt = groupingDataSet.Tables["grouping"];
            dt.Clear();

            foreach (catGroupAggs myGroup in groupAggs)
            {
                addRow(myGroup);
                foreach (catGroupAggs myCat in myGroup.childCats)
                {
                    addRow(myCat);
                }
            }

        }

        private void addRow(catGroupAggs myCatGroup)
        {
            DataTable dt = groupingDataSet.Tables["grouping"];
            var dr = dt.NewRow();
            dr["isCategory"] = myCatGroup.isCategory;
            if (myCatGroup.isCategory)
                {dr["name"] = "   " + myCatGroup.name;}
            else
                { dr["name"] = myCatGroup.name; }

            dr["numGoods"] = myCatGroup.numGoods;
            dr["numBads"] = myCatGroup.numBads ;
            dr["numAccepts"] = myCatGroup.numAccepts ;
            dr["numTotal"] = myCatGroup.numTotal;
            dr["badRate"] = myCatGroup.badRate;
            dr["odds"] = myCatGroup.Odds;
            dr["oddsIndex"] = myCatGroup.goodBadIndex;
            dr["WOE"] = myCatGroup.weightOfEvidence;
            dr["Id"] = myCatGroup.Id;
            dt.Rows.Add(dr);
        }

        public catGroupAggs getGroup(int id)
        {
            foreach (catGroupAggs cg in groupAggs)
            {
                if (cg.Id == id) return cg;
            }
            return null;
        }

        public void moveCategory(int catId, int oldGroupId, int newGroupId)
        {
            catGroupAggs myCat = null;
            catGroupAggs oldGroup = getGroup(oldGroupId);
            // seek Cat
            foreach (catGroupAggs cat in oldGroup.childCats)
            {
                if (cat.Id == catId) { myCat = cat; break; }
            }
            if (myCat == null) return;
            catGroupAggs newGroup = getGroup(newGroupId);
            oldGroup.childCats.Remove(myCat);
            newGroup.childCats.Add(myCat);
}

    } // class
}
