using System;
using System.Security.Cryptography;
using System.Text;
using EventArgsLibrary;
using Robot;
using Utilities;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// this class is used to process the previously decoded message and output events
/// depending on the message for a simpler, higer level use.
/// </summary>

namespace MessageProcessor
{
    public class msgProcessor
    {

        //contains the processor state machine
        public void ProcessMessage(object Sender, DataDecodedArgs e)
        {
            ushort function = e.DecodedFunction;
            byte[] irDistance = new byte[5];

            robot.MotorWays wayGauche;
            robot.MotorWays wayDroit;

            sbyte speedGauche;
            sbyte speedDroit;

            if (e.CheckSumErrorOccured)
                OnCheckSumErrorOccured();
            else 
            { 
                switch(function)
                {
                    case 0x0080: //is TextMessage
                        OnTextMessageProcessed(Encoding.UTF8.GetString(e.DecodedPayload));
                        break;

                    case 0x0030: //is IrMessage
                        for (int i = 0; i < e.DecodedPayload.Length; i++)
                            irDistance[i] = e.DecodedPayload[i];

                        OnIrMessageProcessed(irDistance);
                        break;

                    case 0x0040: //is SpeedMessage
                        speedGauche = (sbyte)e.DecodedPayload[0];
                        speedDroit = (sbyte)e.DecodedPayload[1];

                        if (speedGauche >= 0) //if 0, saying foward...
                            wayGauche = robot.MotorWays.Avance;
                        else                                   //first bit is a 0, going reverse
                            wayGauche = robot.MotorWays.Recule;

                        if(speedDroit >= 0)   //first bit is a 1, going foward
                            wayDroit = robot.MotorWays.Avance;
                        else                                   //first bit is a 0, going reverse
                            wayDroit = robot.MotorWays.Recule;

                        OnSpeedMessageProcessed(speedGauche, speedDroit, wayGauche, wayDroit);
                        break;

                    case 0x0061: //is position data [24 bytes long]
                        if(e.DecodedPayloadLength == 24) //if it's not, invalid, drop packet
                        {
                            byte[] buffer = new byte[4] { e.DecodedPayload[0], e.DecodedPayload[1], e.DecodedPayload[2], e.DecodedPayload[3] };
                            ulong timestamp = Extensions.GetUlong(buffer); ;

                            buffer = new byte[4] { e.DecodedPayload[4], e.DecodedPayload[5], e.DecodedPayload[6], e.DecodedPayload[7] };
                            float xPositionFromOdometry = Extensions.GetFloat(buffer);

                            buffer = new byte[4] { e.DecodedPayload[8], e.DecodedPayload[9], e.DecodedPayload[10], e.DecodedPayload[11] };
                            float yPositionFromOdometry = Extensions.GetFloat(buffer);
                            
                            buffer = new byte[4] { e.DecodedPayload[12], e.DecodedPayload[13], e.DecodedPayload[14], e.DecodedPayload[15] };
                            float angleRadianFromOdometry = Extensions.GetFloat(buffer);

                            buffer = new byte[4] { e.DecodedPayload[16], e.DecodedPayload[17], e.DecodedPayload[18], e.DecodedPayload[19] };
                            float vitesseLineaireFromOdometry = Extensions.GetFloat(buffer);

                            buffer = new byte[4] { e.DecodedPayload[20], e.DecodedPayload[21], e.DecodedPayload[22], e.DecodedPayload[23] };
                            float vitesseAngulaireFromOdometry = Extensions.GetFloat(buffer);

                            OnPositionDataProcessed(timestamp, xPositionFromOdometry, yPositionFromOdometry, angleRadianFromOdometry,
                                                    vitesseLineaireFromOdometry, vitesseAngulaireFromOdometry);
                        }
                        break;
                }
            }

        }


        public event EventHandler<TextDataProcessedArgs> OnTextMessageProcessedEvent;
        public event EventHandler<IrDataProcessedArgs> OnIrMessageProcessedEvent;
        public event EventHandler<SpeedDataProcessedArgs> OnSpeedMessageProcessedEvent;
        public event EventHandler<CheckSumErrorOccuredArgs> OnCheckSumErrorOccuredEvent;
        public event EventHandler<PositionDataProcessedArgs> OnPositionDataProcessedEvent;

        public virtual void OnPositionDataProcessed(ulong timestamp, float xPositionFromOdometry, float yPositionFromOdometry, float angleRadianFromOdometry,
                                                    float vitesseLineaireFromOdometry, float vitesseAngulaireFromOdometry)
        {
            var handler = OnPositionDataProcessedEvent;
            if(handler != null)
            {
                handler(this, new PositionDataProcessedArgs
                {
                    Timestamp = timestamp,
                    XPositionFromOdometry = xPositionFromOdometry,
                    YPositionFromOdometry = yPositionFromOdometry,
                    AngleRadianFromOdometry = angleRadianFromOdometry,
                    VitesseLineaireFromOdometry = vitesseLineaireFromOdometry,
                    VitesseAngulaireFromOdometry = vitesseAngulaireFromOdometry
                });
            }
        }

        public virtual void OnCheckSumErrorOccured()
        {
            var handler = OnCheckSumErrorOccuredEvent;
            if(handler != null)
            {
                handler(this, new CheckSumErrorOccuredArgs{});
            }
        }
        public virtual void OnIrMessageProcessed(byte[] distance)
        {
            var handler = OnIrMessageProcessedEvent;
            if(handler != null)
            {
                handler(this, new IrDataProcessedArgs
                {
                    Distance = distance
                });
            }
        }
        public virtual void OnTextMessageProcessed(string text)
        {
            var handler = OnTextMessageProcessedEvent;
            if(handler != null)
            {
                handler(this, new TextDataProcessedArgs
                {
                    ProcessedText = text
                });
            }
        }
        public virtual void OnSpeedMessageProcessed(sbyte speedGauche, sbyte speedDroit, robot.MotorWays wayGauche, robot.MotorWays wayDroit)
        {
            var handler = OnSpeedMessageProcessedEvent;
            if(handler != null)
            {
                handler(this, new SpeedDataProcessedArgs
                {
                    SpeedGauche = speedGauche,
                    SpeedDroit = speedDroit,
                    WayGauche = wayGauche,
                    WayDroit = wayDroit
                });
            }
        }
    }
}
