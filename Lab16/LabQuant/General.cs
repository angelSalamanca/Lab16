using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabQuant
{
    public class General
    {
        public enum typeOfVariable : byte { String = 1, Integer, Double, Date };
        public enum blockType : byte { System = 1, Input, Internal, Decision, Performance }; // Only Input and Internal make sense
        public static blockType block;

        public readonly static String mainGroupingName = "main";
        public readonly static String missingCatName = "missing";
        public readonly static String wrongCatName = "unexpected";
        public readonly static String noGroupName = "ungrouped";
        public readonly static String[] separator = { "[##@##]" };

        public readonly static String dataDir= "Data";
        public readonly static String modelDir = "Model";
        public readonly static String varFileSuffix = ".var";
        public readonly static String dictionaryName = "Lab.Dictionary.xml";


        public static string toLabName(string rawName)
        {
            var name = rawName.ToCharArray();

            for (int i = 0; i < name.Length; i++)
            {

                switch (name[i])
                {
                    case 'á':
                    case 'à':
                    case 'â':
                    case 'ä':
                        name[i] = 'a';
                        break;
                    case 'é':
                    case 'è':
                    case 'ê':
                    case 'ë':
                        name[i] = 'e';
                        break;
                    case 'í':
                        name[i] = 'i';
                        break;
                    case 'ó':
                        name[i] = 'o';
                        break;
                    case 'ú':
                        name[i] = 'u';
                        break;
                    case 'Á':
                        name[i] = 'A';
                        break;
                    case 'É':
                        name[i] = 'E';
                        break;
                    case 'Í':
                        name[i] = 'I';
                        break;
                    case 'Ó':
                        name[i] = 'O';
                        break;
                    case 'Ú':
                        name[i] = 'U';
                        break;
                } // switch


            } // for
            return new string(name);
        }


    }
}

