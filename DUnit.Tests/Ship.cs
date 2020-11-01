using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DUnit.Tests
{
    public class ShipTests
    {

        private DU.Universe blankUniverse;
        private DU.Ship blankUniverseShip;

        private DU.Universe populatedUniverse;
        private DU.Planet moon;
        private DU.Ship populatedUniverseShip_surface;
        private DU.Ship populatedUniverseShip_atmo;
        private DU.Ship populatedUniverseShip_space;
        private Vector3 moonSurface;
        private Vector3 moonSpace;
        private Vector3 moonHalfSpace;

        [SetUp]
        public void Setup()
        {
            blankUniverse = new DU.Universe();
            blankUniverseShip = new DU.Ship(blankUniverse, Vector3.Zero, Vector3.Zero);


            moon = new DU.Planet(
                        new Vector3(0, 0, 0),
                        7.342e+22,
                        1737400,
                        10000,
                        1
                    );
            populatedUniverse = new DU.Universe(new List<DU.Planet>() { moon });

            moonSurface = Vector3.UnitX * (float)moon.Radius;
            moonSpace = Vector3.UnitX * (float)(moon.Radius + moon.AtmosphereHeight + 10);
            moonHalfSpace = Vector3.UnitX * (float)(moon.Radius + moon.AtmosphereHeight / 2);

            populatedUniverseShip_surface = new DU.Ship(populatedUniverse, moonSurface, Vector3.Zero);
            populatedUniverseShip_atmo = new DU.Ship(populatedUniverse, moonHalfSpace, Vector3.Zero);
            populatedUniverseShip_space = new DU.Ship(populatedUniverse, moonSpace, Vector3.Zero);
        }

        [Test]
        public void ShipIsMotionlessWhenLeftAlone()
        {
            blankUniverseShip.Tick(1);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.Position);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.Velocity);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.Acceleration);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.AngularVelocity);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.AngularAcceleration);
        }
        
        [Test]
        public void ShipMovesWhenThrustIsApplied()
        {
            blankUniverseShip.SetThrust(Vector3.UnitX);
            blankUniverseShip.Tick(1);
            Assert.AreEqual(Vector3.UnitX, blankUniverseShip.Position);
            Assert.AreEqual(Vector3.UnitX, blankUniverseShip.Velocity);
            Assert.AreEqual(Vector3.UnitX, blankUniverseShip.Acceleration);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.AngularVelocity);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.AngularAcceleration);
        }

        [Test]
        public void SgipRotates()
        {
            blankUniverseShip.SetRotation(Vector3.UnitX);
            blankUniverseShip.Tick(1);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.Position);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.Velocity);
            Assert.AreEqual(Vector3.Zero, blankUniverseShip.Acceleration);
            Assert.AreEqual(Vector3.UnitX, blankUniverseShip.AngularVelocity);
            Assert.AreEqual(Vector3.UnitX, blankUniverseShip.AngularAcceleration);
        }

        [Test]
        public void GravityAffectsShip()
        {
            var preAltitude = (populatedUniverseShip_atmo.Position - moon.Position).Length();
            populatedUniverseShip_atmo.Tick(1);
            var postAltitude = (populatedUniverseShip_atmo.Position - moon.Position).Length();
            Assert.IsTrue(postAltitude < preAltitude);

            preAltitude = (populatedUniverseShip_surface.Position - moon.Position).Length();
            populatedUniverseShip_surface.Tick(1);
            postAltitude = (populatedUniverseShip_surface.Position - moon.Position).Length();
            Assert.IsTrue(postAltitude == preAltitude);
            Assert.IsTrue(populatedUniverseShip_surface.IsColliding);
        }
        
        [Test]
        public void AirResistanceAffectsShip()
        {
            populatedUniverseShip_atmo.SetThrust(Vector3.UnitX * 10);
            populatedUniverseShip_atmo.Tick(10);
            populatedUniverseShip_atmo.SetThrust(Vector3.Zero);

            var preVelocity = populatedUniverseShip_atmo.Velocity;
            populatedUniverseShip_atmo.Tick(1);
            var postVelocity = populatedUniverseShip_atmo.Velocity;
            var deltaVelocity = (preVelocity - postVelocity).Length();
            Assert.IsTrue(populatedUniverseShip_atmo.AirResistance.Length() > 0);
            Assert.IsTrue(deltaVelocity > moon.GravityAtPosition(populatedUniverseShip_atmo.Position).Length());

        }
    }
}
