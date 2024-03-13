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
        
        // Функция для расчета барицентрических координат
        public static Vector4 CalculateBarycentricCoordinates(int x, int y, Vector4 v1, Vector4 v2, Vector4 v3)
        {
            var alpha = ((v2.Y - v3.Y) * (x - v3.X) + (v3.X - v2.X) * (y - v3.Y)) /
                        ((v2.Y - v3.Y) * (v1.X - v3.X) + (v3.X - v2.X) * (v1.Y - v3.Y));
            var beta = ((v3.Y - v1.Y) * (x - v3.X) + (v1.X - v3.X) * (y - v3.Y)) /
                       ((v2.Y - v3.Y) * (v1.X - v3.X) + (v3.X - v2.X) * (v1.Y - v3.Y));
            var gamma = 1.0f - alpha - beta;

            return new Vector4(alpha, beta, gamma, 0);
        }

        // Функция для проверки, принадлежит ли точка треугольнику
        public static bool IsPointInTriangle(int x, int y, Vector4 v1, Vector4 v2, Vector4 v3)
        {
            var alpha = ((v2.Y - v3.Y) * (x - v3.X) + (v3.X - v2.X) * (y - v3.Y)) /
                        ((v2.Y - v3.Y) * (v1.X - v3.X) + (v3.X - v2.X) * (v1.Y - v3.Y));
            var beta = ((v3.Y - v1.Y) * (x - v3.X) + (v1.X - v3.X) * (y - v3.Y)) /
                       ((v2.Y - v3.Y) * (v1.X - v3.X) + (v3.X - v2.X) * (v1.Y - v3.Y));
            var gamma = 1.0f - alpha - beta;

            return alpha >= 0 && beta >= 0 && gamma >= 0;
        }
        
        
        
    }
}