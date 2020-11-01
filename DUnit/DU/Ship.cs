using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Linq;

namespace DUnit.DU
{
    public class Ship : Elements.Element
    {
        public Universe Universe { get; private set; }

        public Vector3 Position { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 Acceleration { get; private set; }

        public Vector3 Rotation { get; private set; }
        public Vector3 AngularVelocity { get; private set; }
        public Vector3 AngularAcceleration { get; private set; }


        public Vector3 AirResistance { get; private set; }
        public bool IsColliding { get; private set; }
        public new float Mass { get; private set; }
        public float CrossSectionalArea { get; private set; }
        
        public Vector3 MaxKinematicsAtmoPos { get; private set; }
        public Vector3 MaxKinematicsSpacePos { get; private set; }
        public Vector3 MaxKinematicsAtmoNeg { get; private set; }
        public Vector3 MaxKinematicsSpaceNeg { get; private set; }

        public List<Elements.Element> Elements { get; private set; }

        public Ship(Universe Universe, Vector3 position, Vector3 rotation)
            :base(1, "CoreUnitDynamic")
        {
            this.Universe = Universe;
            this.Position = position;
            this.Rotation = rotation;
            this.Velocity = new Vector3(0, 0, 0);
            this.Mass = 10000;

            this.MaxKinematicsAtmoPos = new Vector3(100000, 100000, 100000);
            this.MaxKinematicsSpacePos = new Vector3(100000, 100000, 100000);
            this.MaxKinematicsAtmoNeg = new Vector3(100000, 100000, 100000);
            this.MaxKinematicsSpaceNeg = new Vector3(100000, 100000, 100000);

            this.CrossSectionalArea = 10;

            this.Elements = new List<Elements.Element>();
            this.Elements.Add(this);
        }

        public void Tick(float seconds)
        {
            

            //Calculate air resistance
            var airDensity = Universe.GetAirDensityAtPosition(Position);
            var airResistance = ((airDensity * CrossSectionalArea) / 2) * Velocity.LengthSquared();
            AirResistance = Vector3.Normalize(Velocity) * (float)(airResistance / Mass);
            if (float.IsNaN(AirResistance.X)) AirResistance = Vector3.Zero;

            var actualAcceleration = GetMaxPossibleAcceleration(Acceleration).Min(Acceleration);
            var appliedAcceleration = actualAcceleration;
            appliedAcceleration -= Universe.CalculateGravityAtPosition(Position);
            appliedAcceleration -= AirResistance;

            Velocity += (appliedAcceleration * seconds);
            var provisionalPosition = Position + (Velocity * seconds);

            if (Universe.IsCollidingWithObject(provisionalPosition))
            {
                //Tonk
                //We dont bounce `round `ere
                Velocity = Vector3.Zero;
                IsColliding = true;
            }
            else
            {
                Position = provisionalPosition;
                IsColliding = false;
            }

            //Rotation
            if (!IsColliding)
            {
                Rotation += AngularVelocity * seconds;
                AngularVelocity += AngularAcceleration * seconds;
            }
            else
            {
                Rotation += Vector3.Zero;
                AngularVelocity += Vector3.Zero;
            }
            
        }

        public void SetThrust(Vector3 accel)
        {
            this.Acceleration = accel;
        }
        public void SetRotation(Vector3 rot)
        {
            this.AngularAcceleration = rot;
        }

        public Vector4 GetAxisKinematics(Vector3 axis)
        {
            var atmoPos = (MaxKinematicsAtmoPos * axis).Length();
            var atmoNeg = (MaxKinematicsAtmoNeg * axis).Length();
            var spacePos = (MaxKinematicsSpacePos * axis).Length();
            var spaceNeg = (MaxKinematicsSpaceNeg * axis).Length();

            return new Vector4(atmoPos, atmoNeg, spacePos, spaceNeg);
        }

        public Vector3 GetMaxPossibleAcceleration(Vector3 direction)
        {
            Vector3 currentPos = Vector3.Zero;
            Vector3 currentNeg = Vector3.Zero;

            if (Universe.GetAirDensityAtPosition(Position) > 0)
            {
                currentNeg = MaxKinematicsAtmoNeg;
                currentPos = MaxKinematicsAtmoPos;
            }
            else
            {
                currentNeg = MaxKinematicsSpaceNeg;
                currentPos = MaxKinematicsSpacePos;
            }

            return new Vector3(
                direction.X >= 0 ? currentPos.X / Mass : currentNeg.X / Mass,
                direction.Y >= 0 ? currentPos.Y / Mass : currentNeg.Y / Mass,
                direction.Z >= 0 ? currentPos.Z / Mass : currentNeg.Z / Mass
                );
        }

        public override dynamic GetTable()
        {
            dynamic core = base.GetTable();

            core.getConstructMass = new Func<float>(() => this.Mass);
            core.getConstructIMass = new Func<float>(() => this.Mass * (float)(1 / Math.Sqrt(1 - Velocity.LengthSquared() / Math.Pow(Universe.C, 2))));
            core.getConstructWorldPos = new Func<float[]>(() => Position.ToLua());
            core.getConstructCrossSection = new Func<float>(() => CrossSectionalArea);
            core.getWorldVelocity = new Func<float[]>(() => Velocity.ToLua());
            core.getWorldAcceleration = new Func<float[]>(() => Acceleration.ToLua());

            core.getConstructWorldOrientationUp = new Func<float[]>(() => (Rotation * Vector3.UnitY).ToLua());
            core.getConstructWorldOrientationRight = new Func<float[]>(() => (Rotation * Vector3.UnitX).ToLua());
            core.getConstructWorldOrientationForward = new Func<float[]>(() => (Rotation * Vector3.UnitZ).ToLua());

            core.getWorldGravity = new Func<float[]>(() => Universe.CalculateGravityAtPosition(Position).ToLua());
            core.g = new Func<float[]>(() => Universe.CalculateGravityAtPosition(Position).ToLua());
            core.getWorldVertical = new Func<float[]>(() => Universe.CalculateGravityAtPosition(Position).ToLua());
            core.getWorldAirFrictionAcceleration = new Func<float[]>(() => AirResistance.ToLua());

            core.getWorldAngularVelocity = new Func<float[]>(() => AngularVelocity.ToLua());
            core.getWorldAngularAcceleration = new Func<float[]>(() => AngularAcceleration.ToLua());
            core.getWorldAirFrictionAngularAcceleration = new Func<float[]>(() => Vector3.Zero.ToLua()); //No idea how to simulate this

            core.getAltitude = new Func<float>(() => (float)Universe.GetAltitude(Position));
            core.getConstructId = new Func<int>(() => 1);
            core.getConstructMass = new Func<float>(() => Mass);
            core.getConstructCrossSection = new Func<float>(() => CrossSectionalArea);

            core.getVelocity = new Func<float[]>(() => Vector3.Zero.ToLua());//Im lazy
            core.getAcceleration = new Func<float[]>(() => Vector3.Zero.ToLua());//Im lazy

            core.getMaxKinematicsParametersAlongAxis = new Func<string, float[], float[]>((T, D) => GetAxisKinematics(new Vector3(D[0], D[1], D[2])).ToLua());

            core.spawnNumberSticker = new Func<int, float, float, float, string, int>((nb, x, y, z, orientation) => -1);
            core.spawnArrowSticker = new Func<float, float, float, string, bool>((x, y, z, orientation) => true);
            core.deleteSticker = new Func<int, bool>((index) => true);
            core.moveSticker = new Func<int, float, float, float, bool>((index, x, y, z) => true);
            core.rotateSticker = new Func<int, float, float, float, bool>((index, angle_x, angle_y, angle_z) => true);

            core.getElementIdList = new Func<IEnumerable<int>>(() => this.Elements.Select(x => x.ID));
            core.getElementTypeById = new Func<int, string>((uid) => this.Elements.Where(x => x.ID == uid).FirstOrDefault()?.ClassName ?? null);
            core.getElementHitPointsById = new Func<int, int>((uid) => this.Elements.Where(x => x.ID == uid).FirstOrDefault()?.HitPoints ?? 0);
            core.getElementMaxHitPointsById = new Func<int, int>((uid) => this.Elements.Where(x => x.ID == uid).FirstOrDefault()?.MaxHitPoints ?? 0);
            core.getElementMassById = new Func<int, float>((uid) => this.Elements.Where(x => x.ID == uid).FirstOrDefault()?.Mass ?? 0);
            core.getElementPositionById = new Func<int, float[]>((uid) => Vector3.Zero.ToLua());
            core.getElementRotationById = new Func<int, float[]>((uid) => Vector3.Zero.ToLua());
            core.getElementTagsById = new Func<int, string>((uid) => "");



            return core;
        }
    }
}
