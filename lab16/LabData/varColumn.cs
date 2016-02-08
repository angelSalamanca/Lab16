using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LabQuant;


namespace LabData
{
    public class varColumn
    {
        private string dataPath;
        
        public List<double> doubleValues;
        public List<Int32> intValues;
        public List<string> stringValues;
        public variable myVariable;

        public  Dictionary<string, int> stringCount;

        private System.IO.FileStream fs;
        
     
        private string filename
        {
            get
            {
                return dataPath + myVariable.name + ".col";
            }
        }



        public varColumn(variable var, string storePath)
        {
            myVariable = var;
            dataPath = storePath;
            switch (var.variableType)
                {
                case General.typeOfVariable.String:
                    this.stringValues = new List<string>();
                    this.stringCount = new Dictionary<string, int>();
                    break;
                case General.typeOfVariable.Integer:
                    this.intValues  = new List<Int32>();
                    break;
                case General.typeOfVariable.Double:
                    this.doubleValues = new List<Double>();
                    break;
            }
        }

        
        public void write()
        {
            switch (myVariable.variableType)
            {
                case General.typeOfVariable.String:
                    stringWrite();
                    break;
                case General.typeOfVariable.Double:
                    doubleWrite();
                    break;
                default:
                    intWrite(); // int and Date
                    break;
            }
        }

        public void read()
        {
            switch (myVariable.variableType)
            {
                case General.typeOfVariable.String:
                    stringRead();
                    break;
                case General.typeOfVariable.Double:
                    doubleRead();
                    break;
                default:
                    intRead(); // int and Date
                    break;
            }
        }

        public void addStringValue(string s)
        {
            this.stringValues.Add(s);
            
            if (this.stringCount.ContainsKey(s))
            {
                this.stringCount[s] = this.stringCount[s] + 1;
            }
            else
            {
                this.stringCount.Add(s, 1);
            }

        }

        private void stringWrite()
        {
            openForWrite();
            StreamWriter sw = new StreamWriter(this.fs, Encoding.UTF8);

            /// Go over column values
            for (int i= 0; i < this.stringValues.Count; i++)
            {
               
                sw.Write(Convert.ToChar((byte)(this.stringValues[i].Length)));
                sw.Write(this.stringValues[i]);
            }
            sw.Close();
            closeStream();
        }

        private void doubleWrite()
        {
            openForWrite();
            BinaryWriter bw = new BinaryWriter(this.fs);
            /// Go over column values
            for (int i = 0; i < this.doubleValues.Count; i++)
            {

                bw.Write(this.doubleValues[i]);
            }
            bw.Close();
            closeStream();
        }
        private void intWrite()
        {
            openForWrite();
            BinaryWriter bw = new BinaryWriter(this.fs);
            /// Go over column values
            for (int i = 0; i < this.intValues.Count; i++)
            {

                bw.Write(this.intValues[i]);
            }
            bw.Close();
            closeStream();
        }

        private void stringRead()
        {
            openForRead();
            var sr = new StreamReader(this.fs, Encoding.UTF8);

            /// Go over column values
            /// Format: length, value
            this.stringValues = new List<String>();
            while (!sr.EndOfStream)
            {
                byte howLong = Convert.ToByte(sr.Read());
                var c = new char[howLong];
                sr.Read(c, 0, c.Length);
                this.stringValues.Add(new string(c));
            }
            sr.Close();
            closeStream();
        }

        private void doubleRead()
        {
            openForRead();
            var br = new BinaryReader(this.fs);

            /// Go over column values
            this.doubleValues = new List<Double>();
            while (br.BaseStream.Position < this.fs.Length) 
            {
                
                this.doubleValues.Add(br.ReadDouble());
            }
            br.Close();
            closeStream();
        }

        private void intRead()
        {

            openForRead();
            var br = new BinaryReader(this.fs);

            /// Go over column values
            this.intValues = new List<Int32>();
            while (br.BaseStream.Position < this.fs.Length)
            {

                this.intValues.Add(br.ReadInt32());
            }
            br.Close();
            closeStream();
        }

        private void openForWrite()
        {
            this.fs = new FileStream(this.filename, FileMode.Create);
        }

        private void openForRead()
        {
            this.fs = new FileStream(this.filename, FileMode.Open);
        }

        private void closeStream()
        {
            this.fs.Close();
        }


    }
}
