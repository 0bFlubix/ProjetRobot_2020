using System.IO.Ports;
using Robot;

/// <summary>
/// This class is used to encode and send commands to the robot
/// </summary>

namespace MessageEncoder
{
    public class Encoder
    {

        //sends a speed command to the robot see comments for further info
        public void UartSendSpeedCommand(SerialPort port, sbyte speedGauche, sbyte speedDroit)
        {
            // the function is taking signed bytes as input and casts them to unsigned bytes to allow easy manipulation
            // at the reception level, a cast to a signed char could be nescessary as the values would be inexact
            byte[] msgPayload = { (byte)speedGauche, (byte)speedDroit };
            UartEncodeAndSendMessage(port, 0x0040, 2, msgPayload);
        }

        public void UartSendLedCommand(SerialPort port,robot.LED led, byte state)
        {
            byte[] msgPayload = { (byte)led, (byte)state };
            UartEncodeAndSendMessage(port, 0x0020, 2, msgPayload);
        }

        //sends encoded UART frames
        public void UartEncodeAndSendMessage(SerialPort port, int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte[] msgToSend = new byte[msgPayloadLength + 6];

            msgToSend[0] = 0xFE;    //SOF = 0xFE
            msgToSend[1] = (byte)(msgFunction >> 8);
            msgToSend[2] = (byte)(msgFunction);
            msgToSend[3] = (byte)(msgPayloadLength >> 8);
            msgToSend[4] = (byte)(msgPayloadLength);

            for (int i = 0; i < msgPayloadLength; i++)  //adds payload to the msgTYoSend Bytelist from byte 5 to msgPayloadLength
                msgToSend[i + 5] = msgPayload[i];

            msgToSend[5 + msgPayloadLength] = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload); //adds checkSum value to the EOF
            port.Write(msgToSend, 0, msgToSend.Length);
        }

        //calculates the UART frame checksum
        public byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte checksum = 0x00;

            checksum ^= (byte)(0xFE ^ (byte)msgFunction ^ (byte)(msgFunction >> 8));

            for (int i = 0; i < msgPayloadLength; i++)
                checksum ^= msgPayload[i];

            return checksum;
        }
    }

}
