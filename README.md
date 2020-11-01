# Purpose
DUnit is a unit testing tool for LUA scripts, specifically those packaged into DU format by DUBuild

# Available Functions
The following functions are available inside the lua environment
- testframework.reset() - Resets the universe to default values :
	- Ship, Positioned at {0,0,0}, Rotation {0,0,0}, Mass 100T, 100000N of thrust in all axis, unlimited angular accel
	- Madis positioned at {-34464, 17465536, 22665536}, 44300 radius, 7000m thick atmosphere at 0.8 density

- testframework.tickphysics(seconds) - Triggers calculation of all physics related things; such as gravity, thrust, position translation, rotation

- testframework.doupdate() - Triggers all lua update functions (defined in the script)
- testframework.doflush() - Triggers all lua flush functions (defined in the script)

# How to use
dotnet run dunit.dll -s [path to json lua script] -t [path to folder containing lua] -l [optional path to export a JUnit format report]