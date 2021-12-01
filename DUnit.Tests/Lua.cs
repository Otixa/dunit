using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DUnit.Tests
{
    public class LuaTests
    {

        DU.DUEnvironment env;
        Script luaEnvironment;

        [SetUp]
        public void Setup()
        {
            env = new DU.DUEnvironment(new DirectoryInfo(Environment.CurrentDirectory));
            luaEnvironment = env.BuildEnvironment();
        }

        [Test]
        public void System_GetTime()
        {
            var result = luaEnvironment.DoString(@"
                return system.getTime()
            ");
            var pre = (double)result.Number;
            Assert.IsTrue(pre > 0);

            result = luaEnvironment.DoString(@"
                return system.getTime()
            ");
            var post = (double)result.Number;
            Assert.IsTrue(post > pre);
        }
        
        [Test]
        public void System_Freeze()
        {
            var result = luaEnvironment.DoString(@"
                local a = system.isFrozen()                
                system.freeze(true)
                return a, system.isFrozen()
            ");
            Assert.IsFalse(result.Tuple[0].Boolean);
            Assert.IsTrue(result.Tuple[1].Boolean);
        }

        [Test]
        public void RunLuaTests()
        {
            var rootDir = AppDomain.CurrentDomain.BaseDirectory;
            var luaDirectory = new DirectoryInfo(Path.Combine(rootDir, "LuaLibraries"));
            var tests = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "TestProject"));
            var eng = new TestEngine(luaDirectory,
                new FileInfo(Path.Combine(tests.FullName, "Standard.json")),
                new DirectoryInfo(Path.Combine(tests.FullName, "Tests")),
                new FileInfo("log.xml"));
        }
    }
}
