using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace DUnit.DU.Elements
{
    public class Emitter : Element
    {
        public Emitter(int id)
            :base(id, "EmitterUnit")
        {

        }

        public override Table GetTable(Script lua)
        {
            var table = base.GetTable(lua);
            table["send"] = new Func<string, string, bool>((channel, message) => true);
            table["getRange"] = new Func<float>(() => 100);

            return table;
        }
    }
}
