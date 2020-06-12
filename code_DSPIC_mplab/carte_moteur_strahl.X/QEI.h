#ifndef QEI_H
#define	QEI_H

#define DISTROUES 281.2
#define FREQ_ECH_QEI 250 //Hz, affects robot acceleration via PWMUpdateSpeed() in timer1

void SendPositionData(void); //sends odometry data via uart(24bytes payload)
void QEIUpdateData(void); //update odometry data
void UartAckAnglSpeedConsigne(void);
void UartAckLinSpeedConsigne(void);

void InitQEI1(void); //init QEI1
void InitQEI2(void); //init QEI2

#endif