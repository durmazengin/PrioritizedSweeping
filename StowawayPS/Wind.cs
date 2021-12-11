using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowawayPS
{
    public class Wind
    {
        private const int MAX_WIND_FORCE = 5;

        WindDirection direction = WindDirection.East;
        int position = 0;

        public Wind(WindDirection windDirection, int windPosition)
        {
            this.direction = windDirection;
            this.position = windPosition;
        }

        public WindDirection getDirection()
        {
            return direction;
        }

        public int getPosition()
        {
            return position;
        }
        public int getPower()
        {
            return Utils.GetRandomNumber(1, MAX_WIND_FORCE);
        }
    }
}
