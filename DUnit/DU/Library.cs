using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace DUnit.DU
{
    public class Library : Elements.ILuaObject
    {
        public Table GetTable(Script lua)
        {
            var library = new Table(lua);

            library["systemResolution3"] = new Func<float[], float[], float[], float[], float[]>((R, F, U, P) =>
            {
                var shipMatrix = new Matrix4x4(R[0], R[1], R[2], 0, F[0], F[1], F[2], 0, U[0], U[1], U[2], 0, 0, 0, 0, 1);//.CreateWorld(Vector3.Zero, new Vector3(F[0], F[1], F[2]), new Vector3(U[0], U[1], U[2]));
                Matrix4x4.Invert(shipMatrix, out var iShipMatrix);
                var t = Vector3.Transform(new Vector3(P[0], P[1], P[2]), iShipMatrix);
                return t.ToLua();

                /*var rotMatrix = new Matrix4x4(R[0], R[1], R[2], 0, F[0], F[1], F[2], 0, U[0], U[1], U[2], 0, 0, 0, 0, 1);
                var inverseRotMatrix = Matrix4x4.Transpose(rotMatrix);               
                var position = new Vector3(P[0], P[1], P[2]);
                return Vector3.Transform(position, inverseRotMatrix).ToLua();*/
            });

            

            return library;
        }
    }
}
