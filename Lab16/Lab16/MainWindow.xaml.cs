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
            myDD = new analyticalDictionary("Sample");
            
            // Create variable
            variable x = myDD.addVar("x", variable.typeOfVariable.Integer);
            variable y = myDD.addVar("y", variable.typeOfVariable.String);
            variable z = myDD.addVar("Time at Job", variable.typeOfVariable.Integer);
            z = myDD.addVar("Residential Status", variable.typeOfVariable.String);
            z = myDD.addVar("Type of Employment", variable.typeOfVariable.String);
            z = myDD.addVar("Annual revenues", variable.typeOfVariable.Double);
            z = myDD.addVar("Loan amount", variable.typeOfVariable.Double);
            z = myDD.addVar("Loan duration", variable.typeOfVariable.Integer);
            z = myDD.addVar("Gender", variable.typeOfVariable.String);


            category goodCat = myDD.addCat(y, "Good", "G");
            category badCat = myDD.addCat(y, "Bad", "B");
            category indetCat = myDD.addCat(y, "Indeterminate", "I");
            category rejectCat = myDD.addCat(y, "Good",  "R");

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

           this.myBiv  = new bivariate(xGrouping, xValues, yGrouping, yValues);
                       
            // echo dictionary
           
            // add new grouping
            myDD.varByName["x"].addGrouping("Other grouping", false);
            myDD.varByName["x"].addGrouping("Other grouping 2", false);
            myDD.varByName["y"].addGrouping("Useless grouping", false);
            myDD.varByName["Type of Employment"].addGrouping("Special grouping", false);


            fillVAndG();
        } // click

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
            string[] ddText = myDD.toText().Split(myDD.separator, StringSplitOptions.None);

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
