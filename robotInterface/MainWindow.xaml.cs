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
using ExtendedSerialPort;
using System.Windows.Threading;
using System.IO;
using System.IO.Ports;

namespace robotInterface_barthelemy
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
  
    public partial class MainWindow : Window
    {
        Robot robot = new Robot();
        //-----new SerialPort
        ReliableSerialPort SerialPort1 = new ReliableSerialPort(" ", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        DispatcherTimer timerAffichage;


        //-------------vars

        string TxText;

        string[] AvportList;
        
       

        public MainWindow()
        {
           
            InitializeComponent();
            SerialPort1.DataReceived += SerialPort1_DataReceived;


            TextBox_Emission.AcceptsReturn = false;
            TextBox_Reception.AcceptsReturn = true;
            TextBox_Reception.IsReadOnly = true;
            TextBox_Emission.SelectionBrush = Brushes.Orange;

            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 0); //100ms tick
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();

            AvportList = SerialPort.GetPortNames();
            for (int i = 0; i < AvportList.Length; i++)
                ListBox_AvailablePorts.Items.Add(AvportList[i].ToString());
            

        }

        private void TimerAffichage_Tick(object sender, EventArgs e) //100ms tick
        {
            if (robot.receivedText != null) //if new message in buffer
            {
                TextBox_Reception.AppendText("[RX " + ListBox_AvailablePorts.SelectedItem.ToString() + "]> " + robot.receivedText + "\n");
                robot.receivedText = null;
            }s
        }

            //-------------Funcs
            private void SendMessage()
        {
            TxText = TextBox_Emission.Text.ToString();
            SerialPort1.WriteLine(TxText);
            TextBox_Reception.AppendText("[TX" + ListBox_AvailablePorts.SelectedItem.ToString() + "]> " + TxText + "\n");
            TextBox_Reception.LineDown();
            TextBox_Emission.Clear();
            TextBox_Emission.Focus();

        }
        //---------------------

        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            robot.receivedText = SerialPort1.ReadExisting();
        }

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
            SerialPort1.Close();
            SerialPort1.PortName = ListBox_AvailablePorts.SelectedItem.ToString();
            GrBx_reception.Header = ListBox_AvailablePorts.SelectedItem.ToString();
            SerialPort1.Open();
        }
    }
}
