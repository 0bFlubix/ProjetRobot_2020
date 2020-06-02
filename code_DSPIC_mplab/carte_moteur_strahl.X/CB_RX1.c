#include <xc.h>
#include <stdio.h>
#include <stdlib.h>
#include "CB_RX1.h"
#include "IO.h"


#define CBRX1_BUFFER_SIZE 128

 

int cbRx1Head = 0;
int cbRx1Tail = 0;
unsigned char cbRx1Buffer[CBRX1_BUFFER_SIZE];

//add a byte to the FIFO
void CB_RX1_Add(unsigned char value)
{
    if(CB_RX1_GetRemainingSize()>0)
    {
      cbRx1Buffer[cbRx1Head] = value;
      cbRx1Head++;
      
      if(cbRx1Head >= CBRX1_BUFFER_SIZE)
          cbRx1Head = 0;
    }
}

//read and returns a byte from FIFO
unsigned char CB_RX1_Get(void)
{
    unsigned char value = cbRx1Buffer[cbRx1Tail];
    cbRx1Tail++;
    
    if(cbRx1Tail >= CBRX1_BUFFER_SIZE)
        cbRx1Tail = 0;

    return value;
}

//returns DataAvailable flag
unsigned char CB_RX1_IsDataAvailable(void)
{
    if(cbRx1Head != cbRx1Tail)
        return 1;
    else
        return 0;
}

//UART byte received interrupt
void __attribute__((interrupt, no_auto_psv)) _U1RXInterrupt(void) {
    
    IFS0bits.U1RXIF = 0; // clear RX interrupt flag
    /* check for receive errors */
    if (U1STAbits.FERR == 1)
        U1STAbits.FERR = 0;

    /* must clear the overrun error to keep uart receiving */
    if (U1STAbits.OERR == 1) 
        U1STAbits.OERR = 0;

    /* get the data */
    while(U1STAbits.URXDA == 1) 
      CB_RX1_Add(U1RXREG); 
}

//returns the number of free bytes left remaining in the FIFO, refer to CBRX1_BUFFER_SIZE to edit the maximum size of the FIFO
int CB_RX1_GetRemainingSize(void)
{
    int rSizeRecep;
    rSizeRecep = CBRX1_BUFFER_SIZE - CB_RX1_GetDataSize();
    return rSizeRecep;
}

//returns the number of recived bytes left in the FIFO
int CB_RX1_GetDataSize(void)
{
    int rSizeRecep;
    if(cbRx1Tail > cbRx1Head)
        rSizeRecep = CBRX1_BUFFER_SIZE - cbRx1Tail + cbRx1Head;
    else
        rSizeRecep = cbRx1Head - cbRx1Tail;
    return rSizeRecep;
}

