using System;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using kOS.Utilities;

namespace kOS.AddOns.StockCamera
{
    [KOSNomenclature("FlightCamera")]
    public class FlightCameraValue : Structure, Safe.IUpdateObserver, IDisposable
    {
        private SharedObjects shared;

        private TriggerInfo camPositionTrigger = null;
        private UserDelegate camPositionDelegate = null;

        private NoDelegate noDelegate;

        public FlightCameraValue(SharedObjects shared)
        {
            this.shared = shared;
            shared.UpdateHandler.AddObserver(this);
            AddSuffix(new string[] { "MODE", "CAMERAMODE" }, new SetSuffix<StringValue>(GetCameraMode, SetCameraMode));
            AddSuffix(new string[] { "FOV", "CAMERAFOV" }, new SetSuffix<ScalarValue>(GetCameraFov, SetCameraFov));
            AddSuffix(new string[] { "PITCH", "CAMERAPITCH" }, new SetSuffix<ScalarValue>(GetCameraPitch, SetCameraPitch));
            AddSuffix(new string[] { "HDG", "HEADING", "CAMERAHDG" }, new SetSuffix<ScalarValue>(GetCameraHdg, SetCameraHdg));
            AddSuffix(new string[] { "DISTANCE", "CAMERADISTANCE" }, new SetSuffix<ScalarValue>(GetCameraDistance, SetCameraDistance));
            AddSuffix(new string[] { "POSITION", "CAMERAPOSITION" }, new SetSuffix<Vector>(GetCameraPosition, SetCameraPosition));
            AddSuffix("POSITIONUPDATER", new SetSuffix<UserDelegate>(GetCameraPositionDelegate, SetCameraPositionDelegate));
            AddSuffix("TARGET", new SetSuffix<Structure>(GetCameraTarget, SetCameraTarget));
            AddSuffix("TARGETPOS", new Suffix<Vector>(GetTargetPosition));
            AddSuffix("PIVOTPOS", new Suffix<Vector>(GetPivotPosition));
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
            var cam = FlightCamera.fetch;
            if (cam.Target != null)
            {
                switch (cam.targetMode)
                {
                    case FlightCamera.TargetMode.Vessel:
                        return new Vector(FlightCamera.fetch.transform.position - shared.Vessel.CoMD + cam.vesselTarget.CoMD - cam.GetPivot().position);
                    case FlightCamera.TargetMode.Part:
                        return new Vector(FlightCamera.fetch.transform.position - shared.Vessel.CoMD + cam.partTarget.transform.position - cam.GetPivot().position);
                    default:
                        return new Vector(FlightCamera.fetch.transform.position - shared.Vessel.CoMD - FlightCamera.fetch.GetPivot().position - cam.GetPivot().position);
                }
            }
            return new Vector(FlightCamera.fetch.transform.position - shared.Vessel.CoMD - FlightCamera.fetch.GetPivot().position);
        }

        private Vector GetPivotPosition()
        {
            var cam = FlightCamera.fetch;
            if (cam != null)
            {
                return new Vector(cam.GetPivot().position - shared.Vessel.CoMD);
            }
            return Vector.Zero;
        }

        private Vector GetTargetPosition()
        {
            var cam = FlightCamera.fetch;
            if (cam.Target != null)
            {
                return new Vector(cam.Target.position - shared.Vessel.CoMD);
            }
            return Vector.Zero;
        }

        private void SetCameraPosition(Vector value)
        {
            if (shared.Vessel.isActiveVessel)
            {
                var cam = FlightCamera.fetch;
                switch (cam.targetMode)
                {
                    case FlightCamera.TargetMode.Vessel:
                        cam.SetCamCoordsFromPosition(value.ToVector3() + shared.Vessel.CoMD - cam.vesselTarget.CoMD + cam.GetPivot().position);
                        break;
                    case FlightCamera.TargetMode.Part:
                        cam.SetCamCoordsFromPosition(value.ToVector3() + shared.Vessel.CoMD - cam.partTarget.transform.position + cam.GetPivot().position);
                        break;
                    default:
                        cam.SetCamCoordsFromPosition(value.ToVector3() + shared.Vessel.CoMD + FlightCamera.fetch.GetPivot().position + cam.GetPivot().position);
                        break;
                }
            }
        }

        private Structure GetCameraTarget()
        {
            var cam = FlightCamera.fetch;
            switch (cam.targetMode)
            {
                case FlightCamera.TargetMode.Vessel:
                    return VesselTarget.CreateOrGetExisting(cam.vesselTarget, shared);
                case FlightCamera.TargetMode.Part:
                    return Suffixed.Part.PartValueFactory.Construct(cam.partTarget, shared);
                case FlightCamera.TargetMode.Transform:
                    throw new KOSException("Flight camera has target set to transform.  This is currently not supported.");
                case FlightCamera.TargetMode.None:
                default:
                    throw new KOSException("Flight camera has no target set.  This should not ever happen, unless KSP changes their API.");
            }
        }

        private void SetCameraTarget(Structure tgt)
        {
            if (shared.Vessel.isActiveVessel)
            {
                var cam = FlightCamera.fetch;
                var ves = tgt as VesselTarget;
                if (ves != null)
                {
                    cam.SetTargetVessel(ves.Vessel);
                }
                else
                {
                    var part = tgt as Suffixed.Part.PartValue;
                    if (part != null)
                    {
                        cam.SetTargetPart(part.Part);
                    }
                }
            }
        }

        private void SetCameraPositionDelegate(UserDelegate del)
        {
            if (del is NoDelegate)
            {
                camPositionDelegate = null;
            }
            else
            {
                camPositionDelegate = del;
            }
        }

        private UserDelegate GetCameraPositionDelegate()
        {
            if (camPositionDelegate == null)
                return noDelegate ?? (noDelegate = new NoDelegate(shared.Cpu));
            return camPositionDelegate;
        }

        public void KOSUpdate(double deltaTime)
        {
            if (camPositionDelegate == null)
            {
                camPositionTrigger = null;
            }
            else if (camPositionTrigger == null)
            {
                camPositionTrigger = camPositionDelegate.TriggerOnFutureUpdate(Safe.Execution.InterruptPriority.Normal);
            }
            else if (camPositionTrigger.CallbackFinished)
            {
                var result = camPositionTrigger.ReturnValue as Vector;
                if (result != null)
                {
                    SetCameraPosition(result);
                    camPositionTrigger = camPositionDelegate.TriggerOnFutureUpdate(Safe.Execution.InterruptPriority.Normal);
                }
                else
                {
                    camPositionTrigger = null;
                    camPositionDelegate = null;
                    throw new KOSInvalidDelegateType("FLIGHTCAMERA:POSITIONUPDATER", "Vector", camPositionTrigger.ReturnValue.KOSName);
                }
            }
        }

        public void Dispose()
        {
            if (shared != null)
            {
                shared.UpdateHandler.RemoveObserver(this);
            }
            shared = null;
            camPositionDelegate = null;
            camPositionTrigger = null;
        }
    }
}