using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net.Http;

/* Herkulex UART params:
Stop Bit : 1
Parity : None
Flow Control : None
Baud Rate : 57,600 / 115,200 / 0.2M / 0.25M / 0.4M / 0.5M / 0.667M

Maximum memory length for any adress in the servo memory is 2 bytes
Minimum packet lengh is 7 bytes
*/


namespace HerkulexController
{
    public class HerkulexController
    {

        //Request a value From memory
        public void EEP_ReadParam(SerialPort port, byte pID, byte memoryAddress)
        {

            byte packetSize = (byte)(9); //reading only one value so fixed length of 9 bytes in the packet (data[0] = addr data[1] = len@addr)
            byte[] data = { memoryAddress, GetMemoryAddrLength(memoryAddress) };

            byte[] packet = new byte[packetSize]; //initializing packet to the right length

            packet[0] = 0xFF; //SOF1
            packet[1] = 0xFF; //SOF2
            packet[2] = packetSize; //packet size
            packet[3] = pID; //servo ID
            packet[4] = (byte)ToServoCommandSet.EEP_READ; //CMD is EEP_READ
            packet[5] = GetChecksum(packetSize, pID, (byte)ToServoCommandSet.EEP_READ, data)[0]; //calc Check Sum1
            packet[6] = GetChecksum(packetSize, pID, (byte)ToServoCommandSet.EEP_READ, data)[1]; //calc Check Sum2
            packet[7] = data[0];
            packet[8] = data[1];

            port.Write(packet, 0, packet.Length); //sending packet with 0 bytes offset
        }

        //writes a value to the specified memory address
        public void EEP_WriteParam(SerialPort port, byte pID, byte memoryAddress, byte[] dataToWrite)
        {
            byte packetSize = (byte)(7 + dataToWrite.Length);

            byte[] packet = new byte[packetSize];

            packet[0] = 0xFF; //SOF1
            packet[1] = 0xFF; //SOF2
            packet[3] = packetSize;
            packet[3] = pID;
            packet[4] = (byte)ToServoCommandSet.EEP_WRITE; //CMD is EEP_WRITE
            packet[5] = (byte)GetChecksum(packetSize, pID, (byte)ToServoCommandSet.EEP_WRITE, dataToWrite)[0]; //calc Check Sum1
            packet[5] = (byte)GetChecksum(packetSize, pID, (byte)ToServoCommandSet.EEP_WRITE, dataToWrite)[1]; //calc Check Sum2
            packet[7] = dataToWrite[0];
            if (packetSize == 9) //if dataToWrite is two bytes long
                packet[8] = dataToWrite[1];

            port.Write(packet, 0, packet.Length); //sending packet with 0 bytes offset
        }



        //all controller commands set
        public enum ToServoCommandSet
        {
            EEP_WRITE = 0x01,
            EEP_READ = 0x02,
            RAM_WRITE = 0x03,
            RAM_READ = 0x04,
            I_JOG = 0x05,
            S_JOG = 0x06,
            STAT = 0x07,
            ROLLBACK = 0x08,
            REBOOT = 0x09
        }

        //all commands ACK set
        public enum ToControllerAckSet
        {
            ack_EEP_WRITE = 0x41,
            ack_EEP_READ = 0x42,
            ack_RAM_WRITE = 0x43,
            ack_RAM_READ = 0x44,
            ack_I_JOG = 0x45,
            ack_S_JOG = 0x46,
            ack_STAT = 0x47,
            ack_ROLLBACK = 0x48,
            ack_REBOOT = 0x49
        }

        //all of the register addrs
        public enum SrvRegAddr
        {
            ID = (byte)(0),                                 //Byte length: 1
            ACK_Policy = (byte)(1),                         //Byte length: 1
            Alarm_LED_Policy = (byte)(2),                   //Byte length: 1
            Torque_policy = (byte)(3),                      //Byte length: 1
            Max_Temperature = (byte)(5),                    //Byte length: 1
            Min_Voltage = (byte)(6),                        //Byte length: 1
            Max_Voltage = (byte)(7),                        //Byte length: 1
            Acceleration_Ratio = (byte)(8),                 //Byte length: 1
            Max_Acceleration = (byte)(9),                   //Byte length: 1
            Dead_Zone = (byte)(10),                         //Byte length: 1
            Saturator_Offset = (byte)(11),                  //Byte length: 1
            Saturator_Slope = (byte)(12),                   //Byte length: 2
            PWM_Offset = (byte)(14),                        //Byte length: 1
            Min_PWM = (byte)(15),                           //Byte length: 1
            Max_PWM = (byte)(16),                           //Byte length: 2
            Overload_PWM_Threshold = (byte)(18),            //Byte length: 2
            Min_Position = (byte)(20),                      //Byte length: 2
            Max_Position = (byte)(22),                      //Byte length: 2
            Position_Kp = (byte)(24),                       //Byte length: 2
            Position_Kd = (byte)(26),                       //Byte length: 2
            Position_Ki = (byte)(28),                       //Byte length: 2
            Pos_FreeFoward_1st_Gain = (byte)(30),           //Byte length: 2
            Pos_FreeFoward_2nd_Gain = (byte)(32),           //Byte length: 2
            LED_Blink_Period = (byte)(38),                  //Byte length: 1
            ADC_Fault_Detect_Period = (byte)(39),           //Byte length: 1
            Packet_Garbage_Detection_Period = (byte)(40),   //Byte length: 1
            Stop_Detection_Period = (byte)(41),             //Byte length: 1
            Overload_Detection_Period = (byte)(42),         //Byte length: 1
            Stop_Threshold = (byte)(41),                    //Byte length: 1
            Inposition_Margin = (byte)(44),                 //Byte length: 1
            Calibration_Difference = (byte)(47),            //Byte length: 1
            Status_Error = (byte)(48),                      //Byte length: 1
            Status_Detail = (byte)(49),                     //Byte length: 1
            Torque_Control = (byte)(52),                    //Byte length: 1
            LED_Control = (byte)(53),                       //Byte length: 1
            Voltage = (byte)(54),                           //Byte length: 2
            Temperature = (byte)(55),                       //Byte length: 2
            Current_Control_Mode = (byte)(56),              //Byte length: 2
            Tick = (byte)(57),                              //Byte length: 2
            Calibrated_Position = (byte)(58),               //Byte length: 2
            Absolute_Position = (byte)(60),                 //Byte length: 2
            Differential_Position = (byte)(62),             //Byte length: 2
            PWM = (byte)(64),                               //Byte length: 2
            Absolute_Goal_Position = (byte)(68),            //Byte length: 2
            Absolute_Desired_Traject_Pos = (byte)(70),      //Byte length: 2
            Desired_Velocity = (byte)(72)                   //Byte length: 1
        }


        //all of the two bytes length only addresses (use in GetMemoryAddrLength)
        private enum TwoBytesCMD
        {
            Saturator_Slope = (byte)(12),                   //Byte length: 2
            Max_PWM = (byte)(16),                           //Byte length: 2
            Overload_PWM_Threshold = (byte)(18),            //Byte length: 2
            Min_Position = (byte)(20),                      //Byte length: 2
            Max_Position = (byte)(22),                      //Byte length: 2
            Position_Kp = (byte)(24),                       //Byte length: 2
            Position_Kd = (byte)(26),                       //Byte length: 2
            Position_Ki = (byte)(28),                       //Byte length: 2
            Pos_FreeFoward_1st_Gain = (byte)(30),           //Byte length: 2
            Pos_FreeFoward_2nd_Gain = (byte)(32),           //Byte length: 2
            Voltage = (byte)(54),                           //Byte length: 2
            Temperature = (byte)(55),                       //Byte length: 2
            Current_Control_Mode = (byte)(56),              //Byte length: 2
            Tick = (byte)(57),                              //Byte length: 2
            Calibrated_Position = (byte)(58),               //Byte length: 2
            Absolute_Position = (byte)(60),                 //Byte length: 2
            Differential_Position = (byte)(62),             //Byte length: 2
            PWM = (byte)(64),                               //Byte length: 2
            Absolute_Goal_Position = (byte)(68),            //Byte length: 2
            Absolute_Desired_Traject_Pos = (byte)(70),      //Byte length: 2
        }

        //checksums calculation
        byte[] GetChecksum(byte packetSize, byte pID, byte CMD, byte[] data)
        {
            //GetCheckSum()[0] returns chckSum1
            //GetCheckSum()[1] returns chckSum2
            byte[] checkSum = new byte[2];
            checkSum[0] = (byte)(packetSize ^ pID ^ CMD);

            for (int i = 0; i < data.Length; i++)
                checkSum[0] ^= data[i];

            checkSum[0] &= 0xFE;
            checkSum[1] = (byte)(~checkSum[0]);

            return checkSum;
        }

        //returns the memory length at the corresponing address
        public byte GetMemoryAddrLength(byte ADDR)
        {
            foreach (TwoBytesCMD cmd in Enum.GetValues(typeof(TwoBytesCMD)))
            {
                if (ADDR == (byte)cmd)
                {
                    return 0x02; //if the address belongs to the two bytes length set, return 2, exit func
                }
            }

            return 0x01; //if the address does not belongs to the two bytes length set, return 1, exit func
        }

        /*
public enum RegAddrMemlen
{
    ID_Len = (byte)(1),                                //Byte length: 1
    ACK_Policy_Len = (byte)(1),                        //Byte length: 1
    Alarm_LED_Policy_Len = (byte)(1),                  //Byte length: 1
    Torque_policy_Len = (byte)(1),                     //Byte length: 1
    Max_Temperature_Len = (byte)(1),                   //Byte length: 1
    Min_Voltage_Len = (byte)(1),                       //Byte length: 1
    Max_Voltage_Len = (byte)(1),                       //Byte length: 1
    Acceleration_Ratio_Len = (byte)(1),                //Byte length: 1
    Max_Acceleration_Len = (byte)(1),                  //Byte length: 1
    Dead_Zone_Len = (byte)(1),                         //Byte length: 1
    Saturator_Offset_Len = (byte)(1),                  //Byte length: 1
    Saturator_Slope_Len = (byte)(2),                   //Byte length: 2
    PWM_Offset_Len = (byte)(1),                        //Byte length: 1
    Min_PWM_Len = (byte)(1),                           //Byte length: 1
    Max_PWM_Len = (byte)(2),                           //Byte length: 2
    Overload_PWM_Threshold_Len = (byte)(2),            //Byte length: 2
    Min_Position_Len = (byte)(2),                      //Byte length: 2
    Max_Position_Len = (byte)(2),                      //Byte length: 2
    Position_Kp_Len = (byte)(2),                       //Byte length: 2
    Position_Kd_Len = (byte)(2),                       //Byte length: 2
    Position_Ki_Len = (byte)(2),                       //Byte length: 2
    Pos_FreeFoward_1st_Gain_Len = (byte)(2),           //Byte length: 2
    Pos_FreeFoward_2nd_Gain_Len = (byte)(2),           //Byte length: 2
    LED_Blink_Period_Len = (byte)(1),                  //Byte length: 1
    ADC_Fault_Detect_Period_Len = (byte)(1),           //Byte length: 1
    Packet_Garbage_Detection_Period_Len = (byte)(1),   //Byte length: 1
    Stop_Detection_Period_Len = (byte)(1),             //Byte length: 1
    Overload_Detection_Period_Len = (byte)(1),         //Byte length: 1
    Stop_Threshold_Len = (byte)(1),                    //Byte length: 1
    Inposition_Margin_Len = (byte)(1),                 //Byte length: 1
    Calibration_Difference_Len = (byte)(1),            //Byte length: 1
    Status_Error_Len = (byte)(1),                      //Byte length: 1
    Status_Detail_Len = (byte)(1),                     //Byte length: 1
    Torque_Control_Len = (byte)(1),                    //Byte length: 1
    LED_Control_Len = (byte)(1),                       //Byte length: 1
    Voltage_Len = (byte)(2),                           //Byte length: 2
    Temperature_Len = (byte)(2),                       //Byte length: 2
    Current_Control_Mode_Len = (byte)(2),              //Byte length: 2
    Tick_Len = (byte)(2),                              //Byte length: 2
    Calibrated_Position_Len = (byte)(2),               //Byte length: 2
    Absolute_Position_Len = (byte)(2),                 //Byte length: 2
    Differential_Position_Len = (byte)(2),             //Byte length: 2
    PWM_Len = (byte)(2),                               //Byte length: 2
    Absolute_Goal_Position_Len = (byte)(2),            //Byte length: 2
    Absolute_Desired_Traject_Pos_Len = (byte)(2),      //Byte length: 2
    Desired_Velocity_Len = (byte)(1)                   //Byte length: 1
}     //all of the memory lengths at X address
*/ //could be handy...
    }
}