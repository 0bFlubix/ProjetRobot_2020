using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using WpfRobotInterface;
using System.Windows.Input;
using System.Diagnostics;
using MessageDecoder;
using MessageEncoder;
using MessageProcessor;
using ExtendedSerialPort;
using EventArgsLibrary;
using System.IO.Ports;
using System.Windows.Interop;

/// <summary>
/// interaction logic
/// </summary>

namespace Robot_App
{
    class Program
    {

        #region class_init

        static ReliableSerialPort SerialStream = new ReliableSerialPort("COM6", 115200, Parity.None, 8, StopBits.One);
        static msgDecoder MsgDecoder = new msgDecoder();
        static msgProcessor MsgProcessor = new msgProcessor();
        static Encoder MsgEncoder = new Encoder();

        #endregion
        
        static bool usingRobotInterface = false;
        static RobotInterface interfaceRobot;

        [STAThread]
        static void Main(string[] args)
        {
            if (usingRobotInterface)
                StartRobotInterface();

            SerialStream.Open();

            //block logic
            SerialStream.DataReceived += MsgDecoder.DecodeMessage;
            MsgDecoder.OnDataDecodedEvent += MsgProcessor.ProcessMessage;
            MsgProcessor.OnPositionDataProcessedEvent += MsgProcessor_OnPositionDataProcessedEvent;

            

            ConsoleWriteColoredText("Logic initialized", ConsoleColor.Yellow);

            Thread.CurrentThread.Join();
        }

        static sbyte Gspeed = 15;
        static sbyte Dspeed = 15;
        static int target = 0;
        private static void MsgProcessor_OnPositionDataProcessedEvent(object sender, PositionDataProcessedArgs e)
        {

            ConsoleWriteColoredText("Angular Speed: " + e.VitesseAngulaireFromOdometry.ToString() + "\n" +
                                    "Linear Speed: " + e.VitesseLineaireFromOdometry + "\n" + 
                                    "X position: " + e.XPositionFromOdometry + "\n" +
                                    "Y position: " + e.YPositionFromOdometry + "\n" + 
                                    "angle[RAD]: " + e.AngleRadianFromOdometry + "\n" +
                                    "botTimestamp: " + e.Timestamp + "\n\n\n", ConsoleColor.Cyan);
        }

        public static void ConsoleWriteColoredText(string text, ConsoleColor c)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }


        static Thread ui_thread;
        static void StartRobotInterface()
        {
            ui_thread = new Thread(() =>
            {
                interfaceRobot = new RobotInterface();
                interfaceRobot.ShowDialog();
            });
            ui_thread.SetApartmentState(ApartmentState.STA);
            ui_thread.Start();
        }
    }
}
