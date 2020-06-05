using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using WpfRobotInterface;
using System.Windows.Input;
using System.Diagnostics;

namespace Robot_App
{
    class Program
    {
        static bool usingRobotInterface = true;
        static RobotInterface interfaceRobot;

        static void Main(string[] args)
        {
            if (usingRobotInterface)
                StartRobotInterface();
            Console.WriteLine("initialized!");
            Console.WriteLine("Press any key to quit...");
            Console.Read();
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
