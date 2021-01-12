using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using kOS.Utilities;

namespace kOS.AddOns.StockCamera
{
	[KOSNomenclature("MapCamera")]
	public class MapCameraValue : Structure
	{
		private SharedObjects shared;

		public MapCameraValue(SharedObjects shared)
		{
			this.shared = shared;

			AddSuffix("SETFILTER", new TwoArgsSuffix<StringValue, BooleanValue>(SetFilter));
			AddSuffix("GETFILTER", new OneArgsSuffix<BooleanValue, StringValue>(GetFilter));
			AddSuffix("COMMNETMODE", new SetSuffix<StringValue>(GetCommNetMode, SetCommNetMode));
			AddSuffix(new string[] { "PITCH", "CAMERAPITCH" }, new SetSuffix<ScalarValue>(GetCameraPitch, SetCameraPitch));
			AddSuffix(new string[] { "HDG", "HEADING", "CAMERAHDG" }, new SetSuffix<ScalarValue>(GetCameraHdg, SetCameraHdg));
			AddSuffix(new string[] { "DISTANCE", "CAMERADISTANCE" }, new SetSuffix<ScalarValue>(GetCameraDistance, SetCameraDistance));
			AddSuffix(new string[] { "POSITION", "CAMERAPOSITION" }, new SetSuffix<Vector>(GetCameraPosition, SetCameraPosition));
			AddSuffix("TARGET", new SetSuffix<Structure>(GetTarget, SetTarget));
			AddSuffix("FILTERNAMES", new Suffix<ListValue>(GetFilterNames));
			AddSuffix("COMMNETNAMES", new Suffix<ListValue>(GetCommNetNames));
		}

		private ListValue GetCommNetNames()
		{
			return new ListValue(Enum.GetNames(typeof(CommNet.CommNetUI.DisplayMode)).Select(s => new StringValue(s)));
		}

		private ListValue GetFilterNames()
		{
			return new ListValue(Enum.GetNames(typeof(MapViewFiltering.VesselTypeFilter)).Select(s => new StringValue(s)));
		}

		private void SetCameraDistance(ScalarValue value)
		{
			PlanetariumCamera.fetch.SetDistance(value / ScaledSpace.ScaleFactor);
		}

		private ScalarValue GetCameraDistance()
		{
			return PlanetariumCamera.fetch.Distance * ScaledSpace.ScaleFactor;
		}

		private void SetCameraPosition(Vector value)
		{
			PlanetariumCamera.fetch.SetCamCoordsFromPosition(ScaledSpace.LocalToScaledSpace(value.ToVector3() + shared.Vessel.CoMD));
		}

		private Vector GetCameraPosition()
		{
			return new Vector(ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.fetch.transform.position) - shared.Vessel.CoMD);
		}

		private void SetCameraHdg(ScalarValue value)
		{
			PlanetariumCamera.fetch.camHdg = (float)Utils.DegreesToRadians(value.GetDoubleValue());
		}

		private ScalarValue GetCameraHdg()
		{
			// note - this converts to degrees
			return PlanetariumCamera.fetch.getYaw();
		}

		private void SetCameraPitch(ScalarValue value)
		{
			var camera = PlanetariumCamera.fetch;
			float pitch = (float)Utils.DegreesToRadians(value.GetDoubleValue());
			camera.camPitch = Math.Max(camera.minPitch, Math.Min(camera.maxPitch, pitch));
		}

		private ScalarValue GetCameraPitch()
		{
			// note - this converts to degrees
			return PlanetariumCamera.fetch.getPitch();
		}

		private Structure GetTarget()
		{
			MapObject target = PlanetariumCamera.fetch.target;

			if (target.vessel != null)
			{
				return VesselTarget.CreateOrGetExisting(PlanetariumCamera.fetch.target.vessel, shared);
			}
			else if (target.celestialBody != null)
			{
				return BodyTarget.CreateOrGetExisting(target.celestialBody, shared);
			}
			else if (target.maneuverNode != null)
			{
				// NOTE: assumption that this node is for the active vessel...
				// is it possible to focus on nodes of other vessels?  Maybe only if it's your target?
				return Node.FromExisting(FlightGlobals.ActiveVessel, target.maneuverNode, shared);
			}
			else
			{
				throw new KOSException("Don't know how to handle mapview target type {0}", target.type);
			}
		}

		private void SetTarget(Structure value)
		{
			if (value is VesselTarget vessel)
			{
				PlanetariumCamera.fetch.SetTarget(vessel.Vessel.mapObject);
			}
			else if (value is BodyTarget body)
			{
				PlanetariumCamera.fetch.SetTarget(body.Body);
			}
			else if (value is Node node)
			{
				var v = shared.Vessel;

				// since KOS doesn't expose the internal ManeuverNode,
				// instead we have to go find it
				if (v.patchedConicSolver != null)
				{
					var maneuverNode = v.patchedConicSolver.maneuverNodes.SingleOrDefault(m => Node.FromExisting(v, m, shared) == node);
					var targetMapObject = maneuverNode == null ? null : 
						PlanetariumCamera.fetch.targets.SingleOrDefault(t => t.maneuverNode == maneuverNode);
							
					if (targetMapObject != null)
					{
						PlanetariumCamera.fetch.SetTarget(targetMapObject);
					}
				}
			}
			else
			{
				throw new KOSException("Invalid mapview target type - must be vessel, body, or node");
			}
		}

		CommNet.CommNetUI.DisplayMode ParseCommNetMode(string name)
		{
			if (Enum.TryParse(name, true, out CommNet.CommNetUI.DisplayMode mode))
			{
				return mode;
			}
			else
			{
				throw new KOSException("Invalid CommNet display mode: {0}", name);
			}
		}

		private void SetCommNetMode(StringValue value)
		{
			CommNet.CommNetUI.ModeFlightMap = ParseCommNetMode(value);
		}

		private StringValue GetCommNetMode()
		{
			return CommNet.CommNetUI.ModeFlightMap.ToString();
		}

		MapViewFiltering.VesselTypeFilter GetFilterValue(string filterName)
		{
			if (Enum.TryParse(filterName, true, out MapViewFiltering.VesselTypeFilter filterValue))
			{
				return filterValue;
			}
			else
			{
				throw new KOSException("Invalid map filter type: {0}", filterName);
			}
		}

		private BooleanValue GetFilter(StringValue filter)
		{
			var filterValue = GetFilterValue(filter);
			int filterState = MapViewFiltering.GetFilterState();

			if (filterValue == MapViewFiltering.VesselTypeFilter.All)
			{
				// assume the last value is the largest
				var values = Enum.GetValues(typeof(MapViewFiltering.VesselTypeFilter));
				int mask = (int)values.GetValue(values.Length - 1) * 2 - 1;

				return filterState == mask;
			}
			else if (filterValue == MapViewFiltering.VesselTypeFilter.None)
			{
				return filterState == 0;
			}
			else
			{
				return (filterState & (int)filterValue) != 0;
			}
		}

		private void SetFilter(StringValue filter, BooleanValue visible)
		{
			var filterValue = GetFilterValue(filter);
			int filterState = MapViewFiltering.GetFilterState();

			if (visible)
			{
				filterState = filterState | (int)filterValue;
			}
			else
			{
				filterState = filterState & ~(int)filterValue;
			}

			MapViewFiltering.SetFilter((MapViewFiltering.VesselTypeFilter)filterState);
		}
	}
}
