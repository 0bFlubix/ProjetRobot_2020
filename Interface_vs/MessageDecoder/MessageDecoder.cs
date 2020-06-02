using System;
using System.Collections.Generic;
using System.IO.Ports;
using EventArgsLibrary;
using MessageEncoder;

/// <summary>
/// This class is used to decode raw incomming messages from the robot
/// and extracts all of the packet info, the output event also returns a CheckSumErrorOccured flag.
/// to function properly the library MessageEncoder is needed which provides the CalculateCheckSum method.
/// Output event name: OnDataDecoded (DataDecodedArgs)
/// </summary>

namespace MessageDecoder
{
    public class msgDecoder
    {
        Encoder mesEncoder = new Encoder(); //init a new encoder to calc checksums
        
        StateReception rcvState = StateReception.Waiting;

        //shared message params
        ushort msgDecodedFunction = 0;
        ushort msgDecodedPayloadLength = 0;
        byte[] msgDecodedPayload;
        byte receivedCheckSum = 0x00;
        bool CheckSumErrorOccured = false;

        byte calculatedCheckSum = 0x00;
        ushort msgDecodedPayloadIndex = 0;

        //messageAvailable shared var
        public bool messageAvailable = false;

        //enum of reception states
        public enum StateReception
        {
            Waiting,
            FunctionMSB,
            FunctionLSB,
            PayloadLengthMSB,
            PayloadLengthLSB,
            Payload,
            CheckSum
        }

        //DecodeMessage input event
        public void DecodeMessage(object sender, DataReceivedArgs e)
        {
            foreach (byte c in e.Data)
            {
                if (rcvState == StateReception.Waiting)
                    messageAvailable = false;

                switch (rcvState)
                {
                    case StateReception.Waiting:
                        if (c == 0xFE)
                            rcvState = StateReception.FunctionMSB;
                        break;

                    case StateReception.FunctionMSB:
                        msgDecodedFunction = (ushort)(c << 8);
                        rcvState = StateReception.FunctionLSB;
                        break;

                    case StateReception.FunctionLSB:
                        msgDecodedFunction += (ushort)(c << 0);
                        rcvState = StateReception.PayloadLengthMSB;
                        break;

                    case StateReception.PayloadLengthMSB:
                        msgDecodedPayloadLength = (ushort)(c << 8);
                        rcvState = StateReception.PayloadLengthLSB;
                        break;

                    case StateReception.PayloadLengthLSB:
                        msgDecodedPayloadLength += (ushort)(c << 0);

                        if (msgDecodedPayloadLength > 0)
                        {
                            msgDecodedPayloadIndex = 0;
                            msgDecodedPayload = new byte[msgDecodedPayloadLength];
                            rcvState = StateReception.Payload;
                        }
                        else
                            rcvState = StateReception.CheckSum; //if no payload, skip to CheckSum state

                        break;

                    case StateReception.Payload:

                        if (msgDecodedPayloadIndex < msgDecodedPayloadLength)
                        {
                            msgDecodedPayload[msgDecodedPayloadIndex] = c;
                            msgDecodedPayloadIndex++;
                            if (msgDecodedPayloadIndex == msgDecodedPayloadLength)
                                rcvState = StateReception.CheckSum;
                        }

                        break;

                    case StateReception.CheckSum:
                        receivedCheckSum = c;
                        calculatedCheckSum = mesEncoder.CalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                        messageAvailable = true;
                        if (calculatedCheckSum == receivedCheckSum)
                        {
                            //Console.WriteLine("checksum ok received: " + receivedCheckSum.ToString("X2") +
                            //                  " calculated: " + calculatedCheckSum.ToString("X2"));
                            CheckSumErrorOccured = false;
                        }
                        else
                        {
                            // Console.WriteLine("checksum error received: " + receivedCheckSum.ToString("X2") +
                            //                   " calculated: " + calculatedCheckSum.ToString("X2"));
                            CheckSumErrorOccured = true;
                        }

                        OnDataDecoded(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload, receivedCheckSum, CheckSumErrorOccured);

                        rcvState = StateReception.Waiting;

                        break;

                    default:
                        rcvState = StateReception.Waiting;
                        break;
                }
            }
        }

        public event EventHandler<DataDecodedArgs> OnDataDecodedEvent;

        public virtual void OnDataDecoded(ushort msgDecodedFunction, ushort msgDecodedPayloadLength,
                                          byte[] msgDecodedPayload, byte rcvCheckSum, bool chksmError)
        {
            var handler = OnDataDecodedEvent;
            if(handler != null)
            {
                handler(this, new DataDecodedArgs
                {
                    DecodedFunction = msgDecodedFunction,
                    DecodedPayloadLength = msgDecodedPayloadLength,
                    DecodedPayload = msgDecodedPayload,
                    DecodedCheckSum = rcvCheckSum,
                    CheckSumErrorOccured = chksmError
                });
            }
        }
    }
}
