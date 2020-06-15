#include <xc.h>
#include "timer.h"
#include "IO.h"
#include "main.h"
#include "PWM.h"
#include "QEI.h"



/*All of the interrupts routines*/


unsigned long timeSample = 0;


//Interruption du timer 1, at FREQ_ECH_QEI frequency, affects robot acceleration
void __attribute__((interrupt, no_auto_psv)) _T1Interrupt(void)
{
    IFS0bits.T1IF = 0;
    
    QEIUpdateData(); //update QEI data
    PWMUpdateSpeed();//update Speed
    PWMSetSpeedConsignePolaire();
    
    if(timestamp - timeSample >= 50)//20Hz
    {
        timeSample = timestamp;        
        SendPositionData();
    }
    
}

//interrupt timer 4
void __attribute__((interrupt, no_auto_psv)) _T4Interrupt(void) { 
    IFS1bits.T4IF = 0; 
    timestamp++; 
} 

