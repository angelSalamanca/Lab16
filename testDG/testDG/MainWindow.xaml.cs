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
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace testDG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<catGroupAggs> names;
        groupingAggs myGrouping;

        public MainWindow()
        {
            InitializeComponent();
            getData();
        }

        private void getData()
        {
           
            
             myGrouping = new groupingAggs();
            /// Group 1
            catGroupAggs myGroup =   myGrouping.addGroup("Group 1", 100);
            myGrouping.addCategory(myGroup, "Low Cat", 1000, 72, 1302, 1456,1);
            myGrouping.addCategory(myGroup, "Mid Cat", 2000, 272, 2302, 2456,2);
            myGrouping.addCategory(myGroup, "High Cat", 3000, 2372, 3302, 3456,3);

            myGroup = myGrouping.addGroup("Group 2", 200);
            myGrouping.addCategory(myGroup, "Cold", 1100, 72, 1302, 1456,4);
            myGrouping.addCategory(myGroup, "Cool", 2200, 272, 2302, 2456,5);
            myGrouping.addCategory(myGroup, "Hot", 3300, 372, 3302, 3456,6);

            myGroup = myGrouping.addGroup("Group 3",300);
            myGrouping.addCategory(myGroup, "North", 1900, 72, 1302, 1456,7);
            myGrouping.addCategory(myGroup, "South", 888, 272, 2302, 2456,8);

            myGroup = myGrouping.addGroup("Last Group", 400);

            myGrouping.propagateOdds();

            names = myGrouping.flatList();

            this.DG1.ItemsSource = names;

            
        }

        public void clickButton(object sender, RoutedEventArgs e)
        {
            bivarForm bvf = new bivarForm(myGrouping);
            this.button.IsEnabled = false;
            bvf.ShowDialog();
        }

       
    } // Window


    

    

}
