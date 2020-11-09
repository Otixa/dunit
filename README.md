# Purpose
DUnit is a unit testing tool for LUA scripts, specifically those packaged into DU format by DUBuild

# The environment
	- Ship, Positioned at {0,0,0}, Rotation {0,0,0}, Mass 100T, 100000N of thrust in all axis, unlimited angular accel
	- Madis positioned at {-34464, 17465536, 22665536}, 44300 radius, 7000m thick atmosphere at 0.8 density

# Available Functions
The following functions are available inside the lua environment

## Core
- core.setConstructWorldPosition( float, float, float) - Moves the ship
- core.setWorldVelocity( float, float, float ) - Sets the ships velocity

## Universe
- universe.getPositionWithAltitude(float) - Gets a vector3 position that is the specified altitude above the planet

## Test Framework
- testframework.tickphysics(seconds) - Triggers calculation of all physics related things; such as gravity, thrust, position translation, rotation
- testframework.doupdate() - Triggers all lua update functions (defined in the script)
- testframework.doflush() - Triggers all lua flush functions (defined in the script)

# How to use
dotnet run dunit.dll -s [path to json lua script, wildcard supported] -t [path to folder containing lua tests] -l [optional path to export a JUnit format report]