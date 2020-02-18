#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include  "ADC.h"
#include "main.h"
#include "Robot.h"

//robot params
/*tested params : FowardSpeed = 20
                  RotationSpeed = 10 
                  SlowDownSpeed = 10
                  TriggerRotationDistance = 30
                  TriggerSlowDownDistance = 35
                  Acceleration = 1
                  PWM timer freq = 100
 */
int FowardSpeed = 30;
int RotationSpeed = 30;
int SlowDownSpeed = 20;
int TriggerRotationDistance[5] = {20, 20, 20, 20, 20};
int TriggerSlowDownDistance[5] = {50, 50, 50, 50, 50};

unsigned char stateRobot;
void OperatingSystemLoop(void)
{
    switch (stateRobot)
    {
        case STATE_ATTENTE:
            timestamp = 0;
            PWMSetSpeedConsigne(0, MOTEUR_DROIT);
            PWMSetSpeedConsigne(0, MOTEUR_GAUCHE);
            stateRobot = STATE_ATTENTE_EN_COURS;

        case STATE_ATTENTE_EN_COURS:
            if (timestamp > 1000)
            stateRobot = STATE_AVANCE;
            break;

        case STATE_AVANCE:
            PWMSetSpeedConsigne(FowardSpeed, MOTEUR_DROIT);
            PWMSetSpeedConsigne(FowardSpeed, MOTEUR_GAUCHE);
            stateRobot = STATE_AVANCE_EN_COURS;
            break;
        case STATE_AVANCE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_GAUCHE:
            PWMSetSpeedConsigne(RotationSpeed, MOTEUR_DROIT);
            PWMSetSpeedConsigne(-15, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_GAUCHE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_DROITE:
            PWMSetSpeedConsigne(-15, MOTEUR_DROIT);
            PWMSetSpeedConsigne(RotationSpeed, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_DROITE_EN_COURS;
            break;
        case STATE_TOURNE_DROITE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_SUR_PLACE_GAUCHE:
            PWMSetSpeedConsigne(RotationSpeed, MOTEUR_DROIT);
            PWMSetSpeedConsigne(-RotationSpeed, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_SUR_PLACE_DROITE:
            PWMSetSpeedConsigne(-RotationSpeed, MOTEUR_DROIT);
            PWMSetSpeedConsigne(RotationSpeed, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS;
            break;
            
        case STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;
            
        case STATE_SLOW_DOWN:
            PWMSetSpeedConsigne(SlowDownSpeed, MOTEUR_GAUCHE);
            PWMSetSpeedConsigne(SlowDownSpeed, MOTEUR_DROIT);
            SetNextRobotStateInAutomaticMode();
            break;

        default :
            stateRobot = STATE_ATTENTE;
            break;
    }
}

unsigned char nextStateRobot = 0;
void SetNextRobotStateInAutomaticMode()
{
    unsigned char positionObstacle = PAS_D_OBSTACLE;
    
    if ( ((robotState.distanceTelemetre3 < TriggerRotationDistance[3]) && (robotState.distanceTelemetre4 < TriggerRotationDistance[4])) ||
              ((robotState.distanceTelemetre2 < TriggerRotationDistance[2]) && (robotState.distanceTelemetre4 < TriggerRotationDistance[4]))
            ) //Obstacle à droite
            positionObstacle = OBSTACLE_A_GAUCHE;   
    
    else if(  ((robotState.distanceTelemetre1 < TriggerRotationDistance[1]) && (robotState.distanceTelemetre0 < TriggerRotationDistance[0])) ||
              ((robotState.distanceTelemetre0 < TriggerRotationDistance[0]) && (robotState.distanceTelemetre2 < TriggerRotationDistance[2]))) //Obstacle à gauche
        positionObstacle = OBSTACLE_A_DROITE;
    
    else  if( 
             (robotState.distanceTelemetre0 < TriggerSlowDownDistance[0] && robotState.distanceTelemetre0 > TriggerRotationDistance[0]) ||
             (robotState.distanceTelemetre1 < TriggerSlowDownDistance[1] && robotState.distanceTelemetre1 > TriggerRotationDistance[1]) ||
             (robotState.distanceTelemetre2 < TriggerSlowDownDistance[2] && robotState.distanceTelemetre2 > TriggerRotationDistance[2]) ||
             (robotState.distanceTelemetre3 < TriggerSlowDownDistance[3] && robotState.distanceTelemetre3 > TriggerRotationDistance[3]) ||
             (robotState.distanceTelemetre4 < TriggerSlowDownDistance[4] && robotState.distanceTelemetre4 > TriggerRotationDistance[4]) 
           )
        positionObstacle = SLOW_DOWN;
    
    //Détermination de la position des obstacles en fonction des télémètres

    

    
    else if(robotState.distanceTelemetre2 < TriggerRotationDistance[2]) //Obstacle en face
        positionObstacle = OBSTACLE_EN_FACE;
    
    else if( (robotState.distanceTelemetre0 > TriggerSlowDownDistance[0]) && 
             (robotState.distanceTelemetre1 > TriggerSlowDownDistance[1]) &&
             (robotState.distanceTelemetre2 > TriggerSlowDownDistance[2]) &&
             (robotState.distanceTelemetre3 > TriggerSlowDownDistance[3]) &&
             (robotState.distanceTelemetre4 > TriggerSlowDownDistance[4])) //pas d?obstacle
        positionObstacle = PAS_D_OBSTACLE;
    

        
    
    //Détermination de l?état à venir du robot
    if (positionObstacle == PAS_D_OBSTACLE)
        nextStateRobot = STATE_AVANCE;
    else if (positionObstacle == OBSTACLE_A_DROITE)
        nextStateRobot = STATE_TOURNE_GAUCHE;
    else if (positionObstacle == OBSTACLE_A_GAUCHE)
        nextStateRobot = STATE_TOURNE_DROITE;
    else if (positionObstacle == OBSTACLE_EN_FACE)
        nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;
    else if(positionObstacle = SLOW_DOWN)
        nextStateRobot = STATE_SLOW_DOWN;
    //Si l?on n?est pas dans la transition de l?étape en cours
    if (nextStateRobot != stateRobot - 1)
        stateRobot = nextStateRobot;
}

int main (void) {
    unsigned int* distance;
    
    //init stuff
    InitOscillator();
    InitTimer23();
    InitTimer1();
    InitTimer4();
    InitIO();
    InitPWM();
    InitADC1();


 while(1){
    if (ADCIsConversionFinished()) 
        {          
             
            ADCClearConversionFinishedFlag(); 
            distance = ADCGetResult(); 
             
            robotState.distanceTelemetre0 = 34 / (((float) distance[0]) * 3.3 / 4096 * 3.2) - 5; 
            robotState.distanceTelemetre1 = 34 / (((float) distance[1]) * 3.3 / 4096 * 3.2) - 5; 
            robotState.distanceTelemetre2 = 34 / (((float) distance[2]) * 3.3 / 4096 * 3.2) - 5; 
            robotState.distanceTelemetre3 = 34 / (((float) distance[3]) * 3.3 / 4096 * 3.2) - 5; 
            robotState.distanceTelemetre4 = 34 / (((float) distance[4]) * 3.3 / 4096 * 3.2) - 5; 
            
             
        } 
 
 }
  
}

