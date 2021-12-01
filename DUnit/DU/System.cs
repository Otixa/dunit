using DUnit.DU.Elements;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DUnit.DU
{
    public class DUSystem : ILuaObject
    {
        private bool lockView = false;
        private bool freeze = false;

        public Table GetTable(Script lua)
        {
            var system = new Table(lua);

            system["getTime"] = new Func<double>(() => (DateTime.UtcNow - new DateTime(2017, 01, 01)).TotalSeconds);
            system["getActionUpdateDeltaTime"] = new Func<float>(() => 0.05f);
            system["lockView"] = new Func<string, bool>((L) => lockView = L == "1");
            system["isViewLocked"] = new Func<bool>(() => lockView);
            system["freeze"] = new Func<bool, bool>((F) => freeze = F);
            system["isFrozen"] = new Func<bool>(() => freeze);
            system["print"] = new Func<string, bool>((Text) => { Console.WriteLine(Text); return true; });
            system["getPlayerName"] = new Func<int, string>((id) => "unreachable");
            system["getPlayerWorldPos"] = new Func<int, float[]>((id) => Vector3.Zero.ToLua());

            system["showScreen"] = new Func<string, bool>((show) => true);
            system["setScreen"] = new Func<string, bool>((content) => true);
            system["getActionKeyName"] = new Func<string, bool>((action) => true);
            system["createWidgetPanel"] = new Func<string, bool>((label) => true);
            system["destroyWidgetPanel"] = new Func<string, bool>((label) => true);
            system["createWidget"] = new Func<string, string, bool>((id, type) => true);
            system["destroyWidget"] = new Func<string, bool>((id) => true);
            system["createData"] = new Func<string, bool>((data) => true);
            system["destroyData"] = new Func<string, bool>((data) => true);
            system["updateData"] = new Func<string, string, bool>((id, data) => true);
            system["addDataToWidget"] = new Func<string, string, bool>((dataid, widgetid) => true);
            system["removeDataFromWidget"] = new Func<string, string, bool>((dataid, widgetid) => true);

            system["getMouseWheel"] = new Func<int>(() => 0);
            system["getMouseDeltaX"] = new Func<float>(() => 0);
            system["getMouseDeltaY"] = new Func<float>(() => 0);
            system["getMousePosX"] = new Func<float>(() => 0);
            system["getMousePosY"] = new Func<float>(() => 0);
            system["getThrottleInputFromMouseWheel"] = new Func<int>(() => 0);
            system["getControlDeviceForwardInput"] = new Func<float>(() => 0);
            system["getControlDeviceYawInput"] = new Func<float>(() => 0);
            system["getControlDeviceLeftRightInput"] = new Func<float>(() => 0);

            system["getScreenWidth"] = new Func<float>(() => 1920);
            system["getScreenHeight"] = new Func<float>(() => 1080);
            system["getFov"] = new Func<float>(() => 70);

            return system;
        }
    }
}
