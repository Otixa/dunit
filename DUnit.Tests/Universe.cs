using DUnit.DU;
using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;

namespace DUnit.Tests
{
    public class UniverseTests
    {

        private Universe blankUniverse;
        private Universe populatedUniverse;
        private Universe dualUniverse;
        private Planet moon;
        private Planet moon2;
        private Vector3 moonSurface;
        private Vector3 moonSpace;
        private Vector3 moonHalfSpace;
        private Vector3 moonLagrange;

        [SetUp]
        public void Setup()
        {
            blankUniverse = new Universe();

            moon = new Planet(
                        new Vector3(0, 0, 0),
                        7.342e+22,
                        1737400,
                        1000,
                        1
                    );

            moon2 = new Planet(
                        new Vector3(1737400 * 1000, 0, 0),
                        7.342e+22,
                        1737400,
                        1000,
                        1
                    );


            populatedUniverse = new Universe(
                new List<Planet>(){
                    moon
                }
            );

            dualUniverse = new Universe(
                new List<Planet>(){
                    moon,
                    moon2
                }
            );

            moonSurface = Vector3.UnitX * (float)moon.Radius;
            moonSpace = Vector3.UnitX * (float)(moon.Radius+moon.AtmosphereHeight+10);
            moonHalfSpace = Vector3.UnitX * (float)(moon.Radius+moon.AtmosphereHeight/2);
            moonLagrange = Vector3.UnitX * (float)(moon.Radius*500);

        }

        [Test]
        public void Gravity()
        {
            Assert.AreEqual(Vector3.Zero, blankUniverse.CalculateGravityAtPosition(Vector3.Zero));
            Assert.AreEqual(moon.SurfaceGravity, populatedUniverse.CalculateGravityAtPosition(moonSurface).Length());
            Assert.AreEqual(Vector3.Zero, dualUniverse.CalculateGravityAtPosition(moonLagrange));
        }

        [Test]
        public void Atmosphere()
        {
            Assert.AreEqual(0, blankUniverse.GetAirDensityAtPosition(Vector3.Zero));
            Assert.AreEqual(1, populatedUniverse.GetAirDensityAtPosition(moonSurface));
            Assert.AreEqual(0, populatedUniverse.GetAirDensityAtPosition(moonSpace));
            Assert.AreEqual(0.5, populatedUniverse.GetAirDensityAtPosition(moonHalfSpace));
        }

        [Test]
        public void Collision()
        {
            Assert.IsFalse(blankUniverse.IsCollidingWithObject(Vector3.Zero));
            Assert.IsFalse(populatedUniverse.IsCollidingWithObject(moonSpace));
            Assert.IsFalse(populatedUniverse.IsCollidingWithObject(moonHalfSpace));
            Assert.IsTrue(populatedUniverse.IsCollidingWithObject(moonSurface));
        }
    }
}