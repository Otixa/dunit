using DUnit.DU.Elements;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace DUnit.DU
{
    public class DUUnit : ILuaObject
    {
        public Table GetTable(Script lua)
        {
            var unit = new Table(lua);



            return unit;
        }
    }
}
