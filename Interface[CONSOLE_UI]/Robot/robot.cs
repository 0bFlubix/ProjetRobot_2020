using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot
{
    //robt state enums
    public static class robot
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

        public static float vitesseLineaireConsigne;
        public static float vitesseAngulaireConsigne;
        public static float vitesseLineaireFromOdometry;
        public static float vitesseAngulaireFromOdometry;
        public static float xPositionFromOdometry;
        public static float yPositionFromOdometry;

        public static ushort[] distanceTelem = new ushort[5];
        public static sbyte actualSpeedRoueGauche; //signed
        public static sbyte actualSpeedRoueDroite; //signed
        public static robot.MotorWays actualWayRoueGauche;
        public static robot.MotorWays actualWayRoueDroite;
    }

}
