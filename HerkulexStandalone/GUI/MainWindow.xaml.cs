using System.Windows;
using HerkulexReceptManager;
using ExtendedSerialPort;
using DataRedirector;
using System.IO.Ports;
using System;
using HerkulexController;

namespace GUI
{

    public partial class MainWindow : Window
    {
        herkulexRecept ReceptionController = new herkulexRecept();
        ServoController ServoControl = new ServoController();
        Redirector Bridge = new Redirector();

        ReliableSerialPort Comport = new ReliableSerialPort("COM6", 9600, Parity.None, 8, StopBits.One);

        public MainWindow()
        {
            InitializeComponent();
            Comport.Open();

            ServoControl.OnControllerCommandSentToRedirectorBridgeEvent += Bridge.RedirectData;
            Bridge.OnRedirectedDataEvent += ReceptionController.HerkulexDecodeIncommingPacket;
            ReceptionController.OnHerkulexIncommingMessageDecodedEvent += ReceptionController_OnHerkulexIncommingMessageDecodedEvent;
        }

        private void ReceptionController_OnHerkulexIncommingMessageDecodedEvent(object sender, EventArgsLibrary.HerkulexIncommingPacketDecodedArgs e)
        {
            //add breakpoint here
        }

        private void Button_EEPREAD_Click(object sender, RoutedEventArgs e)
        {
            //send packet to redirector, Comport not used but nescessary avoiding error
            ServoControl.EEP_ReadParam(Comport, 0x1A, ServoController.SrvRegAddr.Min_Voltage);
        }

        private void Button_EEPWRITE_Click(object sender, RoutedEventArgs e)
        {
            byte[] dataToWrite = { 0x5B, 0x22, 0x44, 0xFF, 0x10, 0x98, 0x12 };
            ServoControl.EEP_WriteParam(Comport, 0x1A, ServoController.SrvRegAddr.Min_Voltage, dataToWrite); // 6.714DCV as min allowed voltage
        }
    }
}
