#include <xc.h>
#include "UART.h"
#include "ChipConfig.h"
#include "CB_TX1.h"
#include "CB_RX1.h"

#define BAUDRATE 115200
#define BRGVAL ((FCY/BAUDRATE)/4)-1

void InitUART(void)
{
    U1MODEbits.STSEL = 0; // 1-stop bit
    U1MODEbits.PDSEL = 0; // No Parity, 8-data bits
    U1MODEbits.ABAUD = 0; // Auto-Baud Disabled
    U1MODEbits.BRGH = 1; // Low Speed mode
    U1BRG = BRGVAL; // BAUD Rate Setting

    U1STAbits.UTXISEL0 = 0; // Interrupt after one Tx character is transmitted
    U1STAbits.UTXISEL1 = 0;
    IFS0bits.U1TXIF = 0; // clear TX interrupt flag
    IEC0bits.U1TXIE = 1; // Disable UART Tx interrupt

    U1STAbits.URXISEL = 0; // Interrupt after one RX character is received;
    IFS0bits.U1RXIF = 0; // clear RX interrupt flag
    IEC0bits.U1RXIE = 1; // Disable UART Rx interrupt

    U1MODEbits.UARTEN = 1; // Enable UART
    U1STAbits.UTXEN = 1; // Enable UART Tx
}
    
//Sends a UART message (freezes the program)
void SendMessageDirect(unsigned char* message, int length)
{
    unsigned char i;
    for(i = 0; i < length; i++)
    {
        while(U1STAbits.UTXBF); //wait while Tx buffer full
        U1TXREG = *(message)++;//Transmit one character
    }
}

unsigned char CalculateChecksum(int msgFunction, int msgPayloadLength, unsigned char * msgPayload)
{
    unsigned char checksum = 0;
    
    checksum ^= 0xFE ^ (unsigned char)(msgFunction >> 0) ^ (unsigned char)(msgFunction >> 8);
    
    int i;
    for(i = 0; i < msgPayloadLength; i++)
        checksum ^= msgPayload[i];
   
    return checksum;
}

void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, unsigned char *  msgPayload)
{
    unsigned char msgToSend[msgPayloadLength + 6];
    msgToSend[0] = 0xFE;    //SOF = 0xFE
    msgToSend[1] = (unsigned char)(msgFunction >> 8);
    msgToSend[2] = (unsigned char)msgFunction;
    msgToSend[3] = (unsigned char)(msgPayloadLength >> 8);
    msgToSend[4] = (unsigned char)msgPayloadLength;
    
    int i;
    for (i = 0; i < msgPayloadLength; i++)  //adds payload to the msgTYoSend Bytelist from byte 5 to msgPayloadLength
        msgToSend[i + 5] = msgPayload[i];

    msgToSend[5 + msgPayloadLength] = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload); //adds checkSum value to the EOF
    SendMessage(msgToSend, sizeof(msgToSend));
}

void UartSendSpeedInfo(char speedGauche, char speedDroit)//using signed binary to distinguish between ways
{
    unsigned char speedData[2] = { speedGauche, speedDroit };
    UartEncodeAndSendMessage(0x0040, 2, speedData);
}
