/* 
 * File:   PWM.h
 * Author: TP-EO-1
 *
 * Created on 6 février 2020, 15:48
 */

#ifndef PWM_H
#define	PWM_H

//#define COEFF_VITESSE_LINEAIRE_PERCENT 25
//#define COEFF_VITESSE_ANGULAIRE_PERCENT 50

void PWMSetSpeedConsignePolaire(void); 
void PWMSetSpeedConsigne(float vitesseEnPourcents, unsigned char moteur);
void InitPWM(void);
void PWMUpdateSpeed(void);
void PWMSetSpeedConsignePolaire(void);

#endif	/* PWM_H */

