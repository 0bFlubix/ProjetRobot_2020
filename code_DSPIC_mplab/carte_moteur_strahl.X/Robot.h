/* 
 * File:   Robot.h
 * Author: TP-EO-1
 *
 * Created on 6 février 2020, 15:26
 */

#ifndef ROBOT_H
#define	ROBOT_H

typedef struct robotStateBITS
{
    union
    {
        struct
        {
            unsigned char taskEnCours;
            
            float vitesseGaucheConsigne;
            float vitesseGaucheCommandeCourante;
            float vitesseDroiteConsigne;
            float vitesseDroiteCommandeCourante;
            
            float distanceTelemetre0; 
            float distanceTelemetre1; 
            float distanceTelemetre2; 
            float distanceTelemetre3; 
            float distanceTelemetre4; 
        };
    };
}ROBOT_STATE_BITS;
extern volatile ROBOT_STATE_BITS robotState;
#endif /*ROBOT_H*/