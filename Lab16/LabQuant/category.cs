using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    public class category
    {
        public string name;
        public Int32 catId;
        public string stringValue;
        public Boolean isMissing;
        public Boolean isWrong;
        public Int32 lowerInt;
        public Int32 upperInt;
        public Boolean closedlower;
        public Boolean closedUpper;
        public double lowerDouble;
        public double upperDouble;
        public variable myVar;

       

        public category(string cName, Int32 cId, variable cVar, Boolean cIsMissing, Boolean cIsWrong)
        {
            this.name = cName;
            this.catId = cId;
            this.isMissing = false;
            this.isWrong = false;
            this.myVar = cVar;

            if (cIsMissing)
                {isMissing = true; this.name = General.missingCatName;}

            if (cIsWrong)
                {isWrong = true; this.name = General.wrongCatName; }
        }

    }
}
