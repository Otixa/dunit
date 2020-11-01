﻿using DUnit.DU.Elements;
using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DUnit.DU
{
    public class System : ILuaObject
    {
        private bool lockView = false;
        private bool freeze = false;
        public dynamic GetTable()
        {
            dynamic system = new LuaTable();

            system.getTime = new Func<double>(() => (DateTime.UtcNow - new DateTime(2017, 01, 01)).TotalMilliseconds / 1000);
            system.getActionUpdateDeltaTime = new Func<float>(() => 0.05f);
            system.lockView = new Func<bool, bool>((L) => lockView = L);
            system.isViewLocked = new Func<bool>(() => lockView);
            system.freeze = new Func<bool, bool>((F) => freeze = F);
            system.isFrozen = new Func<bool>(() => freeze);
            system.print = new Func<string, bool>((Text) => { Console.WriteLine(Text); return true; });
            system.getPlayerName = new Func<int, string>((id) => "unreachable");
            system.getPlayerWorldPos = new Func<int, float[]>((id) => Vector3.Zero.ToLua());

            system.showScreen = new Func<bool, bool>((show) => true);
            system.setScreen = new Func<string, bool>((content) => true);
            system.getActionKeyName = new Func<string, bool>((action) => true);
            system.createWidgetPanel = new Func<string, bool>((label) => true);
            system.destroyWidgetPanel = new Func<string, bool>((label) => true);
            system.createWidget = new Func<string, string, bool>((id, type) => true);
            system.destroyWidget = new Func<string, bool>((id) => true);
            system.createData = new Func<string, bool>((data) => true);
            system.destroyData = new Func<string, bool>((data) => true);
            system.updateData = new Func<string, string, bool>((id, data) => true);
            system.addDataToWidget = new Func<string, string, bool>((dataid, widgetid) => true);
            system.removeDataFromWidget = new Func<string, string, bool>((dataid, widgetid) => true);

            system.getMouseWheel = new Func<int>(() => 0);
            system.getMouseDeltaX = new Func<float>(() => 0);
            system.getMouseDeltaY = new Func<float>(() => 0);
            system.getMousePosX = new Func<float>(() => 0);
            system.getMousePosY = new Func<float>(() => 0);
            system.getThrottleInputFromMouseWheel = new Func<int>(() => 0);
            system.getControlDeviceForwardInput = new Func<float>(() => 0);
            system.getControlDeviceYawInput = new Func<float>(() => 0);
            system.getControlDeviceLeftRightInput = new Func<float>(() => 0);



            return system;
        }
    }
}