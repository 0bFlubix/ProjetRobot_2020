using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using EventArgsLibrary;
using ExtendedSerialPort;
using MessageDecoder;
using MessageProcessor;
using Robot;
using PortSupervisor;
using HerkulexController;
using HerkulexReceptManager;

namespace UI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //instanciation des classes
        #region UIComponentValues

        public class UIComponentvalues
        {
            public Queue<string> serialInputStream_TextMessages = new Queue<string>();
        }

        #endregion UIComponentValues    

        #region moduleInst

        UIComponentvalues ComponentsValues = new UIComponentvalues();
        ReliableSerialPort SerialInputStream;
        msgDecoder FrameDecoder;
        msgProcessor FrameProcessor;
        DispatcherTimer UI_Updater;
        robot RobotModel;

        //test
        HerkulexController.HerkulexController ServoController = new HerkulexController.HerkulexController();
        herkulexRecept ControllerReceptionSide = new herkulexRecept();
        //test

        #endregion moduleInst





        public MainWindow()
        {
            InitializeComponent();
            
            SerialInputStream = new ReliableSerialPort("COM6", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            FrameDecoder = new msgDecoder();
            FrameProcessor = new msgProcessor();
            UI_Updater = new DispatcherTimer();
            UI_Updater.Interval = new TimeSpan(0, 0, 0, 0, 100); //100ms tick
            RobotModel = new robot();

            //block logic
            UI_Updater.Tick += UpdateUI;
           
            SerialInputStream.DataReceived += FrameDecoder.DecodeMessage;
            FrameDecoder.OnDataDecodedEvent += FrameProcessor.ProcessMessage;
            FrameProcessor.OnTextMessageProcessedEvent += FrameProcessor_OnTextMessageProcessedEvent;
            FrameProcessor.OnCheckSumErrorOccuredEvent += FrameProcessor_OnCheckSumErrorOccuredEvent;
            FrameProcessor.OnIrMessageProcessedEvent += FrameProcessor_OnIrMessageProcessedEvent;
            FrameProcessor.OnSpeedMessageProcessedEvent += FrameProcessor_OnSpeedMessageProcessedEvent;

            SerialInputStream.Open();

            //====================================TEST ZONE========================================================
            ServoController.EEP_ReadParam(SerialInputStream, 0xAA, 0x11); //test
            byte[] data = { 0xFF, 0xFF };
            ServoController.EEP_WriteParam(SerialInputStream, 0xAA, 0x11, data);

            SerialInputStream.DataReceived += ControllerReceptionSide.HerkulexDecodeIncommingPacket;
            ControllerReceptionSide.OnHerkulexIncommingMessageDecodedEvent += ControllerReceptionSide_OnHerkulexIncommingMessageDecodedEvent;
            //=====================================================================================================

            UI_Updater.Start();
        }

        //============================================================TEST ZONE 2=======================
        private void ControllerReceptionSide_OnHerkulexIncommingMessageDecodedEvent(object sender, HerkulexIncommingPacketDecodedArgs e)
        {
            ComponentsValues.serialInputStream_TextMessages.Enqueue(e.PacketSize.ToString());
        }
        //===============================================================================================

        private void UpdateUI(object sender, EventArgs e) //100ms update rate
        {
            if (ComponentsValues.serialInputStream_TextMessages.Count > 0)
            {
                TextBox_ReceptionLog.Text += "Received> " + ComponentsValues.serialInputStream_TextMessages.Dequeue() + "\n";
                TextBox_ReceptionLog.PageDown(); 
            }

            UI_telem0_value.Text = RobotModel.distanceTelem[0].ToString();
            UI_telem1_value.Text = RobotModel.distanceTelem[1].ToString();
            UI_telem2_value.Text = RobotModel.distanceTelem[2].ToString();
            UI_telem3_value.Text = RobotModel.distanceTelem[3].ToString();
            UI_telem4_value.Text = RobotModel.distanceTelem[4].ToString();
            UI_SpeedDr_value.Text = RobotModel.actualSpeedRoueDroite.ToString();
            UI_SpeedGa_value.Text = RobotModel.actualSpeedRoueGauche.ToString();
            UI_DirectionDr.Text = RobotModel.actualWayRoueDroite.ToString();
            UI_DirectionGa.Text = RobotModel.actualWayRoueGauche.ToString();

        }

        private void FrameProcessor_OnSpeedMessageProcessedEvent(object sender, SpeedDataProcessedArgs e)
        {
            RobotModel.actualSpeedRoueDroite = e.SpeedDroit;
            RobotModel.actualSpeedRoueGauche = e.SpeedGauche;
            RobotModel.actualWayRoueDroite = e.WayDroit;
            RobotModel.actualWayRoueGauche = e.WayGauche;
        }

        private void FrameProcessor_OnIrMessageProcessedEvent(object sender, IrDataProcessedArgs e)
        {
            for(int i = 0; i < e.Distance.Length; i++)
                RobotModel.distanceTelem[i] = e.Distance[i];
        }

        private void FrameProcessor_OnCheckSumErrorOccuredEvent(object sender, CheckSumErrorOccuredArgs e)
        {
            ComponentsValues.serialInputStream_TextMessages.Enqueue("TransmissionErrorOccured");
        }

        private void FrameProcessor_OnTextMessageProcessedEvent(object sender, TextDataProcessedArgs e)
        {
            ComponentsValues.serialInputStream_TextMessages.Enqueue(e.ProcessedText);
        }


    }
}
