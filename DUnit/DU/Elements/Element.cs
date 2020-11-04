using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace DUnit.DU.Elements
{
    public class Element : ILuaObject
    {
        public int ID { get; private set; }
        public string ClassName { get; private set; }
        public int HitPoints { get; private set; }
        public int MaxHitPoints { get; private set; }
        public float Mass { get; private set; }


        public Dictionary<string, bool> Plugs { get; private set; }

        public Element(int id, string className)
        {
            this.ID = id;
            this.ClassName = className;
            this.Plugs = new Dictionary<string, bool>();
            for (int i = 1; i < 11; i++)
            {
                this.Plugs.Add($"IN-SIGNAL-{i}", false);
                this.Plugs.Add($"OUT-SIGNAL-{i}", false);
            }

            MaxHitPoints = 1000;
            HitPoints = MaxHitPoints;
            Mass = 100;
        }
        public Element(int id, string className, float mass)
            :this(id, className)
        {
            this.Mass = mass;
        }
        public virtual Table GetTable(Script lua)
        {
            var table = new Table(lua);

            table["getId"] = new Func<int>(() => this.ID);
            table["getElementClass"] = new Func<string>(() => this.ClassName);
            table["show"] = new Func<bool>(() => true);
            table["hide"] = new Func<bool>(() => true);

            table["getData"] = new Func<string>(() => "{ }");
            table["getDataId"] = new Func<string>(() => "");
            table["getWidgetType"] = new Func<string>(() => "");
            table["getIntegrity"] = new Func<int>(() => HitPoints/MaxHitPoints);
            table["getHitPoints"] = new Func<int>(() => HitPoints);
            table["getMaxHitPoints"] = new Func<int>(() => MaxHitPoints);
            table["getMass"] = new Func<float>(() => Mass);
            table["setSignalIn"] = new Func<string, bool, bool>((plug, state) => { if (this.Plugs.ContainsKey(plug.ToUpperInvariant())){ this.Plugs[plug.ToUpperInvariant()] = state; return true; } else { return false; } });
            table["setSignalOut"] = new Func<string, bool, bool>((plug, state) => { if (this.Plugs.ContainsKey(plug.ToUpperInvariant())){ this.Plugs[plug.ToUpperInvariant()] = state; return true; } else { return false; } });
            table["getSignalIn"] = new Func<string, bool>((plug) => { if (this.Plugs.ContainsKey(plug.ToUpperInvariant())){ return this.Plugs[plug.ToUpperInvariant()]; } else { return false; } });

            return table;
        }
    }
}
