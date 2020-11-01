using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace DUnit.DU.Elements
{
    public class Unit : Element
    {
        private Ship ship;

        private bool landingGearDeployed = false;

        public Unit(Ship ship, int id, string className)
            :base(id, className)
        {
            this.ship = ship;
        }

        public override dynamic GetTable()
        {
            dynamic unit = base.GetTable();
            unit.getAtmosphereDensity = new Func<float>(() => (float)ship.Universe.GetAirDensityAtPosition(ship.Position));

            unit.setEngineCommand = new Func<string, float[], float[], bool, bool, string, string, string, bool>(
                (Tags, Thrust, Rotation, b1, b2, P1Tags, P2Tags, P3Tags) =>
                {
                    ship.SetThrust(new Vector3(Thrust[0], Thrust[1], Thrust[3]));
                    ship.SetRotation(new Vector3(Rotation[0], Rotation[1], Rotation[3]));
                    return true;
                });

            unit.hide = new Func<bool>(() => true);
            unit.show = new Func<string, bool>((content) => true);

            unit.isAnyLandingGearExtended = new Func<bool>(() => landingGearDeployed);
            unit.retractLandingGears = new Func<bool>(() => { landingGearDeployed = false; return true; });
            unit.extendLandingGears = new Func<bool>(() => { landingGearDeployed = true; return true; });

            return unit;
        }
    }
}
