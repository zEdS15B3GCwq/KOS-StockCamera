using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using kOS.Utilities;

namespace kOS.AddOns.StockCamera
{
    [KOSNomenclature("FlightCamera")]
    public class FlightCameraValue : Structure
    {
        private SharedObjects shared;

        public FlightCameraValue(SharedObjects shared)
        {
            this.shared = shared;
            AddSuffix("CAMERAMODE", new SetSuffix<StringValue>(GetCameraMode, SetCameraMode));
            AddSuffix("CAMERAFOV", new SetSuffix<ScalarValue>(GetCameraFov, SetCameraFov));
            AddSuffix("CAMERAPITCH", new SetSuffix<ScalarValue>(GetCameraPitch, SetCameraPitch));
            AddSuffix("CAMERAHDG", new SetSuffix<ScalarValue>(GetCameraHdg, SetCameraHdg));
            AddSuffix("CAMERADISTANCE", new SetSuffix<ScalarValue>(GetCameraDistance, SetCameraDistance));
            AddSuffix("CAMERAPOSITION", new SetSuffix<Vector>(GetCameraPosition, SetCameraPosition));
        }

        private void SetCameraMode(StringValue value)
        {
            if (shared.Vessel.isActiveVessel)
            {
                SetCameraMode(value.ToString());
            }
        }

        private void SetCameraMode(string value)
        {
            if (shared.Vessel.isActiveVessel)
            {
                switch (value.ToUpper().Trim())
                {
                    case ("AUTO"):
                        FlightCamera.SetMode(FlightCamera.Modes.AUTO);
                        break;

                    case ("CHASE"):
                        FlightCamera.SetMode(FlightCamera.Modes.CHASE);
                        break;

                    case ("FREE"):
                        FlightCamera.SetMode(FlightCamera.Modes.FREE);
                        break;

                    case ("LOCKED"):
                        FlightCamera.SetMode(FlightCamera.Modes.LOCKED);
                        break;

                    case ("ORBITAL"):
                        FlightCamera.SetMode(FlightCamera.Modes.ORBITAL);
                        break;

                    default:
                        throw new KOSException("Cannot set cameramode, invalid input value.");
                }
            }
        }

        private StringValue GetCameraMode()
        {
            return new StringValue(FlightCamera.fetch.mode.ToString());
        }

        private ScalarValue GetCameraFov()
        {
            return ScalarValue.Create(FlightCamera.fetch.FieldOfView);
        }

        private void SetCameraFov(ScalarValue value)
        {
            if (shared.Vessel.isActiveVessel)
            {
                FlightCamera.fetch.FieldOfView = (float)value.GetDoubleValue();
                FlightCamera.fetch.SetFoV((float)value.GetDoubleValue());
            }
        }

        private ScalarValue GetCameraPitch()
        {
            return ScalarValue.Create(Utils.RadiansToDegrees(FlightCamera.fetch.camPitch));
        }

        private void SetCameraPitch(ScalarValue value)
        {
            if (shared.Vessel.isActiveVessel)
            {
                FlightCamera.fetch.camPitch = (float)Utils.DegreesToRadians(value.GetDoubleValue());
            }
        }

        private ScalarValue GetCameraHdg()
        {
            return ScalarValue.Create(Utils.RadiansToDegrees(FlightCamera.fetch.camHdg));
        }

        private void SetCameraHdg(ScalarValue value)
        {
            if (shared.Vessel.isActiveVessel)
            {
                FlightCamera.fetch.camHdg = (float)Utils.DegreesToRadians(value.GetDoubleValue());
            }
        }

        private ScalarValue GetCameraDistance()
        {
            return ScalarValue.Create(FlightCamera.fetch.Distance);
        }

        private void SetCameraDistance(ScalarValue value)
        {
            if (shared.Vessel.isActiveVessel)
            {
                FlightCamera.fetch.SetDistanceImmediate((float)value.GetDoubleValue());
            }
        }

        private Vector GetCameraPosition()
        {
            return new Vector(FlightCamera.fetch.transform.position - FlightCamera.fetch.GetPivot().position);
        }

        private void SetCameraPosition(Vector value)
        {
            if (shared.Vessel.isActiveVessel)
            {
                FlightCamera.fetch.SetCamCoordsFromPosition(value.ToVector3() + FlightCamera.fetch.GetPivot().position);
            }
        }
    }
}