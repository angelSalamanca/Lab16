using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml;

namespace LabQuant
{
    public class analyticalDictionary
    {
        public string name;
        public Dictionary<String, variable> varByName;
        public Dictionary<Int32, variable> varByNum;
        private Int32 varPointer, catPointer, groupingPointer, groupPointer;
        private string labPath;
        private readonly string dictionaryName = "analyticalDictionary";
        private readonly string inputName = "inputVariables";
        private readonly string internalName = "calculatedVariables";
        public readonly string varName = "var";
        private readonly string catName = "cat";
        private readonly string groupingName = "grouping";
        private readonly string groupName = "group";

        public string projectPath
        {
            get
            {
                return this.labPath + this.name + System.IO.Path.DirectorySeparatorChar;
            }
        }

        public string DataPath
        {
            get
            {
                return this.projectPath + General.dataDir + System.IO.Path.DirectorySeparatorChar;
            }
        }

        public string modelPath
        {
            get
            {
                return this.projectPath + General.modelDir + System.IO.Path.DirectorySeparatorChar;
            }
        }

        public string dictionaryFileName
        {
            get
            {
                return this.projectPath + General.dictionaryName;
            }
        }

        


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

        public Boolean isValidName(string aName)
        {
            return Regex.IsMatch(aName, "^[A-Za-z0-9-_ ]+$");
        }

        public String filePath(string filename)
        {
            return this.DataPath + filename + System.IO.Path.DirectorySeparatorChar;
        }

        // 

        public analyticalDictionary(string dName, string dLabPath)
        {
            name = dName;
            labPath = dLabPath;

            varPointer = 0; groupingPointer = 0; catPointer = 0; groupPointer = 0;
            varByName = new Dictionary<string, variable>();
            varByNum = new Dictionary<Int32, variable>();
        }

        public variable addVar(string vName, General.typeOfVariable vType, General.blockType aBlock = General.blockType.Input)
        {
            if (this.varByName.ContainsKey(vName))
            {
                return null; // duplicate
            }

            variable newVar = new variable(vName, vType, getVarNum, this, aBlock);
            this.varByName.Add(vName, newVar);
            this.varByNum.Add(this.varPointer, newVar);
            
            return newVar;
        }

        public category addCat(variable myVar, string cName,  string cValue)
        {
            category myCat = myVar.addCategory(cName, getCatNum, cValue);
            return myCat;

        }

        public grouping  addGrouping(variable myVar, string gName)
        {
            var myGrouping = myVar.addGrouping(gName, false);
            return myGrouping;

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
                sb.Append(General.separator[0]);
                foreach (KeyValuePair<Int32, category> kvp2 in var.myCategories)
                {
                    category cat = kvp2.Value;
                    sb.Append("  cat: ");
                    sb.Append(kvp2.Key.ToString("###0 "));
                    sb.Append(cat.name);
                    sb.Append(General.separator[0]);
                }

                    foreach (KeyValuePair<Int32, grouping> kvpG in var.myGroupings)
                    {
                        grouping grp = kvpG.Value;
                        sb.Append("  grpng: ");
                        sb.Append(kvpG.Key.ToString("###0 "));
                        sb.Append(grp.name);
                        sb.Append(General.separator[0]);
                        foreach (KeyValuePair<Int32, group> kvpg in grp.myGroups)
                            {
                        group myGroup = kvpg.Value;
                        sb.Append("    group: ");
                        sb.Append(kvpg.Key.ToString("###0 "));
                        sb.Append(myGroup.name);
                        sb.Append(General.separator[0]);

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

        public Boolean save()
        {
            var serial = this.toXML();
            System.IO.StreamWriter sw = null;
            try
            {
                sw = new System.IO.StreamWriter(this.dictionaryFileName);
                sw.WriteLine(serial);
                sw.Close();
            }
            catch (Exception e)
            {
                if (sw.BaseStream != null)
                {
                    sw.Close();
                    return false;
                }
            }
            return true;
        }

        public string toXML()
        {
            var xDoc = new XmlDocument();
            // the xml declaration is recommended, but not mandatory
            XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = xDoc.DocumentElement;
            xDoc.InsertBefore(xmlDeclaration, root);

            // string.Empty makes cleaner code
            XmlElement dictNode = xDoc.CreateElement(string.Empty, this.dictionaryName, string.Empty);
            xDoc.AppendChild(dictNode);

            // 2 blocks
            XmlElement inputNode = xDoc.CreateElement(string.Empty, this.inputName, string.Empty);
            dictNode.AppendChild(inputNode);
            XmlElement internalNode = xDoc.CreateElement(string.Empty, this.internalName, string.Empty);
            dictNode.AppendChild(internalNode);

            // All variables now
            foreach (variable var in this.varByNum.Values)
            {
                XmlElement varNode = xDoc.CreateElement(string.Empty, this.varName, string.Empty);
                var nameAttribute = xDoc.CreateAttribute("name");
                nameAttribute.Value = var.name;
                varNode.Attributes.Append(nameAttribute);

                nameAttribute = xDoc.CreateAttribute("id");
                nameAttribute.Value = var.varId.ToString();
                varNode.Attributes.Append(nameAttribute);

                nameAttribute = xDoc.CreateAttribute("type");
                nameAttribute.Value = var.variableType.ToString();
                varNode.Attributes.Append(nameAttribute);

                nameAttribute = xDoc.CreateAttribute("description");
                nameAttribute.Value = var.Description;
                varNode.Attributes.Append(nameAttribute);

                nameAttribute = xDoc.CreateAttribute("description");
                nameAttribute.Value = var.Description;
                varNode.Attributes.Append(nameAttribute);

                nameAttribute = xDoc.CreateAttribute("missingReplacement");
                nameAttribute.Value = var.missingReplacement;
                varNode.Attributes.Append(nameAttribute);

                nameAttribute = xDoc.CreateAttribute("wrongReplacement");
                nameAttribute.Value = var.wrongReplacement;
                varNode.Attributes.Append(nameAttribute);

                // add categories
                XmlElement catsNode = xDoc.CreateElement(String.Empty, "categories", string.Empty);
                varNode.AppendChild(catsNode);

                foreach (category cat in var.myCategories.Values )
                    {
                    XmlElement catNode = xDoc.CreateElement(String.Empty, this.catName, string.Empty);
                    catsNode.AppendChild(catNode);

                    var catAttribute = xDoc.CreateAttribute("name");
                    catAttribute.Value = cat.name;
                    catNode.Attributes.Append(catAttribute);

                    catAttribute = xDoc.CreateAttribute("id");
                    catAttribute.Value = cat.catId.ToString();
                    catNode.Attributes.Append(catAttribute);

                    catAttribute = xDoc.CreateAttribute("stringValue");
                    catAttribute.Value = cat.stringValue;
                    catNode.Attributes.Append(catAttribute);
                    
                    catAttribute = xDoc.CreateAttribute("isMissing");
                    catAttribute.Value = cat.isMissing.ToString();
                    catNode.Attributes.Append(catAttribute);

                    catAttribute = xDoc.CreateAttribute("isWrong");
                    catAttribute.Value = cat.isWrong.ToString();
                    catNode.Attributes.Append(catAttribute);

                    catAttribute = xDoc.CreateAttribute("lowerInt");
                    catAttribute.Value = cat.lowerInt.ToString();
                    catNode.Attributes.Append(catAttribute);

                    catAttribute = xDoc.CreateAttribute("upperInt");
                    catAttribute.Value = cat.upperInt.ToString();
                    catNode.Attributes.Append(catAttribute);

                    catAttribute = xDoc.CreateAttribute("closedLower");
                    catAttribute.Value = cat.closedlower.ToString();
                    catNode.Attributes.Append(catAttribute);

                    catAttribute = xDoc.CreateAttribute("closedUpper");
                    catAttribute.Value = cat.closedUpper.ToString();
                    catNode.Attributes.Append(catAttribute);

                    catAttribute = xDoc.CreateAttribute("lowerDouble");
                    catAttribute.Value = XmlConvert.ToString(cat.lowerDouble);
                    catNode.Attributes.Append(catAttribute);


                    catAttribute = xDoc.CreateAttribute("upperDouble");
                    catAttribute.Value = XmlConvert.ToString(cat.upperDouble);
                    catNode.Attributes.Append(catAttribute);



                    /* 
        public double lowerDouble;
        public double upperDouble;
        */

                }

                // add groupings
                XmlElement groupingsNode = xDoc.CreateElement(String.Empty, "groupings", string.Empty);
                varNode.AppendChild(groupingsNode);

                foreach (grouping grp in var.myGroupings.Values)
                {
                    XmlElement grpNode = xDoc.CreateElement(String.Empty, this.groupingName, string.Empty);
                    groupingsNode.AppendChild(grpNode);

                    var grpAttribute = xDoc.CreateAttribute("name");
                    grpAttribute.Value = grp.name;
                    grpNode.Attributes.Append(grpAttribute);

                    grpAttribute = xDoc.CreateAttribute("id");
                    grpAttribute.Value = grp.groupingId.ToString();
                    grpNode.Attributes.Append(grpAttribute);

                    grpAttribute = xDoc.CreateAttribute("isMain");
                    grpAttribute.Value = grp.isMain.ToString();
                    grpNode.Attributes.Append(grpAttribute);

                    // Groups
                    XmlElement groupsNode = xDoc.CreateElement(String.Empty, "groups", string.Empty);
                    grpNode.AppendChild(groupsNode);

                    foreach (group childGroup in grp.myGroups.Values)
                    {
                        XmlElement childgrpNode = xDoc.CreateElement(String.Empty, this.groupName, string.Empty);
                        groupsNode.AppendChild(childgrpNode );

                        var childAttribute = xDoc.CreateAttribute("name");
                        childAttribute.Value = childGroup.name;
                        childgrpNode.Attributes.Append(childAttribute);


                    }


                }

                if (var.myBlock == General.blockType.Input)
                {
                    inputNode.AppendChild(varNode);
                }
                else
                {
                    internalNode.AppendChild(varNode);
                }

            }

            
            return xDoc.OuterXml;
        }

        
        
        
    } //class
}
