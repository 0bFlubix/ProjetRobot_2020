using System;
using System.Text;
using EventArgsLibrary;
using Robot;

namespace MessageProcessor
{
    public class msgProcessor
    { 
        public void ProcessMessage(object Sender, DataDecodedArgs e)
        {
            ushort function = e.DecodedFunction;
            byte[] irDistance = new byte[5];

            robot.MotorWays wayGauche;
            robot.MotorWays wayDroit;

            byte speedGauche;
            byte speedDroit;

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
                        speedGauche = e.DecodedPayload[1];
                        speedDroit = e.DecodedPayload[0];

                        if ((byte)(speedGauche & 0b10000000) == 128) //first bit is a 1, going foward
                            wayGauche = robot.MotorWays.Avance;
                        else                                   //first bit is a 0, going reverse
                            wayGauche = robot.MotorWays.Recule;

                        if((byte)(speedDroit & 0b10000000) == 128)   //first bit is a 1, going foward
                            wayDroit = robot.MotorWays.Avance;
                        else                                   //first bit is a 0, going reverse
                            wayDroit = robot.MotorWays.Recule;

                        speedDroit &= 0x7F; //set the first bit to 0
                        speedGauche &= 0x7F;//set the first bit to 0

                        OnSpeedMessageProcessed(e.DecodedPayload[1], e.DecodedPayload[0],
                                                wayGauche, wayDroit);


                        break;
                }
            }

        }


        public event EventHandler<TextDataProcessedArgs> OnTextMessageProcessedEvent;
        public event EventHandler<IrDataProcessedArgs> OnIrMessageProcessedEvent;
        public event EventHandler<SpeedDataProcessedArgs> OnSpeedMessageProcessedEvent;
        public event EventHandler<CheckSumErrorOccuredArgs> OnCheckSumErrorOccuredEvent;

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
        public virtual void OnSpeedMessageProcessed(byte speedGauche, byte speedDroit, robot.MotorWays wayGauche, robot.MotorWays wayDroit)
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
