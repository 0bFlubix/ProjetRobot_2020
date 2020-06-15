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

        DispatcherTimer Updater = new DispatcherTimer();

        private readonly int maxPointsOnGraph = 300;
        private ulong UpdaterTimestamp = 0;

        double angularSpeedError = 0;
        double linearSpeedError = 0;

        public RobotInterface()
        {

            InitializeComponent();

            Updater.Interval = new TimeSpan(0, 0, 0, 0, 10);

            //setting graph titles
            Updater.Tick += Updater_Tick;
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

            
            Updater.Start();
        }

        private void Updater_Tick(object sender, EventArgs e)
        {
            textBlock_LinSpeedError.Text = angularSpeedError.ToString();
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
            if (toggleFreeze == 0)
            {
                angularSpeedError = robot.vitesseAngulaireConsigne - e.VitesseAngulaireFromOdometry;
                linearSpeedError = robot.vitesseLineaireConsigne - e.VitesseLineaireFromOdometry;
                Osc_LinearSpeedOdometry.AddPointToLine((int)GraphLineID.LinearSpeed, e.Timestamp, e.VitesseLineaireFromOdometry);
                Osc_AngularSpeedOdometry.AddPointToLine((int)GraphLineID.AngularSpeed, e.Timestamp, e.VitesseAngulaireFromOdometry);
                Osc_AngularSpeedOdometry.AddPointToLine((int)GraphLineID.ErrorAngularSpeed, e.Timestamp, angularSpeedError);
                Osc_LinearSpeedOdometry.AddPointToLine((int)GraphLineID.ErrorLinearSpeed, e.Timestamp, linearSpeedError);
            }

        }
    }
}
