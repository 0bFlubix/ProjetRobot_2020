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
    unsigned char msgDecodedPayload[128]; //128 bytes amximum, use ReadData()
    unsigned char receivedCheckSum;
    unsigned char CheckSumErrorOccured;
    unsigned char calculatedCheckSum;
    unsigned char msgDecodedPayloadIndex;
    unsigned char messageAvailable = 0;
    unsigned char LED_CODE;
    unsigned char LED_STATE;

    unsigned char* ReadData(unsigned char* data, unsigned char bytes)
    {
        unsigned char dataToReturn[bytes];
        int i;
        for(i = 0; i < bytes; i++)
        {
            dataToReturn[i] = data[i];
        }
        return dataToReturn;
    }
    
    void DecodeLoop()
    {
                                            
        if(CB_RX1_IsDataAvailable())
        {
            int i;
            for(i = 0; i < CB_RX1_GetDataSize(); i++)
            {
                unsigned char c = CB_RX1_Get();
                switch(rcvState)
                {
                    case Waiting:
                        if(c == 0xFE)
                            rcvState = FunctionMSB;
                         
                        break;
                        
                    case FunctionMSB:
                        msgDecodedFunction = (unsigned short)(c << 8);
                        rcvState = FunctionLSB;
                        break;
                        
                    case FunctionLSB:
                        msgDecodedFunction += (unsigned short)(c << 0);
                        rcvState = PayloadLengthMSB;
                        break;
                    
                    case PayloadLengthMSB:
                        msgDecodedPayloadLength = (unsigned short)(c << 8);
                        rcvState = PayloadLengthLSB;
                        break;
                        
                    case PayloadLengthLSB:
                        msgDecodedPayloadLength += (unsigned short)(c << 0);
                        if (msgDecodedPayloadLength > 0)
                        {
                            msgDecodedPayloadIndex = 0;
                            rcvState = Payload;
                        }
                        else
                            rcvState = CheckSum; //if no payload, skip to CheckSum state

                        break;
                        
                    case Payload:
                        if (msgDecodedPayloadIndex < msgDecodedPayloadLength)
                        {
                            msgDecodedPayload[msgDecodedPayloadIndex] = c;
                            msgDecodedPayloadIndex++;
                            if (msgDecodedPayloadIndex == msgDecodedPayloadLength)
                                rcvState = CheckSum;
                        }
                        break;
                        
                    case CheckSum:
                        receivedCheckSum = c;
                        calculatedCheckSum = CalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                        if (calculatedCheckSum == receivedCheckSum)
                        {
                            CheckSumErrorOccured = 0;
                        }
                        else
                        {
                           CheckSumErrorOccured = 1;
                        }
                        messageAvailable = 1;
                        rcvState = Waiting;
                        
                        break;
                        
                    default:
                        rcvState = Waiting;
                        break;
                }
            }
        }
        
    }

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
    
    while(1)
    {
        DecodeLoop();
        if(messageAvailable && !CheckSumErrorOccured)
        {
            switch(msgDecodedFunction)
            {
                case SPEED:
                    PWMSetSpeedConsigne((char)ReadData(msgDecodedPayload, msgDecodedPayloadLength)[0], MOTEUR_GAUCHE);
                    PWMSetSpeedConsigne((char)ReadData(msgDecodedPayload, msgDecodedPayloadLength)[1], MOTEUR_DROIT);
                break;

                case LED:
                    LED_CODE = ReadData(msgDecodedPayload, msgDecodedPayloadLength)[0];
                    LED_STATE = ReadData(msgDecodedPayload, msgDecodedPayloadLength)[1];
                    if(LED_CODE == CODE_LED_BLEUE)
                        LED_BLEUE = LED_STATE;
                    else if(LED_CODE == CODE_LED_ORANGE)
                        LED_ORANGE = LED_STATE;
                    else if(LED_CODE == CODE_LED_BLANCHE)
                        LED_BLANCHE = LED_STATE;
                break;
            }
            messageAvailable = 0;
        }
    }
}
        
         /*
 * loopback
        int i ;
        for ( i =0; i< CB_RX1_GetDataSize ( ) ; i++)
        {
            unsigned char c = CB_RX1_Get ( ) ;
            SendMessage(&c , 1 ) ;
        }
         * */
