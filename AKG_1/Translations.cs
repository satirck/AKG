using System.Linq;
using System.Numerics;

namespace AKG_1
{
    public static class Translations
    {

        public static void Transform(Vector4[] vArr, Matrix4x4 rot)
        {
            for (var i = 0; i < vArr.Count(); i++)
            {
                vArr[i] = Vector4.Transform(vArr[i], rot);
            }
        }
    }
}