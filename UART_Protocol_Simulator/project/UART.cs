using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace UART_Protocol_Simulator
{
    class UART
    {

        public byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte checksum = 0x00;

            checksum ^= (byte)(0xFE ^ (byte)msgFunction ^ (byte)(msgFunction >> 8));

            for (int i = 0; i < msgPayloadLength; i++)
                checksum ^= msgPayload[i];

            return checksum;
        }

        //sends encoded UART frames
        public void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload, SerialPort port)
        {
            byte[] msgToSend = new byte[msgPayloadLength + 6];

            msgToSend[0] = 0xFE;    //SOF = 0xFE
            msgToSend[1] = (byte)msgFunction;
            msgToSend[2] = (byte)(msgFunction >> 8);
            msgToSend[3] = (byte)msgPayloadLength;
            msgToSend[4] = (byte)(msgPayloadLength >> 8);

            for (int i = 0; i < msgPayloadLength; i++)  //adds payload to the msgTYoSend Bytelist from byte 5 to msgPayloadLength
                msgToSend[i + 5] = msgPayload[i];

            msgToSend[5 + msgPayloadLength] = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload); //adds checkSum value to the EOF
            port.Write(msgToSend, 0, msgToSend.Length);
        }
    }
}
