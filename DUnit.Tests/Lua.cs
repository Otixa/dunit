using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DUnit.Tests
{
    public class LuaTests
    {

        DU.DUEnvironment env;

        [SetUp]
        public void Setup()
        {
            env = new DU.DUEnvironment();
            env.Scaffold();
        }

        [Test]
        public void System_GetTime()
        {
            var result = env.ExecuteLua(@"
                return system.getTime()
            ");
            var pre = (double)result.Values[0];
            Assert.IsTrue(pre > 0);

            result = env.ExecuteLua(@"
                return system.getTime()
            ");
            var post = (double)result.Values[0];
            Assert.IsTrue(post > pre);
        }
        
        [Test]
        public void System_Freeze()
        {
            var result = env.ExecuteLua(@"
                local a = system.isFrozen()                
                system.freeze(true)
                return a, system.isFrozen()
            ");
            Assert.IsFalse((bool)result.Values[0]);
            Assert.IsTrue((bool)result.Values[1]);
        }



    }
}
