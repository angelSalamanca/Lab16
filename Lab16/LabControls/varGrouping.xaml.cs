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
        private readonly string tempGroupingName = "<click to enter name>";
        private analyticalDictionary myDD;
        private TextBlock savedTb;
        private TreeViewItem editedItem;
        private variable myVariable;
        private Boolean endEditEventHandled;
        private Boolean groupingAdded;
        private Boolean newGroupingAction;
        labMessage myMessage;

        public ImageSource YourImage
        {
            get { return _yourImage; }
            set
            {
                _yourImage = value;
            }
        }
        public varGrouping(analyticalDictionary vgDD)
        {
            InitializeComponent();
            myDD = vgDD;
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
                    addGroupingSon(tItem, kvp.Value.name);
                }
            }

            this.treeView.Items.Add(tItem);
        }

        private TreeViewItem addGroupingSon(TreeViewItem parentVar, string groupingName)
        {
            TreeViewItem gItem = new TreeViewItem();
            gItem.IsExpanded = true;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Height = this.imGrouping.Height;
            sp.Margin = new Thickness(0, 2, 2, 2);

            // image
            Image img = new Image();
            img.Source = this.imGrouping.Source;

            sp.Children.Add(img);


            // and text
            TextBlock  tb = new TextBlock();
            tb.Text = groupingName;
            tb.Margin = new Thickness(10, 0, 2, 0);
            sp.Children.Add(tb);
            gItem.Header = sp;

            parentVar.Items.Add(gItem);
            return (gItem);
        }

        private void addGrouping(object sender, RoutedEventArgs e)
        {
         
            string varName;
            TreeViewItem varItem;            
            StackPanel mySp;

            TreeViewItem myItem = (TreeViewItem) treeView.SelectedItem;
            if (myItem.Parent is TreeView)
            {
                mySp = (StackPanel)myItem.Header;
                varItem = myItem;
            }
            else
            {
                mySp = (StackPanel)((TreeViewItem)myItem.Parent).Header;
                varItem = (TreeViewItem)myItem.Parent;
            }

            varName = ((TextBlock)mySp.Children[1]).Text;
            this.myVariable  = myDD.varByName[varName];
            TreeViewItem groupingItem = this.addGroupingSon(varItem, this.tempGroupingName);
            varItem.IsExpanded = true;
            groupingItem.IsSelected = true;

            // falta editar y después dar de alta el grouping si es ok
            groupingAdded = false;
            endEditEventHandled = false;
            newGroupingAction = true;
            editGroupingName(groupingItem, true);
        }

        private void editGroupingName(TreeViewItem groupingItem, Boolean isNew)
        {
            TextBox tbGrouping = new TextBox();
            tbGrouping.Text = getGroupingName(groupingItem);
            tbGrouping.KeyDown += new KeyEventHandler(tbKeyDown);
            tbGrouping.PreviewLostKeyboardFocus  += new KeyboardFocusChangedEventHandler(tbPreviewLost);
            tbGrouping.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(tbGotKeyboardFocus);

            tbGrouping.Style = (Style)(this.Resources["tbGrouping"]);
            

            this.savedTb = getTextBlock(groupingItem);
            this.editedItem = groupingItem;
            if (!isNew) { groupingAdded = true; }
            setTextControl(groupingItem, tbGrouping);
           
            // Validar nombre de grouping. Si ok, se crea grupo y se cambia el Box por el Block. Si no, aviso y focus.
            
        }

        private void tbGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox source = e.Source as TextBox;

            if (source != null)
            {
                // Change the TextBox color when it obtains focus.
                // StackPanel sp = (StackPanel)source.Parent;
                
                source.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xF2, 0xFF));

                // Clear the TextBox.
                source.Clear();
            }
        }

        private Color AdjustBrightness(Color originalColor , double brightnessFactor)
        {
            
            Color adjustedColor = Color.FromArgb(originalColor.A, (byte)(originalColor.R * brightnessFactor), (byte)(originalColor.G * brightnessFactor),
                        (byte)(originalColor.B * brightnessFactor));
            return adjustedColor;
        }

        private string getGroupingName(TreeViewItem tItem)
        {
            StackPanel sp = (StackPanel)tItem.Header;
            TextBlock tb = (TextBlock)sp.Children[1];
            return tb.Text;
        }
        private TextBlock getTextBlock(TreeViewItem tItem)
        {
            StackPanel sp = (StackPanel)tItem.Header;
            return (TextBlock)sp.Children[1];
            
        }

        private void tbKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.endEditEventHandled = tbEditEnded((TextBox) sender);
                
            }
        }

        private void tbPreviewLost(object sender, RoutedEventArgs e)
        {
            if (endEditEventHandled)
            {
                e.Handled = true; // handled in tbKeyDown - enter
                return;
            }

            this.endEditEventHandled = this.tbEditEnded((TextBox)sender);
            if (!this.endEditEventHandled ) // prevent losing focues
            {
                e.Handled = true;
            }
            
        }

        private Boolean tbEditEnded(TextBox tb)
        {
            this.myMessage = new labMessage();

            if (!this.myDD.isValidName(tb.Text))
            {
               myMessage.showMessage("Error", tb.Text + " is not a valid name (use letters, numbers, space, underscore and dash)", labMessage.messageType.error);
               return false;
            }

           // Save name. Also, new grouping if not already added
                if (groupingAdded)
                   {
                        if (this.myVariable.existsGrouping(tb.Text) & (this.savedTb.Text != tb.Text)) // weskip the error if name unchanged
                        {
                           myMessage.showMessage("Error", "Name " + tb.Text + " is already used in another grouping of the variable", labMessage.messageType.error);
                           return false;
                        }
                     // Update grouping name - rename case 
                   this.myVariable.renameGrouping(savedTb.Text, tb.Text);
                   this.savedTb.Text = tb.Text;
                        setTextControl(editedItem, savedTb);                
  
                    }
                else
                {
                // Also checks name not duplicate
                    groupingAdded = (null != this.myVariable.addGrouping(tb.Text, false));
                    if (groupingAdded)
                      {
                        this.savedTb.Text = tb.Text;
                        setTextControl(editedItem, savedTb);
                        endEditEventHandled = true;
                         }
                    else
                    {
                        myMessage.showMessage("Error", "Name " + tb.Text + " is already used in another grouping of the variable", labMessage.messageType.error);
                        return false;
                    }
                }

            endEditEventHandled = true;
            return true;
           
        } // tbEditEnded
        private void setTextControl(TreeViewItem tItem, TextBox tb)
        {
            StackPanel sp = (StackPanel)tItem.Header;
            sp.Children.RemoveAt(1);
            sp.Children.Add(tb);
            FocusManager.SetFocusedElement(sp, tb);
        }

        private void setTextControl(TreeViewItem tItem, TextBlock tb)
        {
            StackPanel sp = (StackPanel)tItem.Header;
            sp.Children.RemoveAt(1);
            sp.Children.Add(tb);
        }

        private void deleteGrouping(object sender, RoutedEventArgs e)
        {

        }
        private void renameGrouping(object sender, RoutedEventArgs e)
        {
            string varName;
            TreeViewItem varItem;
            StackPanel mySp;

            TreeViewItem myItem = (TreeViewItem)treeView.SelectedItem;
            if (myItem.Parent is TreeView)
            {
                return; // can't rename main
            }
            else
            {
                mySp = (StackPanel)((TreeViewItem)myItem.Parent).Header;
                varItem = (TreeViewItem)myItem.Parent;
            }

            varName = ((TextBlock)mySp.Children[1]).Text;
            this.myVariable = myDD.varByName[varName];
            TreeViewItem groupingItem = myItem;
            varItem.IsExpanded = true;
            groupingItem.IsSelected = true;

            // falta editar y después dar de alta el grouping si es ok
            
            endEditEventHandled = false;
            newGroupingAction = false;
            editGroupingName(groupingItem, false);
        }
    } // class

}
