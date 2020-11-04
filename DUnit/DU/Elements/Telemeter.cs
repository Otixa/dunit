using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace DUnit.DU.Elements
{
    public class Telemeter : Element
    {
        private Ship ship;
        public Telemeter(int id, Ship ship)
            :base(id, "TelemeterUnit")
        {
            this.ship = ship;
        }

        public override Table GetTable(Script lua)
        {
            var table = base.GetTable(lua);
            table["getMaxDistance"] = new Func<float>(() => 100);
            table["getDistance"] = new Func<float>(() => (float)Math.Min(100, ship.Universe.GetAltitude(ship.Position)));

            return table;
        }
    }
}
