using System.Windows;
using HerkulexCom;
using HerkulexReceptManager;
using ExtendedSerialPort;
using System.IO.Ports;
using System;

namespace GUI
{

    public partial class MainWindow : Window
    {
        herkulexRecept ReceptionController = new herkulexRecept();
        ReliableSerialPort Comport = new ReliableSerialPort("COM6", 9600, Parity.None, 8, StopBits.One);

        public MainWindow()
        {
            InitializeComponent();
            Comport.Open();

            Comport.DataReceived += ReceptionController.HerkulexDecodeIncommingPacket;
            ReceptionController.OnHerkulexIncommingMessageDecodedEvent += ReceptionController_OnHerkulexIncommingMessageDecodedEvent;
        }

        private void ReceptionController_OnHerkulexIncommingMessageDecodedEvent(object sender, EventArgsLibrary.HerkulexIncommingPacketDecodedArgs e)
        {
            /*
            foreach(byte b in e.PacketData)
            {
                Console.WriteLine(b.ToString());
            }
            */
            Console.WriteLine(e.CheckSum1);
            Console.WriteLine("---separator---");
        }
    }
}
