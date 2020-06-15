#ifndef UART_H
#define UART_H

#define CMD_SPEED 0x40
#define CMD_TEXT 0x80
#define CMD_LED 0x20
#define CMD_IR 0x30 //old
#define CMD_POSITION_DATA 0x0061

#define CMD_ANGULAR_SPEED_CONSIGNE 0x0088
#define CMD_LINEAR_SPEED_CONSIGNE 0x00AA

#define Waiting 0
#define FunctionMSB 1
#define FunctionLSB 2
#define PayloadLengthMSB 3
#define PayloadLengthLSB 4
#define Payload 5
#define CheckSum 6

void InitUART(void);
void SendMessageDirect(unsigned char* message, int length);//WORKING
void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, unsigned char *  msgPayload);//not tested
void UartSendSpeedInfo(char speedGauche, char speedDroit);//WORKING, using signed binary to distinguish between ways

#endif