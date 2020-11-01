using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DUnit.DU
{
    public class Planet
    {
        public Vector3 Position { get; private set; }
        public double Mass { get; private set; }
        public double Radius { get; private set; }
        public double AtmosphereHeight { get; private set; }
        public double AtmosphereDensity { get; private set; }
        public float SurfaceGravity => GravityAtPosition(Vector3.UnitX * (float)Radius).Length();

        public Planet(
            Vector3 position,
            double mass,
            double radius,
            double atmoHeight,
            double atmoDensity)
        {
            this.Position = position;
            this.Mass = mass;
            this.Radius = radius;
            this.AtmosphereHeight = atmoHeight;
            this.AtmosphereDensity = atmoDensity;
        }

        public Vector3 GravityAtPosition(Vector3 pos)
        {
            var relativePosition = pos - Position;
            var strength =  (float)(Universe.G * (Mass / relativePosition.LengthSquared()));
            return Vector3.Normalize(relativePosition) * strength;
        }

        public double GetAtmosphericDensityAtPosition(Vector3 pos)
        {
            var relativePosition = pos - Position;
            var altitude = relativePosition.Length() - Radius;
            if (altitude >  AtmosphereHeight) return 0;
            return AtmosphereDensityAtAltitude(altitude);
        }

        private double AtmosphereDensityAtAltitude(double altitude)
        {
            var rel = 1 - Math.Min(1, altitude / AtmosphereHeight);
            return rel * AtmosphereDensity;
        }
    }
}
