using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace testDG
{
    public class catGroupAggs
    {
        private double counter;

        private double nGoods, nBads, nAccept, nTotal;

        public double totalOdds;

        public Int32 Id;

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
        public double numBads { 
                get
            {
                if (!isCategory)
                {
                    counter = 0;
                    foreach (catGroupAggs child in childcats)
                    {
                        counter += child.numBads;
                    }
                    return counter;
                }
                else
                {
                    return nBads;
                }
            }
            set
            {
                nBads = value;
            }
        }
        public double numAccepts
        {
            get
            {
                if (!isCategory)
                {
                    counter = 0;
                    foreach (catGroupAggs child in childcats)
                    {
                        counter += child.numAccepts;
                    }
                    return counter;
                }
                else
                {
                    return nAccept;
                }
            }
            set
            {
                nAccept = value;
            }
        }
        public double numTotal
        {
            get
            {
                if (!isCategory)
                {
                    counter = 0;
                    foreach (catGroupAggs child in childcats)
                    {
                        counter += child.numTotal;
                    }
                    return counter;
                }
                else
                {
                    return nTotal;
                }
            }
            set
            {
                nTotal = value;
            }
        }

        public Color BackColor
        {
            get
            {
                if (isCategory)
                {
                    return Color.FromArgb(255, 255, 255, 255); /// white
                }
                else
                {
                    return Color.FromArgb(255, 100, 200, 0); 
                }
            }
        }

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

        public double goodBadIndex
        {
            get
                {
                if (totalOdds > 0.0)
                {
                    return Odds / totalOdds;
                }
                else
                {
                    return 0.0;
                }
            }

        }

        public double weightOfEvidence
        {
            get
            {
                double gbi = goodBadIndex;
                if (gbi != 0)
                {
                    return Math.Log(gbi) * 100;
                }
                else return 0;
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



    } /// class

    
}
