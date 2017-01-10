kOS Stock Camera Addon
**********************

#Description

This project is an addon for [kOS](https://github.com/KSP-KOS/KOS), which is a
mod for the game [Kerbal Space Program](https://kerbalspaceprogram.com/).  It
provides a method for controlling the game's stock camera position and
orientation.  The "stock" designation is important, as other mods that change
the camera's behavior may conflict with this functionality.

#Structures

##CAMERA

* `FLIGHTCAMERA` - Get Only - `FLIGHTCAMERA` - Returns the object which allows
control of the camera in the flight scene (see below).

##FLIGHTCAMERA

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

#Building

You must have an IDE or compiler capable of building Visual Studio solutions
(.sln files).  For the sake of simplicity, this repository assumes that it will
be located next to the kOS repository, in the same parent directory.  All
references match those of kOS, and use relative paths pointing to the files in
the kOS repository.
