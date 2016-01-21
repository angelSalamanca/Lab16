using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testDG
{
    public class catGroupAggs
    {
        private double counter;

        private double nGoods, nBads, nAccept, nTotal;
        public string name { get; set; }


        public double numGoods
        {
            get
            {
                if (!isCategory)
                {
                    counter = 0;
                    foreach (catGroupAggs child in childcats)
                    {
                        counter += child.numGoods;
                    }
                    return counter;
                }
                else
                {
                    return nGoods;
                }
            }
            set
            {
                nGoods = value;
            }
        }
        public double numBads { get; set; }
        public double numAccepts { get; set; }
        public double numTotal { get; set; }

        public Boolean isCategory { get; set; }

        private List<catGroupAggs> childcats;

        public List<catGroupAggs> childCats
        {
            get {
                if (!isCategory)
                { return childcats; }
                else
                { return null; }
            }
            
        }
        public double Odds
        {
            get
            {
                if (numBads>0)
                { return numGoods / numBads; }
                else
                { return 0.0; }
            }
        }
        public double badRate
        {
            get
            {
                if (numGoodsPlusBads > 0)
                { return numBads / numGoodsPlusBads; }
                else
                { return 0.0; }
            }
        }

        public double numGoodsPlusBads
        {
            get
            {
                return numGoods + numBads;
            }
        }

        public catGroupAggs()
        {
            childcats = new List<catGroupAggs>();
        }

        public void addCat(catGroupAggs newCat)
        {
            if (!isCategory)
            {
                this.childcats.Add(newCat);
            }
        }

    }

    
}
