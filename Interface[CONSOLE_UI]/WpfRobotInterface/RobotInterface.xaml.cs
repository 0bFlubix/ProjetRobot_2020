using System;
using System.Windows;
using System.Windows.Input;
using Robot;
using System.Windows.Threading;
using EventArgsLibrary;


namespace WpfRobotInterface
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 
    public partial class RobotInterface : Window
    {

        DispatcherTimer GraphUpdater = new DispatcherTimer();
        double angularSpeedConsigne = 2;
        double linearSpeedConsigne = 1;
        private readonly int maxPointsOnGraph = 300;
        private ulong UpdaterTimestamp = 0;

        public RobotInterface()
        {
            InitializeComponent();

            GraphUpdater.Interval = new TimeSpan(0, 0, 0, 0, 10);
            GraphUpdater.Tick += GraphUpdater_Tick; //subscribe to the tick event
            GraphUpdater.Start();

            //setting graph titles
            Osc_LinearSpeedOdometry.SetTitle("Linear speed from odometry");
            Osc_AngularSpeedOdometry.SetTitle("Angular speed from odometry");
            Osc_ErrorLinearAngularSpeed.SetTitle("Error Linear/angular speed");
            
            

            //actual values
            Osc_LinearSpeedOdometry.AddOrUpdateLine((int)GraphLineID.LinearSpeed, maxPointsOnGraph, "LinearSpeed");
            Osc_AngularSpeedOdometry.AddOrUpdateLine((int)GraphLineID.AngularSpeed, maxPointsOnGraph, "AngularSpeed");

            //errors
            Osc_AngularSpeedOdometry.AddOrUpdateLine((int)GraphLineID.ErrorAngularSpeed, maxPointsOnGraph, "AngularSpeedError");
            Osc_LinearSpeedOdometry.AddOrUpdateLine((int)GraphLineID.ErrorLinearSpeed, maxPointsOnGraph, "LinearSpeedError");

            Osc_AngularSpeedOdometry.ChangeLineColor((int)GraphLineID.ErrorAngularSpeed, System.Drawing.Color.DarkViolet);
            Osc_LinearSpeedOdometry.ChangeLineColor((int)GraphLineID.ErrorLinearSpeed, System.Drawing.Color.LightGreen);
        }

        private void GraphUpdater_Tick(object sender, EventArgs e)
        {
            if(toggleFreeze == 0)
            {

                UpdaterTimestamp++;
            }
        }

        private enum GraphLineID
        {
            LinearSpeed = 1,
            AngularSpeed = 2,
            ErrorAngularSpeed = 3,
            ErrorLinearSpeed = 4
        }

        byte toggleFreeze = 0x00;
        private void Window_KeyDown(object sender, KeyEventArgs e) //pause feature
        {
            if (e.Key == Key.Space)
                toggleFreeze ^= 0x01;
        }

        //Incoming Events
        public void OnPositionDataProcessedEvent(object sender, PositionDataProcessedArgs e)
        {
            double angularSpeedError = angularSpeedConsigne - e.VitesseAngulaireFromOdometry;
            double linearSpeedError = linearSpeedConsigne - e.VitesseLineaireFromOdometry;
            Osc_LinearSpeedOdometry.AddPointToLine((int)GraphLineID.LinearSpeed, e.Timestamp, e.VitesseLineaireFromOdometry);
            Osc_AngularSpeedOdometry.AddPointToLine((int)GraphLineID.AngularSpeed, e.Timestamp, e.VitesseLineaireFromOdometry);
            Osc_AngularSpeedOdometry.AddPointToLine((int)GraphLineID.ErrorAngularSpeed, e.Timestamp, angularSpeedError);
            Osc_LinearSpeedOdometry.AddPointToLine((int)GraphLineID.ErrorLinearSpeed, e.Timestamp, linearSpeedError);
            textBlock_LinSpeedError.Text = linearSpeedError.ToString();
        }
    }
}
