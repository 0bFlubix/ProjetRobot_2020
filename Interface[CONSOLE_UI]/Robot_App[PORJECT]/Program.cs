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
using Robot;
using System.Windows.Media;
using System.Windows.Markup.Localizer;

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
        
        static bool usingRobotInterface = true;
        static bool appLocked = true;
        static RobotInterface interfaceRobot;

        static string userInputCommand;
        static string[] splittedCommand;


        [STAThread]
        static void Main(string[] args)
        { 
            ////block logic
            SerialStream.DataReceived += MsgDecoder.DecodeMessage;
            MsgDecoder.OnDataDecodedEvent += MsgProcessor.ProcessMessage;


            //MsgEncoder.UartSendSpeedCommand(SerialStream, 0, 0);

            if (usingRobotInterface)
                StartRobotInterface();

            SerialStream.Open();

            ConsoleWriteColoredText("Initialized", ConsoleColor.Yellow);
            while(appLocked)
            {
                Console.Write(">");
                userInputCommand = Console.ReadLine();
                splittedCommand = ProcessUserCommand(userInputCommand);
                switch (splittedCommand[0])
                {
                    case "SetSpeed":
                        robot.vitesseAngulaireConsigne = Convert.ToSByte(splittedCommand[1]);
                        MsgEncoder.UartSendSpeedCommand(SerialStream, Convert.ToSByte(splittedCommand[1]), Convert.ToSByte(splittedCommand[2]));
                        break;

                    case "anglSpeed":
                        robot.vitesseLineaireConsigne = Convert.ToSByte(splittedCommand[1]);
                        MsgEncoder.UartSendAngularSpeedConsigne(SerialStream, Convert.ToSByte(splittedCommand[1]));
                        break;

                    case "linSpeed":
                        MsgEncoder.UartSendLinearSpeedConsigne(SerialStream, Convert.ToSByte(splittedCommand[1]));
                        break;

                    case "st":
                            MsgEncoder.UartSendSpeedCommand(SerialStream, 0,0);
                        break;

                    default:
                        ConsoleWriteColoredText("Unknown command : '" + splittedCommand[0] + "'", ConsoleColor.Red);
                        break;
                }
            }

            //Thread.CurrentThread.Join();
        }


        private static string[] ProcessUserCommand(string input)
        {
            string[] args = input.Split(' ');
            return args;
        }

        private static void MsgProcessor_OnPositionDataProcessedEvent(object sender, PositionDataProcessedArgs e)
        {

            //ConsoleWriteColoredText("Angular Speed: " + e.VitesseAngulaireFromOdometry.ToString() + "\n" +
            //                        "Linear Speed: " + e.VitesseLineaireFromOdometry + "\n" + 
            //                        "X position: " + e.XPositionFromOdometry + "\n" +
            //                        "Y position: " + e.YPositionFromOdometry + "\n" + 
            //                        "angle[RAD]: " + e.AngleRadianFromOdometry + "\n" +
            //                        "botTimestamp: " + e.Timestamp + "\n\n\n", ConsoleColor.Cyan);

            robot.vitesseLineaireFromOdometry = e.VitesseLineaireFromOdometry;
            robot.vitesseAngulaireFromOdometry = e.VitesseAngulaireFromOdometry;
            robot.xPositionFromOdometry = e.XPositionFromOdometry;
            robot.yPositionFromOdometry = e.YPositionFromOdometry;
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
                MsgProcessor.OnPositionDataProcessedEvent += interfaceRobot.OnPositionDataProcessedEvent;
                interfaceRobot.ShowDialog();
            });
            ui_thread.SetApartmentState(ApartmentState.STA);
            ui_thread.Start();
        }
    }
}
