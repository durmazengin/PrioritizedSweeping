using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StowawayPS
{
    public partial class ReportForm : Form
    {
        private const int POWER = 5;
        private const int MARGIN = 2;
        List<State> statesVisited;
        int gridsHorizontal;
        int gridsVertical;

        public ReportForm(List<State> listStatesVisited, int numOfGridsHorizontal, int numOfGridsVertical)
        {
            InitializeComponent();

            statesVisited = listStatesVisited;
            gridsHorizontal = numOfGridsHorizontal;
            gridsVertical = numOfGridsVertical;

            this.Text += String.Format(" (Values E-{0})", POWER);
        }

        public void showStates()
        {
            Graphics gridDrawer = pnlRewards.CreateGraphics();
            int unitWidth = (pnlRewards.Width - 2 * MARGIN) / gridsHorizontal;
            int unitHeight = (pnlRewards.Height - 2 * MARGIN) / gridsVertical;

            Brush brushClean = new SolidBrush(Color.White);
            gridDrawer.FillRectangle(brushClean, new Rectangle(0, 0, pnlRewards.Width, pnlRewards.Height));


            /*
               all width to be painted is not equal to panel width 
               all height to be painted is not equal to panel height
               since points are integer and "allWidth / row count" can be decimal
               then they are rounded to integer. then width decreased
               for example allWidth = 600, row count 11, 
                         then cell width 50
                         colored all width 50 * 11 = 550

               and scenario for height (allHeight / column count)
               so recalculate 
               */
            int allWidth = unitWidth * gridsHorizontal;
            int allHeight = unitHeight * gridsVertical;

            Pen pen = new Pen(Color.Black);

            // draw vertical lines
            for (int i = 0; i <= gridsHorizontal; i++)
            {
                gridDrawer.DrawLine(pen, new Point(i * unitWidth, MARGIN), new Point(i * unitWidth, allHeight));
            }
            // draw horizontal lines
            for (int j = 0; j <= gridsVertical; j++)
            {
                gridDrawer.DrawLine(pen, new Point(MARGIN, j * unitHeight), new Point(allWidth, j * unitHeight));
            }

            Brush brushTextcolor = new SolidBrush(Color.Black);
            for (int i = 0; i < statesVisited.Count;i++)
            {
                int x = statesVisited[i].getXPosition() - 1;
                int y = statesVisited[i].getYPosition() - 1;
                double stateVal = 0;
                for (int j = 0; j < Action.COUNT; j++)
                {
                    stateVal += statesVisited[i].getActionReward(j);
                }
                String strVal = String.Format("{0:0.00}", stateVal* Math.Pow(10, POWER));
                gridDrawer.DrawString(strVal + "", new Font("Arial", 8), brushTextcolor,
                        new Point(x * unitWidth + MARGIN, y * unitHeight + MARGIN));
            }
            
        }
        public void clear()
        {
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReportForm_Shown(object sender, EventArgs e)
        {
            showStates();
        }
    }
}
