using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LabQuant;

namespace LabControls
{
    public class varAndGroupingSelector : TreeView
    {


        public void addVariable(variable var)
        {
            TreeViewItem tItem = new TreeViewItem();
            tItem.IsExpanded = true;
            tItem.Header = var.name;
            
            foreach (KeyValuePair<int, grouping> kvp in var.myGroupings)
            {
                if (!kvp.Value.isMain)
                {
                    TreeViewItem gItem = new TreeViewItem();
                    gItem.IsExpanded = true;
                    gItem.Header = kvp.Value.name;
                    tItem.Items.Add(gItem);
                }
            }
                
            this.Items.Add(tItem);
        }

    }
}
