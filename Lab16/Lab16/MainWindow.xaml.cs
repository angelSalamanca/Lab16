﻿using System;
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



namespace Lab16
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Create dictionary
            analyticalDictionary myDD = new analyticalDictionary("Sample");
            
            // Create variable
            variable x = myDD.addVar("x", variable.typeOfVariable.Integer);
            variable y = myDD.addVar("y", variable.typeOfVariable.String);

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

            bivariate myBiv = new bivariate(xGrouping, xValues, yGrouping, yValues);

            byte a = 0;
            // echo dictionary
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
    }
}
