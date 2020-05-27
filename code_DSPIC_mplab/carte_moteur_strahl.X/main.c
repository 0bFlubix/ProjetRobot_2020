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




int main(void) {
    //init stuff
    InitOscillator();
    InitTimer23();
    InitTimer1();
    InitTimer4();
    InitIO();
    InitPWM();
    InitADC1();
    InitUART();

        /* LOOPBACK TEST
        int i;
        for(i = 0; i < CB_RX1_GetDataSize(); i++)
        {
            unsigned char c = CB_RX1_Get();
            SendMessage(&c, 1);
        }

       */
    
        unsigned char text[7] = "UART_OK";
        unsigned char speedTransmit[2] = { 0x0A, 0xFA };
        unsigned long timeSample = 0;

        while(1)
        {
            if(timestamp - timeSample >= 100)
            {
                speedTransmit[0] += 10;
                speedTransmit[1] += 1;
                timeSample = timestamp;
                UartSendSpeedInfo(speedTransmit[0], speedTransmit[1]);
                UartEncodeAndSendMessage(0x0080, 7, text);
            }      
        }
    }

