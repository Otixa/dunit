using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using MoonSharp.Interpreter;

namespace DUnit.DU.Elements
{
    public class Unit : Element
    {
        private DUConstruct ship;

        private bool landingGearDeployed = false;

        public Unit(DUConstruct ship, int id, string className)
            :base(id, className)
        {
            this.ship = ship;
        }

        public override Table GetTable(Script lua)
        {
            Table unit = base.GetTable(lua);

            unit["getAtmosphereDensity"] = new Func<float>(() => (float)ship.Universe.GetAirDensityAtPosition(ship.Position));

            unit["setEngineCommand"] = new Func<string, float[], float[], bool, bool, string, string, string, bool>(
                (Tags, Thrust, Rotation, b1, b2, P1Tags, P2Tags, P3Tags) =>
                {
                    ship.SetThrust(new Vector3(Thrust[0], Thrust[1], Thrust[3]));
                    ship.SetRotation(new Vector3(Rotation[0], Rotation[1], Rotation[3]));
                    return true;
                });

            unit["hideWidget"] = new Func<bool>(() => true);
            unit["showWidget"] = new Func<bool>(() => true);

            unit["isAnyLandingGearExtended"] = new Func<bool>(() => landingGearDeployed);
            unit["retractLandingGears"] = new Func<bool>(() => { landingGearDeployed = false; return true; });
            unit["extendLandingGears"] = new Func<bool>(() => { landingGearDeployed = true; return true; });

            unit["setTimer"] = new Func<string, float, bool>((Name, Interval) => true);

            unit["getMasterPlayerWorldPosition"] = new Func<float[]>(() => Vector3.Zero.ToLua());

            return unit;
        }
    }
}
