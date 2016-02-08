using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using LabQuant;
using LabControls;



namespace Lab16
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        analyticalDictionary myDD;
        bivariate myBiv;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;
            // Create dictionary
            myDD = new analyticalDictionary("Sample", "D:\\work\\Lab projects\\");
            
            // Create variable
            variable x = myDD.addVar("x", General.typeOfVariable.Integer);
            variable y = myDD.addVar("y", General.typeOfVariable.String);
            variable z = myDD.addVar("Time at Job", General.typeOfVariable.Integer);
            z = myDD.addVar("Residential Status", General.typeOfVariable.String);
            z = myDD.addVar("Type of Employment", General.typeOfVariable.String);
            z = myDD.addVar("Annual revenues", General.typeOfVariable.Double);
            var la = myDD.addVar("Loan amount", General.typeOfVariable.Double);
            z = myDD.addVar("Loan duration", General.typeOfVariable.Integer);
            z = myDD.addVar("Gender", General.typeOfVariable.String);

            myDD.addGrouping(la, "Extravagant");

            var someCat = myDD.addCat(la, "low", string.Empty);
            someCat.lowerDouble = -15E15/0.13;
            someCat.upperDouble = 90.0 / 7.0;

            category goodCat = myDD.addCat(y, "Good", "G");
            category badCat = myDD.addCat(y, "Bad", "B");
            category indetCat = myDD.addCat(y, "Indeterminate", "I");
            category rejectCat = myDD.addCat(y, "Rejected",  "R");

            grouping xGrouping = x.getGroupingByName("main");
            myDD.addIntGroup(xGrouping, "0-10", 0, 10, true, false);
            myDD.addIntGroup(xGrouping, "10-25", 10, 25, true, false);
            myDD.addIntGroup(xGrouping, "25-60", 25, 60, true, false);
            myDD.addIntGroup(xGrouping, "60-100", 60, 100, true, false);

            grouping yGrouping = y.getGroupingByName("main");
            myDD.groupCat(goodCat, myDD.addTextGroup(yGrouping, "Good"));
            myDD.groupCat(badCat, myDD.addTextGroup(yGrouping, "Bad"));
            myDD.groupCat(indetCat, myDD.addTextGroup(yGrouping, "Indet"));
            myDD.groupCat(rejectCat, myDD.addTextGroup(yGrouping, "Reject"));


            string serial = this.myDD.toXML();
            System.IO.StreamWriter sw = new System.IO.StreamWriter("D:\\Kill\\ad.xml");
            sw.WriteLine(serial);
            sw.Close();

            groupAssigner yAssigner = new groupAssigner(yGrouping);
            group assignedGroupY = yAssigner.assignTextGroup("0");
            groupAssigner xAssigner = new groupAssigner(xGrouping);
            group assignedGroupX = xAssigner.assignIntGroup(22);

            // artificial lists
            int numObs = 1000;
            Random r = new Random();

            List<int> xValues = new List<int>();
            List<string> yValues = new List<string>();
            for (int i = 0; i< numObs; i++)
            {
                xValues.Add(r.Next(0, 100));
                int gbi = r.Next(1, 4);
                switch (gbi)
                {
                    case 1: yValues.Add("G"); break;
                    case 2: yValues.Add("B"); break;
                    case 3: yValues.Add("I"); break;
                    case 4: yValues.Add("R"); break;
                }
            }

            // Write to file
            var vCol = new LabData.varColumn(myDD.varByName["y"], "D:\\Kill\\");
            vCol.stringValues = yValues;
            vCol.write();
            vCol.read();


            this.myBiv  = new bivariate(xGrouping, xValues, yGrouping, yValues);
                       
            // add new grouping
            myDD.varByName["x"].addGrouping("Other grouping", false);
            myDD.varByName["x"].addGrouping("Other grouping 2", false);
            myDD.varByName["y"].addGrouping("Useless grouping", false);
            myDD.varByName["Type of Employment"].addGrouping("Special grouping", false);


            fillVAndG();
        } // click

        private void tabdeli_Click(object sender, RoutedEventArgs e)
        {
            var projectFolder = "D:\\work\\Lab projects\\";
            var myLoader = new LabData.tabDeliLoader("New project", projectFolder, "D:\\minilab.csv", "Cards base", ';', new System.Globalization.CultureInfo("es"));
            analyticalDictionary labDict = myLoader.Load();
            myLoader.loadData();
            
            var writeSuccess = labDict.save();


            this.listView.Items.Add("Imported");
        }

        private void fillVAndG()
        {
            varGrouping  vAndG = new varGrouping(myDD);
            vAndG.HorizontalAlignment = HorizontalAlignment.Stretch;
            vAndG.VerticalAlignment = VerticalAlignment.Stretch;
            this.sPanel.Children.Add(vAndG);

            foreach(KeyValuePair<int, variable> kvp in this.myDD.varByNum)
            {
                vAndG.addVariable(kvp.Value);
            }
        } // fillVAndG

        private void echoDictionary()
        {
            string[] ddText = myDD.toText().Split(General.separator, StringSplitOptions.None);

            foreach (string line in ddText)
            {
                this.listView.Items.Add(line);
            }

            // echo counts
            this.listView.Items.Add("--- Counts ---");
            foreach (KeyValuePair<pairOfInt, int> kvp in myBiv.myXTab.xCounts)
            {

                int g1 = kvp.Key.i1;
                int g2 = kvp.Key.i2;
                int howMany = kvp.Value;
                string l = g1 + " - " + g2 + " : " + howMany;
                this.listView.Items.Add(l);
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.echoDictionary();
        }
    } // class
} // namespace
