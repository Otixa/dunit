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
        }
        public DUEnvironment(DirectoryInfo requirePath, OutputModule module)
            :this(requirePath)
        {
            this.OutputModule = module;
        }

        public bool LoadScriptIntoEnvironment(Script Lua, OutputModule module)
        {
            var testframework = new Table(Lua);
            Lua.Globals["testFramework"] = testframework;

            //testframework.reset = new Func<bool>(() => Reset());
            testframework["tickPhysics"] = new Func<float, bool>((S) => TickPhysics(S));
            testframework["update"] = new Table(Lua);
            testframework["flush"] = new Table(Lua);

            testframework["doUpdate"] = Lua.LoadFunction(@"function() for k,v in pairs(testFramework.update) do v() end end");
            testframework["doFlush"] = Lua.LoadFunction(@"function() for k,v in pairs(testFramework.flush) do v() end end");

            //ExecuteLua(@"json = require (""dkjson"")");
            Lua.DoString(@"require (""Helpers"")");
            Lua.DoString(@"require (""AxisCommand"")");
            Lua.DoString(@"require (""Navigator"")");
            Lua.DoString(@"require (""pl/init"")");
            Lua.DoString(@"require (""cpml/sgui"")");
            Lua.DoString(@"require (""ST/FLuant"")");
            //ExecuteLua("testframework.doupdate = function() for k,v in pairs(testframework.update) do v() end end");
            //ExecuteLua("testframework.doflush = function() for k,v in pairs(testframework.flush) do v() end end");

            //Some wierd DU implementation stuff

            Lua.DoString(@"
                _G.orig_type = _G.type
                
                _G.type = function(o)
                    local t = _G.orig_type(o)
                    if t == ""table"" or t == ""userdata"" then
                        local mt = getmetatable(o) or {}
                        if mt._name then return mt._name end
                    end
                    return t
                end
                    
                _G.types = { type=_G.type }


                --WARNING, the below line is not compliant with DU - should be removed once its no longer needed
                _G.typeof = _G.type
                
            ");

            //Do start shit

            foreach (var startModule in module.Handlers.Where(x => x.Filter.Signature.StartsWith("start")))
            {
                try
                {
                    Lua.DoString(startModule.Code);
                } catch (ScriptRuntimeException e)
                {
                    var errorLineRegex_SingleLine = new System.Text.RegularExpressions.Regex(@"chunk_([\d]+):\(([\d]+),([\d]+)-([\d]+)");
                    var errorLineRegex_MultiLine = new System.Text.RegularExpressions.Regex(@"chunk([\d]+)+:\(([\d]+),([\d]+)-([\d]+),([\d]+)");
                    var errorLineRegex_Classic = new System.Text.RegularExpressions.Regex(@"chunk_([\d]+)+:\(([\d]+),([\d]+)\)");
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
                        sourceName = slResult.Groups[1].Value;
                        startLine = slResult.Groups[2].Value;
                        startChar = slResult.Groups[3].Value;
                        endChar = slResult.Groups[4].Value;
                    }
                    else if (cResult.Success)
                    {
                        sourceName = cResult.Groups[1].Value;
                        startLine = cResult.Groups[2].Value;
                        startChar = cResult.Groups[3].Value;
                    }

                    var classNameDetails = classNameRegex.Match(startModule.Code);

                    MoonSharp.Interpreter.Debugging.SourceCode sourceFile = null;
                    var sourceFileLine = string.Empty;
                    if (sourceName != String.Empty)
                    {
                        var sourceFileID = int.Parse(sourceName);
                        sourceFile = Lua.GetSourceCode(sourceFileID);
                        sourceFileLine = sourceFile.Lines[int.Parse(startLine)];
                        var sourceFileClass = classNameRegex.Match(sourceFile.Code);
                        if (sourceFileClass.Success) classNameDetails = sourceFileClass;
                    }
                    

                    if (classNameDetails.Success)
                    {
                        sourceName = classNameDetails.Groups[1].Value;
                    }

                    var errorMessage = $"{e.Message} processing slot{startModule.Key} in {sourceName} Line:{startLine} Col:{startChar} to Line:{endLine} Char:{endChar} Contents: {sourceFileLine}";

                    throw new Exception(errorMessage);
                }
            }
            
            

            int slotID = 1;
            foreach (var startModule in module.Handlers.Where(x => x.Filter.Signature.StartsWith("update")))
            {
                Lua.DoString($"testFramework.update.slot{slotID} = function() {startModule.Code} end");
                slotID++;
            }


            slotID = 1;
            foreach (var startModule in module.Handlers.Where(x => x.Filter.Signature.StartsWith("flush")))
            {
                Lua.DoString($"testFramework.flush.slot{slotID} = function() {startModule.Code} end");
                slotID++;
            }
           

            return true;
        }

        public Script BuildEnvironment()
        {
            var Lua = new Script();

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

            if (OutputModule != null) LoadScriptIntoEnvironment(Lua, OutputModule);

            return Lua;
        }

        public bool TickPhysics(float seconds)
        {
            Ship.Tick(seconds);
            return true;
        }

        public Table LoadTest(Script environment, string code)
        {
            var chunkResult = environment.DoString(code);
            if (chunkResult.Type != DataType.Table) throw new Exception("Test module did not return a table");
            return chunkResult.Table;
        }
    }
}
