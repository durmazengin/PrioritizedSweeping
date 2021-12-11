using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StowawayPS
{
    public enum EnvironmentResponse
    {
        NOT_AVAILABLE,
        AVAILABLE,
        GOAL
    }
    public enum WindDirection
    {
        East,
        South,
        West,
        North
    }
    public class Environment
    {
        private const int MARGIN = 5;
        private const int WIND_PNL_THICKNESS = 20;
        private const int NUM_OF_WINDS = 4;

        Panel pnlBackground = null;
        int gridsHorizontal = 0;
        int gridsVertical = 0;
        int unitWidth = 0;
        int unitHeight =0;
        Color backgroundColor = Color.White;

        Point agentPosition = new Point(1, 1);
        Point goalPosition = new Point(4, 4);

        Wind[] winds = new Wind[NUM_OF_WINDS];

        Color gridLineColor = Color.Black;
        bool animated = true;
        bool appStopped = false;

        Image[] windImages = new Image[4];

        private bool reinitialized = false;
        public Environment(Panel background)
        {
            pnlBackground = background;
            System.Threading.Thread threadAnimate = new System.Threading.Thread(new System.Threading.ThreadStart(checkAnimate));
            threadAnimate.Start();

            windImages[(int)WindDirection.East] = Image.FromFile("resources\\wind_east.png");
            windImages[(int)WindDirection.South] = Image.FromFile("resources\\wind_south.png");
            windImages[(int)WindDirection.West] = Image.FromFile("resources\\wind_west.png");
            windImages[(int)WindDirection.North] = Image.FromFile("resources\\wind_north.png");
        }
        void checkAnimate()
        {
            while(true)
            {
                animated = System.IO.File.Exists("animate");
                System.Threading.Thread.Sleep(1000);
                if(appStopped)
                {
                    break;
                }
            }
        }
        public void onAppStopped()
        {
            appStopped = true;
        }
        public void drawGrids(int columnCount, int rowCount, Color lineColor, Color backColor)
        {
            gridsHorizontal = rowCount;
            gridsVertical = columnCount;
            backgroundColor = backColor;
            gridLineColor = lineColor;
            
            unitWidth = (pnlBackground.Width - 2 * WIND_PNL_THICKNESS - MARGIN) / gridsHorizontal;
            unitHeight = (pnlBackground.Height - 2 * WIND_PNL_THICKNESS - MARGIN) / gridsVertical;
            
            goalPosition = new Point(goalPosition.X, goalPosition.Y);

            paintAll();
        }
        internal EnvironmentResponse tryAction(int xPosition, int yPosition)
        {
            if((xPosition < 1) || (yPosition < 1) || (xPosition > gridsVertical) || (yPosition > gridsHorizontal))
            {
                return EnvironmentResponse.NOT_AVAILABLE;
            }
            Point oldPosition = agentPosition;
            /*
             * there may be wind
             */
            agentPosition = checkAndApplyWinds(new Point(xPosition, yPosition));
            
            moveAgent(oldPosition, agentPosition);

            if((agentPosition.X == goalPosition.X) && (agentPosition.Y == goalPosition.Y))
            {
                if (animated)
                {
                    System.Threading.Thread.Sleep(500);
                }
                return EnvironmentResponse.GOAL;
            }
            return EnvironmentResponse.AVAILABLE;
        }
        /// <summary>
        /// Function loops for all winds, uses getWindEffect function 
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <returns>next point</returns>
        private Point checkAndApplyWinds(Point currentPosition)
        {
            Point nextPosition = currentPosition;

            for(int i = 0; i < NUM_OF_WINDS; i++)
            {
                if(null != winds[i])
                {
                    if ((currentPosition.Y == winds[i].getPosition()) &&
                        (winds[i].getDirection() == WindDirection.East || winds[i].getDirection() == WindDirection.West))
                    {
                        // found a wind position
                        if (Utils.GetRandomNumber(0, 2) > 0) // does winding
                        {
                            nextPosition = getWindEffect(winds[i], currentPosition);
                        }
                        break;
                    }
                    else if ((currentPosition.X == winds[i].getPosition()) &&
                       (winds[i].getDirection() == WindDirection.North || winds[i].getDirection() == WindDirection.South))
                    {
                        // found a wind position
                        if (Utils.GetRandomNumber(0, 2) > 0) // does winding
                        {
                            nextPosition = getWindEffect(winds[i], currentPosition);
                        }
                        break;
                    }
                }
            }

            return nextPosition;
        }

        private Point getWindEffect(Wind wind, Point currentPosition)
        {
            Point nextPosition = currentPosition;
            int step = wind.getPower();

            if ((currentPosition.Y == wind.getPosition()) &&
                (wind.getDirection() == WindDirection.East || wind.getDirection() == WindDirection.West))
            {
                if (wind.getDirection() == WindDirection.East)
                {
                    nextPosition.X += step;
                    if (nextPosition.X > gridsVertical)
                    {
                        nextPosition.X = gridsVertical;
                    }
                }
                else
                {
                    if (step >= nextPosition.X)
                    {
                        step = nextPosition.X - 1;
                    }
                    nextPosition.X -= step;
                }
            }
            else if ((currentPosition.X == wind.getPosition()) &&
               (wind.getDirection() == WindDirection.North || wind.getDirection() == WindDirection.South))
            {
                if (wind.getDirection() == WindDirection.South)
                {
                    nextPosition.Y += step;
                    if (nextPosition.Y > gridsHorizontal)
                    {
                        nextPosition.Y = gridsHorizontal;
                    }
                }
                else
                {
                    if (step >= nextPosition.Y)
                    {
                        step = nextPosition.Y - 1;
                    }
                    nextPosition.Y -= step;
                }

            }
            return nextPosition;
        }

        internal Point getAgentPosition()
        {
            return agentPosition;
        }

        private void moveAgent(Point oldPosition, Point agentPosition)
        {
            if (animated)
            {
                Graphics gridDrawer = pnlBackground.CreateGraphics();
                Brush brushBackcolor = new SolidBrush(backgroundColor);

                // clear old position
                gridDrawer.FillRectangle(brushBackcolor, new Rectangle(
                    (oldPosition.X - 1) * unitWidth + WIND_PNL_THICKNESS,
                    (oldPosition.Y - 1) * unitHeight + WIND_PNL_THICKNESS, 
                    unitWidth, unitHeight));


                Brush brushAgentcolor = new SolidBrush(gridLineColor);
                // set new position for agent
                gridDrawer.FillEllipse(brushAgentcolor, new Rectangle(
                    (agentPosition.X - 1) * unitWidth + WIND_PNL_THICKNESS,
                    (agentPosition.Y - 1) * unitHeight + WIND_PNL_THICKNESS, 
                    unitWidth, unitHeight));

                System.Threading.Thread.Sleep(2);
            }
        }
        /// <summary>
        /// SetAgentStartPosition draws agent start position
        /// it is called when setting system
        /// do not call on take action
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        internal void setAgentStartPosition(int xPosition, int yPosition)
        {
            agentPosition = new Point(xPosition, yPosition);
            paintAll();
            reinitialized = true;

        }
        /// <summary>
        /// SetGoalPosition draws target positon where agent to succeed
        /// it is called on setting environment
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        internal void setGoalPosition(int xPosition, int yPosition)
        {
            goalPosition = new Point(xPosition, yPosition);
            paintAll();
            reinitialized = true;
        }

        private void paintAll()
        {
            if (!animated)
            {
                if (!reinitialized)
                    return;
            }
            reinitialized = false;

            Graphics gridDrawer = pnlBackground.CreateGraphics();
            // firstly clear all field
            Brush brushClean = new SolidBrush(Color.White);
            gridDrawer.FillRectangle(brushClean, new Rectangle(0, 0, 
                pnlBackground.Width, 
                pnlBackground.Height));


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

            // Fill background with selected color
            Brush brushBackcolor = new SolidBrush(backgroundColor);
            gridDrawer.FillRectangle(brushBackcolor, 
                new Rectangle(WIND_PNL_THICKNESS, WIND_PNL_THICKNESS, allWidth, allHeight));

            if (animated)
            {
                // write column / row numbers
                Brush brushTextcolor = new SolidBrush(Color.Black);
                for (int i = 0; i < gridsHorizontal; i++)
                {
                    gridDrawer.DrawString((i + 1) + "", new Font("Arial", 8), brushTextcolor,
                        new Point(i * unitWidth + WIND_PNL_THICKNESS, 0));
                }// draw horizontal lines
                for (int j = 0; j < gridsVertical; j++)
                {
                    gridDrawer.DrawString((j + 1) + "", new Font("Arial", 8), brushTextcolor,
                          new Point(0, j * unitHeight + WIND_PNL_THICKNESS));
                }
            }
            // Drawings for wind
            for(int i = 0; i < NUM_OF_WINDS; i++)
            {
                if(null != winds[i])
                {
                    drawWind(gridDrawer, winds[i]);
                }
            }
            
            Pen pen = new Pen(gridLineColor);

            // draw vertical lines
            for (int i = 0; i <= gridsHorizontal; i++)
            {
                gridDrawer.DrawLine(pen, 
                    new Point(i * unitWidth + WIND_PNL_THICKNESS, WIND_PNL_THICKNESS), 
                    new Point(i * unitWidth + WIND_PNL_THICKNESS, WIND_PNL_THICKNESS + allHeight));
            }
            // draw horizontal lines
            for (int j = 0; j <= gridsVertical; j++)
            {
                gridDrawer.DrawLine(pen, 
                    new Point(WIND_PNL_THICKNESS, j * unitHeight + WIND_PNL_THICKNESS), 
                    new Point(allWidth + WIND_PNL_THICKNESS, j * unitHeight + WIND_PNL_THICKNESS));
            }
            
            Brush brushAgentcolor = new SolidBrush(gridLineColor);
            // set new position for agent
            gridDrawer.FillEllipse(brushAgentcolor, new Rectangle(
                (agentPosition.X - 1) * unitWidth + WIND_PNL_THICKNESS,
                (agentPosition.Y - 1) * unitHeight + WIND_PNL_THICKNESS, 
                unitWidth, unitHeight));

            Brush brushGoalcolor = new SolidBrush(Color.Green);
            // set new position for agent
            gridDrawer.FillEllipse(brushGoalcolor, new Rectangle(
                (goalPosition.X - 1) * unitWidth + WIND_PNL_THICKNESS,
                (goalPosition.Y - 1) * unitHeight + WIND_PNL_THICKNESS, 
                unitWidth, unitHeight));
        }

        private void drawWind(Graphics gridDrawer, Wind wind)
        {
            int windXPosition = 0;
            int windYPosition = 0;
            switch (wind.getDirection())
            {
                case WindDirection.East:
                    windXPosition = 0;
                    windYPosition = wind.getPosition() * unitHeight;
                    break;
                case WindDirection.South:
                    windXPosition = wind.getPosition() * unitWidth;
                    windYPosition = 0;
                    break;
                case WindDirection.West:
                    windXPosition = (gridsHorizontal + 1) * unitWidth;
                    windYPosition = wind.getPosition() * unitWidth;
                    break;
                case WindDirection.North:
                    windXPosition = wind.getPosition() * unitWidth;
                    windYPosition = (gridsVertical + 1) * unitHeight;
                    break;
            }
            gridDrawer.DrawImage(windImages[(int)wind.getDirection()], 
                new Rectangle(windXPosition, windYPosition, unitWidth, unitHeight));

        }

        internal bool getIsAnimate()
        {
            return animated;
        }

        internal void setWind(int windNr, WindDirection direction, int position)
        {
            if (position == 0)
            {
                winds[windNr - 1] = null;
            }
            else
            {
                winds[windNr - 1] = new Wind(direction, position);
            }

            paintAll();
        }

        internal Wind getCurrentWind(int windNr)
        {
            return winds[windNr - 1];
        }
    }
}
