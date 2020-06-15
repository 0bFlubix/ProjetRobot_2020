using System.Windows;
using HerkulexReceptManager;
using ExtendedSerialPort;
using DataRedirector;
using System.IO.Ports;
using System;
using HerkulexController;
using System.Windows.Documents;

namespace GUI
{

    public partial class MainWindow : Window
    {

        ServoController ServoController = new ServoController();


        ReliableSerialPort Comport = new ReliableSerialPort("COM7", 9600, Parity.None, 8, StopBits.One);

        public MainWindow()
        {
            InitializeComponent();

            Comport.DataReceived += Comport_DataReceived;

            Comport.Open();

            
            
        }

        private void Comport_DataReceived(object sender, EventArgsLibrary.DataReceivedArgs e)
        {
            //breakHere
        }

        private void ReceptionController_OnHerkulexIncommingMessageDecodedEvent(object sender, EventArgsLibrary.HerkulexIncommingPacketDecodedArgs e)
        {
            //add breakpoint here
        }

        private void Button_EEPREAD_Click(object sender, RoutedEventArgs e)
        {
            //send packet to redirector, Comport not used but nescessary avoiding error
            //ServoControl.EEP_ReadParam(Comport, 0x1A, (byte)ServoController.SrvRegAddr.Min_Voltage);
        }

        private void Button_EEPWRITE_Click(object sender, RoutedEventArgs e)
        {
            byte[] dataToWrite = { 0x5B, 0x22, 0x44, 0xFF, 0x10, 0x98, 0x12 };
            //ServoControl.EEP_WriteParam(Comport, 0x1A, (byte)ServoController.SrvRegAddr.Min_Voltage, dataToWrite); // 6.714DCV as min allowed voltage
        }

        private void Button_DebugSend_Click(object sender, RoutedEventArgs e)
        {
            //Config new tags
            ServoController.IJOG_TAG tagServo1 = new ServoController.IJOG_TAG();
            tagServo1.ID = 0xFD; //broadcast ID
            tagServo1.mode = ServoController.JOG_MODE.positionControlJOG;
            tagServo1.playTime = 0x3C;
            tagServo1.PositionGoal = 0x0002;
            tagServo1.LED_BLUE_ON = false;
            tagServo1.LED_GREEN_ON = true;
            tagServo1.LED_RED_ON = false;

            System.Collections.Generic.List<ServoController.IJOG_TAG> TAG_LIST = new System.Collections.Generic.List<ServoController.IJOG_TAG>();
            TAG_LIST.Add(tagServo1);

            ServoController.I_JOG(Comport, TAG_LIST);
        }
    }
}
