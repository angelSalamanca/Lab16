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

namespace testDG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<catGroupAggs> names;
        public MainWindow()
        {
            InitializeComponent();
            getData();
        }

        private void getData()
        {
           
            
            groupingAggs myGrouping = new groupingAggs();
            /// Group 1
            catGroupAggs myGroup =   myGrouping.addGroup("Group 1");
            myGrouping.addCategory(myGroup, "Low Cat", 1000, 72, 1302, 1456);
            myGrouping.addCategory(myGroup, "Mid Cat", 2000, 272, 2302, 2456);
            myGrouping.addCategory(myGroup, "High Cat", 3000, 372, 3302, 3456);

            myGroup = myGrouping.addGroup("Group 2");
            myGrouping.addCategory(myGroup, "Cold", 1000, 72, 1302, 1456);
            myGrouping.addCategory(myGroup, "Cool", 2000, 272, 2302, 2456);
            myGrouping.addCategory(myGroup, "Hot", 3000, 372, 3302, 3456);

            myGroup = myGrouping.addGroup("Group 3");
            myGrouping.addCategory(myGroup, "North", 1000, 72, 1302, 1456);
            myGrouping.addCategory(myGroup, "South", 2000, 272, 2302, 2456);

            myGroup = myGrouping.addGroup("Last Group");

            names = myGrouping.flatList();







            this.DG1.ItemsSource = names;
        }

        private int getRowIndex(DataGridRow dgRow)
        {
            return DG1.ItemContainerGenerator.IndexFromContainer(dgRow);
        }

        private DataGridRow getRowAt(int rowIndex)
        {
            return (DataGridRow)DG1.ItemContainerGenerator.ContainerFromIndex(rowIndex);
        }

        public void expand(object sender, RoutedEventArgs e )
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;
            while (!(obj is DataGridRow) && obj != null) obj = VisualTreeHelper.GetParent(obj);

            /// disregard for categories
            catGroupAggs catGroupRow = (catGroupAggs)(((DataGridRow)obj).Item);

            if (!catGroupRow.isCategory)
            {
                DataGridRow myRow = (DataGridRow)obj;
                double rowHeight = myRow.ActualHeight;
                /// go over children and toggle visibility
                int rowIndex = getRowIndex(myRow) + 1;

                myRow = getRowAt(rowIndex);

                while (null!=myRow && ((catGroupAggs)myRow.Item).isCategory)
                {
                    myRow.Height= rowHeight - myRow.ActualHeight ;
                    rowIndex += 1;
                    myRow = getRowAt(rowIndex);

                }



            }
        }
    }

    
}
