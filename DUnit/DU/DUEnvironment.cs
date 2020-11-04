using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Linq;
using System.IO;
using MoonSharp.Interpreter.Loaders;

namespace DUnit.DU
{
    public class DUEnvironment
    {
        private Script Lua;
        private DirectoryInfo requirePath;

        private List<string> UpdateSlots;
        private List<string> FlushSlots;

        private Universe Universe;
        private Ship Ship;
        private Elements.Unit Unit;
        private OutputModule OutputModule;

        public DUEnvironment(DirectoryInfo requirePath)
        {

            this.requirePath = requirePath;
            Reset();
        }
        public DUEnvironment(DirectoryInfo requirePath, OutputModule module)
            :this(requirePath)
        {
            this.OutputModule = module;
        }

        public bool LoadScript(OutputModule module)
        {
            var testframework = new Table(Lua);
            Lua.Globals["testframework"] = testframework;

            //testframework.reset = new Func<bool>(() => Reset());
            testframework["tickphysics"] = new Func<float, bool>((S) => TickPhysics(S));
            testframework["update"] = new Table(Lua);
            testframework["flush"] = new Table(Lua);

            testframework["doupdate"] = Lua.LoadFunction(@"function() for k,v in pairs(testframework.update) do v() end end");
            testframework["doflush"] = Lua.LoadFunction(@"function() for k,v in pairs(testframework.flush) do v() end end");

            //ExecuteLua(@"json = require (""dkjson"")");
            ExecuteLua(@"require (""Helpers"")");
            ExecuteLua(@"require (""AxisCommand"")");
            ExecuteLua(@"require (""Navigator"")");
            ExecuteLua(@"require (""pl/init"")");
            ExecuteLua(@"require (""cpml/sgui"")");
            //ExecuteLua("testframework.doupdate = function() for k,v in pairs(testframework.update) do v() end end");
            //ExecuteLua("testframework.doflush = function() for k,v in pairs(testframework.flush) do v() end end");

            //Some wierd DU implementation stuff

            ExecuteLua(@"
                _G.orig_type = _G.type
                
                _G.type = function(o)
                    local t = _G.orig_type(o)
                    if t == ""table"" then
                        local mt = getmetatable(o) or {}
                        if mt._name then return mt._name end
                    end
                    return t
                end
                    
                _G.types = { type=_G.type }
                
            ");

            //Do start shit

            foreach (var startModule in module.Handlers.Where(x => x.Filter.Signature.StartsWith("start")))
            {
                try
                {
                    ExecuteLua(startModule.Code);
                } catch (ScriptRuntimeException e)
                {
                    var errorLineRegex_SingleLine = new System.Text.RegularExpressions.Regex(@"chunk_[\d]+:\(([\d]+),([\d]+)-([\d]+)");
                    var errorLineRegex_MultiLine = new System.Text.RegularExpressions.Regex(@"chunk_[\d]+:\(([\d]+),([\d]+)-([\d]+),([\d]+)");
                    var errorLineRegex_Classic = new System.Text.RegularExpressions.Regex(@"chunk_[\d]+:\(([\d]+),([\d]+)\)");
                    var classNameRegex = new System.Text.RegularExpressions.Regex(@"[ \t]*--@class[ \t]+(\S+)");

                    var slResult = errorLineRegex_SingleLine.Match(e.DecoratedMessage);
                    var mlResult = errorLineRegex_MultiLine.Match(e.DecoratedMessage);
                    var cResult = errorLineRegex_Classic.Match(e.DecoratedMessage);

                    var sourceName = string.Empty;
                    var startLine = string.Empty;
                    var startChar = string.Empty;
                    var endLine = string.Empty;
                    var endChar = string.Empty;

                    if (mlResult.Success)
                    {
                        sourceName = mlResult.Groups[1].Value;
                        startLine = mlResult.Groups[2].Value;
                        startChar = mlResult.Groups[3].Value;
                        endLine = mlResult.Groups[4].Value;
                        endChar = mlResult.Groups[5].Value;
                    }
                    else if (slResult.Success)
                    {
                        sourceName = mlResult.Groups[1].Value;
                        startLine = mlResult.Groups[2].Value;
                        startChar = mlResult.Groups[3].Value;
                        endChar = mlResult.Groups[4].Value;
                    }
                    else if (cResult.Success)
                    {
                        sourceName = mlResult.Groups[1].Value;
                        startLine = mlResult.Groups[2].Value;
                        startChar = mlResult.Groups[3].Value;
                    }

                    var classNameDetails = classNameRegex.Match(startModule.Code);

                    MoonSharp.Interpreter.Debugging.SourceCode sourceFile = null;
                    var sourceFileLine = string.Empty;
                    if (sourceName.Contains("chunk_"))
                    {
                        var sourceFileID = int.Parse(sourceName.Split('_')[1]);
                        sourceFile = Lua.GetSourceCode(sourceFileID);
                        sourceFileLine = sourceFile.Lines[int.Parse(startLine)];
                    }
                    

                    if (classNameDetails.Success)
                    {
                        sourceName = classNameDetails.Groups[1].Value;
                    }

                    var errorMessage = $"{e.Message} in slot{startModule.Key} ({sourceName}) Line/Char {startLine}:{startChar} -> {endLine}:{endChar} Line Contents : {sourceFileLine}";

                    throw new Exception(errorMessage);
                }
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

        public bool Reset()
        {
            Lua = new Script();

            ((ScriptLoaderBase)Lua.Options.ScriptLoader).ModulePaths = new string[] { $"{Path.Join(requirePath.FullName, "?.lua")}", $"{Path.Join(requirePath.FullName, "?", "?.lua")}" };

            UpdateSlots = new List<string>();
            FlushSlots = new List<string>();

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
            Unit = new Elements.Unit(Ship, 2, "CockpitHovercraftUnit");
            Ship.AddElement(new Elements.Emitter(3));
            Ship.AddElement(new Elements.Receiver(4));
            Ship.AddElement(new Elements.Telemeter(5, Ship));

            Lua.Globals["core"] = Ship.GetTable(Lua);
            Lua.Globals["unit"] = Unit.GetTable(Lua);
            Lua.Globals["system"] = new DUSystem().GetTable(Lua);
            Lua.Globals["library"] = new Library().GetTable(Lua);

            foreach(var element in Ship.Elements)
            {
                Lua.Globals[Guid.NewGuid().ToString()] = element.GetTable(Lua);
            }

            if (OutputModule != null) LoadScript(OutputModule);

            return true;
        }

        public bool TickPhysics(float seconds)
        {
            Ship.Tick(seconds);
            return true;
        }

        public DynValue ExecuteLua(string code)
        {
            var chunk = Lua.LoadString(code);
            return Lua.Call(chunk);
            //return Environment.dochunk(code, Guid.NewGuid().ToString());
        }
    }
}
