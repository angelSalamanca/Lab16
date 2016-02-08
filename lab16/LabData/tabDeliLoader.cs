using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabQuant;
using System.IO;
using System.Globalization;

namespace LabData
{
    public class tabDeliLoader
    {
        private analyticalDictionary myDict;
        private Char mySep;
        private String fileName, outFileName;
        private Dictionary<String, List<String>> strValues;
        private Dictionary<String, List<Int32>> intValues;
        private Dictionary<String, List<Double>> contValues;
        private StreamReader sr;
        private List<String> varList;
        private Int32 guessedRecordLength;
        private loadResult opResult;
        private General.typeOfVariable[,] typeList;
        private CultureInfo myCulture;
        private List<varColumn> columns;
        private readonly int maxTextItems = 100;
        
        public enum loadResult { Success = 0, DuplicateVar }

        public tabDeliLoader(string dName, string dLabPath, string fName, string outfName, Char sep, CultureInfo dCulture)
        {
            myDict = new analyticalDictionary(dName, dLabPath);
            mySep = sep;
            fileName = fName;
            outFileName = outfName;
            myCulture = dCulture;
        }

        public analyticalDictionary  Load()
        {
            strValues = new Dictionary<String, List<String>>();
            intValues = new Dictionary<String, List<Int32>>();
            contValues = new Dictionary<String, List<Double>>();

            sr = new StreamReader(fileName);

            if (readHeader())
            {
                if (readLines())
                {
                    populateDS();
                }
            }

            sr.Close();
            return this.myDict;
        }

        // build a list of variable names. Must be unique.
        private bool readHeader()
        {
            List<String> words = this.getWords();
            varList = new List<String>();


            foreach (String varName in words)
            {
                var properVarName = General.toLabName(varName);
                if (varList.Contains(properVarName))
                {
                    this.opResult = loadResult.DuplicateVar;
                    return false;
                }
                varList.Add(properVarName);
            }
            return true;
        }

        // Guess variable type scanning first 10 records
        private Boolean readLines()
        {

            int myInt;
            double myDouble;
           

            this.guessedRecordLength = 0;
            typeList = (General.typeOfVariable[,])Array.CreateInstance(typeof(General.typeOfVariable), 10, varList.Count);

            for (int nl = 1; nl <= 10; nl++) // read 10 lines
            {
                List<String> words = this.getWords(true);
                // go over words
                for (int nw = 0; nw < Math.Min(words.Count, varList.Count); nw++)
                {
                    var varName = this.varList[nw];
                    // Is it integer?
                    if (int.TryParse(words[nw], NumberStyles.Any, myCulture, out myInt))
                    {
                        typeList[nl - 1, nw] = General.typeOfVariable.Integer;
                    }
                    else
                    {
                        if (Double.TryParse(words[nw], NumberStyles.Any, myCulture, out myDouble))
                        {
                            typeList[nl - 1, nw] = General.typeOfVariable.Double ;
                        }
                        else
                        {
                            typeList[nl - 1, nw] = General.typeOfVariable.String;
                        }
                    }
                } //nw
        } // nl
        this.guessedRecordLength = this.guessedRecordLength / 10;
            
        // Decide of var type
        for (int nv = 0; nv < varList.Count; nv++)
            {
                General.typeOfVariable varType = General.typeOfVariable.Integer ; // Default
                for (int nl = 0; nl < typeList.GetLength(0) - 1; nl++)
                { 
				if (typeList[nl, nv] == General.typeOfVariable.String )
                    {
                       varType = General.typeOfVariable.String;
                       break;
                    }
               if (typeList[nl, nv] == General.typeOfVariable.Double )
                    {
                        varType = General.typeOfVariable.Double;
                    }
                
            }
                typeList[0, nv] = varType;
            }
        
        return true;
    }
        private List<String> getWords(Boolean addToLength = false)
        {
            var L = sr.ReadLine();
            if (addToLength)
            {
                this.guessedRecordLength += L.Length;
            }
            return L.Split(mySep).ToList();
        }

        private Boolean  populateDS()
        { 
        
		for (int nv = 0; nv < varList.Count; nv++)
            {
                
            var oldVar = myDict.addVar(varList[nv], typeList[0, nv], General.blockType.Input);

                if (oldVar != null)
                {
                    switch (typeList[0, nv])
                    {
                        case General.typeOfVariable.Integer:
                            intValues.Add(varList[nv], new List<Int32>());
                            break;
                    case General.typeOfVariable.Double:
                            contValues.Add(varList[nv], new List<Double>());
                            break;
                    case General.typeOfVariable.String:
                            strValues.Add(varList[nv], new List<String>());
                            break;
                    }
                }

        }
		
		return true;
	}

        public void loadData()
        {
            columns = new List<varColumn>();
            int intValue = Int32.MinValue ;
            double doubleValue = Double.NaN ;
            
            // Create dir if needed
            Directory.CreateDirectory(this.myDict.filePath(this.outFileName));
            // init columns
            for (int i = 0; i < this.varList.Count; i++)
            {
                string varName = this.varList[i];
                var var = this.myDict.varByName[varName];
                //string filename = this.myDict.filePath(this.outFileName + Path.DirectorySeparatorChar) + varName + General.varFileSuffix;
                columns.Add(new varColumn(var, this.myDict.filePath(this.outFileName + Path.DirectorySeparatorChar)));
            }

            sr = new StreamReader(this.fileName);
            sr.ReadLine(); // skip header

            // init progressbar

            while (sr.Peek()>-1)
            {
                // fine progress step
                var words = this.getWords();

                for (int i= 0; i< this.varList.Count; i++)
                {
                    string varName = this.varList[i];
                    var var = this.myDict.varByName[varName];
                    // get the value
                    switch (var.variableType)
                    {
                        case General.typeOfVariable.String:
                            if (i >= words.Count)
                                columns[i].addStringValue(string.Empty);
                            else
                                columns[i].addStringValue(words[i]);
                            break;
                        case General.typeOfVariable.Integer:
                        case General.typeOfVariable.Date:
                            if (i >= words.Count)
                                columns[i].stringValues.Add(String.Empty);
                            else
                                Int32.TryParse(words[i], NumberStyles.Any, this.myCulture, out intValue);
                                columns[i].intValues.Add(intValue);
                            break;
                        case General.typeOfVariable.Double:
                            if (i >= words.Count)
                                columns[i].stringValues.Add(String.Empty);
                            else
                                Double.TryParse(words[i], NumberStyles.Any, this.myCulture, out doubleValue);
                                columns[i].doubleValues.Add(doubleValue);
                            break;
                    }
                    
                }
            }

            sr.Close();

            foreach (varColumn varcol in columns)
                varcol.write();

            this.autoCatGeneration();
     
    }
    private void autoCatGeneration()
        {
   
            // String first
            foreach (varColumn varCol in this.columns)

            {
                switch (varCol.myVariable.variableType)
                {
                    case General.typeOfVariable.String:
                        if (varCol.stringCount.Count <= this.maxTextItems)
                        {
                            var strVals = varCol.stringCount.Keys.ToList();
                            strVals.Sort();
                            foreach (string strVal in strVals)
                            {
                                if (strVal != String.Empty)
                                {
                                    this.myDict.addCat(varCol.myVariable, strVal, strVal); // do not create empty Cat, missing is just as good
                                }
                            }
                         }
                        break;
                    case General.typeOfVariable.Integer:
                    case General.typeOfVariable.Date:
                    case General.typeOfVariable.Double:
                        break;
                }
            }
                
            /*
            ' Int now 
                    For Each kp As keyvaluepair(Of String, List(Of Int32)) In Me.intValues
                        varRow = DS.BuscarVar(kp.Key)
                        If kp.Value.Count <= Me.maxIntItems  Then
                            kp.Value.Sort
                            For Each catValue As Int32 In kp.Value
                                Dim catRow As DataRow = DS.CreaCat(varRow, catValue.ToString(Unica.Entorno.EngineCUlture))
                                catRow("Desde") = catValue
                                catRow("Hasta") = catValue
                            Next

                        End If

                    Next

                    ' And Cont
                    For Each kp As keyvaluepair(Of String, List(Of Double)) In Me.contValues
                        varRow = DS.BuscarVar(kp.Key)
                        If kp.Value.Count <= Me.maxContItems Then
                            kp.Value.Sort
                            For Each catvalue As Double In kp.Value
                                Dim catRow As DataRow = DS.CreaCat(varRow, catValue.ToString(Unica.Entorno.EngineCUlture))
                                catRow("Desde") = catValue
                                catRow("Hasta") = catValue
                            Next

                        End If
                        }
            */
        }




    }
}
