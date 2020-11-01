using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DUnit.DU
{
    public class DUEnvironment
    {
        private readonly Lua Engine;
        private readonly dynamic Environment;

        private Universe Universe;
        private Ship Ship;
        private Elements.Unit Unit;

        public DUEnvironment()
        {
            Engine = new Lua();
            Environment = Engine.CreateEnvironment();

            
        }

        private void Scaffold()
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

            Ship = new Ship(Vector3.Zero, Vector3.Zero);
            Unit = new Elements.Unit(Ship, 2, "HoverchairController");

            Environment.Core = Ship.GetTable();
            Environment.Unit = Unit.GetTable();
            Environment.System = new System().GetTable();

        }
    }
}
