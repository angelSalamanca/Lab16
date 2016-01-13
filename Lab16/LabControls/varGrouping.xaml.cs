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

namespace LabControls
{
    /// <summary>
    /// Interaction logic for varGrouping.xaml
    /// </summary>
    public partial class varGrouping : UserControl
    {
        ImageSource _yourImage;

        public ImageSource YourImage
        {
            get { return _yourImage; }
            set
            {
                _yourImage = value;
            }
        }
        public varGrouping()
        {
            InitializeComponent();
        }

        public void addVariable(variable var)
        {
            StackPanel sp;
            TreeViewItem tItem = new TreeViewItem();
            tItem.IsExpanded = false;
            sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            sp.Height = this.imGrouping.Height;
            sp.Margin = new Thickness(0, 2, 2, 2);

            // image
            Image img = new Image();
            if (var.variableType == variable.typeOfVariable.String) img.Source = this.imAlpha.Source;
            if (var.variableType == variable.typeOfVariable.Integer) img.Source = this.imInteger.Source;
            if (var.variableType == variable.typeOfVariable.Double) img.Source = this.imDouble.Source;

            sp.Children.Add(img);


            // and text
            TextBlock tb = new TextBlock();
            tb.Text = var.name;
            tb.Margin = new Thickness(4, 0, 2, 0);
            sp.Children.Add(tb);

            tItem.Header = sp;


            // BitmapImage BitImg = (BitmapImage )this.FindResource("grouping.png");
            // BitmapImage BitImg = new BitmapImage(new Uri("alpha.var.png"));


            foreach (KeyValuePair<int, grouping> kvp in var.myGroupings)
            {
                if (!kvp.Value.isMain)
                {
                    TreeViewItem gItem = new TreeViewItem();
                    gItem.IsExpanded = true;
                    sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    sp.Height = this.imGrouping.Height;
                    sp.Margin = new Thickness(0, 2, 2, 2);

                    // image
                    img = new Image();
                    img.Source = this.imGrouping.Source;
                    
                    sp.Children.Add(img);


                    // and text
                    tb = new TextBlock();
                    tb.Text = kvp.Value.name;
                    tb.Margin = new Thickness(10, 0, 2, 0);
                    sp.Children.Add(tb);
                    gItem.Header = sp;
                    
                    tItem.Items.Add(gItem);
                }
            }

            this.treeView.Items.Add(tItem);
        }

        private void addGrouping(object sender, RoutedEventArgs e)
        {

        }

        private void deleteGrouping(object sender, RoutedEventArgs e)
        {

        }
    } // class

}
