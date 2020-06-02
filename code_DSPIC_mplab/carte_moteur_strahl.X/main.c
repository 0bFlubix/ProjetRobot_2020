#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include <libpic30.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include "ADC.h"
#include "main.h"
#include "Robot.h"
#include "UART.h"
#include "CB_TX1.h"
#include "CB_RX1.h"

    unsigned char rcvState = Waiting;
    unsigned short msgDecodedFunction;
    unsigned short msgDecodedPayloadLength;
    unsigned char* msgDecodedPayload;
    unsigned char receivedCheckSum;
    unsigned char CheckSumErrorOccured;
    unsigned char calculatedCheckSum;
    unsigned char msgDecodedPayloadIndex;


int main(void) 
{
    //init stuff
    InitOscillator();
    InitTimer23();
    InitTimer1();
    InitTimer4();
    InitIO();
    InitPWM();
    InitADC1();
    InitUART();
    
   
    unsigned long timeSVar = 0;
    unsigned char speedG = 0;
    unsigned char speedD = 0;
    while(1)
    {
        if(timestamp - timeSVar > 200)
        {
            UartSendSpeedInfo(speedG, speedD);
            speedG += 2;
            speedD += 1;
            timeSVar = timestamp;
        }
         
    }
}

   