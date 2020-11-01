using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Linq;

namespace DUnit.DU
{
    public class Universe
    { 
        public static double G => 6.67 * Math.Pow(10, -11);
        public static double C => 8333;

        private List<Planet> planets;

        public Universe()
        {
            this.planets = new List<Planet>();
        }
        public Universe(IEnumerable<Planet> planets)
            :base()
        {
            this.planets = new List<Planet>(planets);
        }

        public Vector3 CalculateGravityAtPosition(Vector3 position)
        {
            var vec = Vector3.Zero;
            foreach (var planet in planets)
            {
                vec += planet.GravityAtPosition(position);
            }

            return vec;
        }

        public bool IsCollidingWithObject(Vector3 position)
        {
            foreach (var planet in planets)
            {
                if ((position - planet.Position).Length() <= planet.Radius) return true;
            }
            return false;
        }

        public double GetAirDensityAtPosition(Vector3 position)
        {
            foreach (var planet in planets)
            {
                    var airDensity = planet.GetAtmosphericDensityAtPosition(position);
                    if (airDensity > 0) return airDensity;
            }
            return 0;
        }

        public double GetAltitude(Vector3 position)
        {
            return planets.Min(x => (x.Position - position).Length());
        }
    }
}
