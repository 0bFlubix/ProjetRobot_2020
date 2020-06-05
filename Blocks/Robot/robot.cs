using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot
{
    //robt state enums
    public class robot
    {
        public enum Motors
        {
            Droit, Gauche
        }

        public enum LED
        {
            ORANGE = 1,
            BLUE = 2,
            WHITE = 3
        }

        public enum Sensors
        {
            
        }

        public enum MotorWays
        {
            Avance, Recule
        }

        public ushort[] distanceTelem = new ushort[5];
        public sbyte actualSpeedRoueGauche; //signed
        public sbyte actualSpeedRoueDroite; //signed
        public robot.MotorWays actualWayRoueGauche;
        public robot.MotorWays actualWayRoueDroite;
    }

}
