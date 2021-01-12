kOS Stock Camera Addon
**********************

# Description

This project is an addon for [kOS](https://github.com/KSP-KOS/KOS), which is a
mod for the game [Kerbal Space Program](https://kerbalspaceprogram.com/).  It
provides a method for controlling the game's stock camera position and
orientation.  The "stock" designation is important, as other mods that change
the camera's behavior may conflict with this functionality.

# Structures

## CAMERA

* `FLIGHTCAMERA` - Get Only - `FLIGHTCAMERA` - Returns the object which allows
control of the camera in the flight scene (see below).

* `MAPCAMERA`* - Get Only - `MAPCAMERA` - Returns the object which allows control of the camera in mapview.

## FLIGHTCAMERA

* `MODE` - Get Or Set - `String` - Returns the currently selected camera mode.
When set, changes the camera's mode.  Valid options are `"AUTO"`, `"CHASE"`,
`"FREE"`, `"LOCKED"`, and `"ORBITAL"`.
* `FOV` - Get Or Set - `Scalar` - Returns or sets the "field of view" for the
flight camera.  This is essentially the same concept as "zoom" for real cameras.
* `PITCH` - Get Or Set - `Scalar` - Returns or sets the pitch component of the
camera position rotation.  The actual direction this moves the camera depends on
the frame of reference of the currently selected mode.
* `HEADING` or `HDG` - Get Or Set - `Scalar` - Returns or sets the yaw component
of the camera position rotation.  The actual direction this moves the camera
depends on the frame of reference of the currently selected mode.
* `DISTANCE` - Get Or Set - `Scalar` - Returns or sets the distance component
of the camera position, the magnitude applied to the rotation defined by
pitch and heading above.
* `POSITION` - Get Or Set - `Vector` - Returns or sets the camera's position
using a cpu vessel centered vector.  The pitch, heading, and distance components
above are automatically calculated based on the vector.  **Important: changing
the camera's target does not change the reference origin of the position
vector.** Always set the position using vectors based on the cpu vessel.
* `TARGET` - Get Or Set - `Part` or `Vessel` - Returns or sets the vessel or
part that the camera is pointing at.  This is the same as the "Aim here" feature
in stock KSP.
* `TARGETPOS` - Get Only - For debugging purposes only.
* `PIVOTPOS` - Get Only - For debugging purposes only.
* `POSITIONUPDATER` - Get Or Set - `UserDelegate` - A delegate automatically
called once per tick to update the camera position.  Initially this will return
a `DONOTHING` delegate.  Set it back to `DONOTHING` to stop the automatic
position updates.

## MAPCAMERA

* `SETFILTER(string, boolean)` - Sets whether objects of a given type should be visible in map mode.  See `FILTERNAMES` for a list of valid names.
* `GETFILTER(string)` - returns a boolean indicating whether objects of the specified type are visible.  See `FILTERNAMES` for a list of valid names.
* `COMMNETMODE` - get, set - string - Gets or sets the current commnet display mode.  See `COMMNETNAMES` for a list of valid modes.
* `PITCH` or `CAMERAPITCH` - get, set - scalar - gets or sets the pitch angle of the camera, relative to the ecliptic plane.
* `HDG` or `HEADING` or `CAMERAHDG` - get, set - scalar - gets or sets the camera heading (yaw)
* `DISTANCE` or `CAMERADISTANCE` - get, set - scalar - returns the camera distance from the camera pivot, in meters.  Note that mapview will enforce certain limits on this value.
* `POSITION` or `CAMERAPOSITION` - get, set - vector - gets or sets the position of the camera, in ship-raw coordinates.  Note that the camera will always be facing towards the pivot point.
* `TARGET` - get, set - [vessel, body, node] - gets or sets the pivot object.  May be a vessel, body, or node.
* `FILTERNAMES` - get - list - returns a list of the valid filter names for use with `SETFILTER` and `GETFILTER`
* `COMMNETNAMES` - get - list - returns a list of the valid commnet display mode names for use with `COMMNETMODE`

# Building

You must have an IDE or compiler capable of building Visual Studio solutions
(.sln files).  For the sake of simplicity, this repository assumes that it will
be located next to the kOS repository, in the same parent directory.  All
references match those of kOS, and use relative paths pointing to the files in
the kOS repository.
