using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace DUnit.DU.Elements
{
    public class Receiver : Element
    {
        public Receiver(int id)
            :base(id, "ReceiverUnit")
        {

        }

        public override Table GetTable(Script lua)
        {
            var table = base.GetTable(lua);
            table["getRange"] = new Func<float>(() => 100);

            return table;
        }
    }
}
