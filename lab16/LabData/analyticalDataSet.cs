using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabQuant;

namespace LabData
{
    public class analyticalDataSet
    {
        private analyticalDictionary myDict;
        private string name;

        public analyticalDataSet(analyticalDictionary DD, string dataSetName)
        {
            this.myDict = DD;
            this.name = dataSetName;
        }



    }
}
