using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Linq;

namespace DUnit.DU
{
    public class DUEnvironment
    {
        private readonly Lua Engine;
        private bool Scaffolded;
        public dynamic Environment { get; private set; }

        private List<string> UpdateSlots;
        private List<string> FlushSlots;

        private Universe Universe;
        private Ship Ship;
        private Elements.Unit Unit;

        public DUEnvironment()
        {
            Engine = new Lua();
            Environment = Engine.CreateEnvironment();
            Scaffolded = false;

            UpdateSlots = new List<string>();
            FlushSlots = new List<string>();
        }

        public bool LoadScript(OutputModule module)
        {
            dynamic testframework = new LuaTable();
            Environment.testframework = testframework;

            testframework.reset = new Func<bool>(() => Scaffold());
            testframework.tickphysics = new Func<float, bool>((S) => TickPhysics(S));
            testframework.update = new LuaTable();
            testframework.flush = new LuaTable();

            ExecuteLua($"testframework.doupdate = function() for k,v in pairs(testframework.update) do v() end end");
            ExecuteLua($"testframework.doflush = function() for k,v in pairs(testframework.flush) do v() end end");

            //Do start shit
            foreach (var startModule in module.Handlers.Where(x => x.Filter.Signature.StartsWith("start")))
            {
                ExecuteLua(startModule.Code);
            }

            int slotID = 1;
            foreach (var startModule in module.Handlers.Where(x => x.Filter.Signature.StartsWith("update")))
            {
                ExecuteLua($"testframework.update.slot{slotID} = function() {startModule.Code} end");
                slotID++;
            }


            slotID = 1;
            foreach (var startModule in module.Handlers.Where(x => x.Filter.Signature.StartsWith("flush")))
            {
                ExecuteLua($"testframework.flush.slot{slotID} = function() {startModule.Code} end");
                slotID++;
            }

            return true;
        }

        public bool Scaffold()
        {
            Universe = new Universe(
                new List<Planet>(){
                    new Planet(
                        new Vector3(-34464, 17465536, 22665536),
                        1962656722,
                        44300,
                        7000,
                        0.8
                    ) 
                }
            );

            Ship = new Ship(Universe, Vector3.Zero, Vector3.Zero);
            Unit = new Elements.Unit(Ship, 2, "HoverchairController");

            Environment.core = Ship.GetTable();
            Environment.unit = Unit.GetTable();
            Environment.system = new System().GetTable();

            Scaffolded = true;
            return true;
        }

        public bool TickPhysics(float seconds)
        {
            Ship.Tick(seconds);
            return true;
        }

        public LuaResult ExecuteLua(string code)
        {
            return Environment.dochunk(code, Guid.NewGuid().ToString());
        }
    }
}
