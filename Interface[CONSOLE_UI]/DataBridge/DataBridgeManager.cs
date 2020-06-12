using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventArgsLibrary;
/// <summary>
///to redirect data to a class
/// </summary>
namespace DataBridgeManager
{
    public class DataBridge
    {

       public void SendPositionDataToBridge(ulong timestamp, float xPositionFromOdometry, float yPositionFromOdometry, float angleRadianFromOdometry,
                                                    float vitesseLineaireFromOdometry, float vitesseAngulaireFromOdometry)
        {
            OnPositionRedirectData(timestamp, xPositionFromOdometry, yPositionFromOdometry, angleRadianFromOdometry,
                                                    vitesseLineaireFromOdometry, vitesseAngulaireFromOdometry);
        }

        public event EventHandler<DataBridgeOutArgs> DataBridgeOutEvent;

        public virtual void OnPositionRedirectData(ulong timestamp, float xPositionFromOdometry, float yPositionFromOdometry, float angleRadianFromOdometry,
                                                    float vitesseLineaireFromOdometry, float vitesseAngulaireFromOdometry)
        {
            var handler = DataBridgeOutEvent;
            if(handler != null)
            {
                handler(this, new DataBridgeOutArgs
                {
                    Timestamp = timestamp,
                    XPositionFromOdometry = xPositionFromOdometry,
                    YPositionFromOdometry = yPositionFromOdometry,
                    AngleRadianFromOdometry = angleRadianFromOdometry,
                    VitesseLineaireFromOdometry = vitesseLineaireFromOdometry,
                    VitesseAngulaireFromOdometry = vitesseAngulaireFromOdometry
                });
            }
        }

    }
    
}
