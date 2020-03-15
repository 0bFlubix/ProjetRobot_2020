using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot
{
    //modèle du robot
    public class robot
    {
        public enum Motors
        {
            Droit, Gauche
        }

        public enum Sensors
        {
            Telem0, Telem1, Telem2, Telem3, Telem4
        }

        public enum MotorWays
        {
            Avance, Recule
        }

        public ushort[] distanceTelem = new ushort[5];
        public ushort actualSpeedRoueGauche;
        public ushort actualSpeedRoueDroite;
        public robot.MotorWays actualWayRoueGauche;
        public robot.MotorWays actualWayRoueDroite;
    }

}
