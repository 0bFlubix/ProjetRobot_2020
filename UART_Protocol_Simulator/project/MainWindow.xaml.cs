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
using System.IO.Ports;
using ExtendedSerialPort;

namespace UART_Protocol_Simulator
{


    public partial class MainWindow : Window
    {
        //selection states
        private enum SelectionCommand
        {
            TXT, LED, IR, SPD
        }

        ReliableSerialPort serialPort = new ReliableSerialPort(" ", 115200, System.IO.Ports.Parity.None,
                                                                8, System.IO.Ports.StopBits.One);
        UART uartFuncs = new UART();
        Brush selectionColor = Brushes.DodgerBlue;
        DispatcherTimer ComponentsUpdateThread;
        Random pldRand = new Random();

        int payloadLength = 0;
        double estimatedGenTime = 0;

        string[] availableSerialPorts;

        //Frame vars
        byte SOF = 0xFE;
        byte CMD1 = 0x00;
        byte CMD0 = 0x00;
        byte PSIZE1 = 0x00;
        byte PSIZE0 = 0x00;
        byte checkSum = 0x00;
        byte[] payload;

        byte errChkSum;

        List<byte> payloadContent = new List<byte>();

        bool TXT_Selected = false;
        bool LED_Selected = false;
        bool IR_Selected = false;
        bool SPD_Selected = false;

        byte tlmIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            SelectMessageCommand(SelectionCommand.TXT); //select txt mode for init
            PSIZE0 = 0x03;      //set a size 3 payload for init
            UpdateMsgValues();  //calculate payload size and all
            randomizePayload(); //generate a random payload for init

            ComponentsUpdateThread = new DispatcherTimer();
            ComponentsUpdateThread.Interval = new TimeSpan(0, 0, 0, 0, 5); //5ms UI update rate
            ComponentsUpdateThread.Tick += ComponentsUpdateThread_Tick;
            ComponentsUpdateThread.Start();
        }


        #region threads

        //main update thread
        private void ComponentsUpdateThread_Tick(object sender, EventArgs e)
        {
            UpdateMsgValues();
            UI_UpdateSelectionState();
            UI_updateMessageContent();
        }

        #endregion threads

        #region functions

        //returns a list of all available ports
        public string[] GetPorts()
        {
            string[] SerialPorts = SerialPort.GetPortNames();
            return SerialPorts;
        }

        //generates a random payload
        private void randomizePayload()
        {
            if (payloadLength > 0)
            {
                payloadContent.Clear();

                if (tlmIndex > 4)
                    tlmIndex = 0;
                else
                    tlmIndex++;

                    payloadContent.Add(tlmIndex);

                for (int i = 1; i < payloadLength; i++)
                    payloadContent.Add((byte)pldRand.Next(0x41, 0x7A));

                payload = new byte[payloadLength];
                payloadContent.CopyTo(payload);

                checkSum = uartFuncs.CalculateChecksum((int)CMD0, payloadLength, payload);

                UI_updatePayloadContent();
            }

        }

        //updates message related values
        private void UpdateMsgValues()
        {
            estimatedGenTime = 0.004 * payloadLength;
            payloadLength = ((int)(PSIZE1 << 8) + PSIZE0);
        }

        //updates the UI payload related objects
        private void UI_updatePayloadContent()
        {
            
            textBox_payloadContent.Clear();
            foreach (byte b in payloadContent)
                textBox_payloadContent.Text += "0x" + b.ToString("X2") + " ";
        }

        //updates the command selection UI related bjects
        private void UI_UpdateSelectionState()
        {
            if (TXT_Selected)
            {
                TXT_underSelect.Visibility = Visibility.Visible;
                CMD0 = 0x80;
            }

            else
                TXT_underSelect.Visibility = Visibility.Hidden;

            if (LED_Selected)
            { 
                LED_underSelect.Visibility = Visibility.Visible;
                CMD0 = 0x20;
                PSIZE1 = 0; 
                PSIZE0 = 2; //LED payload : Fixed 2 bytes length
            }
            else
                LED_underSelect.Visibility = Visibility.Hidden; 

            if (IR_Selected)
            {
                IR_underSelect.Visibility = Visibility.Visible;
                CMD0 = 0x30;
                PSIZE1 = 0;
                PSIZE0 = 5; //IR payload : Fixed 5 bytes length
            }
            else
                IR_underSelect.Visibility = Visibility.Hidden;

            if (SPD_Selected)
            {
                SPD_underSelect.Visibility = Visibility.Visible;
                CMD0 = 0x40;
                PSIZE1 = 0;
                PSIZE0 = 2; //SPD payload : Fixed 2 bytes length
            }
            else
                SPD_underSelect.Visibility = Visibility.Hidden;

            if (LED_Selected || IR_Selected || SPD_Selected)
                textBox_payloadSizeInt_int.IsEnabled = false;
            else
                textBox_payloadSizeInt_int.IsEnabled = true;
        }

        //clear all selections (does not updates the UI)
        private void ResetCommandSelections()
        {
           TXT_Selected = false;
           LED_Selected = false;
           IR_Selected = false;
           SPD_Selected = false;
        }

        //selects a command (updates the UI)
        private void SelectMessageCommand(SelectionCommand command)
        {
            ResetCommandSelections();

            switch(command)
            {
                case SelectionCommand.TXT:
                    TXT_Selected = true;
                    break;

                case SelectionCommand.LED:
                    LED_Selected = true;
                    break;

                case SelectionCommand.IR:
                    IR_Selected = true;
                    break;

                case SelectionCommand.SPD:
                    SPD_Selected = true;
                    break;
            }

            UI_UpdateSelectionState(); 
        }

        //updates the message related UI objects
        private void UI_updateMessageContent()
        {
            textBlock_sof_hex.Text = "0x" + SOF.ToString("X2");
            textBlock_CMD1_hex.Text = "0x" + CMD1.ToString("X2");
            textBlock_CMD0_hex.Text = "0x" + CMD0.ToString("X2");
            textBlock_payloadSize1_hex.Text = "0x" + PSIZE1.ToString("X2");
            textBlock_payloadSize0_hex.Text = "0x" + PSIZE0.ToString("X2");
            textBlock_CheckSum_hex.Text = "0x" + checkSum.ToString("X2");
            textBox_payloadSizeInt_int.Text = payloadLength.ToString();

            //update com list
            availableSerialPorts = GetPorts();

            foreach (string port in availableSerialPorts)
            {
                if (!listBox_avPorts.Items.Contains(port))
                    listBox_avPorts.Items.Add(port);
            }

            foreach (string port_check in listBox_avPorts.Items)
            {
                if (!availableSerialPorts.Contains(port_check))
                { listBox_avPorts.Items.Remove(port_check); break; }

            }
            Array.Clear(availableSerialPorts, 0, availableSerialPorts.Length);


            if (payloadLength > 1000)
            { 
                textBlock_randWarning.Text = "~" + estimatedGenTime.ToString() + "s";
                textBlock_randWarning.Visibility = Visibility.Visible;
            }
            else
                textBlock_randWarning.Visibility = Visibility.Hidden;

            if (errChkSum == checkSum)
                textBlock_CheckSum_hex.Foreground = Brushes.Red;
            else
                textBlock_CheckSum_hex.Foreground = Brushes.Magenta;
        }

        #endregion functions

        #region configs

        //PSIZE1 config with mousewheel
        private void textBlock_payloadSize1_hex_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                PSIZE1++;
            else
                PSIZE1--;
        }

        //PSIZE0 config with mousewheel
        private void textBlock_payloadSize0_hex_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                PSIZE0++;
            else
                PSIZE0--;
        }

        //PSIZE config via textBox
        private void textBox_payloadSizeInt_int_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ushort payloadSize = UInt16.Parse(textBox_payloadSizeInt_int.Text);
                PSIZE1 = 0;
                PSIZE0 = 0;
                PSIZE1 += (byte)(payloadSize >> 8);
                PSIZE0 += (byte)(payloadSize);
            }
            catch(Exception)
            {
                PSIZE1 = 0;
                PSIZE0 = 0;
                textBox_payloadSizeInt_int.Clear();
            }
        }

        //TXT MouseDown event, set CMD0 to 0x80
        private void textBlock_CMD0_textSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectMessageCommand(SelectionCommand.TXT);
            }
        }

        //LED MouseDown event, set CMD0 to 0x20
        private void textBlock_CMD0_LedSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectMessageCommand(SelectionCommand.LED);
            }
        }

        //IR MouseDown event, set CMD0 to 0x30
        private void textBlock_CMD0_IrSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectMessageCommand(SelectionCommand.IR);
            }
        }

        //Speed MouseDown event, set CMD0 to 0x40
        private void textBlock_CMD0_SpeedSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectMessageCommand(SelectionCommand.SPD);
            }
        }

        //randomize button, randomizes the payload content
        private void button_RandomizePayload_Click(object sender, RoutedEventArgs e)
        {
            randomizePayload();
        }

        //updates the port when selected
        private void listBox_avPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                serialPort.Close();
                serialPort.PortName = listBox_avPorts.SelectedItem.ToString();
                serialPort.Open();

            }
            catch (Exception) 
            { serialPort.PortName = " "; }


        }

        //sends the UART frame
        private void button_SendFrame_Click(object sender, RoutedEventArgs e)
        {
            if(serialPort.PortName.Contains("COM"))
                uartFuncs.UartEncodeAndSendMessage((int)CMD0, payloadLength, payload,checkSum, serialPort);
        }
        #endregion configs

        #region Selection
        //PSIZE1 MouseExit select color
        private void textBlock_payloadSize1_hex_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlock_payloadSize1_hex.Foreground = Brushes.White;
        }

        //PSIZE0 MouseEnter select color
        private void textBlock_payloadSize0_hex_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock_payloadSize0_hex.Foreground = selectionColor;
        }

        //PSIZE0 MouseExit select color
        private void textBlock_payloadSize0_hex_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlock_payloadSize0_hex.Foreground = Brushes.White;
        }

        //PSIZE1 MouseEnter select color
        private void textBlock_payloadSize1_hex_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock_payloadSize1_hex.Foreground = selectionColor;
        }

        //TXT MouseEnter select color
        private void textBlock_CMD0_textSelect_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock_CMD0_textSelect.Foreground = selectionColor;
        }

        //TXT MouseExit select color
        private void textBlock_CMD0_textSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlock_CMD0_textSelect.Foreground = Brushes.White;
        }

        //LED MouseEnter select color
        private void textBlock_CMD0_LedSelect_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock_CMD0_LedSelect.Foreground = selectionColor;
        }

        //LED MouseExit select color
        private void textBlock_CMD0_LedSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlock_CMD0_LedSelect.Foreground = Brushes.White;
        }

        //IR MouseEnter select color
        private void textBlock_CMD0_IrSelect_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock_CMD0_IrSelect.Foreground = selectionColor;
        }

        //IR MouseExit select color
        private void textBlock_CMD0_IrSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlock_CMD0_IrSelect.Foreground = Brushes.White;
        }

        //SPD MouseEnter select color
        private void textBlock_CMD0_SpeedSelect_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock_CMD0_SpeedSelect.Foreground = selectionColor;
        }

        //SPD MouseExit select color
        private void textBlock_CMD0_SpeedSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlock_CMD0_SpeedSelect.Foreground = Brushes.White;
        }

        private void button_InstertError_Click(object sender, RoutedEventArgs e)
        {
            errChkSum = (byte)pldRand.Next(0, 255); ;
            while(errChkSum == checkSum)
                errChkSum = (byte)pldRand.Next(0, 255);
            checkSum = errChkSum;
        }

        #endregion Selection


    }
}
