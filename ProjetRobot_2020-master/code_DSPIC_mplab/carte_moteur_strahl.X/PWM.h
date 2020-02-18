/* 
 * File:   PWM.h
 * Author: TP-EO-1
 *
 * Created on 6 février 2020, 15:48
 */

#ifndef PWM_H
#define	PWM_H


//void PWMSetSpeed (float vitesseEnPourcents, unsigned char moteur);
void PWMSetSpeedConsigne(float vitesseEnPourcents, unsigned char moteur);
void InitPWM(void);
void PWMUpdateSpeed();

#endif	/* PWM_H */

