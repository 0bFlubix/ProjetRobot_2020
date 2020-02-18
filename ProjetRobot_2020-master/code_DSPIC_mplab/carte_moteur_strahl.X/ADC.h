/* 
 * File:   ADC.h
 * Author: TP-EO-5
 *
 * Created on 12 février 2020, 14:33
 */

#ifndef ADC_H
#define	ADC_H

void InitADC1(void);
void ADC1StartConversionSequence();
unsigned int * ADCGetResult(void);
unsigned char ADCIsConversionFinished(void);
void ADCClearConversionFinishedFlag(void);


#endif	/* ADC_H */

