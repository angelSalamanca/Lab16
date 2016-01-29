using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using testDG;
using System.Windows.Input;



namespace testDG.catAndGroupVM
{
    public class catAndGroupVM : INotifyPropertyChanged
    {

        private DataGrid myDataGrid;
        private expandCommand myButtonCommand;
        public catAndGroupVM()
        {
            myButtonCommand = new expandCommand(this);
        }

        public ICommand arrowClick // Point 3
        {
            get
            {
                return myButtonCommand ;
            }
        }

        public void getDataGrid(DependencyObject control)
        {
            if (myDataGrid == null)
            {
                do
                {
                    control = VisualTreeHelper.GetParent(control);
                    if (control is DataGrid)
                    {
                        myDataGrid = (DataGrid) control;
                        break;
                    }
                }
                while (true);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaiseChange(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        private int getRowIndex(DataGridRow dgRow)
        {
            getDataGrid(dgRow);
            return myDataGrid.ItemContainerGenerator.IndexFromContainer(dgRow);
        }

        private DataGridRow getRowAt(int rowIndex)
        {
            return (DataGridRow)myDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
        }


        public void expand(object sender, RoutedEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            ToggleButton tb = (ToggleButton)sender;

            while (!(obj is DataGridRow) && obj != null) obj = VisualTreeHelper.GetParent(obj);

            actualExpand(tb, (DataGridRow)obj);

        }

        public void actualExpand(ToggleButton tb, DataGridRow myRow)
        {
            /// disregard for categories
            catGroupAggs catGroupRow = (catGroupAggs)(myRow.Item);

            if (!catGroupRow.isCategory)
            {

                if ((Boolean)(!tb.IsChecked))
                {
                    Path cPath = new Path();
                    cPath.Data = Geometry.Parse("M0,0 10,0 5,5 Z");
                    cPath.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("SteelBlue"));
                    tb.Content = cPath;
                }
                else
                {
                    Path cPath = new Path();
                    cPath.Data = Geometry.Parse("M2,0 2,10 7,5 Z");
                    cPath.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("SteelBlue"));
                    tb.Content = cPath;
                }



                double rowHeight = myRow.ActualHeight;
                /// go over children and toggle visibility
                int rowIndex = getRowIndex(myRow) + 1;

                myRow = getRowAt(rowIndex);

                while (null != myRow && ((catGroupAggs)myRow.Item).isCategory)
                {
                    myRow.Height = rowHeight - myRow.ActualHeight;
                    ((catGroupAggs)myRow.Item).numGoods = ((catGroupAggs)myRow.Item).numGoods + 1;
                    rowIndex += 1;
                    myRow = getRowAt(rowIndex);

                }
            }
            else /// category
            {
                tb.Visibility = Visibility.Hidden;

            }

            /// Go over all arrows


        }

      

    } // class

    public class expandCommand : ICommand 
    {
        private catAndGroupVM obj; 
        public expandCommand(catAndGroupVM _obj) // Point 2
        {
            obj = _obj;
        }
        public bool CanExecute(object parameter)
        {
            return true; // Point 3
        }

        public event EventHandler CanExecuteChanged;


        public void Execute(object parameter)
        {

            byte a = 0;
            ///obj.expand(); // Point 4
        }
    }

    public class ConvertItemToArrow : IValueConverter
        {
            #region IValueConverter Members
            //Convert the Item to an Index
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                int rowindex;
                catGroupAggs cg;
               
                try
                {
                    //Get the DataRowView that is being passed to the Converter
                    if (!(value is catGroupAggs)) return "";

                    cg = (catGroupAggs)value;

                    //Get the CollectionView from the DataGrid that is using the converter
                    DataGrid dg = (DataGrid)Application.Current.MainWindow.FindName("DG1");

                    CollectionView cv = (CollectionView)dg.Items;
                    //Get the index of the DataRowView from the CollectionView

                    rowindex = cv.IndexOf(cg) + 1;

                    if (cg.isCategory)
                    { return ""; }
                    else
                    {
                        return "M0,0 10,0 5,5 Z";
                    }


                }
                catch (Exception e)
                {
                    throw new NotImplementedException(e.Message);
                }
            }
            //One way binding, so ConvertBack is not implemented
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }


    
} // namespace
