#ifndef UART_H
#define UART_H

void InitUART(void);
void SendMessageDirect(unsigned char* message, int length);
void SendMessageDirect(unsigned char* message, int length);
void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, unsigned char *  msgPayload);
void UartSendSpeedInfo(char speedGauche, char speedDroit);//using signed binary to distinguish between ways

#endif