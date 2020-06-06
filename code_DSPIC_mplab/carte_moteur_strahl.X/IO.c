/*
 * File:   IO.c
 */

#include <xc.h>
#include "IO.h"

void InitIO()
{
    // IMPORTANT : désactiver les entrées analogiques, sinon on perd les entrées numériques
    ANSELA = 0; // 0 desactive
    ANSELB = 0;
    ANSELC = 0;
    ANSELD = 0;
    ANSELE = 0;
    ANSELF = 0;
    ANSELG = 0;

    //********** Configuration des sorties : _TRISxx = 0 ********************************
    // LED
    _TRISC10 = 0;  // LED Orange
    _TRISG6 = 0; //LED Blanche
    _TRISG7 = 0; // LED Bleue
    _TRISA0 = 1;
    
    // Moteurs 
    _TRISB14 = 0;
    _TRISB15 = 0;
    _TRISC6 = 0;
    _TRISC7 = 0;
    
    //********** Configuration des entrées : _TRISxx = 1 ********************************   
    // ADC
    _TRISB1 = 1;
    _TRISC0 = 1;
    _TRISC11 = 1;
    _TRISG9 = 1;
    _TRISE15 = 1;    
    //**************config I/O UART
    _U1RXR = 0b011000; //remappe l'entrée UART1_RX sur RPI24 [datasheet P.169]
    _RP36R = 0b000001; //U1TX [000001] RP36 tied to UART1 Transmit
    
    //*********** QEI
    _QEA2R = 97;    //assign QUEI A to pin RP97
    _QEB2R = 96;    //assign QEI B to pin RP96
    
    _QEA1R = 70;    //asign QEI A to pin RP70
    _QEB1R = 69;    //assign QEU B to pin RP69
}

