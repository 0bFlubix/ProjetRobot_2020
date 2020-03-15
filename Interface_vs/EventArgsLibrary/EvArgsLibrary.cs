using System;
using Robot;


namespace EventArgsLibrary
{
    //ExtendedSerialPort: DataReceivedEvent
    public class DataReceivedArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }

    //MessageDecoder: DataDecodedEvent
    public class DataDecodedArgs : EventArgs
    {
        public ushort DecodedFunction { get; set; }
        public ushort DecodedPayloadLength { get; set; }
        public byte[] DecodedPayload { get; set; }
        public byte DecodedCheckSum { get; set; }
        public bool CheckSumErrorOccured { get; set; }
    }

    //MessageProcessor: Processed data is text event
    public class TextDataProcessedArgs : EventArgs
    {
        public string ProcessedText { get; set; }
    }

    //MessageProcessor: Processed data is speed event
    public class SpeedDataProcessedArgs : EventArgs
    {
        public byte SpeedGauche { get; set; }
        public byte SpeedDroit { get; set; }
        public robot.MotorWays WayGauche { get; set; }
        public robot.MotorWays WayDroit { get; set; }
    }

    //MessageProcessor: Processed data is InfaredDistance event
    public class IrDataProcessedArgs : EventArgs
    {
        public byte[] Distance { get; set; }
    }

    //MessageProcessor: processed data ErrorOccured event
    public class CheckSumErrorOccuredArgs : EventArgs {}

    //PortSupervisor: available ports changed event
    public class AvailablePortChangedArgs : EventArgs
    {
        public string[] AvailableSerialPorts { get; set; }
    }
}
