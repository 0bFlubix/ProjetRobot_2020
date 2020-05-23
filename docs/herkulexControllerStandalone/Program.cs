using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using HerkulexCom;

namespace herkulexControllerStandalone
{
    class Controller
    {
        Thread t1 = new Thread();
        static void Main(string[] args)
        {
            HerkulexCom.HerkulexCom ServoController = new HerkulexCom.HerkulexCom();
            SerialPort SerialStream = new SerialPort();
            SerialStream.BaudRate = 115200;
            SerialStream.Parity = Parity.None;
            SerialStream.StopBits = StopBits.One;
            SerialStream.PortName = "COM6";
            SerialStream.Open();

            byte[] data = { 0x11, 0xFA };
            ServoController.EEP_WriteParam(SerialStream, 0xAA, 0x01, data);

            SerialStream.DataReceived += SerialStream_DataReceived;
        }

        private static void SerialStream_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            Console.Write("hehe");
            Console.ReadKey();
        }

    }
}
