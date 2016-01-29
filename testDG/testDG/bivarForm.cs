using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C1.Win.C1FlexGrid;
using System.Drawing.Drawing2D;

namespace testDG
{
    public partial class bivarForm : Form
    {
        groupingAggs myGroupingAggs;
        private readonly int stdWidth = 60 ;
        private readonly int arrowWidth = 20;
        private readonly int stdHeight = 15;
        SolidBrush arrowBrush = new SolidBrush(Color.FromArgb(255, 96, 96, 112));
        private Point[] arrowPoints;
        private int oldGroupId, draggedCatId;

        public bivarForm(groupingAggs myGroupingAggsNew)
        {
            InitializeComponent();
            myGroupingAggs = myGroupingAggsNew;
            myGroupingAggs.buildGroupingDataSet();
            catGroupFlex.DataSource = myGroupingAggs.dataSource;
            initialStyling();
            styleGrid();
        }

        private void styleGrid()
        {
        
            for (int r=1; r<catGroupFlex.Rows.Count; r++)
            {
                var flexRow = catGroupFlex.Rows[r];

                if ((bool)flexRow["isCategory"])
                {
                    flexRow.Style = catGroupFlex.Styles["Normal"];
                    
                }
                else
                {
                    flexRow.Style = catGroupFlex.Styles["Group"];
                    
                }

            }

        }

        private void initialStyling()
        {
            catGroupFlex.Rows[0].Height = 2 * stdHeight;
            catGroupFlex.Cols["isCategory"].Width = arrowWidth;
            catGroupFlex.Cols["isCategory"].Caption = "";

            /// catGroupFlex.Cols["isCategory"].Visible = false;

            catGroupFlex.Cols["name"].Caption = "Name";
            catGroupFlex.Cols["name"].Width = 3 * stdWidth;

            catGroupFlex.Cols["numGoods"].Caption = "# Goods";
            catGroupFlex.Cols["numGoods"].Format = "N00";

            catGroupFlex.Cols["numBads"].Caption = "# Bads";
            catGroupFlex.Cols["numBads"].Format = "N00";

            catGroupFlex.Cols["numAccepts"].Caption = "# Accepts";
            catGroupFlex.Cols["numGoods"].Format = "N00";

            catGroupFlex.Cols["numTotal"].Caption = "# Total";
            catGroupFlex.Cols["numTotal"].Format = "N00";

            catGroupFlex.Cols["badRate"].Caption = "Bad rate";
            catGroupFlex.Cols["badRate"].Format = "P02";

            catGroupFlex.Cols["odds"].Caption = "Odds";
            catGroupFlex.Cols["odds"].Format = "N02";

            catGroupFlex.Cols["oddsIndex"].Caption = "Odds Index";
            catGroupFlex.Cols["oddsIndex"].Format = "N02";

            catGroupFlex.Cols["WOE"].Caption = "Weight of Evidence";
            catGroupFlex.Cols["WOE"].Format = "N02";

            catGroupFlex.Cols["Id"].Visible = false; // group/cat num not shown

            catGroupFlex.OwnerDrawCell += myDrawCell;
            catGroupFlex.Click += clickOnFlex;

            arrowPoints = new Point[3];

            foreach (C1.Win.C1FlexGrid.Row fRow in catGroupFlex.Rows)
            {
                fRow.UserData = true; /// expanded
            }

            catGroupFlex.BeforeMouseDown += new BeforeMouseDownEventHandler(this.flexBeforeMouseDown);
            catGroupFlex.DragOver += new DragEventHandler(this.flex_DragOver);
            catGroupFlex.DragDrop += new DragEventHandler(this.flex_DragDrop);

        }

        private void myDrawCell(object sender, OwnerDrawCellEventArgs e)
            {
            /// only column 0 and group
            if (e.Col != 0 || e.Row == 0) return;

            if ((bool)this.catGroupFlex[e.Row, e.Col])
            {
                e.Handled = true;
                return;
            }
           
            e.DrawCell(DrawCellFlags.Background);
            bool isExpanded = (bool)catGroupFlex.Rows[e.Row].UserData;
            // Ok, if it's collapsed we do one thing, if not other
            if (isExpanded)
            {
                arrowPoints[0] = new Point(e.Bounds.Location.X + 5, e.Bounds.Location.Y + 6);
                arrowPoints[1] = new Point(e.Bounds.Location.X + 15, e.Bounds.Location.Y + 6);
                arrowPoints[2] = new Point(e.Bounds.Location.X + 10, e.Bounds.Location.Y + 11);
            }
            else
            {
                arrowPoints[0] = new Point(e.Bounds.Location.X + 7, e.Bounds.Location.Y + 4);
                arrowPoints[1] = new Point(e.Bounds.Location.X + 7, e.Bounds.Location.Y + 14);
                arrowPoints[2] = new Point(e.Bounds.Location.X + 13, e.Bounds.Location.Y + 9);
            }
            // Define fill mode.
            FillMode newFillMode = FillMode.Alternate ;
           /// e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill polygon to screen.
            e.Graphics.FillPolygon(arrowBrush, arrowPoints, newFillMode);

            // We' re done drawing this cell.
            e.Handled = true;
            }

        /// <summary>
        /// expand - collapse group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickOnFlex(object sender, EventArgs e)
        {
            var ht = catGroupFlex.HitTest();

            if (ht.Row  < 1) return; /// not a cat/group

            if ((bool)this.catGroupFlex[ht.Row, 0]) return; /// cat, no collapse operation

            /// So it's a group
            bool isExpanded = !(bool)catGroupFlex.Rows[ht.Row].UserData;
            catGroupFlex.Rows[ht.Row].UserData = isExpanded;
            
            for (int r=ht.Row + 1; r<catGroupFlex.Rows.Count; r++)
            {
                /// exit if is group
                if (!(bool)this.catGroupFlex[r, 0]) break;
                catGroupFlex.Rows[r].Visible = isExpanded;
            }

           catGroupFlex.Invalidate();
            
        }

        private void flexBeforeMouseDown(object sender, BeforeMouseDownEventArgs e)
        {
            //start dragging when the user clicks the cell
            C1FlexGrid flex = (C1FlexGrid)sender;
            var hti = flex.HitTest(e.X, e.Y);

            if (hti.Type == HitTestTypeEnum.Cell)
            {
                //select the catGroupAggs
                int r = hti.Row;
                               
                // forbid groups
                if (!isCat(r))
                {
                    return;
                }
                oldGroupId = getGroupIdFromFlexRow(r);
               
                // It's a cat, do drag drop
                DragDropEffects dd = flex.DoDragDrop(flex.Clip, DragDropEffects.Move);
                draggedCatId = (int)flex[r, "Id"];
                //if it worked, delete row from source (it's a move)
                

            }
        }

        private void flex_DragOver(object sender, DragEventArgs e)
        {
            //check that we have the type of data we want
            if (e.Data.GetDataPresent(typeof(string)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void flex_DragDrop(object sender, DragEventArgs e)
        {
            //find the drop position
            C1FlexGrid flex = (C1FlexGrid)sender;
            Point pt = flex.PointToClient(new Point(e.X, e.Y));
            var  hti = flex.HitTest(pt.X, pt.Y);

            int r = hti.Row;
                    
            int newGroupId = getGroupIdFromFlexRow(r);
            if (newGroupId != oldGroupId)
            {
                myGroupingAggs.moveCategory(draggedCatId, oldGroupId, newGroupId);
                myGroupingAggs.addRows();
            }
            
        }

    private int getGroupIdFromFlexRow(int r)
        {
            int id = 0;

            for (int catRow = r; catRow > 0; catRow--)
            {
                if (!isCat(catRow))
                {
                    id = (int)catGroupFlex[catRow, "Id"];
                    break;
                }
            }
            return id;
        }

    private bool isCat(int r)
        {
            return (bool)catGroupFlex[r, "isCategory"];
        }

    } //class
}
