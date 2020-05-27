#include <xc.h>
#include "timer.h"
#include "IO.h"
#include "main.h"
#include "PWM.h"



/*All of the interrupts routines*/



//Interruption du timer 1
void __attribute__((interrupt, no_auto_psv)) _T1Interrupt(void)
{
    IFS0bits.T1IF = 0;
    PWMUpdateSpeed();
}

//interrupt timer 4
void __attribute__((interrupt, no_auto_psv)) _T4Interrupt(void) { 
    IFS1bits.T4IF = 0; 
    timestamp++; 

} 

