using System;
using System.Collections.Generic;
using System.Windows;
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
            public Queue<string> DebugMessages = new Queue<string>();
        }

        #endregion UIComponentValues    

        #region classInit

        UIComponentvalues ComponentsValues = new UIComponentvalues();
        ReliableSerialPort SerialStream;
        msgDecoder FrameDecoder;
        msgProcessor FrameProcessor;
        MessageEncoder.Encoder MsgEncoder = new MessageEncoder.Encoder();
        DispatcherTimer UI_Updater;
        robot RobotModel;

        #endregion ClassInit


        public MainWindow()
        {
            InitializeComponent();
            
            SerialStream = new ReliableSerialPort("COM6", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            FrameDecoder = new msgDecoder();
            FrameProcessor = new msgProcessor();
            UI_Updater = new DispatcherTimer();
            UI_Updater.Interval = new TimeSpan(0, 0, 0, 0, 100); //100ms tick
            RobotModel = new robot();

            //block logic
            UI_Updater.Tick += UpdateUI;
           
            SerialStream.DataReceived += FrameDecoder.DecodeMessage;
            FrameDecoder.OnDataDecodedEvent += FrameProcessor.ProcessMessage;
            FrameProcessor.OnTextMessageProcessedEvent += FrameProcessor_OnTextMessageProcessedEvent;
            FrameProcessor.OnCheckSumErrorOccuredEvent += FrameProcessor_OnCheckSumErrorOccuredEvent;
            FrameProcessor.OnIrMessageProcessedEvent += FrameProcessor_OnIrMessageProcessedEvent;
            FrameProcessor.OnSpeedMessageProcessedEvent += FrameProcessor_OnSpeedMessageProcessedEvent;

            SerialStream.Open();
            UI_Updater.Start();
        }

        //============================================================TEST ZONE 2=======================
        private void ControllerReceptionSide_OnHerkulexIncommingMessageDecodedEvent(object sender, HerkulexIncommingPacketDecodedArgs e)
        {
            ComponentsValues.DebugMessages.Enqueue(e.PacketSize.ToString());
        }
        //===============================================================================================

        private void UpdateUI(object sender, EventArgs e) //100ms update rate
        {
            if (ComponentsValues.DebugMessages.Count > 0)
            {
                TextBox_ReceptionLog.Text += "Received> " + ComponentsValues.DebugMessages.Dequeue() + "\n";
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
            ComponentsValues.DebugMessages.Enqueue("TransmissionErrorOccured");
        }

        private void FrameProcessor_OnTextMessageProcessedEvent(object sender, TextDataProcessedArgs e)
        {
            ComponentsValues.DebugMessages.Enqueue(e.ProcessedText);
        }

        private void Button_SendSpeedCommand_Click(object sender, RoutedEventArgs e)
        {
            MsgEncoder.UartSendSpeedCommand(SerialStream, Convert.ToSByte(TextBox_SpeedGauche.Text), Convert.ToSByte(TextBox_SpeedDroit.Text));
            ComponentsValues.DebugMessages.Enqueue("SpeedCMD out! Gauche:" + TextBox_SpeedGauche.Text + " Gauche:" + TextBox_SpeedDroit.Text);
        }

        byte lbstate = 0x00;
        private void TGLBLUE_Click(object sender, RoutedEventArgs e)
        {
            lbstate ^= 0x01;
            MsgEncoder.UartSendLedCommand(SerialStream, robot.LED.BLUE, lbstate);
        }

        byte lostate = 0x00;
        private void TGLORG_Click(object sender, RoutedEventArgs e)
        {
            lostate ^= 0x01;
            MsgEncoder.UartSendLedCommand(SerialStream, robot.LED.ORANGE, lostate);
        }

        byte lwstate = 0x00;
        private void TGLWHT_Click(object sender, RoutedEventArgs e)
        {
            lwstate ^= 0x01;
            MsgEncoder.UartSendLedCommand(SerialStream, robot.LED.WHITE, lwstate);
        }
    }
}
