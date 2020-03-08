using ExtendedSerialPort;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Text;

//GITHUB CHECK COMMENT 03/05

namespace robotInterface_barthelemy
{
    public partial class MainWindow : Window
    {
        //init new instances
        UART UART = new UART();
        ReliableSerialPort SerialPort1 = new ReliableSerialPort(" ", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        DispatcherTimer timerAffichage;

        //init vars
        string TxText;
        string[] AvportList;

        public MainWindow()
        {

            InitializeComponent();
            //add DataReceived Event
            SerialPort1.DataReceived += SerialPort1_DataReceived;

            //GUI objects params
            TextBox_Emission.AcceptsReturn = false;
            TextBox_Reception.AcceptsReturn = true;
            TextBox_Reception.IsReadOnly = true;
            TextBox_Emission.SelectionBrush = Brushes.Orange;

            //new thread for displaying RX messages [100ms tick]
            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick; //add tick Event
            timerAffichage.Start();

            //list all available ports at first start
            AvportList = SerialPort.GetPortNames();
            for (int i = 0; i < AvportList.Length; i++)
                ListBox_AvailablePorts.Items.Add(AvportList[i].ToString());

        }

        #region Functions
        //sends a message to the current COM port (string as output format) [DEPRECIATED]
        private void SendMessage()
        {
            TxText = TextBox_Emission.Text.ToString();
            SerialPort1.WriteLine(TxText);
            TextBox_Reception.AppendText("[TX " + ListBox_AvailablePorts.SelectedItem.ToString() + "]> " + TxText + "\n");
            TextBox_Reception.LineDown();
            TextBox_Emission.Clear();
            TextBox_Emission.Focus();
        }
        #endregion Functions

        #region EventsAndTimers

        //displayTimer 100ms tick, displays incomming RX messages
        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            while(UART.rcvBytesQueue.Count>0)
            {
                byte b = UART.rcvBytesQueue.Dequeue();
                UART.DecodeMessage(b);
                TextBox_Reception.Text += "0x" + b.ToString("X2") + " ";
                if (UART.rcvBytesQueue.Count == 0)
                    { TextBox_Reception.Text += "\n"; TextBox_Reception.LineDown(); }

               
            }
        }
        
        //dataReceived Event
        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            foreach (byte b in e.Data)
            {
                UART.rcvBytesQueue.Enqueue(b);
                //UART.DecodeMessage(b);
            }
        }

        //OnClick send button
        private void Button_Envoi_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        //key bindings
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage();
        }

        //dynamic port selection
        private void ListBox_AvailablePorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SerialPort1.Close();
                SerialPort1.PortName = ListBox_AvailablePorts.SelectedItem.ToString();
                GrBx_reception.Header = ListBox_AvailablePorts.SelectedItem.ToString();
                SerialPort1.Open(); 
            }
            catch (System.UnauthorizedAccessException) //if the port is Busy
            {
                GrBx_reception.Header = ListBox_AvailablePorts.SelectedItem.ToString() + " is Busy";
            }
        }

        //OnClick ClearButton
        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Reception.Clear();
        }

        //OnClick TestButton, sends a text frame "BONJOUR"
        private void Button_test_Click(object sender, RoutedEventArgs e)
        {
            byte[] msgArray = Encoding.ASCII.GetBytes("Bonjour");
            UART.UartEncodeAndSendMessage(0x0080, msgArray.Length, msgArray, SerialPort1);
        }

        #endregion EventsAndTimers


    }
}