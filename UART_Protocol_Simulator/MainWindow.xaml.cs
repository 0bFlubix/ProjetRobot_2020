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

namespace UART_Protocol_Simulator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Brush selectionColor = Brushes.DodgerBlue;

        DispatcherTimer ComponentsUpdateThread;

        byte SOF = 0xFE;
        byte CMD1 = 0x00;
        byte CMD0 = 0x00;
        byte PSIZE1 = 0x00;
        byte PSIZE0 = 0x00;
        byte checkSum = 0x00;
        byte[] payload;

        bool TXT_Selected = false;
        bool LED_Selected = false;
        bool IR_Selected = false;
        bool SPD_Selected = false;

        public MainWindow()
        {
            InitializeComponent();

            ComponentsUpdateThread = new DispatcherTimer();
            ComponentsUpdateThread.Interval = new TimeSpan(0, 0, 0, 0, 10); //100ms UI update rate
            ComponentsUpdateThread.Tick += ComponentsUpdateThread_Tick;
            ComponentsUpdateThread.Start();
        }


        #region threads

        private void ComponentsUpdateThread_Tick(object sender, EventArgs e)
        {
            UI_UpdateSelectionState();
            UI_updateMessageContent();
        }

        #endregion threads

        #region functions

        private void UI_UpdateSelectionState()
        {
            if (TXT_Selected)
                TXT_underSelect.Visibility = Visibility.Visible;
            else
                TXT_underSelect.Visibility = Visibility.Hidden;

            if (LED_Selected)
                LED_underSelect.Visibility = Visibility.Visible;
            else
                LED_underSelect.Visibility = Visibility.Hidden;

            if (IR_Selected)
                IR_underSelect.Visibility = Visibility.Visible;
            else
                IR_underSelect.Visibility = Visibility.Hidden;

            if (SPD_Selected)
                SPD_underSelect.Visibility = Visibility.Visible;
            else
                SPD_underSelect.Visibility = Visibility.Hidden;
        }

        private void ResetCommandSelections()
        {
           TXT_Selected = false;
           LED_Selected = false;
           IR_Selected = false;
           SPD_Selected = false;
        }

        private void UI_updateMessageContent()
        {
            textBlock_sof_hex.Text = "0x" + SOF.ToString("X2");
            textBlock_CMD1_hex.Text = "0x" + CMD1.ToString("X2");
            textBlock_CMD0_hex.Text = "0x" + CMD0.ToString("X2");
            textBlock_payloadSize1_hex.Text = "0x" + PSIZE1.ToString("X2");
            textBlock_payloadSize0_hex.Text = "0x" + PSIZE0.ToString("X2");
            textBlock_CheckSum_hex.Text = "0x" + checkSum.ToString("X2");
            textBox_payloadSizeInt_int.Text = ((int)(PSIZE1 << 8) + PSIZE0).ToString();
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
                ResetCommandSelections();
                TXT_Selected = true;
                CMD0 = 0x80;
            }
        }

        //LED MouseDown event, set CMD0 to 0x20
        private void textBlock_CMD0_LedSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ResetCommandSelections();
                LED_Selected = true;
                CMD0 = 0x20;
            }
        }

        //IR MouseDown event, set CMD0 to 0x30
        private void textBlock_CMD0_IrSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ResetCommandSelections();
                IR_Selected = true;
                CMD0 = 0x30;
            }
        }

        //Speed MouseDown event, set CMD0 to 0x40
        private void textBlock_CMD0_SpeedSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ResetCommandSelections();
                SPD_Selected = true;
                CMD0 = 0x40;
            }
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

        #endregion Selection

        
    }
}
