using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;

namespace kOS.AddOns.StockCamera
{
    [kOSAddon("CAMERA")]
    [KOSNomenclature("CAMERAAddon")]
    public class Addon : Suffixed.Addon
    {
        private FlightCameraValue flightCam;

        public Addon(SharedObjects shared) : base(shared)
        {
            AddSuffix("FLIGHTCAMERA", new Suffix<FlightCameraValue>(GetFlightCamera));
        }

        public override BooleanValue Available()
        {
            return BooleanValue.True;
        }

        public FlightCameraValue GetFlightCamera()
        {
            if (flightCam == null)
            {
                flightCam = new FlightCameraValue(shared);
            }
            return flightCam;
        }
    }
}