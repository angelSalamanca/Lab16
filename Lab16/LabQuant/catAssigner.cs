using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    class catAssigner
    {
        variable myVariable;

        public catAssigner(variable aVariable)
        {
            myVariable = aVariable ;
        }

        public category assignTextCategory(string sValue)
        {
            foreach (KeyValuePair<Int32, category> kvp in myVariable.myCategories)
            {
                if (kvp.Value.stringValue == sValue)
                {
                    return kvp.Value ;
                }
            }

            return myVariable.unexpectedCat;
        }

  
    } // catAssigner
}
