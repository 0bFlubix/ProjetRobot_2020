using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventArgsLibrary;

namespace HerkulexReceptManager
{
    public class herkulexRecept
    {

        receptionStates rcvState = receptionStates.Waiting;

        public byte packetSize;
        public byte pID;
        public byte cmd;
        public byte checkSum1;
        public byte checkSum2;
        public byte[] packetData;

        public byte packetDataByteIndex = 0;

        //recept states enum
        public enum receptionStates
        {
            Waiting,
            sof1,
            sof2,
            packetSize,
            pID,
            CMD,
            checkSum1,
            checkSum2,
            data
        }

        //DataReceived input event, decoding the packet with a state machine for each received bytes
        public void HerkulexDecodeIncommingPacket(object sender, DataReceivedArgs e)
        {
            foreach(byte b in e.Data)
            {
                
                //state machine
                switch (rcvState)
                {
                    case receptionStates.Waiting:
                        if (b == 0xFF)
                            rcvState = receptionStates.sof2;
                        break;

                    case receptionStates.sof2:
                        if (b == 0xFF)
                            rcvState = receptionStates.packetSize;
                        break;

                    case receptionStates.packetSize:
                        packetSize = b;
                        packetData = new byte[packetSize - 7]; //init to the data size
                        rcvState = receptionStates.pID;
                        break;

                    case receptionStates.pID:
                        pID = b;
                        rcvState = receptionStates.CMD;
                        break;

                    case receptionStates.CMD:
                        cmd = b;
                        rcvState = receptionStates.checkSum1;
                        break;

                    case receptionStates.checkSum1:
                        checkSum1 = b;
                        rcvState = receptionStates.checkSum2;
                        break;

                    case receptionStates.checkSum2:
                        checkSum2 = b;
                        rcvState = receptionStates.data;
                        break;

                    case receptionStates.data:
                        if(packetDataByteIndex < packetSize)
                        {
                            packetData[packetDataByteIndex] = b;
                            packetDataByteIndex++;
                        }

                        packetDataByteIndex = 0;
                        OnDataDecoded(packetSize, pID, cmd, checkSum1, checkSum2, packetData); //fire the decoded data event
                        rcvState = receptionStates.Waiting; //back to waiting
                        break;
                }
            }
        }

        //declare new output event: HerkulexIncommingPacketDecodedArgs
        public event EventHandler<HerkulexIncommingPacketDecodedArgs> OnHerkulexIncommingMessageDecodedEvent;

        public virtual void OnDataDecoded(byte packetSize, byte pID,byte cmd, byte checkSum1, byte checkSum2, byte[] packetData)
        {
            var handler = OnHerkulexIncommingMessageDecodedEvent;
            if (handler != null)
            {
                handler(this, new HerkulexIncommingPacketDecodedArgs
                {
                    PacketSize = packetSize,
                    PID = pID,
                    CMD = cmd,
                    CheckSum1 = checkSum1,
                    CheckSum2 = checkSum2,
                    PacketData = packetData
                });
            }
        }
    }
}
