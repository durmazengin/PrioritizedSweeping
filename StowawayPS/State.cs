using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowawayPS
{
    public class State
    {
        /*
         * 4 actions describes
         * - LEFT
         * - UP
         * - RIGHT
         * - DOWN
         */

        private int xPosition = 1;
        private int yPosition = 1;
        
        private int stateValue = 0;
        private double[] actionRewards = new double[Action.COUNT];
        private int[] nextStates = new int[Action.COUNT];
        private int priorAction = -1;

        int lastAction = -1;
        
        public State(int x, int y)
        {
            xPosition = x;
            yPosition = y;
            for(int i = 0; i < Action.COUNT;i++)
            {
                nextStates[i] = -1;// no next state yet
            }
        }

        public int getXPosition()
        {
            return xPosition;
        }

        public int getYPosition()
        {
            return yPosition;
        }

        public double getActionReward(int action)
        {
            return actionRewards[action];
        }

        public void setActionReward(int action, double reward)
        {
            actionRewards[action] = reward;
        }

        public int getStateValue()
        {
            return stateValue;
        }

        public void setStateValue(int reward)
        {
            stateValue = reward;
        }

        public int getNextSate(int action)
        {
            return nextStates[action];
        }

        public void setNextSate(int action, int statePosition)
        {
            nextStates[action] = statePosition;
        }

        public int getPriorAction()
        {
            return priorAction;
        }
        public void setPriorAction(int value)
        {
            priorAction = value;
        }
        /* Object Related Functions*/

        public override int GetHashCode()
        { 
            return xPosition * 10000 + yPosition;
        }
        public override bool Equals(object objToCompare)
        {
            if(objToCompare is State)
            {
                State state = (State)objToCompare;
                if(state.getXPosition() == this.getXPosition() && state.getYPosition() == this.getYPosition())
                {
                    /*bool isEqual = base.Equals(objToCompare);
                    if(!isEqual)
                    {
                        isEqual = true;
                    }*/
                    return true;
                }
            }
            return base.Equals(objToCompare);
        }
        
        internal void setLastAction(int selectedAction)
        {
            lastAction = selectedAction;
        }
        internal int getLastAction()
        {
            return lastAction;
        }
    }
}
