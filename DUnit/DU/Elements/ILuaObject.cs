using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace DUnit.DU.Elements
{
    public interface ILuaObject
    {

        public MoonSharp.Interpreter.Table GetTable(Script lua);
    }
}
