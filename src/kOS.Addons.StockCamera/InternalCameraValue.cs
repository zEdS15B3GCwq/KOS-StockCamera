﻿using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using UnityEngine;

namespace kOS.AddOns.StockCamera
{
	[KOSNomenclature("InternalCamera")]
	public class InternalCameraValue : Structure
	{
		private SharedObjects shared;

		public InternalCameraValue(SharedObjects shared)
		{
			this.shared = shared;

			AddSuffix(new string[] { "PITCH", "CAMERAPITCH" }, new SetSuffix<ScalarValue>(GetCameraPitch, SetCameraPitch));
			AddSuffix(new string[] { "ROT", "ROTATION", "CAMERAROTATION" }, new SetSuffix<ScalarValue>(GetCameraRot, SetCameraRot));
			AddSuffix(new string[] { "FOV", "CAMERAFOV" }, new SetSuffix<ScalarValue>(GetCameraFoV, SetCameraFoV));
			AddSuffix(new string[] { "ACTIVEKERBAL"}, new SetSuffix<CrewMember>(GetActiveKerbal, SetActiveKerbal));
			AddSuffix(new string[] { "ACTIVE" }, new NoArgsSuffix<BooleanValue>(GetActive));
		}

		private ScalarValue GetCameraPitch()
		{
			return InternalCamera.Instance.currentPitch;
		}

		private void SetCameraPitch(ScalarValue value)
		{
			var camera = InternalCamera.Instance;
			camera.currentPitch = Mathf.Clamp(value, camera.minPitch, camera.maxPitch);
		}

		private ScalarValue GetCameraRot()
		{
			return InternalCamera.Instance.currentRot;
		}

		private void SetCameraRot(ScalarValue value)
		{
			var camera = InternalCamera.Instance;
			camera.currentRot = Mathf.Clamp(value, -camera.maxRot, camera.maxRot);
		}

		private ScalarValue GetCameraFoV()
		{
			return InternalCamera.Instance.currentFoV;
		}

		private void SetCameraFoV(ScalarValue value)
		{
			var camera = InternalCamera.Instance;
			camera.currentZoom = Mathf.Clamp(value / camera.initialZoom, camera.maxZoom, camera.minZoom);
		}

		private CrewMember GetActiveKerbal()
		{
			var cameraManager = CameraManager.Instance;
			if (cameraManager.currentCameraMode == CameraManager.CameraMode.IVA)
			{
				return new CrewMember(cameraManager.IVACameraActiveKerbal.protoCrewMember, shared);
			}
			throw new KOSException("Failed to get active crewmember");
		}

        private void SetActiveKerbal(CrewMember crewMember)
        {
            if (!shared.Vessel.isActiveVessel)
                throw new KOSException("ActiveKerbal can only be set on the activevessel!");

            // kOS's CrewMember wrapper (kOS.Suffixed.CrewMember) does not expose the underlying
            // ProtoCrewMember as a public property in the version you have.  Instead, find the
            // ProtoCrewMember on the vessel by matching the wrapper's public Name.
            ProtoCrewMember target = null;
            foreach (var p in shared.Vessel.crew)
            {
                if (p != null && p.name == crewMember.Name)
                {
                    target = p;
                    break;
                }
            }

            if (target == null)
                throw new KOSException($"CrewMember {crewMember.Name} is not on Vessel {shared.Vessel.name}!");

            var cameraManager = CameraManager.Instance;
            cameraManager.SetCameraIVA(target.KerbalRef, true);
        }

        private BooleanValue GetActive()
		{
			return CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA;
		}
	}
}
