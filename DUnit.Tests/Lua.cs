using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
            env = new DU.DUEnvironment(new System.IO.DirectoryInfo(Environment.CurrentDirectory));
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



    }
}
